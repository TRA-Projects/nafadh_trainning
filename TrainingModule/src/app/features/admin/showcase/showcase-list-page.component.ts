/// <reference types="@angular/localize" />

import { NgTemplateOutlet } from '@angular/common';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import { SHOWCASE_CATEGORIES, SHOWCASE_STATUSES, type ShowcaseItemDto } from '../../../core/api/models/showcase.models';
import { ShowcaseApiService } from '../../../core/api/services/showcase-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { ModalCloseButtonComponent } from '../../../core/ui/modal-close-button/modal-close-button.component';
import { formatShowcaseDate, showcaseCategoryLabel, showcaseStatusLabel, showcaseStatusPillClass } from './showcase-labels';

const PAGE_SIZE_OPTIONS = [5, 10, 15] as const;
type PageSize = (typeof PAGE_SIZE_OPTIONS)[number];
const SEARCH_DEBOUNCE_MS = 300;

@Component({
  selector: 'app-showcase-list-page',
  standalone: true,
  imports: [NgTemplateOutlet, FormsModule, ModalCloseButtonComponent],
  templateUrl: './showcase-list-page.component.html',
  styleUrl: './showcase-list-page.component.scss',
})
export class ShowcaseListPageComponent implements OnInit {
  private readonly api = inject(ShowcaseApiService);
  private readonly asyncExec = inject(AsyncExecutorService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  readonly locale = inject(LocaleService);

  readonly pageSize = signal<PageSize>(5);
  readonly pageIndex = signal(0);
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  readonly categories = SHOWCASE_CATEGORIES;
  readonly statuses = SHOWCASE_STATUSES;
  readonly drawerOpen = signal(false);
  readonly actionsMenuRowId = signal<number | null>(null);

  readonly draftQuery = signal('');
  readonly query = signal('');
  readonly filterCategory = signal('');
  readonly filterStatus = signal('');

  readonly rows = signal<ShowcaseItemDto[]>([]);
  readonly totalCount = signal(0);

  /** Popups: read-only detail and delete confirmation. */
  readonly viewing = signal<ShowcaseItemDto | null>(null);
  readonly deleting = signal<ShowcaseItemDto | null>(null);

  readonly searchLabel = $localize`:@@jobseeker.jobsFind.search:Search`;
  readonly searchPlaceholder = $localize`:@@jobseeker.showcase.searchPlaceholder:Search by title or email`;
  readonly clearFiltersAria = $localize`:@@jobseeker.jobSeekerSearch.clearFilters:Clear filters`;
  readonly advancedFiltersLabel = $localize`:@@jobseeker.jobSeekersList.tabAdvanced:Advanced filter`;
  readonly rowActionsMenuAria = $localize`:@@jobseeker.admin.systemConfig.rowActionsMenu:Row actions`;
  readonly addLabel = $localize`:@@jobseeker.showcase.add:New record`;
  readonly viewActionLabel = $localize`:@@jobseeker.common.view:View`;
  readonly editActionLabel = $localize`:@@jobseeker.common.edit:Edit`;
  readonly deleteActionLabel = $localize`:@@jobseeker.common.delete:Delete`;

  readonly hasActiveFilters = computed(() => !!this.query() || !!this.filterCategory() || !!this.filterStatus());
  readonly advancedFilterCount = computed(() => (this.filterCategory() ? 1 : 0) + (this.filterStatus() ? 1 : 0));

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

  title(row: ShowcaseItemDto): string {
    return (this.locale.locale() === 'ar' ? row.Title_ar || row.Title_en : row.Title_en || row.Title_ar) || '—';
  }

  categoryLabel(category: string): string {
    return showcaseCategoryLabel(category);
  }

  statusLabel(status: string): string {
    return showcaseStatusLabel(status);
  }

  statusPillClass(status: string): string {
    return showcaseStatusPillClass(status);
  }

  formatDate(iso: string | null | undefined): string {
    return formatShowcaseDate(iso);
  }

  formatAmount(value: number | null | undefined): string {
    return value == null ? '—' : Number(value).toLocaleString();
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
    this.filterCategory.set('');
    this.filterStatus.set('');
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
    void this.router.navigate(['/admin/showcase/new']);
  }

  edit(row: ShowcaseItemDto): void {
    this.actionsMenuRowId.set(null);
    this.viewing.set(null);
    void this.router.navigate(['/admin/showcase', row.Id]);
  }

  openView(row: ShowcaseItemDto): void {
    this.actionsMenuRowId.set(null);
    this.viewing.set(row);
  }

  closeView(): void {
    this.viewing.set(null);
  }

  viewTitle(row: ShowcaseItemDto): string {
    return this.title(row);
  }

  viewDescription(row: ShowcaseItemDto): string {
    return row.Description?.trim() || '—';
  }

  askDelete(row: ShowcaseItemDto): void {
    this.actionsMenuRowId.set(null);
    if (!row.CanEdit) return;
    this.deleting.set(row);
  }

  closeDelete(): void {
    this.deleting.set(null);
  }

  async confirmDelete(): Promise<void> {
    const row = this.deleting();
    if (!row) return;
    this.deleting.set(null);
    try {
      await this.asyncExec.execute(() => firstValueFrom(this.api.delete(row.Id)), {
        recordSuccess: true,
        progressMessage: $localize`:@@jobseeker.showcase.deleting:Deleting…`,
        successMessage: $localize`:@@jobseeker.showcase.deleted:Record deleted.`,
      });
    } catch {
      return;
    }
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
            this.api.getPage({
              pageIndex: this.pageIndex(),
              pageSize: this.pageSize(),
              q: this.query(),
              category: this.filterCategory(),
              status: this.filterStatus(),
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
