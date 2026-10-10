/// <reference types="@angular/localize" />

import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { firstValueFrom } from 'rxjs';
import { map } from 'rxjs/operators';

import { JobseekerRegistrationHttpService } from '../../core/api/services/jobseeker-registration-http.service';
import { PlatformAuthSettingsService } from '../../core/api/services/platform-auth-settings.service';
import { AuthService } from '../../core/auth/auth.service';
import {
  COMPLETE_PROFILE_PATH,
  isJobSeekerProfileIncomplete,
} from '../../core/auth/post-login-destination.util';
import { writeRegistrationEmailVerified } from '../../core/auth/registration.storage';
import { LocaleService } from '../../core/locale/locale.service';
import { JOBSEEKER_PROFILE_ACTIONS } from '../../core/profile/jobseeker-profile-actions.token';
import { registrationActions } from '../../core/store/registration/registration.actions';
import { registrationFeature } from '../../core/store/registration/registration.reducer';
import { AsyncExecutorService } from '../../core/ui/async-executor.service';
import { AssetUrlPipe } from '../../core/ui/asset-url.pipe';

/**
 * Post-registration / post-login email step. When `REGISTRATION_EMAIL_ENABLED` = Y, the user verifies via email link
 * (no manual button). When N, the user confirms via `POST register-jobseeker-verify-email`.
 */
@Component({
  selector: 'app-jobseeker-email-verification-page',
  standalone: true,
  imports: [AssetUrlPipe],
  templateUrl: './jobseeker-email-verification-page.component.html',
  styleUrl: '../login/login.component.scss',
})
export class JobseekerEmailVerificationPageComponent {
  readonly locale = inject(LocaleService);
  private readonly router = inject(Router);
  private readonly store = inject(Store);
  private readonly auth = inject(AuthService);
  private readonly reg = inject(JobseekerRegistrationHttpService);
  private readonly authSettings = inject(PlatformAuthSettingsService);
  private readonly asyncExec = inject(AsyncExecutorService);
  private readonly profileActions = inject(JOBSEEKER_PROFILE_ACTIONS);

  readonly email = toSignal(this.store.select(registrationFeature.selectEmail).pipe(map((e) => e ?? '')), {
    initialValue: '',
  });
  readonly emailVerified = toSignal(this.store.select(registrationFeature.selectEmailVerified), {
    initialValue: false,
  });

  readonly verifying = signal(false);

  readonly emailDisplay = computed(() => this.email() || '—');
  /** Manual verify button — only when `REGISTRATION_EMAIL_ENABLED` = N. */
  readonly showManualEmailVerify = computed(() => !this.authSettings.registrationEmailEnabled());

  constructor() {
    void firstValueFrom(this.authSettings.load()).catch(() => undefined);
  }

  goToLogin(): void {
    // Login after unverified sign-in still holds JWTs; clear them so loginGuard
    // does not redirect straight back to this page.
    this.auth.setLoggedIn(false);
    void this.router.navigateByUrl('/login');
  }

  async proceedAfterVerified(): Promise<void> {
    this.auth.setEmailVerificationPending(false);
    if (!this.auth.isLoggedIn()) {
      this.goToLogin();
      return;
    }
    try {
      const profile = await firstValueFrom(this.profileActions.getMe());
      const incomplete = isJobSeekerProfileIncomplete(profile);
      this.auth.setProfileCompletionPending(incomplete);
      void this.router.navigateByUrl(incomplete ? COMPLETE_PROFILE_PATH : this.auth.homePath());
    } catch {
      void this.router.navigateByUrl(this.auth.homePath());
    }
  }

  proceed(): void {
    void this.proceedAfterVerified();
  }

  async verifyEmail(): Promise<void> {
    if (this.verifying()) {
      return;
    }
    if (!this.showManualEmailVerify()) {
      // Link-based verification: stay here until mailbox link confirms; Continueto login if not signed in.
      if (this.auth.isLoggedIn() && this.auth.emailVerificationPending()) {
        return;
      }
      this.goToLogin();
      return;
    }
    if (this.emailVerified()) {
      this.proceed();
      return;
    }

    const email = this.email().trim();
    if (!email) {
      return;
    }

    this.verifying.set(true);
    try {
      await this.asyncExec.execute(() => firstValueFrom(this.reg.verifyJobseekerEmail(email)), {
        recordSuccess: true,
        progressMessage: $localize`:@@jobseeker.registration.verifyingEmail:Verifying email…`,
        successMessage: $localize`:@@jobseeker.registration.verifyEmailSuccess:Email verified successfully.`,
      });
      const verifiedAtIso = new Date().toISOString();
      writeRegistrationEmailVerified(verifiedAtIso);
      this.store.dispatch(registrationActions.markEmailVerified({ verifiedAtIso }));
      await this.proceedAfterVerified();
    } catch {
      // Error surfaced via ui-async toast.
    } finally {
      this.verifying.set(false);
    }
  }
}
