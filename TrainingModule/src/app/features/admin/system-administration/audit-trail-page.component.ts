/// <reference types="@angular/localize" />

import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

import {
  AdminSystemApiService,
  type SystemAuditTrailRowDto,
  type SystemAuditValueHistoryRowDto,
} from '../../../core/api/services/admin-system-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { AssetUrlPipe } from '../../../core/ui/asset-url.pipe';
import {
  AUDIT_TRAIL_MODULES,
  auditTrailEntityTypesForModule,
  defaultEntityTypeForModule,
} from './audit-trail-catalog.util';

const PAGE_SIZE_OPTIONS = [5, 10, 15] as const;
type PageSize = (typeof PAGE_SIZE_OPTIONS)[number];
type ViewMode = 'changes' | 'value-history';

@Component({
  selector: 'app-audit-trail-page',
  standalone: true,
  imports: [FormsModule, AssetUrlPipe],
  templateUrl: './audit-trail-page.component.html',
  styleUrl: './audit-trail-page.component.scss',
})
export class AuditTrailPageComponent implements OnInit {
  private readonly api = inject(AdminSystemApiService);
  private readonly asyncExec = inject(AsyncExecutorService);
  readonly locale = inject(LocaleService);

  readonly viewMode = signal<ViewMode>('changes');
  readonly pageSize = signal<PageSize>(5);
  readonly pageIndex = signal(0);
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  readonly drawerOpen = signal(false);

  readonly moduleOptions = AUDIT_TRAIL_MODULES;
  readonly filterModule = signal(AUDIT_TRAIL_MODULES[0]?.value ?? '');
  readonly filterEntityType = signal(defaultEntityTypeForModule(this.filterModule()));
  readonly filterEntityReference = signal('');
  readonly filterField = signal('');
  readonly filterAction = signal('');
  readonly filterSource = signal('');
  readonly filterUpdatedBy = signal('');
  readonly filterUpdatedFrom = signal('');
  readonly filterUpdatedTo = signal('');
  readonly filterValue = signal('');

  readonly searchLabel = $localize`:@@jobseeker.jobsFind.search:Search`;
  readonly clearFiltersAria = $localize`:@@jobseeker.jobSeekerSearch.clearFilters:Clear filters`;
  readonly advancedFiltersLabel = $localize`:@@jobseeker.jobSeekersList.tabAdvanced:Advanced filter`;

  readonly entityTypeOptions = computed(() => auditTrailEntityTypesForModule(this.filterModule()));

  readonly hasActiveFilters = computed(() => {
    const defaultModule = AUDIT_TRAIL_MODULES[0]?.value ?? '';
    const defaultEntity = defaultEntityTypeForModule(defaultModule);
    return (
      this.filterModule() !== defaultModule ||
      this.filterEntityType() !== defaultEntity ||
      !!this.filterEntityReference().trim() ||
      !!this.filterField().trim() ||
      !!this.filterAction() ||
      !!this.filterSource() ||
      !!this.filterUpdatedBy().trim() ||
      !!this.filterUpdatedFrom().trim() ||
      !!this.filterUpdatedTo().trim() ||
      !!this.filterValue().trim()
    );
  });

  readonly advancedFilterCount = computed(() => {
    let n = 0;
    const defaultModule = AUDIT_TRAIL_MODULES[0]?.value ?? '';
    const defaultEntity = defaultEntityTypeForModule(defaultModule);
    if (this.filterModule() !== defaultModule) n += 1;
    if (this.filterEntityType() !== defaultEntity) n += 1;
    if (this.filterEntityReference().trim()) n += 1;
    if (this.filterField().trim()) n += 1;
    if (this.filterValue().trim()) n += 1;
    if (this.filterUpdatedFrom().trim()) n += 1;
    if (this.filterUpdatedTo().trim()) n += 1;
    if (this.viewMode() === 'changes') {
      if (this.filterAction()) n += 1;
      if (this.filterSource()) n += 1;
      if (this.filterUpdatedBy().trim()) n += 1;
    }
    return n;
  });

  readonly changeRows = signal<SystemAuditTrailRowDto[]>([]);
  readonly historyRows = signal<SystemAuditValueHistoryRowDto[]>([]);
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

  readonly stillCurrentLabel = $localize`:@@jobseeker.admin.auditTrail.stillCurrent:Still current`;

