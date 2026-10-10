/// <reference types="@angular/localize" />

import { NgTemplateOutlet } from '@angular/common';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

import {
  AdminSystemApiService,
  type NotificationTransactionLogRowDto,
} from '../../../core/api/services/admin-system-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { AssetUrlPipe } from '../../../core/ui/asset-url.pipe';
import { ModalCloseButtonComponent } from '../../../core/ui/modal-close-button/modal-close-button.component';

const PAGE_SIZE_OPTIONS = [5, 10, 15] as const;
type PageSize = (typeof PAGE_SIZE_OPTIONS)[number];

@Component({
  selector: 'app-notification-transactions-page',
  standalone: true,
  imports: [NgTemplateOutlet, FormsModule, AssetUrlPipe, ModalCloseButtonComponent],
  templateUrl: './notification-transactions-page.component.html',
  styleUrl: './notification-transactions-page.component.scss',
})
export class NotificationTransactionsPageComponent implements OnInit {
  private readonly api = inject(AdminSystemApiService);
  private readonly asyncExec = inject(AsyncExecutorService);
  private readonly destroyRef = inject(DestroyRef);
  readonly locale = inject(LocaleService);

  readonly pageSize = signal<PageSize>(5);
  readonly pageIndex = signal(0);
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  readonly drawerOpen = signal(false);
  readonly actionsMenuRowId = signal<number | null>(null);

  readonly filterChannel = signal('');
  readonly filterReceiver = signal('');
  readonly filterAction = signal('');
  readonly filterSuccess = signal<'' | 'true' | 'false'>('');
  readonly filterCreatedFrom = signal('');
  readonly filterCreatedTo = signal('');

  readonly searchLabel = $localize`:@@jobseeker.jobsFind.search:Search`;
  readonly clearFiltersAria = $localize`:@@jobseeker.jobSeekerSearch.clearFilters:Clear filters`;
  readonly advancedFiltersLabel = $localize`:@@jobseeker.jobSeekersList.tabAdvanced:Advanced filter`;
  readonly rowActionsMenuAria = $localize`:@@jobseeker.admin.systemConfig.rowActionsMenu:Row actions`;
  readonly viewActionLabel = $localize`:@@jobseeker.common.view:View`;
  readonly resendActionLabel = $localize`:@@jobseeker.admin.notificationTransactions.resend:Resend`;
  readonly failureDetailsLabel = $localize`:@@jobseeker.admin.notificationTransactions.failureDetails:Failure details`;

  readonly hasActiveFilters = computed(
    () =>
      !!this.filterChannel() ||
      !!this.filterReceiver().trim() ||
      !!this.filterAction().trim() ||
      !!this.filterSuccess() ||
      !!this.filterCreatedFrom().trim() ||
      !!this.filterCreatedTo().trim()
  );

  readonly advancedFilterCount = computed(() => {
    let n = 0;
    if (this.filterChannel()) n += 1;
    if (this.filterReceiver().trim()) n += 1;
    if (this.filterAction().trim()) n += 1;
    if (this.filterSuccess()) n += 1;
    if (this.filterCreatedFrom().trim()) n += 1;
    if (this.filterCreatedTo().trim()) n += 1;
    return n;
  });

  readonly viewOpen = signal(false);
  readonly viewing = signal<NotificationTransactionLogRowDto | null>(null);
  readonly failureReasonOpen = signal(false);
  readonly failureReasonText = signal('');

