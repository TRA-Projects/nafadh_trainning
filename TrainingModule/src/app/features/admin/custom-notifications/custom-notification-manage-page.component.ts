/// <reference types="@angular/localize" />

import { NgTemplateOutlet } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { debounceTime, startWith } from 'rxjs/operators';

import {
  CUSTOM_NOTIFICATION_REGISTRATION_FILTERS,
  type CustomNotificationDto,
  type CustomNotificationRecipientPreviewDto,
  type CustomNotificationRegistrationFilter,
  type CustomNotificationTargetDto,
  type UpsertCustomNotificationRequestDto,
} from '../../../core/api/models/custom-notifications.models';
import type {
  IdentityAccessOrganizationRowDto,
  IdentityAccessUserGroupDto,
  IdentityAccessUserRowDto,
} from '../../../core/api/models/identity-access.models';
import { IdentityAccessApiService } from '../../../core/api/services/identity-access-api.service';
import { NotificationsApiService } from '../../../core/api/services/notifications-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { BackArrowButtonComponent } from '../../../core/ui/back-arrow-button/back-arrow-button.component';
import { FormPipelineService } from '../../../core/ui/form-pipeline.service';
import { isFormPipelineSuccess } from '../../../core/ui/form-pipeline.types';
import {
  AngularValidators,
  ValidationErrorKeys,
  ValidationMessagePipe,
  anyControlTrue,
  arrayMinLength,
  dateTimeNotInPast,
} from '@validators';
import { customNotificationRegistrationFilterLabel, customNotificationStatusLabel } from './custom-notification-labels';
import {
  addTarget,
  coversEveryOrganization,
  hasAllOrganizations,
  isoToLocalInput,
  localInputToIso,
  normalizeRegistrationFilter,
  registrationFilterOf,
  removeTarget,
  setAllOrganizations,
  setJobSeekers,
  targetKey,
} from './custom-notification-targets.util';

const TITLE_MAX = 256;
const SMS_BODY_MAX = 1000;
const LOOKUP_PAGE_SIZE = 8;
const LOOKUP_DEBOUNCE_MS = 300;

export const CHANNEL_KEYS = ['SendEmail', 'SendSms', 'SendSystem'] as const;

export function buildCustomNotificationForm(fb: FormBuilder) {
  return fb.nonNullable.group(
    {
      Title_en: ['', [AngularValidators.required, AngularValidators.maxLength(TITLE_MAX)]],
      Title_ar: ['', [AngularValidators.required, AngularValidators.maxLength(TITLE_MAX)]],
      Body_en: ['', AngularValidators.required],
      Body_ar: ['', AngularValidators.required],
      SendEmail: [false],
      SendSms: [false],
      SendSystem: [true],
      SendNow: [true],
      ScheduledAt: [{ value: '', disabled: true }, [AngularValidators.required, dateTimeNotInPast()]],
      Targets: new FormControl<CustomNotificationTargetDto[]>([], {
        nonNullable: true,
        validators: arrayMinLength(1, ValidationErrorKeys.notificationTargetRequired),
      }),
    },
    { validators: anyControlTrue(CHANNEL_KEYS, ValidationErrorKeys.notificationChannelRequired) }
  );
}

