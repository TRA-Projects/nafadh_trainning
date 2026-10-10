import { Component, input, output } from '@angular/core';
import { NgClass } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';

import type { IdentityAccessPermissionDto } from '../../core/api/models/identity-access.models';
import { canShowNav } from '../../core/auth/identity-access-nav';
import { AssetUrlPipe } from '../../core/ui/asset-url.pipe';
export type MainLayoutNavVariant = 'admin' | 'operator' | 'jobseeker';

@Component({
  selector: 'app-main-layout-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, NgClass, AssetUrlPipe],
  templateUrl: './main-layout-sidebar.component.html',
})
export class MainLayoutSidebarComponent {
  /** Sidebar variant from `userType` groupings. */
  readonly variant = input.required<MainLayoutNavVariant>();

  readonly permissions = input<IdentityAccessPermissionDto | null>(null);

  readonly activeDropdown = input<string | null>(null);

  readonly toggleDropdown = output<string>();
  readonly navLinkClick = output<void>();

  onToggle(key: string): void {
    this.toggleDropdown.emit(key);
  }

  onNav(): void {
    this.navLinkClick.emit();
  }

  canShow(url: string): boolean {
    return canShowNav(url, this.permissions());
  }
}