  ngOnInit(): void {
    void this.reload();
  }

  moduleLabel(opt: { labelEn: string; labelAr: string }): string {
    return this.locale.locale() === 'ar' ? opt.labelAr : opt.labelEn;
  }

  entityTypeLabel(opt: { labelEn: string; labelAr: string }): string {
    return this.locale.locale() === 'ar' ? opt.labelAr : opt.labelEn;
  }

  setModule(module: string): void {
    this.filterModule.set(module ?? '');
    const allowed = auditTrailEntityTypesForModule(this.filterModule());
    const current = this.filterEntityType();
    if (!allowed.some((e) => e.value === current)) {
      this.filterEntityType.set(allowed[0]?.value ?? '');
    }
  }

  setEntityType(entityType: string): void {
    this.filterEntityType.set(entityType ?? '');
  }

  setViewMode(mode: ViewMode): void {
    if (this.viewMode() === mode) return;
    this.viewMode.set(mode);
    this.pageIndex.set(0);
    void this.reload();
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

  trackChange(_i: number, row: SystemAuditTrailRowDto): number {
    return row.Id;
  }

  trackHistory(i: number, row: SystemAuditValueHistoryRowDto): string {
    return `${row.EntityReference}|${row.Field}|${row.ValidFrom}|${i}`;
  }

  private resetFilterValues(): void {
    const defaultModule = AUDIT_TRAIL_MODULES[0]?.value ?? '';
    this.filterModule.set(defaultModule);
    this.filterEntityType.set(defaultEntityTypeForModule(defaultModule));
    this.filterEntityReference.set('');
    this.filterField.set('');
    this.filterAction.set('');
    this.filterSource.set('');
    this.filterUpdatedBy.set('');
    this.filterUpdatedFrom.set('');
    this.filterUpdatedTo.set('');
    this.filterValue.set('');
  }

  private async reload(): Promise<void> {
    try {
      await this.asyncExec.execute(
        async () => {
          if (this.viewMode() === 'value-history') {
            const page = await firstValueFrom(
              this.api.getAuditValueHistoryPage({
                pageIndex: this.pageIndex(),
                pageSize: this.pageSize(),
                module: this.filterModule(),
                entityType: this.filterEntityType(),
                entityReference: this.filterEntityReference(),
                field: this.filterField(),
                value: this.filterValue(),
                rangeFrom: this.toIsoOrEmpty(this.filterUpdatedFrom()),
                rangeTo: this.toIsoOrEmpty(this.filterUpdatedTo(), true),
              })
            );
            this.historyRows.set(page?.rows ?? []);
            this.changeRows.set([]);
            this.totalCount.set(page?.total ?? 0);
            this.pageIndex.set(page?.pageIndex ?? this.pageIndex());
            return;
          }

          const page = await firstValueFrom(
            this.api.getAuditTrailPage({
              pageIndex: this.pageIndex(),
              pageSize: this.pageSize(),
              module: this.filterModule(),
              entityType: this.filterEntityType(),
              entityReference: this.filterEntityReference(),
              field: this.filterField(),
              action: this.filterAction(),
              source: this.filterSource(),
              updatedByUserId: this.filterUpdatedBy(),
              updatedFrom: this.toIsoOrEmpty(this.filterUpdatedFrom()),
              updatedTo: this.toIsoOrEmpty(this.filterUpdatedTo(), true),
              value: this.filterValue(),
            })
          );
          this.changeRows.set(page?.rows ?? []);
          this.historyRows.set([]);
          this.totalCount.set(page?.total ?? 0);
          this.pageIndex.set(page?.pageIndex ?? this.pageIndex());
        },
        {
          progressMessage: $localize`:@@jobseeker.common.loading:Loading…`,
          recordSuccess: false,
        }
      );
    } catch {
      this.changeRows.set([]);
      this.historyRows.set([]);
      this.totalCount.set(0);
    }
  }

  /** datetime-local → ISO; end-of-day when `endOfDay`. */
  private toIsoOrEmpty(local: string, endOfDay = false): string {
    const raw = (local ?? '').trim();
    if (!raw) return '';
    const d = new Date(raw);
    if (Number.isNaN(d.getTime())) return '';
    if (endOfDay && raw.length <= 10) {
      d.setHours(23, 59, 59, 999);
    }
    return d.toISOString();
  }
}
