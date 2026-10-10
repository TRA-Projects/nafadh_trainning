/// <reference types="@angular/localize" />

import { Component, DestroyRef, ElementRef, HostListener, effect, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { firstValueFrom } from 'rxjs';

import type { UserNotificationDto } from '../../core/api/models/custom-notifications.models';
import { NotificationsApiService } from '../../core/api/services/notifications-api.service';
import { AuthService } from '../../core/auth/auth.service';
import { notificationsFeature } from '../../core/store/notifications/notifications.reducer';

const UNREAD_POLL_MS = 60_000;

/** Badge text for the bell: hidden at 0, capped at "99+". */
export function unreadBadgeText(count: number): string {
  if (!count || count <= 0) return '';
  return count > 99 ? '99+' : String(count);
}

@Component({
  selector: 'app-notifications-bell',
  standalone: true,
  templateUrl: './notifications-bell.component.html',
})
export class NotificationsBellComponent {
  private readonly api = inject(NotificationsApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly store = inject(Store);
  private readonly host = inject(ElementRef<HTMLElement>);
  private readonly destroyRef = inject(DestroyRef);

  readonly unreadCount = this.store.selectSignal(notificationsFeature.selectUnreadCount);
  readonly recent = this.store.selectSignal(notificationsFeature.selectRecent);
  readonly open = signal(false);
  readonly loadingRecent = signal(false);

  readonly bellAria = $localize`:@@jobseeker.notificationsInbox.bellAria:Notifications`;
  readonly badgeText = unreadBadgeText;

  private pollTimer: ReturnType<typeof setInterval> | null = null;

  constructor() {
    effect(() => {
      if (this.auth.loggedIn()) {
        this.startPolling();
      } else {
        this.stopPolling();
      }
    });
    this.destroyRef.onDestroy(() => this.stopPolling());
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.open() && !this.host.nativeElement.contains(event.target as Node)) {
      this.open.set(false);
    }
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    this.open.set(false);
  }

  async toggle(): Promise<void> {
    const next = !this.open();
    this.open.set(next);
    if (!next) return;
    this.loadingRecent.set(true);
    try {
      await firstValueFrom(this.api.getRecent());
      await firstValueFrom(this.api.getUnreadCount());
    } catch {
      /* keep cached rows */
    } finally {
      this.loadingRecent.set(false);
    }
  }

  formatDate(iso: string): string {
    const d = new Date(iso);
    return Number.isNaN(d.getTime()) ? iso : d.toLocaleString();
  }

  async markRead(row: UserNotificationDto): Promise<void> {
    if (row.IsRead) return;
    try {
      await firstValueFrom(this.api.markRead(row.Id));
    } catch {
      /* ignore; next poll resyncs */
    }
  }

  async markAllRead(): Promise<void> {
    try {
      await firstValueFrom(this.api.markAllRead());
    } catch {
      /* ignore; next poll resyncs */
    }
  }

  viewAll(): void {
    this.open.set(false);
    void this.router.navigate(['/notifications']);
  }

  private startPolling(): void {
    if (this.pollTimer) return;
    void this.refreshCount();
    this.pollTimer = setInterval(() => void this.refreshCount(), UNREAD_POLL_MS);
  }

  private stopPolling(): void {
    if (this.pollTimer) {
      clearInterval(this.pollTimer);
      this.pollTimer = null;
    }
  }

  private async refreshCount(): Promise<void> {
    if (!this.auth.loggedIn()) return;
    try {
      await firstValueFrom(this.api.getUnreadCount());
    } catch {
      /* transient; retry on next tick */
    }
  }
}
