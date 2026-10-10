/// <reference types="@angular/localize" />

import { NgTemplateOutlet } from '@angular/common';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import {
  CUSTOM_NOTIFICATION_STATUSES,
  type CustomNotificationRowDto,
  type CustomNotificationStatus,
} from '../../../core/api/models/custom-notifications.models';
import { NotificationsApiService } from '../../../core/api/services/notifications-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import {
  customNotificationChannelsLabel,
  customNotificationRegistrationFilterLabel,
  customNotificationStatusLabel,
} from './custom-notification-labels';

const PAGE_SIZE_OPTIONS = [5, 10, 15] as const;
type PageSize = (typeof PAGE_SIZE_OPTIONS)[number];
const SEARCH_DEBOUNCE_MS = 300;

@Component({
  selector: 'app-custom-notifications-list-page',
  standalone: true,
  imports: [NgTemplateOutlet, FormsModule],
  templateUrl: './custom-notifications-list-page.component.html',
  styleUrl: './custom-notifications-list-page.component.scss',
})
export class CustomNotificationsListPageComponent implements OnInit {
  private readonly api = inject(NotificationsApiService);
  private readonly asyncExec = inject(AsyncExecutorService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  readonly locale = inject(LocaleService);

  readonly pageSize = signal<PageSize>(5);
  readonly pageIndex = signal(0);
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  readonly statuses = CUSTOM_NOTIFICATION_STATUSES;
  readonly drawerOpen = signal(false);
  readonly actionsMenuRowId = signal<number | null>(null);

  readonly draftQuery = signal('');
  readonly query = signal('');
  readonly filterStatus = signal('');
  readonly filterChannel = signal('');

  readonly rows = signal<CustomNotificationRowDto[]>([]);
  readonly totalCount = signal(0);

  readonly searchLabel = $localize`:@@jobseeker.jobsFind.search:Search`;
  readonly searchPlaceholder = $localize`:@@jobseeker.customNotifications.searchPlaceholder:Search by title`;
  readonly clearFiltersAria = $localize`:@@jobseeker.jobSeekerSearch.clearFilters:Clear filters`;
  readonly advancedFiltersLabel = $localize`:@@jobseeker.jobSeekersList.tabAdvanced:Advanced filter`;
  readonly rowActionsMenuAria = $localize`:@@jobseeker.admin.systemConfig.rowActionsMenu:Row actions`;
  readonly addLabel = $localize`:@@jobseeker.customNotifications.add:New notification`;
  readonly viewActionLabel = $localize`:@@jobseeker.common.view:View`;
  readonly editActionLabel = $localize`:@@jobseeker.common.edit:Edit`;
  readonly cancelActionLabel = $localize`:@@jobseeker.customNotifications.cancelSend:Cancel sending`;

  readonly hasActiveFilters = computed(() => !!this.query() || !!this.filterStatus() || !!this.filterChannel());
  readonly advancedFilterCount = computed(() => (this.filterStatus() ? 1 : 0) + (this.filterChannel() ? 1 : 0));

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

  private searchTimer: ReturnType<typeof setTimeout> | null = null;

  constructor() {
    const onDocClick = (event: MouseEvent) => {
      if (this.actionsMenuRowId() == null) return;
      const el = event.target as HTMLElement | null;
      if (!el?.closest?.('[data-row-actions-menu]')) {
        this.actionsMenuRowId.set(null);
      }
    };
    document.addEventListener('click', onDocClick, true);
    this.destroyRef.onDestroy(() => {
      document.removeEventListener('click', onDocClick, true);
      this.clearSearchDebounce();
    });
  }

  ngOnInit(): void {
    void this.reload();
  }

  title(row: CustomNotificationRowDto): string {
    return (this.locale.locale() === 'ar' ? row.Title_ar || row.Title_en : row.Title_en || row.Title_ar) || '—';
  }

  channelsLabel(row: CustomNotificationRowDto): string {
    return customNotificationChannelsLabel(row);
  }

  statusLabel(status: string): string {
    return customNotificationStatusLabel(status);
  }

  targetsLabel(row: CustomNotificationRowDto): string {
    const parts: string[] = [];
    if (row.UserTargetCount > 0) {
      parts.push($localize`:@@jobseeker.customNotifications.targetsUsers:${row.UserTargetCount}:count: users`);
    }
    if (row.GroupTargetCount > 0) {
      parts.push($localize`:@@jobseeker.customNotifications.targetsGroups:${row.GroupTargetCount}:count: groups`);
    }
    const orgsFilter = row.AllOrganizationsFilter ?? 'All';
    if (row.AllOrganizations) {
      parts.push(
        this.withFilter($localize`:@@jobseeker.customNotifications.allOrganizations:All organizations`, orgsFilter)
      );
    }
    if (row.OrganizationTargetCount > 0 && !(row.AllOrganizations && orgsFilter === 'All')) {
      parts.push(
        $localize`:@@jobseeker.customNotifications.targetsOrganizations:${row.OrganizationTargetCount}:count: organizations`
      );
    }
    if (row.JobSeekersFilter) {
      parts.push(
        this.withFilter($localize`:@@jobseeker.customNotifications.allJobSeekers:All job seekers`, row.JobSeekersFilter)
      );
    }
    return parts.join(' · ') || '—';
  }

  private withFilter(label: string, filter: string): string {
    return filter === 'All' ? label : `${label} (${customNotificationRegistrationFilterLabel(filter)})`;
  }

  deliveryLabel(row: CustomNotificationRowDto): string {
    if (row.Status === 'Scheduled' || row.Status === 'Cancelled') return '—';
    return `${row.SentCount} / ${row.TotalRecipients}`;
  }

  statusPillClass(status: CustomNotificationStatus | string): string {
    const base = 'inline-flex items-center rounded border px-2 py-0.5 text-xs font-semibold ';
    switch (status) {
      case 'Sent':
        return base + 'border-success/30 bg-success/10 text-success';
      case 'Failed':
        return base + 'border-danger/30 bg-danger/10 text-danger';
      case 'PartiallyFailed':
        return base + 'border-warning/30 bg-warning/10 text-warning';
      case 'Cancelled':
        return base + 'border-white-dark/30 bg-white-dark/10 text-white-dark';
      default:
        return base + 'border-primary/30 bg-primary/10 text-primary';
    }
  }

  formatDate(iso: string | null | undefined): string {
    if (!iso) return '—';
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return iso;
    return d.toLocaleString();
  }

  onSearchInputChange(value: string): void {
    this.draftQuery.set((value ?? '').toString());
    this.clearSearchDebounce();
    this.searchTimer = setTimeout(() => this.search(), SEARCH_DEBOUNCE_MS);
  }

  search(): void {
    this.clearSearchDebounce();
    this.query.set(this.draftQuery().trim());
    this.pageIndex.set(0);
    this.drawerOpen.set(false);
    void this.reload();
  }

  openDrawer(): void {
    this.drawerOpen.set(true);
  }

  closeDrawer(): void {
    this.drawerOpen.set(false);
  }

  applyDrawer(): void {
    this.search();
  }

  clearDrawerFilters(): void {
    this.draftQuery.set('');
    this.filterStatus.set('');
    this.filterChannel.set('');
  }

  clearFilters(): void {
    this.clearDrawerFilters();
    this.query.set('');
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

  toggleActionsMenu(rowId: number, event: Event): void {
    event.stopPropagation();
    this.actionsMenuRowId.update((current) => (current === rowId ? null : rowId));
  }

  goToCreate(): void {
    void this.router.navigate(['/admin/custom-notifications/new']);
  }

  open(row: CustomNotificationRowDto): void {
    this.actionsMenuRowId.set(null);
    void this.router.navigate(['/admin/custom-notifications', row.Id]);
  }

  async cancel(row: CustomNotificationRowDto): Promise<void> {
    this.actionsMenuRowId.set(null);
    if (!row.CanEdit) return;
    if (!confirm($localize`:@@jobseeker.customNotifications.cancelConfirm:Cancel this scheduled notification?`)) return;
    await this.asyncExec.execute(() => firstValueFrom(this.api.cancelAdminNotification(row.Id)), {
      recordSuccess: true,
      progressMessage: $localize`:@@jobseeker.customNotifications.cancelling:Cancelling…`,
      successMessage: $localize`:@@jobseeker.customNotifications.cancelled:Notification cancelled.`,
    });
    await this.reload();
  }

  private clearSearchDebounce(): void {
    if (this.searchTimer) {
      clearTimeout(this.searchTimer);
      this.searchTimer = null;
    }
  }

  private async reload(): Promise<void> {
    try {
      await this.asyncExec.execute(
        async () => {
          const page = await firstValueFrom(
            this.api.getAdminPage({
              pageIndex: this.pageIndex(),
              pageSize: this.pageSize(),
              q: this.query(),
              status: this.filterStatus(),
              channel: this.filterChannel(),
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
}
