/// <reference types="@angular/localize" />

import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import { PlatformAuthSettingsService } from '../../core/api/services/platform-auth-settings.service';
import { buildPkiSsoPath } from '../../core/auth/platform-auth.constants';
import { readPkiRegistrationContext } from '../../core/auth/pki-registration.storage';
import { LocaleService } from '../../core/locale/locale.service';
import { getRuntimeAuthModuleOrigin } from '../../core/runtime-config';

/** Starts TRA PKI SSO for jobseeker registration when `PKI_ENABLED` = Y. */
@Component({
  selector: 'app-jobseeker-register-pki-page',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="panel mx-auto mt-10 max-w-lg p-6">
      <h1 class="text-xl font-semibold" i18n="@@jobseeker.registration.pkiTitle">PKI registration</h1>
      <p class="mt-3 text-white-dark" i18n="@@jobseeker.registration.pkiIntro">
        Continue with your civil ID through the PKI gateway to create your job seeker account.
      </p>
      <div class="mt-6 flex flex-wrap gap-2">
        <a routerLink="/login" class="btn btn-outline-primary btn-sm" i18n="@@jobseeker.registration.backToSignIn">
          Back to Sign In
        </a>
        @if (pkiUrl) {
          <a [href]="pkiUrl" class="btn btn-primary btn-sm" i18n="@@jobseeker.registration.pkiContinue">
            Continue with PKI
          </a>
        }
      </div>
    </div>
  `,
})
export class JobseekerRegisterPkiPageComponent implements OnInit {
  readonly locale = inject(LocaleService);
  private readonly router = inject(Router);
  private readonly authSettings = inject(PlatformAuthSettingsService);

  readonly pkiUrl = (() => {
    const base = getRuntimeAuthModuleOrigin();
    if (!base) {
      return '';
    }
    return `${base.replace(/\/$/, '')}${buildPkiSsoPath('JobSeeker')}`;
  })();

  ngOnInit(): void {
    void firstValueFrom(this.authSettings.load()).then((s) => {
      if (!s.PkiEnabled) {
        void this.router.navigateByUrl('/register/jobseeker', { replaceUrl: true });
        return;
      }
      // Already completed PKI once → finish registration form.
      if (readPkiRegistrationContext()) {
        void this.router.navigateByUrl('/register/jobseeker', { replaceUrl: true });
      }
    });
  }
}
