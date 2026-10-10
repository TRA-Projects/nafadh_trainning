/// <reference types="@angular/localize" />

import { CommonModule } from '@angular/common';
import { Component, DestroyRef, ElementRef, OnInit, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { firstValueFrom } from 'rxjs';

import { AdminSystemApiService, type NotificationTemplateListItemDto } from '../../../core/api/services/admin-system-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { adminSystemActions } from '../../../core/store/admin-system/admin-system.actions';
import { adminSystemFeature } from '../../../core/store/admin-system/admin-system.reducer';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { AssetUrlPipe } from '../../../core/ui/asset-url.pipe';

const PAGE_SIZE_OPTIONS = [5, 10, 15] as const;
type PageSize = (typeof PAGE_SIZE_OPTIONS)[number];

@Component({
  selector: 'app-notification-templates-list-page',
  standalone: true,
  imports: [CommonModule, AssetUrlPipe],
  templateUrl: './notification-templates-list-page.component.html',
  styleUrl: './notification-templates-list-page.component.scss',
})
export class NotificationTemplatesListPageComponent implements OnInit {
  private readonly api = inject(AdminSystemApiService);
  private readonly store = inject(Store);
  private readonly router = inject(Router);
  private readonly asyncExec = inject(AsyncExecutorService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly host = inject(ElementRef<HTMLElement>);
  readonly locale = inject(LocaleService);

  readonly searchQuery = signal('');
  readonly pageSize = signal<PageSize>(5);
  readonly pageIndex = signal(0);
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  readonly actionsMenuRowId = signal<string | null>(null);

  readonly rowActionsMenuAria = $localize`:@@jobseeker.admin.notificationTemplates.rowActionsMenu:Row actions`;

  private readonly pageCache = toSignal(this.store.select(adminSystemFeature.selectNotificationTemplatesPage), {
    initialValue: null,
  });

  readonly rows = computed(() => this.pageCache()?.rows ?? []);
  readonly totalCount = computed(() => this.pageCache()?.total ?? 0);
  readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));
  readonly safePageIndex = computed(() => this.pageCache()?.pageIndex ?? this.pageIndex());
  readonly canPrev = computed(() => this.safePageIndex() > 0);
  readonly canNext = computed(() => this.safePageIndex() < this.totalPages() - 1);

  readonly pageFooterLabel = computed(() => {
    const total = this.totalCount();
    if (total === 0) {
      return $localize`:@@jobseeker.jobsList.pagerEmpty:Showing 0 of 0`;
    }
    const start = this.safePageIndex() * this.pageSize() + 1;
    const end = Math.min(total, (this.safePageIndex() + 1) * this.pageSize());
    return `${$localize`:@@jobseeker.common.showing:Showing`} ${start}-${end} ${$localize`:@@jobseeker.common.of:of`} ${total}`;
  });

  readonly yesLabel = $localize`:@@jobseeker.common.yes:Yes`;
  readonly noLabel = $localize`:@@jobseeker.common.no:No`;

  constructor() {
    const onDocClick = (event: Event) => {
      if (this.actionsMenuRowId() == null) {
        return;
      }
      const target = event.target as Node | null;
      if (!target || !this.host.nativeElement.contains(target)) {
        this.actionsMenuRowId.set(null);
        return;
      }
      const el = target as HTMLElement;
      if (!el.closest?.('[data-row-actions-menu]')) {
        this.actionsMenuRowId.set(null);
      }
    };
    document.addEventListener('click', onDocClick, true);
    this.destroyRef.onDestroy(() => document.removeEventListener('click', onDocClick, true));
  }

  ngOnInit(): void {
    void this.reload();
  }

  labelLocalized(row: NotificationTemplateListItemDto): string {
    return this.locale.locale() === 'ar' ? row.LabelAr : row.LabelEn;
  }

  toggleRowActionsMenu(actionCode: string, event: Event): void {
    event.stopPropagation();
    this.actionsMenuRowId.update((current) => (current === actionCode ? null : actionCode));
  }

  closeRowActionsMenu(): void {
    this.actionsMenuRowId.set(null);
  }

  openEdit(row: NotificationTemplateListItemDto): void {
    void this.router.navigate(['/admin/notification-templates', row.ActionCode]);
  }

  setSearchQuery(value: string): void {
    this.searchQuery.set((value ?? '').toString());
    this.pageIndex.set(0);
    void this.reload();
  }

  setPageSize(size: number): void {
    const next = PAGE_SIZE_OPTIONS.includes(size as PageSize) ? (size as PageSize) : 5;
    this.pageSize.set(next);
    this.pageIndex.set(0);
    void this.reload();
  }

  prevPage(): void {
    if (this.canPrev()) {
      this.pageIndex.update((p) => p - 1);
      void this.reload();
    }
  }

  nextPage(): void {
    if (this.canNext()) {
      this.pageIndex.update((p) => p + 1);
      void this.reload();
    }
  }

  private async reload(): Promise<void> {
    try {
      await this.asyncExec.execute(
        () =>
          firstValueFrom(
            this.api.getNotificationTemplatesPage({
              pageIndex: this.pageIndex(),
              pageSize: this.pageSize(),
              q: this.searchQuery(),
            })
          ),
        {
          recordSuccess: false,
          progressMessage: $localize`:@@jobseeker.admin.notificationTemplates.loadingProgress:Loading notification templates…`,
        }
      );
    } catch {
      // Drop stale NgRx rows so a dead API does not keep showing the last successful page.
      this.store.dispatch(
        adminSystemActions.cacheNotificationTemplatesPage({
          page: { rows: [], total: 0, pageIndex: 0, pageSize: this.pageSize() },
        })
      );
    }
  }
}
