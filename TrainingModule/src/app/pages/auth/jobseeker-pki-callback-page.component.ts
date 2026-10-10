/// <reference types="@angular/localize" />

import { Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { firstValueFrom } from 'rxjs';

import { AuthService } from '../../core/auth/auth.service';
import { navVendorTypeFromProfileType, vendorTypeFromAccessToken } from '../../core/auth/auth-jwt.util';
import { MOCK_USER_ID_STORAGE_KEY } from '../../core/auth/mock-user.storage';
import {
  isJobSeekerEmailUnverified,
  isJobSeekerProfileIncomplete,
} from '../../core/auth/post-login-destination.util';
import { writePkiRegistrationContext } from '../../core/auth/pki-registration.storage';
import { writeRegistrationContext } from '../../core/auth/registration.storage';
import { LocaleService } from '../../core/locale/locale.service';
import { JOBSEEKER_PROFILE_ACTIONS } from '../../core/profile/jobseeker-profile-actions.token';
import { uiAsyncActions } from '../../core/store/ui-async/ui-async.actions';
import { uiAsyncStrings } from '../../core/store/ui-async/ui-async.i18n';
import { AsyncExecutorService } from '../../core/ui/async-executor.service';

const VERIFY_EMAIL_PATH = '/register/jobseeker/verify-email';
const COMPLETE_PROFILE_PATH = '/complete-profile';
const REGISTER_PATH = '/register/jobseeker';

/**
 * Receives PKI SSO handoff from AuthModule `pkicallbackJobSeeker`
 * (`/auth/pki-callback#accessToken=…` or `#tempToken=…`).
 */
@Component({
  selector: 'app-jobseeker-pki-callback-page',
  standalone: true,
  template: `
    <div class="flex min-h-screen items-center justify-center p-6">
      <p class="text-sm text-white-dark">
        @if (error()) {
          {{ error() }}
        } @else {
          <span i18n="@@jobseeker.pki.callback.working">Completing PKI sign-in…</span>
        }
      </p>
    </div>
  `,
})
export class JobseekerPkiCallbackPageComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);
  private readonly profileActions = inject(JOBSEEKER_PROFILE_ACTIONS);
  private readonly asyncExec = inject(AsyncExecutorService);
  private readonly store = inject(Store);
  private readonly locale = inject(LocaleService);

  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    void this.completeHandoff();
  }

  private async completeHandoff(): Promise<void> {
    const params = this.readHashParams();
    // Drop secrets from the address bar ASAP.
    if (typeof history !== 'undefined' && history.replaceState) {
      history.replaceState(null, '', '/auth/pki-callback');
    }

    const queryError = params.get('error')?.trim();
    if (queryError) {
      this.error.set(queryError);
      this.notifyError(queryError);
      void this.router.navigateByUrl('/login');
      return;
    }

    const existed = (params.get('isUserExisted') ?? '').toLowerCase() === 'true';
    if (existed) {
      const accessToken = (params.get('accessToken') ?? '').trim();
      const refreshToken = (params.get('refreshToken') ?? '').trim();
      if (!accessToken || !refreshToken) {
        this.fail($localize`:@@jobseeker.pki.callback.missingTokens:PKI sign-in did not return tokens.`);
        return;
      }
      await this.finishExistingUser(accessToken, refreshToken);
      return;
    }

    const tempToken = (params.get('tempToken') ?? '').trim();
    const civilId = (params.get('civilId') ?? '').trim();
    const mobile = (params.get('mobile') ?? '').trim();
    if (!tempToken || !civilId) {
      this.fail($localize`:@@jobseeker.pki.callback.missingTemp:PKI registration data is missing. Try PKI sign-in again.`);
      return;
    }

    writePkiRegistrationContext({
      tempToken,
      civilId,
      mobile,
    });
    void this.router.navigateByUrl(REGISTER_PATH);
  }

  private async finishExistingUser(accessToken: string, refreshToken: string): Promise<void> {
    try {
      await this.asyncExec.execute(
        async () => {
          this.auth.persistTokens(accessToken, refreshToken, true);
          sessionStorage.removeItem(MOCK_USER_ID_STORAGE_KEY);

          let vt = navVendorTypeFromProfileType(vendorTypeFromAccessToken(accessToken));
          let emailUnverified = false;
          let incomplete = false;
          let email = '';
          try {
            const profile = await firstValueFrom(this.profileActions.getMe());
            vt = navVendorTypeFromProfileType(profile.VendorType?.Id) ?? vt;
            emailUnverified = isJobSeekerEmailUnverified(profile);
            incomplete = !emailUnverified && isJobSeekerProfileIncomplete(profile);
            email = (profile.EmailAddress ?? '').trim();
          } catch {
            // JWT vendor type fallback.
          }
          if (vt == null) {
            vt = '1';
          }
          this.auth.setEmailVerificationPending(emailUnverified);
          this.auth.setProfileCompletionPending(incomplete);
          this.auth.setLoggedIn(true, { vendorType: vt });

          if (emailUnverified) {
            const normalized = email.toLowerCase();
            writeRegistrationContext(normalized, null);
            void this.router.navigateByUrl(VERIFY_EMAIL_PATH);
            return;
          }
          void this.router.navigateByUrl(incomplete ? COMPLETE_PROFILE_PATH : this.auth.homePath());
        },
        {
          recordSuccess: false,
          progressMessage: $localize`:@@jobseeker.pki.callback.signingIn:Signing in with PKI…`,
        }
      );
    } catch {
      this.fail($localize`:@@jobseeker.pki.callback.failed:Could not complete PKI sign-in.`);
    }
  }

  private fail(message: string): void {
    this.error.set(message);
    this.notifyError(message);
    void this.router.navigateByUrl('/login');
  }

  private notifyError(message: string): void {
    const fallback = message.trim() || uiAsyncStrings(this.locale.locale()).genericError;
    this.store.dispatch(uiAsyncActions.recordError({ error: message, fallbackMessage: fallback }));
  }

  private readHashParams(): URLSearchParams {
    const hash = (typeof location !== 'undefined' ? location.hash : '').replace(/^#/, '');
    if (hash) {
      return new URLSearchParams(hash);
    }
    // Fallback: query string (e.g. AuthModule error redirects).
    const search = typeof location !== 'undefined' ? location.search : '';
    return new URLSearchParams(search.startsWith('?') ? search.slice(1) : search);
  }
}