  readonly rows = signal<NotificationTransactionLogRowDto[]>([]);
  readonly totalCount = signal(0);
  readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));
  readonly safePageIndex = computed(() => Math.min(this.pageIndex(), Math.max(0, this.totalPages() - 1)));
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

  readonly viewIsEmail = computed(() => {
    const row = this.viewing();
    return !!row && (row.ChannelType ?? '').toLowerCase() === 'email';
  });

  readonly viewEmailSrcdoc = computed(() => this.viewing()?.MessageBody ?? '');

  readonly successLabel = $localize`:@@jobseeker.admin.notificationTransactions.statusSuccess:Success`;
  readonly failedLabel = $localize`:@@jobseeker.admin.notificationTransactions.statusFailed:Failed`;

  constructor() {
    const onDocClick = (event: MouseEvent) => {
      if (this.actionsMenuRowId() == null) return;
      const el = event.target as HTMLElement | null;
      if (!el?.closest?.('[data-row-actions-menu]')) {
        this.actionsMenuRowId.set(null);
      }
    };
    document.addEventListener('click', onDocClick, true);
    this.destroyRef.onDestroy(() => document.removeEventListener('click', onDocClick, true));
  }

  ngOnInit(): void {
    void this.reload();
  }

  actionLabel(row: NotificationTransactionLogRowDto): string {
    const ar = row.ActionLabelAr?.trim();
    const en = row.ActionLabelEn?.trim();
    const picked = this.locale.locale() === 'ar' ? ar || en : en || ar;
    return picked || row.ActionCode?.trim() || '—';
  }

  openDrawer(): void {
    this.drawerOpen.set(true);
  }

  closeDrawer(): void {
    this.drawerOpen.set(false);
  }

  applyDrawer(): void {
    this.drawerOpen.set(false);
    this.search();
  }

  clearDrawerFilters(): void {
    this.resetFilterValues();
  }

  search(): void {
    this.pageIndex.set(0);
    void this.reload();
  }

  clearFilters(): void {
    this.resetFilterValues();
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

  formatDate(iso: string | null | undefined): string {
    if (!iso) return '—';
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return iso;
    return d.toLocaleString();
  }

  statusLabel(row: NotificationTransactionLogRowDto): string {
    return row.IsSuccess ? this.successLabel : this.failedLabel;
  }

  hasFailureReason(row: NotificationTransactionLogRowDto): boolean {
    return !row.IsSuccess && !!row.FailureReason?.trim();
  }

  statusPillClass(row: NotificationTransactionLogRowDto): string {
    return row.IsSuccess
      ? 'inline-flex items-center rounded border px-2 py-0.5 text-xs font-semibold border-success/30 bg-success/10 text-success'
      : 'inline-flex items-center rounded border px-2 py-0.5 text-xs font-semibold border-danger/30 bg-danger/10 text-danger';
  }

  toggleActionsMenu(rowId: number, event: Event): void {
    event.stopPropagation();
    this.actionsMenuRowId.update((current) => (current === rowId ? null : rowId));
  }

  closeActionsMenu(): void {
    this.actionsMenuRowId.set(null);
  }

  openView(row: NotificationTransactionLogRowDto): void {
    this.closeActionsMenu();
    this.viewing.set(row);
    this.viewOpen.set(true);
  }

  closeView(): void {
    this.viewOpen.set(false);
    this.viewing.set(null);
  }

  openFailureReason(row: NotificationTransactionLogRowDto, event?: Event): void {
    event?.stopPropagation();
    this.closeActionsMenu();
    this.failureReasonText.set(row.FailureReason?.trim() || '—');
    this.failureReasonOpen.set(true);
  }

  closeFailureReason(): void {
    this.failureReasonOpen.set(false);
    this.failureReasonText.set('');
  }

  async resend(row: NotificationTransactionLogRowDto): Promise<void> {
    this.closeActionsMenu();
    await this.asyncExec.execute(() => firstValueFrom(this.api.resendNotificationTransaction(row.Id)), {
      recordSuccess: true,
      progressMessage: $localize`:@@jobseeker.admin.notificationTransactions.resending:Resending…`,
      successMessage: $localize`:@@jobseeker.admin.notificationTransactions.resendSuccess:Notification resent.`,
    });
    await this.reload();
  }

  trackRow(_i: number, row: NotificationTransactionLogRowDto): number {
    return row.Id;
  }

  private resetFilterValues(): void {
    this.filterChannel.set('');
    this.filterReceiver.set('');
    this.filterAction.set('');
    this.filterSuccess.set('');
    this.filterCreatedFrom.set('');
    this.filterCreatedTo.set('');
  }

  private async reload(): Promise<void> {
    try {
      await this.asyncExec.execute(
        async () => {
          const page = await firstValueFrom(
            this.api.getNotificationTransactionsPage({
              pageIndex: this.pageIndex(),
              pageSize: this.pageSize(),
              channelType: this.filterChannel(),
              receiver: this.filterReceiver(),
              actionCode: this.filterAction(),
              isSuccess: this.filterSuccess(),
              createdFrom: this.toIsoOrEmpty(this.filterCreatedFrom()),
              createdTo: this.toIsoOrEmpty(this.filterCreatedTo(), true),
            })
          );
          this.rows.set(page?.rows ?? []);
          this.totalCount.set(page?.total ?? 0);
          this.pageIndex.set(page?.pageIndex ?? this.pageIndex());
        },
        {
          recordSuccess: false,
          progressMessage: $localize`:@@jobseeker.common.loading:Loading…`,
        }
      );
    } catch {
      this.rows.set([]);
      this.totalCount.set(0);
    }
  }

  private toIsoOrEmpty(localDatetime: string, endOfMinute = false): string {
    const raw = (localDatetime ?? '').trim();
    if (!raw) return '';
    const d = new Date(raw);
    if (Number.isNaN(d.getTime())) return '';
    if (endOfMinute) {
      d.setSeconds(59, 999);
    }
    return d.toISOString();
  }
}
