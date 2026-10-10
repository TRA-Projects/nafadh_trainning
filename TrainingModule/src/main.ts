/// <reference types="@angular/localize" />

import { registerLocaleData } from '@angular/common';
import localeAr from '@angular/common/locales/ar';
import { loadTranslations } from '@angular/localize';
import { bootstrapApplication } from '@angular/platform-browser';

import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { readPersistedLocale } from './app/core/locale/locale-persistence';
import { mergeArTranslations } from './app/core/translations/merge-ar-translations';
import { installPublicAssetUrlRewriter } from './app/core/public-url';
import { loadRuntimeConfig } from './app/core/runtime-config';

/** Side effect: keeps validation `$localize` catalog in the bundle for `ng extract-i18n`. */
import './app/core/validators/validation-messages.i18n';

async function main(): Promise<void> {
  await loadRuntimeConfig();

  if (readPersistedLocale() === 'ar') {
    registerLocaleData(localeAr);
    loadTranslations(mergeArTranslations());
  }

  await bootstrapApplication(AppComponent, appConfig);
  installPublicAssetUrlRewriter();
}

main().catch((err) => console.error(err));
