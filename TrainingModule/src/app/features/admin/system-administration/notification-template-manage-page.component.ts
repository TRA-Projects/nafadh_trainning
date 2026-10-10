/// <reference types="@angular/localize" />

import { Component, OnInit, computed, inject, signal, viewChild } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { startWith } from 'rxjs/operators';

import {
  AdminSystemApiService,
  type NotificationPlaceholderDto,
  type RenderedNotificationDto,
} from '../../../core/api/services/admin-system-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { BackArrowButtonComponent } from '../../../core/ui/back-arrow-button/back-arrow-button.component';
import { FormPipelineService } from '../../../core/ui/form-pipeline.service';
import { isFormPipelineSuccess } from '../../../core/ui/form-pipeline.types';
import {
  FormPageTabsComponent,
  type FormPageTabItem,
} from '../../../core/ui/form-page-tabs/form-page-tabs.component';
import { HtmlEmailEditorComponent } from '../../../core/ui/html-email-editor/html-email-editor.component';
import { ModalCloseButtonComponent } from '../../../core/ui/modal-close-button/modal-close-button.component';
import { richTextRequired, ValidationMessagePipe } from '@validators';

type ManageTab = 'main' | 'ar' | 'en';

type FocusField =
  | 'EmailSubjectEn'
  | 'EmailSubjectAr'
  | 'EmailBodyEn'
  | 'EmailBodyAr'
  | 'SmsBodyEn'
  | 'SmsBodyAr'
  | null;