@Component({
  selector: 'app-custom-notification-manage-page',
  standalone: true,
  imports: [NgTemplateOutlet, ReactiveFormsModule, BackArrowButtonComponent, ValidationMessagePipe],
  templateUrl: './custom-notification-manage-page.component.html',
  styleUrl: './custom-notification-manage-page.component.scss',
})
export class CustomNotificationManagePageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(NotificationsApiService);
  private readonly identityApi = inject(IdentityAccessApiService);
  private readonly fb = inject(FormBuilder);
  private readonly formPipeline = inject(FormPipelineService);
  private readonly destroyRef = inject(DestroyRef);
  readonly locale = inject(LocaleService);

  readonly backToList = '/admin/custom-notifications';
  readonly backToListAria = $localize`:@@jobseeker.customNotifications.backToList:Back to custom notifications`;
  readonly bannerTitle = $localize`:@@jobseeker.customNotifications.title:Custom notifications`;
  readonly allOrganizationsLabel = $localize`:@@jobseeker.customNotifications.allOrganizations:All organizations`;
  readonly allJobSeekersLabel = $localize`:@@jobseeker.customNotifications.allJobSeekers:All job seekers`;
  readonly registrationFilters = CUSTOM_NOTIFICATION_REGISTRATION_FILTERS;
  readonly removeTargetAria = $localize`:@@jobseeker.customNotifications.removeTarget:Remove`;
  readonly userSearchPlaceholder = $localize`:@@jobseeker.customNotifications.userSearchPlaceholder:Search users by name or email`;
  readonly orgSearchPlaceholder = $localize`:@@jobseeker.customNotifications.orgSearchPlaceholder:Search organizations by name or CR number`;

  readonly form = buildCustomNotificationForm(this.fb);

  readonly notificationId = signal<number | null>(null);
  readonly loaded = signal<CustomNotificationDto | null>(null);
  readonly readOnly = computed(() => !!this.loaded() && !this.loaded()!.CanEdit);

  readonly groups = signal<IdentityAccessUserGroupDto[]>([]);
  readonly selectedGroupId = signal<string>('');

  readonly userQuery = signal('');
  readonly userResults = signal<IdentityAccessUserRowDto[]>([]);
  readonly orgQuery = signal('');
  readonly orgResults = signal<IdentityAccessOrganizationRowDto[]>([]);

  readonly preview = signal<CustomNotificationRecipientPreviewDto | null>(null);

  private readonly formValue = toSignal(this.form.valueChanges.pipe(startWith(this.form.getRawValue())), {
    initialValue: this.form.getRawValue(),
  });

  readonly targets = computed(() => {
    this.formValue();
    return this.form.controls.Targets.value;
  });
  readonly userTargets = computed(() => this.targets().filter((t) => t.TargetType === 'User'));
  readonly groupTargets = computed(() => this.targets().filter((t) => t.TargetType === 'UserGroup'));
  readonly orgTargets = computed(() => this.targets().filter((t) => t.TargetType === 'Organization'));
  readonly allOrganizations = computed(() => hasAllOrganizations(this.targets()));
  readonly everyOrganization = computed(() => coversEveryOrganization(this.targets()));
  readonly allOrganizationsFilter = computed(() => registrationFilterOf(this.targets(), 'AllOrganizations') ?? 'All');
  readonly jobSeekers = computed(() => this.targets().some((t) => t.TargetType === 'JobSeekers'));
  readonly jobSeekersFilter = computed(() => registrationFilterOf(this.targets(), 'JobSeekers') ?? 'All');
  readonly sendNow = computed(() => {
    this.formValue();
    return this.form.controls.SendNow.value;
  });

  readonly pageTitle = computed(() => {
    if (this.notificationId() == null) {
      return $localize`:@@jobseeker.customNotifications.newTitle:New notification`;
    }
    const row = this.loaded();
    if (!row) return this.bannerTitle;
    return (this.locale.locale() === 'ar' ? row.Title_ar || row.Title_en : row.Title_en || row.Title_ar) || this.bannerTitle;
  });

  readonly submitLabel = computed(() =>
    this.sendNow()
      ? $localize`:@@jobseeker.customNotifications.sendNowAction:Send now`
      : $localize`:@@jobseeker.customNotifications.scheduleAction:Schedule`
  );

  private userTimer: ReturnType<typeof setTimeout> | null = null;
  private orgTimer: ReturnType<typeof setTimeout> | null = null;

  constructor() {
    this.form.controls.SendNow.valueChanges.pipe(takeUntilDestroyed()).subscribe((now) => {
      const c = this.form.controls.ScheduledAt;
      if (now) c.disable({ emitEvent: false });
      else if (!this.readOnly()) c.enable({ emitEvent: false });
      this.form.updateValueAndValidity();
    });

    this.form.controls.SendSms.valueChanges.pipe(takeUntilDestroyed()).subscribe((sms) => this.applySmsLimits(sms));

    this.form.controls.Targets.valueChanges
      .pipe(debounceTime(400), takeUntilDestroyed())
      .subscribe((targets) => void this.refreshPreview(targets));

    this.route.paramMap.pipe(takeUntilDestroyed()).subscribe((params) => {
      const raw = params.get('id');
      const id = raw && /^\d+$/.test(raw) ? Number(raw) : null;
      this.notificationId.set(id);
      if (id != null) void this.load(id);
    });

    this.destroyRef.onDestroy(() => {
      if (this.userTimer) clearTimeout(this.userTimer);
      if (this.orgTimer) clearTimeout(this.orgTimer);
    });

    void this.loadGroups();
  }

  isRequired(name: keyof typeof this.form.controls): boolean {
    return this.form.controls[name].hasValidator(AngularValidators.required);
  }

  registrationFilterLabel(filter: CustomNotificationRegistrationFilter | string | null | undefined): string {
    return customNotificationRegistrationFilterLabel(filter);
  }

  audienceChipLabel(label: string, filter: CustomNotificationRegistrationFilter): string {
    return filter === 'All' ? label : `${label} · ${this.registrationFilterLabel(filter)}`;
  }

  statusLabel(status: string): string {
    return customNotificationStatusLabel(status);
  }

  formatDate(iso: string | null | undefined): string {
    if (!iso) return '—';
    const d = new Date(iso);
    return Number.isNaN(d.getTime()) ? iso : d.toLocaleString();
  }

  trackTarget(_i: number, t: CustomNotificationTargetDto): string {
    return targetKey(t);
  }

  groupLabel(g: IdentityAccessUserGroupDto): string {
    return (this.locale.locale() === 'ar' ? g.NameAr || g.NameEn : g.NameEn || g.NameAr) || g.Code;
  }

  userLabel(u: IdentityAccessUserRowDto): string {
    const name = (this.locale.locale() === 'ar' ? u.NameAr || u.NameEn : u.NameEn || u.NameAr)?.trim();
    return name ? `${name} (${u.Email})` : u.Email;
  }

  orgLabel(o: IdentityAccessOrganizationRowDto): string {
    const name = (this.locale.locale() === 'ar' ? o.NameAr || o.NameEn : o.NameEn || o.NameAr)?.trim();
    return name || o.CrNumber || String(o.Id);
  }

  onUserQueryChange(value: string): void {
    this.userQuery.set(value ?? '');
    if (this.userTimer) clearTimeout(this.userTimer);
    const q = this.userQuery().trim();
    if (!q) {
      this.userResults.set([]);
      return;
    }
    this.userTimer = setTimeout(async () => {
      try {
        const page = await firstValueFrom(this.identityApi.getUsersPage({ pageIndex: 0, pageSize: LOOKUP_PAGE_SIZE, q, isActive: true }));
        this.userResults.set(page?.rows ?? []);
      } catch {
        this.userResults.set([]);
      }
    }, LOOKUP_DEBOUNCE_MS);
  }

  onOrgQueryChange(value: string): void {
    this.orgQuery.set(value ?? '');
    if (this.orgTimer) clearTimeout(this.orgTimer);
    const q = this.orgQuery().trim();
    if (!q) {
      this.orgResults.set([]);
      return;
    }
    this.orgTimer = setTimeout(async () => {
      try {
        const page = await firstValueFrom(this.identityApi.getOrganizationsPage({ pageIndex: 0, pageSize: LOOKUP_PAGE_SIZE, q }));
        this.orgResults.set(page?.rows ?? []);
      } catch {
        this.orgResults.set([]);
      }
    }, LOOKUP_DEBOUNCE_MS);
  }

  addUser(u: IdentityAccessUserRowDto): void {
    this.setTargets(addTarget(this.targets(), { TargetType: 'User', UserId: u.Id, Label: this.userLabel(u) }));
    this.userQuery.set('');
    this.userResults.set([]);
  }

  addSelectedGroup(): void {
    const id = Number(this.selectedGroupId());
    const g = this.groups().find((x) => x.Id === id);
    if (!g) return;
    this.setTargets(addTarget(this.targets(), { TargetType: 'UserGroup', UserGroupId: g.Id, Label: this.groupLabel(g) }));
    this.selectedGroupId.set('');
  }

  addOrganization(o: IdentityAccessOrganizationRowDto): void {
    this.setTargets(addTarget(this.targets(), { TargetType: 'Organization', OrganizationId: o.Id, Label: this.orgLabel(o) }));
    this.orgQuery.set('');
    this.orgResults.set([]);
  }

  toggleAllOrganizations(enabled: boolean): void {
    this.setAllOrganizationsTarget(enabled, this.allOrganizationsFilter());
  }

  setAllOrganizationsFilter(value: string): void {
    this.setAllOrganizationsTarget(true, normalizeRegistrationFilter(value));
  }

  toggleJobSeekers(enabled: boolean): void {
    this.setTargets(setJobSeekers(this.targets(), enabled, this.allJobSeekersLabel, this.jobSeekersFilter()));
  }

  setJobSeekersFilter(value: string): void {
    this.setTargets(setJobSeekers(this.targets(), true, this.allJobSeekersLabel, normalizeRegistrationFilter(value)));
  }

  private setAllOrganizationsTarget(enabled: boolean, filter: CustomNotificationRegistrationFilter): void {
    this.setTargets(setAllOrganizations(this.targets(), enabled, this.allOrganizationsLabel, filter));
    if (enabled && filter === 'All') {
      this.orgQuery.set('');
      this.orgResults.set([]);
    }
  }

  remove(t: CustomNotificationTargetDto): void {
    this.setTargets(removeTarget(this.targets(), t));
  }

  async save(): Promise<void> {
    if (this.readOnly()) return;
    const id = this.notificationId();
    const result = await this.formPipeline.run({
      form: this.form,
      work: () => {
        const body = this.buildRequest();
        return firstValueFrom(id == null ? this.api.createAdminNotification(body) : this.api.updateAdminNotification(id, body));
      },
      execute: {
        progressMessage: $localize`:@@jobseeker.common.saving:Saving…`,
        successMessage: this.form.controls.SendNow.value
          ? $localize`:@@jobseeker.customNotifications.queuedSuccess:Notification queued for sending.`
          : $localize`:@@jobseeker.customNotifications.scheduledSuccess:Notification scheduled.`,
      },
    });
    if (!isFormPipelineSuccess(result)) return;
    void this.router.navigate([this.backToList]);
  }

  cancelEdit(): void {
    void this.router.navigate([this.backToList]);
  }

  private buildRequest(): UpsertCustomNotificationRequestDto {
    const v = this.form.getRawValue();
    return {
      Title_en: v.Title_en.trim(),
      Title_ar: v.Title_ar.trim(),
      Body_en: v.Body_en.trim(),
      Body_ar: v.Body_ar.trim(),
      SendEmail: v.SendEmail,
      SendSms: v.SendSms,
      SendSystem: v.SendSystem,
      SendNow: v.SendNow,
      ScheduledAtUtc: v.SendNow ? null : localInputToIso(v.ScheduledAt),
      Targets: v.Targets,
    };
  }

  private setTargets(targets: CustomNotificationTargetDto[]): void {
    const c = this.form.controls.Targets;
    c.setValue(targets);
    c.markAsTouched();
    c.markAsDirty();
  }

  private applySmsLimits(sms: boolean): void {
    for (const c of [this.form.controls.Body_en, this.form.controls.Body_ar]) {
      c.setValidators(
        sms ? [AngularValidators.required, AngularValidators.maxLength(SMS_BODY_MAX)] : AngularValidators.required
      );
      c.updateValueAndValidity({ emitEvent: false });
    }
  }

  private async refreshPreview(targets: CustomNotificationTargetDto[]): Promise<void> {
    if (!targets?.length) {
      this.preview.set(null);
      return;
    }
    try {
      this.preview.set(await firstValueFrom(this.api.previewRecipients(targets)));
    } catch {
      this.preview.set(null);
    }
  }

  private async loadGroups(): Promise<void> {
    try {
      const types = await firstValueFrom(this.identityApi.getGroups());
      this.groups.set((types ?? []).flatMap((t) => t.Groups ?? []));
    } catch {
      this.groups.set([]);
    }
  }

  private async load(id: number): Promise<void> {
    const row = await firstValueFrom(this.api.getAdminNotification(id));
    this.loaded.set(row);
    this.form.reset({
      Title_en: row.Title_en,
      Title_ar: row.Title_ar,
      Body_en: row.Body_en,
      Body_ar: row.Body_ar,
      SendEmail: row.SendEmail,
      SendSms: row.SendSms,
      SendSystem: row.SendSystem,
      SendNow: false,
      ScheduledAt: isoToLocalInput(row.ScheduledAtUtc),
      Targets: row.Targets ?? [],
    });
    if (!row.CanEdit) {
      this.form.disable();
    }
  }
}
