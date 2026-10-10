import { Component, DestroyRef, OnDestroy, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs/operators';

import { rewritePublicAssetUrls } from './core/public-url';
import { LocaleService } from './core/locale/locale.service';
import { installRowActionsMenuOverlay } from './core/ui/row-actions-menu-overlay';
import { UiAsyncToastHostComponent } from './core/ui/ui-async-toast-host.component';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, UiAsyncToastHostComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnDestroy {
  /** Ensures document `dir`/`lang` sync runs from app bootstrap (see LocaleService). */
  private readonly _locale = inject(LocaleService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly cleanupDevGuards: Array<() => void> = [];
  private readonly disposeRowActionsOverlay = installRowActionsMenuOverlay();

  constructor() {
    this.destroyRef.onDestroy(() => this.disposeRowActionsOverlay());

    this.router.events
      .pipe(
        filter((e): e is NavigationEnd => e instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => {
        rewritePublicAssetUrls();
        for (const delayMs of [0, 50, 200, 500]) {
          globalThis.setTimeout(rewritePublicAssetUrls, delayMs);
        }
      });

    if (!environment.production) {
      this.enableDevInspectGuards();
    }
  }

  ngOnDestroy(): void {
    for (const fn of this.cleanupDevGuards) {
      fn();
    }
  }

  /**
   * Dev-only guard: allow browser inspect/context menu even if another script tries to block it.
   * Uses capture phase to stop anti-inspect handlers from running.
   */
  private enableDevInspectGuards(): void {
    const unblockContextMenu = (event: Event): void => {
      event.stopImmediatePropagation();
    };
    document.addEventListener('contextmenu', unblockContextMenu, true);
    this.cleanupDevGuards.push(() => document.removeEventListener('contextmenu', unblockContextMenu, true));

    const unblockInspectShortcuts = (event: KeyboardEvent): void => {
      const isF12 = event.key === 'F12';
      const isCtrlShiftInspect = (event.ctrlKey || event.metaKey) && event.shiftKey && ['I', 'J', 'C'].includes(event.key.toUpperCase());
      if (isF12 || isCtrlShiftInspect) {
        event.stopImmediatePropagation();
      }
    };
    document.addEventListener('keydown', unblockInspectShortcuts, true);
    this.cleanupDevGuards.push(() => document.removeEventListener('keydown', unblockInspectShortcuts, true));
  }
}
