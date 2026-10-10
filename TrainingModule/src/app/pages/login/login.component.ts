/// <reference types="@angular/localize" />

import { HttpClient } from '@angular/common/http';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { firstValueFrom } from 'rxjs';
import { map } from 'rxjs/operators';

import { ApiPaths } from '../../core/api/api-paths';
import { PlatformAuthSettingsService } from '../../core/api/services/platform-auth-settings.service';
import { navVendorTypeFromProfileType, vendorTypeFromAccessToken } from '../../core/auth/auth-jwt.util';
import { AuthService } from '../../core/auth/auth.service';
import { buildPkiSsoPath } from '../../core/auth/platform-auth.constants';
import type { MockUserRow } from '../../core/auth/mock-user.types';
import { MOCK_USER_ID_STORAGE_KEY } from '../../core/auth/mock-user.storage';
import { getAccessToken } from '../../core/auth/auth-token.storage';
import {
  COMPLETE_PROFILE_PATH,
  VERIFY_EMAIL_PATH,
  isJobSeekerEmailUnverified,
  isJobSeekerProfileIncomplete,
} from '../../core/auth/post-login-destination.util';
import { writeRegistrationContext } from '../../core/auth/registration.storage';
import { LocaleService } from '../../core/locale/locale.service';
import { resolveMockLoginEmail } from '../../core/mocks/mock-users.state';
import { JOBSEEKER_PROFILE_ACTIONS } from '../../core/profile/jobseeker-profile-actions.token';
import { type ApiData, unwrapData } from '../../core/profile/jobseeker-profile-api';
import { registrationActions } from '../../core/store/registration/registration.actions';
import { uiAsyncActions } from '../../core/store/ui-async/ui-async.actions';
import { uiAsyncStrings } from '../../core/store/ui-async/ui-async.i18n';
import { ModalCloseButtonComponent } from '../../core/ui/modal-close-button/modal-close-button.component';
import { AsyncExecutorService } from '../../core/ui/async-executor.service';
import { isMockLoginEnabled } from '../../core/environment/environment-flags';
import { getRuntimeAuthModuleOrigin, getRuntimeFreelancerMvcOrigin } from '../../core/runtime-config';

