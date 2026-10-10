/// <reference types="@angular/localize" />

import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { firstValueFrom } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter } from 'rxjs/operators';
import { TimeoutError } from 'rxjs';

import { ApiPaths } from '../../core/api/api-paths';
import { registrationErrorDisplay } from '../../core/api/registration-api-error.i18n';
import { JobseekerRegistrationHttpService } from '../../core/api/services/jobseeker-registration-http.service';
import { PlatformAuthSettingsService } from '../../core/api/services/platform-auth-settings.service';
import type {
  CivilIdLookupResponse,
  RegisterJobseekerDto,
  RegisterJobseekerResponse,
} from '../../core/api/models/jobseeker-registration.models';
import { AuthService } from '../../core/auth/auth.service';
import { MOCK_USER_ID_STORAGE_KEY } from '../../core/auth/mock-user.storage';
import type { MockUserRow } from '../../core/auth/mock-user.types';
import {
  clearPkiRegistrationContext,
  readPkiRegistrationContext,
} from '../../core/auth/pki-registration.storage';
import { writeRegistrationContext } from '../../core/auth/registration.storage';
import { LocaleService } from '../../core/locale/locale.service';
import { registrationActions } from '../../core/store/registration/registration.actions';
import { AsyncExecutorService } from '../../core/ui/async-executor.service';
import { ModalCloseButtonComponent } from '../../core/ui/modal-close-button/modal-close-button.component';
import { FormPipelineService } from '../../core/ui/form-pipeline.service';
import { FormPipelineResult, isFormPipelineSuccess } from '../../core/ui/form-pipeline.types';
import {
  AngularValidators,
  ValidationMessagePipe,
  emailFormat,
  phoneDigits,
  requiredTrimmed,
} from '@validators';
import { environment } from '../../../environments/environment';

/** Default nationality for jobseeker registration (`Profile.IsOmani` when code is OM). */
const DEFAULT_NATIONALITY_CODE = 'OM';

