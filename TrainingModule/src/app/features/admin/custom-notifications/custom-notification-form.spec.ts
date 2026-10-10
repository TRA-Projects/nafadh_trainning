import { FormBuilder } from '@angular/forms';

import { buildCustomNotificationForm } from './custom-notification-manage-page.component';
import {
  addTarget,
  normalizeRegistrationFilter,
  registrationFilterOf,
  setAllOrganizations,
  setJobSeekers,
} from './custom-notification-targets.util';

describe('Custom notification form rules', () => {
  function validForm() {
    const form = buildCustomNotificationForm(new FormBuilder());
    form.patchValue({
      Title_en: 'Hello',
      Title_ar: 'مرحبا',
      Body_en: 'Body',
      Body_ar: 'نص',
      Targets: [{ TargetType: 'User', UserId: 'u1', Label: 'User 1' }],
    });
    return form;
  }

  it('requires at least one channel and at least one target', () => {
    const form = validForm();
    expect(form.valid).toBeTrue();

    form.patchValue({ SendEmail: false, SendSms: false, SendSystem: false });
    expect(form.errors?.['notificationChannelRequired']).toBeTrue();

    form.patchValue({ SendSystem: true, Targets: [] });
    expect(form.controls.Targets.errors?.['notificationTargetRequired']).toBeTruthy();
  });

  it('validates the schedule only when "Send later" is chosen, and rejects past times', () => {
    const form = validForm();
    const schedule = form.controls.ScheduledAt;
    expect(schedule.disabled).toBeTrue();

    schedule.enable();
    schedule.setValue('2000-01-01T10:00');
    expect(schedule.errors?.['notificationSchedulePast']).toBeTrue();

    const future = new Date(Date.now() + 3_600_000);
    schedule.setValue(future.toISOString());
    expect(schedule.valid).toBeTrue();
  });

  it('dedupes targets and lets "All organizations" replace individual organizations', () => {
    let targets = addTarget([], { TargetType: 'User', UserId: 'ABC', Label: 'A' });
    targets = addTarget(targets, { TargetType: 'User', UserId: 'abc', Label: 'A again' });
    targets = addTarget(targets, { TargetType: 'Organization', OrganizationId: 7, Label: 'Org 7' });
    expect(targets.length).toBe(2);

    targets = setAllOrganizations(targets, true, 'All');
    expect(targets.map((t) => t.TargetType)).toEqual(['User', 'AllOrganizations']);

    targets = addTarget(targets, { TargetType: 'Organization', OrganizationId: 8, Label: 'Org 8' });
    expect(targets.some((t) => t.TargetType === 'Organization')).toBeFalse();
  });

  it('filtered "All organizations" keeps individual organizations and the job seekers audience carries its filter', () => {
    let targets = addTarget([], { TargetType: 'Organization', OrganizationId: 7, Label: 'Org 7' });
    targets = setAllOrganizations(targets, true, 'All organizations', 'NotRegistered');
    targets = addTarget(targets, { TargetType: 'Organization', OrganizationId: 8, Label: 'Org 8' });
    expect(targets.filter((t) => t.TargetType === 'Organization').length).toBe(2);
    expect(registrationFilterOf(targets, 'AllOrganizations')).toBe('NotRegistered');

    targets = setJobSeekers(targets, true, 'All job seekers', 'Registered');
    targets = setJobSeekers(targets, true, 'All job seekers', normalizeRegistrationFilter('notregistered'));
    expect(targets.filter((t) => t.TargetType === 'JobSeekers').length).toBe(1);
    expect(registrationFilterOf(targets, 'JobSeekers')).toBe('NotRegistered');

    targets = setJobSeekers(targets, false, 'All job seekers');
    expect(registrationFilterOf(targets, 'JobSeekers')).toBeNull();
    expect(normalizeRegistrationFilter('bogus')).toBe('All');
  });
});
