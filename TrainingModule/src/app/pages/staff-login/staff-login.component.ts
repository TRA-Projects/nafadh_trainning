/// <reference types="@angular/localize" />

import { HttpClient } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { firstValueFrom } from 'rxjs';
import { map } from 'rxjs/operators';

import { ApiPaths } from '../../core/api/api-paths';
import { PlatformAuthSettingsService } from '../../core/api/services/platform-auth-settings.service';
import { navVendorTypeFromProfileType, vendorTypeFromAccessToken } from '../../core/auth/auth-jwt.util';
import { AuthService } from '../../core/auth/auth.service';
import { getAccessToken } from '../../core/auth/auth-token.storage';
import { MOCK_USER_ID_STORAGE_KEY } from '../../core/auth/mock-user.storage';
import { LocaleService } from '../../core/locale/locale.service';
import { resolvePublicUrl } from '../../core/public-url';
import { JOBSEEKER_PROFILE_ACTIONS } from '../../core/profile/jobseeker-profile-actions.token';
import { type ApiData, unwrapData } from '../../core/profile/jobseeker-profile-api';
import { uiAsyncActions } from '../../core/store/ui-async/ui-async.actions';
import { uiAsyncStrings } from '../../core/store/ui-async/ui-async.i18n';
import { AsyncExecutorService } from '../../core/ui/async-executor.service';
import { AssetUrlPipe } from '../../core/ui/asset-url.pipe';
import { isMockLoginEnabled } from '../../core/environment/environment-flags';

interface AuthLoginTokens {
  AccessToken: string;
  RefreshToken: string;
}

interface StaffLoginOtpPayload {
  RequiresOtp?: boolean;
  EncryptedOtp?: string;
  AccessToken?: string;
  RefreshToken?: string;
}