@Component({
  selector: 'app-jobseeker-register-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, ValidationMessagePipe, ModalCloseButtonComponent],
  templateUrl: './jobseeker-register-page.component.html',
  styleUrl: '../login/login.component.scss',
})
export class JobseekerRegisterPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly http = inject(HttpClient);
  private readonly reg = inject(JobseekerRegistrationHttpService);
  private readonly authSettings = inject(PlatformAuthSettingsService);
  private readonly asyncExec = inject(AsyncExecutorService);
  private readonly formPipeline = inject(FormPipelineService);
  private readonly auth = inject(AuthService);
  private readonly store = inject(Store);
  readonly locale = inject(LocaleService);
  private readonly destroyRef = inject(DestroyRef);

  readonly useMockBackend = environment.useMockBackend;

  readonly submitError = signal<string | null>(null);
  readonly loaded = signal(false);
  readonly submitting = signal(false);
  readonly otpSubmitting = signal(false);
  readonly civilLookupMessage = signal<string | null>(null);
  readonly civilLookupLoading = signal(false);
  readonly civilLookupOk = signal(false);
  readonly otpModalOpen = signal(false);
  readonly otpPreviewCode = signal('');
  readonly otpSessionId = signal('');
  readonly otpInput = signal('');
  readonly otpError = signal<string | null>(null);
  readonly pendingRegistrationBody = signal<RegisterJobseekerDto | null>(null);
  readonly civilDataLocked = signal(false);
  /** Set when arriving from AuthModule PKI callback with a temp token. */
  readonly pkiTempToken = signal<string | null>(null);
  readonly otpSecondsLeft = signal(0);
  readonly otpTimerText = computed(() => {
    const total = this.otpSecondsLeft();
    const m = Math.floor(total / 60)
      .toString()
      .padStart(2, '0');
    const s = (total % 60).toString().padStart(2, '0');
    return `${m}:${s}`;
  });
  /** Demo OTP box — hidden when `REGISTRATION_SMS_ENABLED` = Y. */
  readonly showOtpPreview = computed(() => !this.authSettings.registrationSmsEnabled());
  private otpTimerId: ReturnType<typeof setInterval> | null = null;

  readonly form = this.fb.nonNullable.group(
    {
      civilId: ['', [AngularValidators.required, requiredTrimmed()]],
      email: ['', [AngularValidators.required, emailFormat()]],
      name_en: ['', [AngularValidators.required, requiredTrimmed()]],
      name_ar: ['', [AngularValidators.required, requiredTrimmed()]],
      mobileNumber: ['', [AngularValidators.required, requiredTrimmed(), phoneDigits()]],
    },
    { updateOn: 'change' }
  );

  ngOnInit(): void {
    this.form.controls.civilId.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
      this.civilLookupOk.set(false);
      this.civilLookupMessage.set(null);
      this.clearCivilPrefillFields();
    });
    this.form.controls.civilId.valueChanges
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        debounceTime(500),
        distinctUntilChanged(),
        filter((v) => (v ?? '').toString().trim().length >= 6)
      )
      .subscribe(() => {
        void this.lookupCivilId();
      });
    void firstValueFrom(this.authSettings.load())
      .then((settings) => {
        if (settings.PkiEnabled && !this.useMockBackend) {
          const pki = readPkiRegistrationContext();
          if (!pki) {
            void this.router.navigateByUrl('/register/jobseeker/pki', { replaceUrl: true });
            return;
          }
          this.applyPkiRegistrationContext(pki);
        }
      })
      .catch(() => undefined)
      .finally(() => this.loaded.set(true));
  }

  private applyPkiRegistrationContext(pki: {
    tempToken: string;
    civilId: string;
    mobile: string;
  }): void {
    this.pkiTempToken.set(pki.tempToken);
    this.form.patchValue({
      civilId: pki.civilId,
      mobileNumber: pki.mobile || this.form.controls.mobileNumber.value,
    });
    // Civil ID always locked after PKI; names stay editable unless Nafath returns them.
    this.form.controls.civilId.disable({ emitEvent: false });
    if (pki.mobile) {
      this.form.controls.mobileNumber.disable({ emitEvent: false });
    }
    this.civilDataLocked.set(false);
    void this.lookupCivilId();
  }

  isRequired(controlName: keyof typeof this.form.controls): boolean {
    return this.form.controls[controlName].hasValidator(AngularValidators.required);
  }

  async lookupCivilId(): Promise<void> {
    const civilId = (this.form.getRawValue().civilId ?? '').trim();
    this.civilLookupMessage.set(null);
    this.civilLookupOk.set(false);
    if (!civilId || civilId.length < 6) {
      return;
    }
    this.civilLookupLoading.set(true);
    this.form.controls.civilId.markAsTouched();
    try {
      const result = await firstValueFrom(this.reg.lookupCivilId(civilId));
      this.applyCivilLookup(result);
    } catch (err) {
      this.clearCivilPrefillFields();
      this.civilLookupOk.set(false);
      this.form.controls.civilId.setErrors({ civilLookupFailed: true });
      const apiMessage = registrationErrorDisplay(err);
      this.civilLookupMessage.set(
        apiMessage ||
          $localize`:@@jobseeker.registration.civilLookupFailed:Could not fetch civil ID details. Please try again.`
      );
    } finally {
      this.civilLookupLoading.set(false);
    }
  }

  /** Drop Nafath-prefilled name/mobile when civil ID changes or lookup fails. */
  private clearCivilPrefillFields(): void {
    this.civilDataLocked.set(false);
    const patch: {
      name_en: string;
      name_ar: string;
      mobileNumber?: string;
    } = {
      name_en: '',
      name_ar: '',
    };
    // Keep PKI-bound mobile; otherwise clear previous MOL mobile.
    if (!this.pkiTempToken()) {
      patch.mobileNumber = '';
      if (this.form.controls.mobileNumber.disabled) {
        this.form.controls.mobileNumber.enable({ emitEvent: false });
      }
    }
    this.form.patchValue(patch, { emitEvent: false });
  }

  private applyCivilLookup(result: CivilIdLookupResponse): void {
    this.form.controls.civilId.markAsTouched();
    this.form.controls.civilId.setErrors(null);
    this.civilLookupOk.set(false);
    if (result.Status === 'not_found') {
      this.clearCivilPrefillFields();
      this.form.controls.civilId.setErrors({ notFound: true });
      this.civilLookupMessage.set($localize`:@@jobseeker.registration.civilLookupNotFound:No data found for this civil ID.`);
      return;
    }
    if (result.Status === 'already_registered') {
      this.clearCivilPrefillFields();
      this.form.controls.civilId.setErrors({ alreadyRegistered: true });
      this.civilLookupMessage.set(
        this.pkiTempToken()
          ? $localize`:@@jobseeker.registration.civilLookupRegisteredPki:This civil ID is already registered. Sign in with PKI instead.`
          : $localize`:@@jobseeker.registration.civilLookupRegistered:This civil ID is already registered.`
      );
      return;
    }
    const nameEn = (result.Name_en ?? '').trim();
    const nameAr = (result.Name_ar ?? '').trim();
    const mobile = (result.MobileNumber ?? '').trim();
    // Lock name fields only when Nafath/mock returned values.
    this.civilDataLocked.set(!!(nameEn || nameAr));
    this.form.patchValue({
      name_en: nameEn || this.form.controls.name_en.value,
      name_ar: nameAr || this.form.controls.name_ar.value,
      mobileNumber: mobile || this.form.controls.mobileNumber.value,
    });
    if (mobile && this.pkiTempToken() && !this.form.controls.mobileNumber.disabled) {
      this.form.controls.mobileNumber.disable({ emitEvent: false });
    }
    this.civilLookupOk.set(true);
    this.civilLookupMessage.set($localize`:@@jobseeker.registration.civilLookupOk:Civil ID verified.`);
  }

  async submit(): Promise<void> {
    if (this.submitting()) {
      return;
    }
    this.submitting.set(true);
    this.submitError.set(null);
    try {
      const pipelineTimeoutMs = 10000;
      const resultOrTimeout = await Promise.race<FormPipelineResult<RegisterJobseekerDto> | 'pipeline-timeout'>([
        this.formPipeline.run({
          form: this.form,
          asyncValidate: async () => {
            if (!this.useMockBackend) {
              return true;
            }
            const email = this.form.controls.email.value.trim().toLowerCase();
            if (!email) {
              return true;
            }
            const res = await firstValueFrom(this.http.get<{ users: MockUserRow[] }>(ApiPaths.mockUsers));
            if (res.users.some((u) => u.email.trim().toLowerCase() === email)) {
              this.form.controls.email.setErrors({ emailNotUnique: true });
              return false;
            }
            return true;
          },
          work: async () => {
            const v = this.form.getRawValue();
            const tempToken = this.pkiTempToken();
            return {
              CivilId: v.civilId.trim(),
              Email: v.email.trim(),
              Name_en: v.name_en.trim(),
              Name_ar: v.name_ar.trim(),
              MobileNumber: v.mobileNumber.trim(),
              NationalityCode: DEFAULT_NATIONALITY_CODE,
              PrimarySpecificationId: '',
              SubSpecificationId: '',
              ...(tempToken ? { TempToken: tempToken } : {}),
            };
          },
          execute: {
            recordSuccess: false,
          },
        }),
        new Promise<'pipeline-timeout'>((resolve) => {
          setTimeout(() => resolve('pipeline-timeout'), pipelineTimeoutMs);
        }),
      ]);

      if (resultOrTimeout === 'pipeline-timeout') {
        this.submitError.set(
          $localize`:@@jobseeker.registration.pipelineTimeout:Registration flow timed out. Please retry.`
        );
        return;
      }
      const result = resultOrTimeout;

      if (isFormPipelineSuccess(result)) {
        const body = result.value;
        this.pendingRegistrationBody.set(body);
        try {
          const otp = await this.asyncExec.execute(
            async () => {
              await firstValueFrom(this.reg.validateRegistration(body));
              return firstValueFrom(
                this.reg.requestOtp(body.CivilId, body.MobileNumber ?? '', body.Email)
              );
            },
            {
              recordSuccess: false,
              progressMessage: $localize`:@@jobseeker.registration.validating:Validating registration…`,
            }
          );
          this.otpSessionId.set(otp.SessionId);
          this.otpPreviewCode.set(otp.OtpCode ?? '');
          this.startOtpCountdown(otp.ExpiresInSeconds);
          this.otpInput.set('');
          this.otpError.set(null);
          this.otpModalOpen.set(true);
        } catch {
          // Error surfaced via onError + ui-async snapshot.
        }
        return;
      }
      if (result.reason === 'invalid') {
        this.submitError.set(
          $localize`:@@jobseeker.registration.invalidForm:Please review the required fields and fix validation errors.`
        );
        return;
      }
      if (result.reason === 'server_validation') {
        this.submitError.set($localize`:@@jobseeker.registration.serverValidation:Please review highlighted fields and try again.`);
        return;
      }
      if (result.reason === 'submit_error' && result.error instanceof TimeoutError) {
        this.submitError.set(
          $localize`:@@jobseeker.registration.timeoutError:Registration request timed out. Please check backend availability and try again.`
        );
        return;
      }
      if (result.reason === 'submit_error') {
        if (result.error instanceof HttpErrorResponse && result.error.status === 0) {
          this.submitError.set(
            $localize`:@@jobseeker.registration.networkError:Could not reach the registration service. Please verify backend URL/CORS and try again.`
          );
          return;
        }
        const msg = registrationErrorDisplay(result.error);
        this.submitError.set(
          msg ?? $localize`:@@jobseeker.registration.submitFailed:Registration could not be completed.`
        );
      }
    } finally {
      this.submitting.set(false);
    }
  }

  closeOtpModal(): void {
    if (this.otpSubmitting()) {
      return;
    }
    this.otpModalOpen.set(false);
    this.otpError.set(null);
    this.clearOtpCountdown();
  }

  async confirmOtpAndRegister(): Promise<void> {
    if (this.otpSubmitting()) {
      return;
    }
    const body = this.pendingRegistrationBody();
    if (!body) {
      this.otpError.set($localize`:@@jobseeker.registration.otpMissingContext:Registration data is missing. Please retry.`);
      return;
    }
    this.otpSubmitting.set(true);
    this.otpError.set(null);
    try {
      const entered = this.otpInput().trim();
      if (!entered) {
        this.otpError.set($localize`:@@jobseeker.registration.otpRequired:Please enter the OTP code.`);
        return;
      }
      let verified;
      try {
        verified = await firstValueFrom(this.reg.verifyOtp(this.otpSessionId(), entered));
      } catch {
        this.otpError.set(
          $localize`:@@jobseeker.registration.otpVerifyFailed:Could not verify OTP. Please try again.`
        );
        return;
      }
      if (!verified.Valid) {
        this.otpError.set($localize`:@@jobseeker.registration.otpInvalid:Invalid OTP code. Please try again.`);
        return;
      }
      const data: RegisterJobseekerResponse = await this.asyncExec.execute(
        () => firstValueFrom(this.reg.registerJobseeker(body)),
        {
          recordSuccess: false,
          progressMessage: $localize`:@@jobseeker.registration.completing:Completing registration…`,
        }
      );
      this.clearOtpCountdown();
      this.otpModalOpen.set(false);
      clearPkiRegistrationContext();
      this.pkiTempToken.set(null);
      this.auth.setLoggedIn(false);
      sessionStorage.setItem(MOCK_USER_ID_STORAGE_KEY, data.MockUserId);
      this.auth.setProfileCompletionPending(true);

      const registeredEmail = body.Email.trim().toLowerCase();
      writeRegistrationContext(registeredEmail, data.MockUserId);
      this.store.dispatch(
        registrationActions.setRegistrationContext({ email: registeredEmail, mockUserId: data.MockUserId })
      );

      void this.router.navigateByUrl('/register/jobseeker/verify-email');
    } catch {
      // Register API errors: ui-async toast only.
    } finally {
      this.otpSubmitting.set(false);
    }
  }

  async resendOtp(): Promise<void> {
    if (this.otpSubmitting()) {
      return;
    }
    const body = this.pendingRegistrationBody();
    if (!body) {
      return;
    }
    try {
      const otp = await this.asyncExec.execute(
        () =>
          firstValueFrom(
            this.reg.requestOtp(body.CivilId, body.MobileNumber ?? '', body.Email)
          ),
        {
          recordSuccess: false,
          progressMessage: $localize`:@@jobseeker.registration.resendingOtp:Resending OTP…`,
        }
      );
      this.otpSessionId.set(otp.SessionId);
      this.otpPreviewCode.set(otp.OtpCode ?? '');
      this.startOtpCountdown(otp.ExpiresInSeconds);
      this.otpError.set(null);
    } catch {
      // Error surfaced via onError + ui-async snapshot.
    }
  }

  private startOtpCountdown(seconds: number): void {
    this.clearOtpCountdown();
    this.otpSecondsLeft.set(seconds);
    this.otpTimerId = setInterval(() => {
      const current = this.otpSecondsLeft();
      if (current <= 1) {
        this.otpSecondsLeft.set(0);
        this.clearOtpCountdown();
        return;
      }
      this.otpSecondsLeft.set(current - 1);
    }, 1000);
  }

  private clearOtpCountdown(): void {
    if (this.otpTimerId) {
      clearInterval(this.otpTimerId);
      this.otpTimerId = null;
    }
  }
}