@Component({
  selector: 'app-notification-template-manage-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    BackArrowButtonComponent,
    FormPageTabsComponent,
    HtmlEmailEditorComponent,
    ModalCloseButtonComponent,
    ValidationMessagePipe,
  ],
  templateUrl: './notification-template-manage-page.component.html',
  styleUrl: './notification-template-manage-page.component.scss',
})
export class NotificationTemplateManagePageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(AdminSystemApiService);
  private readonly fb = inject(FormBuilder);
  private readonly formPipeline = inject(FormPipelineService);
  readonly locale = inject(LocaleService);

  readonly backToList = '/admin/notification-templates';
  readonly backToListAria = $localize`:@@jobseeker.admin.notificationTemplates.backToListAria:Back to notification templates`;
  readonly pageBannerTitle = $localize`:@@jobseeker.admin.notificationTemplates.title:Notification templates`;

  readonly formTabs: FormPageTabItem[] = [
    { id: 'main', label: $localize`:@@jobseeker.admin.notificationTemplates.tabMain:Main details` },
    { id: 'ar', label: $localize`:@@jobseeker.admin.notificationTemplates.tabArabic:Arabic` },
    { id: 'en', label: $localize`:@@jobseeker.admin.notificationTemplates.tabEnglish:English` },
  ];

  readonly emailBodyEnEditor = viewChild<HtmlEmailEditorComponent>('emailBodyEnEditor');
  readonly emailBodyArEditor = viewChild<HtmlEmailEditorComponent>('emailBodyArEditor');

  readonly actionCode = signal('');
  readonly placeholders = signal<NotificationPlaceholderDto[]>([]);
  readonly previewOpen = signal(false);
  readonly previewResult = signal<RenderedNotificationDto | null>(null);
  readonly focusedField = signal<FocusField>(null);
  readonly activeTab = signal<ManageTab>('main');

  readonly form = this.fb.nonNullable.group({
    LabelEn: ['', Validators.required],
    LabelAr: ['', Validators.required],
    EmailSubjectEn: ['', Validators.required],
    EmailSubjectAr: ['', Validators.required],
    EmailBodyEn: ['', richTextRequired()],
    EmailBodyAr: ['', richTextRequired()],
    SmsBodyEn: ['', [Validators.required, Validators.maxLength(500)]],
    SmsBodyAr: ['', [Validators.required, Validators.maxLength(500)]],
    IsActive: [true],
    EmailEnabled: [true],
    SmsEnabled: [true],
  });

  readonly smsEnCount = computed(() => (this.form.controls.SmsBodyEn.value ?? '').length);
  readonly smsArCount = computed(() => (this.form.controls.SmsBodyAr.value ?? '').length);

  private readonly formRevision = toSignal(
    this.form.valueChanges.pipe(startWith(this.form.getRawValue())),
    { initialValue: this.form.getRawValue() },
  );

  readonly pageTitle = computed(() => {
    this.formRevision();
    const isArabic = this.locale.locale() === 'ar';
    const label = isArabic ? this.form.controls.LabelAr.value : this.form.controls.LabelEn.value;
    const trimmed = (label ?? '').trim();
    return trimmed || this.actionCode();
  });

  readonly saveSuccessMessage = $localize`:@@jobseeker.admin.notificationTemplates.saveSuccess:Notification template saved`;

  constructor() {
    this.route.paramMap.pipe(takeUntilDestroyed()).subscribe((params) => {
      const code = params.get('actionCode') ?? '';
      this.actionCode.set(code);
      if (code) {
        void this.load(code);
      }
    });
  }

  ngOnInit(): void {
    // param subscription loads data
  }

  isRequired(name: keyof typeof this.form.controls): boolean {
    return this.form.controls[name].hasValidator(Validators.required);
  }

  placeholderLabel(p: NotificationPlaceholderDto): string {
    return this.locale.locale() === 'ar' ? p.LabelAr : p.LabelEn;
  }

  setFocusedField(field: FocusField): void {
    this.focusedField.set(field);
  }

  setTab(tab: string): void {
    const next = (tab === 'ar' || tab === 'en' || tab === 'main' ? tab : 'main') as ManageTab;
    if (this.activeTab() === next) {
      return;
    }
    this.activeTab.set(next);
    this.focusedField.set(null);
  }

  private previewLocale(): 'en' | 'ar' {
    const tab = this.activeTab();
    if (tab === 'ar') {
      return 'ar';
    }
    if (tab === 'en') {
      return 'en';
    }
    return this.locale.locale() === 'ar' ? 'ar' : 'en';
  }

  insertPlaceholder(token: string): void {
    const field = this.focusedField();
    if (!field) {
      return;
    }

    if (field === 'EmailBodyEn' || field === 'EmailBodyAr') {
      const editor =
        field === 'EmailBodyEn' ? this.emailBodyEnEditor() : this.emailBodyArEditor();
      editor?.insertToken(token);
      return;
    }

    const control = this.form.controls[field];
    const current = control.value ?? '';
    control.setValue(`${current}{${token}}`);
    control.markAsDirty();
  }

  async save(): Promise<void> {
    const code = this.actionCode();
    if (!code) {
      return;
    }

    const result = await this.formPipeline.run({
      form: this.form,
      work: async () => {
        const v = this.form.getRawValue();
        return firstValueFrom(this.api.updateNotificationTemplate(code, v));
      },
      execute: {
        progressMessage: $localize`:@@jobseeker.common.saving:Saving…`,
        successMessage: this.saveSuccessMessage,
      },
    });

    if (!isFormPipelineSuccess(result)) {
      return;
    }
  }

  async openPreview(): Promise<void> {
    const code = this.actionCode();
    if (!code) {
      return;
    }

    const preview = await firstValueFrom(this.api.previewNotificationTemplate(code, this.previewLocale()));
    this.previewResult.set(preview);
    this.previewOpen.set(true);
  }

  closePreview(): void {
    this.previewOpen.set(false);
    this.previewResult.set(null);
  }

  private async load(actionCode: string): Promise<void> {
    const [template, placeholderData] = await Promise.all([
      firstValueFrom(this.api.getNotificationTemplate(actionCode)),
      firstValueFrom(this.api.getNotificationTemplatePlaceholders(actionCode)),
    ]);

    this.placeholders.set(placeholderData.Placeholders ?? []);
    this.form.reset({
      LabelEn: template.LabelEn,
      LabelAr: template.LabelAr,
      EmailSubjectEn: template.EmailSubjectEn,
      EmailSubjectAr: template.EmailSubjectAr,
      EmailBodyEn: template.EmailBodyEn,
      EmailBodyAr: template.EmailBodyAr,
      SmsBodyEn: template.SmsBodyEn,
      SmsBodyAr: template.SmsBodyAr,
      IsActive: template.IsActive,
      EmailEnabled: template.EmailEnabled,
      SmsEnabled: template.SmsEnabled,
    });
  }
}