@Component({
  selector: 'app-staff-login',
  standalone: true,
  imports: [AssetUrlPipe, ReactiveFormsModule],
  templateUrl: './staff-login.component.html',
  styleUrl: '../login/login.component.scss',
})
export class StaffLoginComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);
  private readonly http = inject(HttpClient);
  private readonly fb = inject(FormBuilder);
  private readonly profileActions = inject(JOBSEEKER_PROFILE_ACTIONS);
  private readonly authSettings = inject(PlatformAuthSettingsService);
  private readonly asyncExec = inject(AsyncExecutorService);
  private readonly store = inject(Store);

  readonly locale = inject(LocaleService);
  readonly loginPanelBackground = `url('${resolvePublicUrl('/images/auth/login.svg')}')`;
  readonly useMockLogin = isMockLoginEnabled();

  readonly inProgress = signal(false);
  readonly otpStep = signal(false);
  readonly encryptedOtp = signal('');
  readonly pendingEmail = signal('');
  readonly pendingRemember = signal(false);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    rememberme: [false],
  });

  readonly otpForm = this.fb.nonNullable.group({
    otp: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(10)]],
  });

  ngOnInit(): void {
    void firstValueFrom(this.authSettings.load()).catch(() => undefined);
    sessionStorage.removeItem(MOCK_USER_ID_STORAGE_KEY);
    if (getAccessToken()) {
      this.auth.restoreSessionFromTokens();
      if (this.auth.isLoggedIn() && !this.auth.isJobSeekerNav()) {
        void this.router.navigateByUrl(this.auth.homePath());
      }
    }
  }

  async onSubmit(event: Event): Promise<void> {
    event.preventDefault();
    if (this.useMockLogin) {
      this.notifyError(
        $localize`:@@jobseeker.staffLogin.error.mock:Staff login is not available in mock mode. Use the public login with admin@mock.local.`
      );
      return;
    }

    const raw = this.form.getRawValue();
    const email = raw.email.trim();
    const password = raw.password;
    if (!email || !password) {
      this.notifyError(
        $localize`:@@jobseeker.login.error.credentials:Enter a valid email and a password of at least 8 characters.`
      );
      return;
    }

    this.inProgress.set(true);
    try {
      await this.asyncExec.execute(
        async () => {
          const result = await firstValueFrom(
            this.http
              .post<ApiData<StaffLoginOtpPayload | AuthLoginTokens> | StaffLoginOtpPayload | AuthLoginTokens>(
                ApiPaths.authStaffLogin,
                { email, password, rememberme: raw.rememberme, staffOnly: true }
              )
              .pipe(map(unwrapData))
          );

          const otpPayload = result as StaffLoginOtpPayload;
          if (otpPayload.RequiresOtp || (otpPayload.EncryptedOtp && !otpPayload.AccessToken)) {
            this.encryptedOtp.set((otpPayload.EncryptedOtp ?? '').trim());
            this.pendingEmail.set(email);
            this.pendingRemember.set(raw.rememberme);
            this.otpStep.set(true);
            this.otpForm.reset({ otp: '' });
            return result;
          }

          const tokens = result as AuthLoginTokens;
          await this.finishWithTokens(tokens, email, raw.rememberme);
          return result;
        },
        {
          recordSuccess: false,
          progressMessage: $localize`:@@jobseeker.login.signingIn:Signing in…`,
        }
      );
    } catch {
      // ui-async toast
    } finally {
      this.inProgress.set(false);
    }
  }

  async onVerifyOtp(event: Event): Promise<void> {
    event.preventDefault();
    const otp = this.otpForm.controls.otp.value.trim();
    const email = this.pendingEmail();
    const encryptedOtp = this.encryptedOtp();
    if (!otp || !email || !encryptedOtp) {
      this.notifyError(
        $localize`:@@jobseeker.staffLogin.error.otpRequired:Enter the verification code sent to your mobile.`
      );
      return;
    }

    this.inProgress.set(true);
    try {
      await this.asyncExec.execute(
        async () => {
          const tokens = await firstValueFrom(
            this.http
              .post<ApiData<AuthLoginTokens> | AuthLoginTokens>(ApiPaths.authVerifyLogin, {
                email,
                otp,
                encryptedOtp,
                staffOnly: true,
              })
              .pipe(map(unwrapData))
          );
          await this.finishWithTokens(tokens, email, this.pendingRemember());
          return tokens;
        },
        {
          recordSuccess: false,
          progressMessage: $localize`:@@jobseeker.staffLogin.verifyingOtp:Verifying code…`,
        }
      );
    } catch {
      // ui-async toast
    } finally {
      this.inProgress.set(false);
    }
  }

  backToPassword(): void {
    this.otpStep.set(false);
    this.encryptedOtp.set('');
    this.otpForm.reset({ otp: '' });
  }

  private async finishWithTokens(tokens: AuthLoginTokens, email: string, rememberme: boolean): Promise<void> {
    if (!tokens?.AccessToken) {
      throw new Error('Missing tokens');
    }
    this.auth.persistTokens(tokens.AccessToken, tokens.RefreshToken, rememberme);
    sessionStorage.removeItem(MOCK_USER_ID_STORAGE_KEY);

    let vt = navVendorTypeFromProfileType(vendorTypeFromAccessToken(tokens.AccessToken));
    try {
      const profile = await firstValueFrom(this.profileActions.getMe());
      vt = navVendorTypeFromProfileType(profile.VendorType?.Id) ?? vt;
    } catch {
      // JWT vendor type
    }
    if (vt == null || vt === '1') {
      this.auth.setLoggedIn(false);
      this.notifyError(
        $localize`:@@jobseeker.staffLogin.error.notStaff:This account cannot use staff sign-in.`
      );
      return;
    }
    this.auth.setLoggedIn(true, { vendorType: vt });
    void this.router.navigateByUrl(this.auth.homePath());
  }

  private notifyError(message: string): void {
    const fallback = message.trim() || uiAsyncStrings(this.locale.locale()).genericError;
    this.store.dispatch(uiAsyncActions.recordError({ error: message, fallbackMessage: fallback }));
  }
}
