/// <reference types="@angular/localize" />

import { Component, computed, DestroyRef, effect, HostListener, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, ActivatedRouteSnapshot, NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs/operators';
import { Store } from '@ngrx/store';

import { firstValueFrom } from 'rxjs';

import { AuthAccountsApiService } from '../../core/api/services/auth-accounts-api.service';
import type {
  IdentityAccessAccountRowDto,
  IdentityAccessPermissionDto,
} from '../../core/api/models/identity-access.models';
import { AuthService } from '../../core/auth/auth.service';
import { ACCESS_TOKEN_KEY, getAccessToken } from '../../core/auth/auth-token.storage';
import { navVendorTypeFromProfileType, vendorTypeFromAccessToken } from '../../core/auth/auth-jwt.util';
import { normalizePermissions } from '../../core/auth/identity-access-nav';
import { LocaleService } from '../../core/locale/locale.service';
import { uiAsyncFeature } from '../../core/store/ui-async/ui-async.reducer';
import { selectUiAsyncInProgress } from '../../core/store/ui-async/ui-async.selectors';

import { AssetUrlPipe } from '../../core/ui/asset-url.pipe';
import { HeaderProfileService } from './header-profile.service';
import { MainLayoutNavVariant, MainLayoutSidebarComponent } from './main-layout-sidebar.component';
import { NotificationsBellComponent } from './notifications-bell.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, MainLayoutSidebarComponent, AssetUrlPipe, NotificationsBellComponent],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent {
  readonly year = new Date().getFullYear();

  readonly a11yToggleSidebar = $localize`:@@jobseeker.layout.a11y.toggleSidebar:Toggle sidebar`;
  readonly a11yOpenMenu = $localize`:@@jobseeker.layout.a11y.openMenu:Open menu`;
  readonly breadcrumbNavAria = $localize`:@@jobseeker.layout.breadcrumb.navAria:Breadcrumb`;

  readonly auth = inject(AuthService);
  readonly locale = inject(LocaleService);
  private readonly store = inject(Store);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly accountsApi = inject(AuthAccountsApiService);
  private readonly headerProfile = inject(HeaderProfileService);

  /** Which sidebar to show (`userType`: 0 admin, 1 job seeker, 2|4 operator). */
  readonly sidebarVariant = computed<MainLayoutNavVariant>(() => {
    if (this.auth.isAdminNav()) {
      return 'admin';
    }
    if (this.auth.isOperatorNav()) {
      return 'operator';
    }
    return 'jobseeker';
  });

  /** Global async progress (NgRx `uiAsync` feature; see {@link AsyncExecutorService}). */
  readonly uiAsync = {
    inProgress: toSignal(this.store.select(selectUiAsyncInProgress), { initialValue: false }),
    progressMessage: toSignal(this.store.select(uiAsyncFeature.selectProgressMessage), { initialValue: '' }),
  };

  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  /** Same semantics as shared NAFADH layout: theme CSS pairs this with `.vertical` for mobile drawer + desktop collapse. */
  readonly sidebarOpen = signal(false);

  /** One dropdown open at a time (matches NAFADH nav behavior). */
  readonly activeDropdown = signal<string | null>(null);

  readonly userMenuOpen = signal(false);
  readonly accounts = signal<IdentityAccessAccountRowDto[]>([]);
  readonly permissions = signal<IdentityAccessPermissionDto | null>(null);
  readonly canSwitchAccount = computed(() => this.accounts().length > 1);

  readonly headerAvatarUrl = computed(() => this.headerProfile.avatarUrl());
  readonly userMenuDisplayName = computed(() => {
    const name = this.headerProfile.displayName().trim();
    if (name) {
      return name;
    }
    const labels = this.auth.uiRoleLabel();
    return this.locale.locale() === 'ar' ? labels.ar : labels.en;
  });

  /** Home + each activated route segment that has a title (or inferred section label). */
  readonly breadcrumbs = signal<{ routerLink: string; label: string }[]>([]);

  constructor() {
    if (typeof document !== 'undefined') {
      document.documentElement.classList.remove('dark');
    }
    if (typeof window !== 'undefined') {
      window.localStorage.removeItem('jobseeker.theme');
    }

    this.router.events
      .pipe(
        filter((e): e is NavigationEnd => e instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => {
        this.rebuildBreadcrumbs();
        this.syncAdminIdentityAccessDropdown();
      });
    effect(() => {
      this.locale.locale();
      this.rebuildBreadcrumbs();
    });
    queueMicrotask(() => {
      this.rebuildBreadcrumbs();
      this.syncAdminIdentityAccessDropdown();
      void this.loadAccounts();
      void this.loadPermissions();
      void this.headerProfile.ensureLoaded();
    });
  }

  accountLabel(row: IdentityAccessAccountRowDto): string {
    const group = this.locale.locale() === 'ar' ? row.UserGroupNameAr : row.UserGroupNameEn;
    return group?.trim() || row.UserGroupCode || row.Id;
  }

  async switchAccount(row: IdentityAccessAccountRowDto): Promise<void> {
    if (row.IsCurrent) {
      return;
    }
    const tokens = await firstValueFrom(this.accountsApi.switchAccount(row.Id));
    const remember = !!localStorage.getItem(ACCESS_TOKEN_KEY);
    this.auth.persistTokens(tokens.AccessToken, tokens.RefreshToken, remember);
    const vt = navVendorTypeFromProfileType(vendorTypeFromAccessToken(tokens.AccessToken)) ?? this.auth.vendorType();
    this.auth.setLoggedIn(true, { vendorType: vt });
    this.userMenuOpen.set(false);
    this.headerProfile.reset();
    await this.loadAccounts();
    await this.loadPermissions();
    void this.headerProfile.ensureLoaded();
    await this.router.navigateByUrl(this.auth.homePath());
  }

  private async loadAccounts(): Promise<void> {
    if (!this.auth.isLoggedIn()) {
      this.accounts.set([]);
      return;
    }
    try {
      const rows = await firstValueFrom(this.accountsApi.getAccounts());
      this.accounts.set(rows ?? []);
    } catch {
      this.accounts.set([]);
    }
  }

  private async loadPermissions(): Promise<void> {
    if (!this.auth.isLoggedIn()) {
      this.permissions.set(null);
      return;
    }
    try {
      const data = await firstValueFrom(this.accountsApi.getPermissions());
      // Fail open (`null`) when payload is missing; SystemAdmin JWT forces Bypass.
      this.permissions.set(normalizePermissions(data, getAccessToken()));
    } catch {
      this.permissions.set(normalizePermissions(null, getAccessToken()));
    }
  }

  private rebuildBreadcrumbs(): void {
    const ar = this.locale.locale() === 'ar';
    const homePath = this.auth.homePath();
    const items: { routerLink: string; label: string }[] = [
      { routerLink: homePath, label: $localize`:@@jobseeker.dashboard.breadcrumb.home:Home` },
    ];
    let acc = '';
    let snap: ActivatedRouteSnapshot | null = this.activatedRoute.snapshot.firstChild;
    while (snap) {
      const piece = snap.url.map((u) => u.path).join('/');
      if (piece) {
        acc = acc ? `${acc}/${piece}` : `/${piece}`;
      }
      const link = acc.startsWith('/') ? acc : `/${acc}`;
      let label = '';
      const d = snap.data as Record<string, unknown>;
      if (ar && typeof d['titleAr'] === 'string' && d['titleAr'].trim()) {
        label = String(d['titleAr']).trim();
      } else if (typeof d['title'] === 'string' && d['title'].trim()) {
        label = String(d['title']).trim();
      } else if (snap.routeConfig?.path === 'dashboard') {
        const db = this.auth.dashboardBreadcrumbLabel();
        label = ar ? db.ar : db.en;
      } else {
        label = this.inferSectionBreadcrumbLabel(snap.routeConfig?.path);
      }
      if (label && piece) {
        const prev = items[items.length - 1];
        if (prev && prev.label === label) {
          items[items.length - 1] = { routerLink: link, label };
        } else {
          items.push({ routerLink: link, label });
        }
      }
      snap = snap.firstChild;
    }
    this.breadcrumbs.set(items);
  }

  private inferSectionBreadcrumbLabel(path: string | undefined): string {
    if (!path) {
      return '';
    }
    const base = path.split('/')[0]?.split(':')[0] ?? '';
    switch (base) {
      case 'admin':
        return $localize`:@@jobseeker.layout.breadcrumb.admin:Admin`;
      case 'operator':
        return $localize`:@@jobseeker.layout.breadcrumb.operator:Operator`;
      default:
        return '';
    }
  }

  /** Keep Identity & access open on its admin routes. */
  private syncAdminIdentityAccessDropdown(): void {
    if (this.sidebarVariant() !== 'admin') {
      return;
    }
    const path = this.router.url.split('?')[0] ?? '';
    if (path.startsWith('/admin/users') || path.startsWith('/admin/roles') || path.startsWith('/admin/template-roles') || path.startsWith('/admin/groups')) {
      this.activeDropdown.set('admin-identity-access');
      return;
    }
    const systemAdminPrefixes = [
      '/admin/system-configuration',
      '/admin/constants-management',
      '/admin/notification-templates',
      '/admin/audit-trail',
      '/admin/notification-transactions',
      '/admin/custom-notifications',
    ];
    if (systemAdminPrefixes.some((p) => path.startsWith(p))) {
      this.activeDropdown.set('admin-fsd-51');
    }
  }

  toggleSidebar(): void {
    this.sidebarOpen.update((v) => !v);
  }

  toggleDropdown(key: string): void {
    this.activeDropdown.update((cur) => (cur === key ? null : key));
  }

  toggleLocale(): void {
    this.locale.toggle();
  }

  toggleUserMenu(event: Event): void {
    event.stopPropagation();
    this.userMenuOpen.update((v) => !v);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.user-dropdown')) {
      this.userMenuOpen.set(false);
    }
  }

  onNavLinkClick(): void {
    if (typeof window !== 'undefined' && window.innerWidth < 1024) {
      this.sidebarOpen.set(false);
    }
  }

  logout(): void {
    this.userMenuOpen.set(false);
    this.headerProfile.reset();
    this.auth.setLoggedIn(false);
    void this.router.navigate(['/login']);
  }
}