/** Matches API `Data` payload for `POST /auth/login` (Freelancer `LoginResponseDto` fields). */
interface AuthLoginTokens {
  AccessToken: string;
  RefreshToken: string;
}

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, ModalCloseButtonComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);
  private readonly http = inject(HttpClient);
  private readonly fb = inject(FormBuilder);
  private readonly profileActions = inject(JOBSEEKER_PROFILE_ACTIONS);
  private readonly authSettings = inject(PlatformAuthSettingsService);
  private readonly asyncExec = inject(AsyncExecutorService);
  private readonly store = inject(Store);

  readonly locale = inject(LocaleService);
  readonly useMockLogin = isMockLoginEnabled();

  /** Email/password when mock login or PKI is off (`PKI_ENABLED` = N). */
  readonly showNormalLogin = computed(() => this.useMockLogin || !this.authSettings.pkiEnabled());
  /** PKI SSO when `PKI_ENABLED` = Y (real API only). */
  readonly showPkiLogin = computed(() => !this.useMockLogin && this.authSettings.pkiEnabled());

  readonly mockUsers = signal<MockUserRow[]>([]);

  readonly inProgress = signal(false);
  readonly registerOpen = signal(false);
  readonly registerType = signal('');

  readonly freelancerMvcOrigin = getRuntimeFreelancerMvcOrigin();

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: [''],
    rememberme: [false],
  });

  ngOnInit(): void {
    void firstValueFrom(this.authSettings.load()).catch(() => undefined);
    if (this.useMockLogin) {
      this.http.get<{ users: MockUserRow[] }>(ApiPaths.mockUsers).subscribe({
        next: (res) => this.mockUsers.set(res.users),
      });
      return;
    }
    sessionStorage.removeItem(MOCK_USER_ID_STORAGE_KEY);
    if (getAccessToken()) {
      this.auth.restoreSessionFromTokens();
      if (this.auth.isLoggedIn()) {
        void this.redirectAfterAuthenticatedSession();
      }
    }
  }

  closeRegister(): void {
    this.registerOpen.set(false);
  }

  continueRegister(): void {
    const t = this.registerType();
    if (!t) {
      return;
    }
    if (t === 'RegisterJobseeker') {
      this.registerOpen.set(false);
      if (this.authSettings.pkiEnabled() && !this.useMockLogin) {
        void this.router.navigateByUrl('/register/jobseeker/pki');
      } else {
        void this.router.navigateByUrl('/register/jobseeker');
      }
      return;
    }
    const base = this.freelancerMvcOrigin.trim();
    if (!base) {
      this.registerOpen.set(false);
      return;
    }
    window.location.href = `${base.replace(/\/$/, '')}/Account/${t}`;
    this.registerOpen.set(false);
  }

  continueWithPki(): void {
    const base = getRuntimeAuthModuleOrigin().trim();
    if (!base) {
      this.notifyError(
        $localize`:@@jobseeker.login.error.authModuleOrigin:PKI gateway origin is not configured (authModuleOrigin).`
      );
      return;
    }
    window.location.href = `${base.replace(/\/$/, '')}${buildPkiSsoPath('JobSeeker')}`;
  }

  /** Surfaces inline copy through the global Vristo mixin toast (`app-ui-async-toast-host`). */
  private notifyError(message: string): void {
    const fallback = message.trim() || uiAsyncStrings(this.locale.locale()).genericError;
    this.store.dispatch(uiAsyncActions.recordError({ error: message, fallbackMessage: fallback }));
  }

  private redirectUnverifiedEmail(email: string): void {
    const normalized = email.trim().toLowerCase();
    writeRegistrationContext(normalized, null);
    this.store.dispatch(registrationActions.setRegistrationContext({ email: normalized, mockUserId: null }));
    this.auth.setEmailVerificationPending(true);
    void this.router.navigateByUrl(VERIFY_EMAIL_PATH);
  }

  private async redirectAfterAuthenticatedSession(): Promise<void> {
    if (this.auth.isJobSeekerNav() && this.auth.emailVerificationPending()) {
      void this.router.navigateByUrl(VERIFY_EMAIL_PATH);
      return;
    }
    if (this.auth.isJobSeekerNav() && this.auth.profileCompletionPending()) {
      void this.router.navigateByUrl(COMPLETE_PROFILE_PATH);
      return;
    }
    if (!this.auth.isJobSeekerNav()) {
      void this.router.navigateByUrl(this.auth.homePath());
      return;
    }
    try {
      const profile = await firstValueFrom(this.profileActions.getMe());
      if (isJobSeekerEmailUnverified(profile)) {
        this.redirectUnverifiedEmail(profile.EmailAddress || this.form.controls.email.value);
        return;
      }
      this.auth.setEmailVerificationPending(false);
      const incomplete = isJobSeekerProfileIncomplete(profile);
      this.auth.setProfileCompletionPending(incomplete);
      void this.router.navigateByUrl(incomplete ? COMPLETE_PROFILE_PATH : this.auth.homePath());
    } catch {
      void this.router.navigateByUrl(this.auth.homePath());
    }
  }

  async onSubmit(event: Event): Promise<void> {
    event.preventDefault();

    if (this.useMockLogin) {
      const users = this.mockUsers();
      if (users.length === 0) {
        return;
      }
      const emailVal = resolveMockLoginEmail(this.form.controls.email.value);
      const u = users.find((x) => x.email.toLowerCase() === emailVal);
      if (u) {
        this.auth.setLoggedIn(true, {
          vendorType: String(u.vendorTypeId),
          mockUserId: u.id,
        });
        this.auth.setProfileCompletionPending(u.profileCompletionPending === true);
        const dest =
          this.auth.isJobSeekerNav() && this.auth.profileCompletionPending()
            ? COMPLETE_PROFILE_PATH
            : this.auth.homePath();
        void this.router.navigateByUrl(dest);
        return;
      }
      this.notifyError(
        $localize`:@@jobseeker.login.error.mockEmail:Email is not in mock users. Use a demo seeker like aisha@mock.local, or jobseeker@mock.local, operator@mock.local, or admin@mock.local.`
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
          const body = {
            email,
            password,
            rememberme: raw.rememberme,
          };
          const tokens = await firstValueFrom(
            this.http
              .post<ApiData<AuthLoginTokens> | AuthLoginTokens>(ApiPaths.authLogin, body)
              .pipe(map(unwrapData))
          );
          this.auth.persistTokens(tokens.AccessToken, tokens.RefreshToken, raw.rememberme);
          sessionStorage.removeItem(MOCK_USER_ID_STORAGE_KEY);

          let vt = navVendorTypeFromProfileType(vendorTypeFromAccessToken(tokens.AccessToken));
          let emailUnverified = false;
          let incomplete = false;
          try {
            const profile = await firstValueFrom(this.profileActions.getMe());
            vt = navVendorTypeFromProfileType(profile.VendorType?.Id) ?? vt;
            emailUnverified = isJobSeekerEmailUnverified(profile);
            incomplete = !emailUnverified && isJobSeekerProfileIncomplete(profile);
          } catch {
            // Fall back to JWT vendor type when profile fetch fails.
          }
          if (vt == null) {
            vt = '1';
          }
          this.auth.setEmailVerificationPending(emailUnverified);
          this.auth.setProfileCompletionPending(incomplete);
          this.auth.setLoggedIn(true, { vendorType: vt });
          if (emailUnverified) {
            this.redirectUnverifiedEmail(email);
          } else {
            void this.router.navigateByUrl(incomplete ? COMPLETE_PROFILE_PATH : this.auth.homePath());
          }
          return tokens;
        },
        {
          recordSuccess: false,
          progressMessage: $localize`:@@jobseeker.login.signingIn:Signing in…`,
        }
      );
    } catch {
      // API errors: ui-async mixin toast only.
    } finally {
      this.inProgress.set(false);
    }
  }

  onRegisterTypeChange(event: Event): void {
    this.registerType.set((event.target as HTMLSelectElement).value);
  }
}
