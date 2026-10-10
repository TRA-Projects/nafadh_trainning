/// <reference types="@angular/localize" />

import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Store } from '@ngrx/store';
import { firstValueFrom } from 'rxjs';

import type { UserNotificationDto } from '../../core/api/models/custom-notifications.models';
import { NotificationsApiService } from '../../core/api/services/notifications-api.service';
import { notificationsFeature } from '../../core/store/notifications/notifications.reducer';
import { AsyncExecutorService } from '../../core/ui/async-executor.service';

const PAGE_SIZE_OPTIONS = [5, 10, 15] as const;
type PageSize = (typeof PAGE_SIZE_OPTIONS)[number];

@Component({
  selector: 'app-my-notifications-page',
  standalone: true,
  templateUrl: './my-notifications-page.component.html',
  styleUrl: './my-notifications-page.component.scss',
})
export class MyNotificationsPageComponent implements OnInit {
  private readonly api = inject(NotificationsApiService);
  private readonly asyncExec = inject(AsyncExecutorService);
  private readonly store = inject(Store);

  readonly pageSize = signal<PageSize>(5);
  readonly pageIndex = signal(0);
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  readonly unreadOnly = signal(false);

  private readonly page = this.store.selectSignal(notificationsFeature.selectInboxPage);
  readonly unreadCount = this.store.selectSignal(notificationsFeature.selectUnreadCount);
  readonly rows = computed(() => this.page()?.rows ?? []);
  readonly totalCount = computed(() => this.page()?.total ?? 0);

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

  ngOnInit(): void {
    void this.reload();
    void firstValueFrom(this.api.getUnreadCount()).catch(() => undefined);
  }

  formatDate(iso: string): string {
    const d = new Date(iso);
    return Number.isNaN(d.getTime()) ? iso : d.toLocaleString();
  }

  setUnreadOnly(value: boolean): void {
    this.unreadOnly.set(value);
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

  async markRead(row: UserNotificationDto): Promise<void> {
    if (row.IsRead) return;
    await this.asyncExec.execute(() => firstValueFrom(this.api.markRead(row.Id)), { recordSuccess: false });
    if (this.unreadOnly()) await this.reload();
  }

  async markAllRead(): Promise<void> {
    await this.asyncExec.execute(() => firstValueFrom(this.api.markAllRead()), {
      recordSuccess: true,
      successMessage: $localize`:@@jobseeker.notificationsInbox.allMarkedRead:All notifications marked as read.`,
    });
    if (this.unreadOnly()) {
      this.pageIndex.set(0);
      await this.reload();
    }
  }

  private async reload(): Promise<void> {
    try {
      await this.asyncExec.execute(
        () => firstValueFrom(this.api.getInboxPage(this.pageIndex(), this.pageSize(), this.unreadOnly())),
        { recordSuccess: false, progressMessage: $localize`:@@jobseeker.common.loading:Loading…` }
      );
    } catch {
      /* executor surfaces the error */
    }
  }
}
