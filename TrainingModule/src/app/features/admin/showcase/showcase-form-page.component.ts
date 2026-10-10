/// <reference types="@angular/localize" />

import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import { SHOWCASE_CATEGORIES, SHOWCASE_STATUSES, type ShowcaseItemDto } from '../../../core/api/models/showcase.models';
import { ShowcaseApiService } from '../../../core/api/services/showcase-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { BackArrowButtonComponent } from '../../../core/ui/back-arrow-button/back-arrow-button.component';
import { FormPipelineService } from '../../../core/ui/form-pipeline.service';
import { isFormPipelineSuccess } from '../../../core/ui/form-pipeline.types';
import { AngularValidators, ValidationMessagePipe } from '@validators';
import { buildShowcaseForm, showcaseFormValue, showcaseRequest, showcaseTitleRequired } from './showcase-form';
import { showcaseCategoryLabel, showcaseStatusLabel } from './showcase-labels';

@Component({
  selector: 'app-showcase-form-page',
  standalone: true,
  imports: [ReactiveFormsModule, BackArrowButtonComponent, ValidationMessagePipe],
  templateUrl: './showcase-form-page.component.html',
  styleUrl: './showcase-form-page.component.scss',
})
export class ShowcaseFormPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(ShowcaseApiService);
  private readonly asyncExec = inject(AsyncExecutorService);
  private readonly fb = inject(FormBuilder);
  private readonly formPipeline = inject(FormPipelineService);
  readonly locale = inject(LocaleService);

  readonly backToList = '/admin/showcase';
  readonly backToListAria = $localize`:@@jobseeker.showcase.backToList:Back to UI showcase`;
  readonly bannerTitle = $localize`:@@jobseeker.showcase.title:UI showcase`;
  readonly categories = SHOWCASE_CATEGORIES;
  readonly statuses = SHOWCASE_STATUSES;

  readonly form = buildShowcaseForm(this.fb);

  readonly itemId = signal<number | null>(null);
  readonly loaded = signal<ShowcaseItemDto | null>(null);
  readonly readOnly = computed(() => !!this.loaded() && !this.loaded()!.CanEdit);

  readonly pageTitle = computed(() => {
    if (this.itemId() == null) {
      return $localize`:@@jobseeker.showcase.newTitle:New record`;
    }
    const row = this.loaded();
    if (!row) return this.bannerTitle;
    return (this.locale.locale() === 'ar' ? row.Title_ar || row.Title_en : row.Title_en || row.Title_ar) || this.bannerTitle;
  });

  constructor() {
    this.route.paramMap.pipe(takeUntilDestroyed()).subscribe((params) => {
      const raw = params.get('id');
      const id = raw && /^\d+$/.test(raw) ? Number(raw) : null;
      this.itemId.set(id);
      if (id != null) void this.load(id);
    });
  }

  isRequired(name: keyof typeof this.form.controls): boolean {
    const c = this.form.controls[name];
    return c.hasValidator(AngularValidators.required) || c.hasValidator(showcaseTitleRequired);
  }

  categoryLabel(category: string): string {
    return showcaseCategoryLabel(category);
  }

  statusLabel(status: string): string {
    return showcaseStatusLabel(status);
  }

  async save(): Promise<void> {
    if (this.readOnly()) return;
    const id = this.itemId();
    const result = await this.formPipeline.run({
      form: this.form,
      work: () => {
        const body = showcaseRequest(this.form);
        return firstValueFrom(id == null ? this.api.create(body) : this.api.update(id, body));
      },
      execute: {
        progressMessage: $localize`:@@jobseeker.common.saving:Saving…`,
        successMessage: $localize`:@@jobseeker.showcase.saved:Record saved.`,
      },
    });
    if (!isFormPipelineSuccess(result)) return;
    void this.router.navigate([this.backToList]);
  }

  cancel(): void {
    void this.router.navigate([this.backToList]);
  }

  private async load(id: number): Promise<void> {
    try {
      const row = await this.asyncExec.execute(() => firstValueFrom(this.api.get(id)), {
        recordSuccess: false,
        progressMessage: $localize`:@@jobseeker.common.loading:Loading…`,
      });
      this.loaded.set(row);
      this.form.reset(showcaseFormValue(row));
      if (!row.CanEdit) this.form.disable();
    } catch {
      this.loaded.set(null);
    }
  }
}
