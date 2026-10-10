import { FormBuilder } from '@angular/forms';

import type { ShowcaseItemDto, UpsertShowcaseItemRequestDto } from '../../../core/api/models/showcase.models';
import { AngularValidators, dateRangeOrder, emailFormat, minNumber, phoneDigits, requiredTrimmed } from '@validators';

export const SHOWCASE_TITLE_MAX = 200;
export const SHOWCASE_DESCRIPTION_MAX = 1000;
export const SHOWCASE_MIN_AMOUNT = 1;

/** Shared instance so `hasValidator` can detect it for the required `*` marker. */
export const showcaseTitleRequired = requiredTrimmed();

/** Same rules as `ShowcaseRules.Validate` on the API. */
export function buildShowcaseForm(fb: FormBuilder) {
  return fb.group(
    {
      Title_en: fb.nonNullable.control('', [showcaseTitleRequired, AngularValidators.maxLength(SHOWCASE_TITLE_MAX)]),
      Title_ar: fb.nonNullable.control('', [showcaseTitleRequired, AngularValidators.maxLength(SHOWCASE_TITLE_MAX)]),
      Category: fb.nonNullable.control('', AngularValidators.required),
      Status: fb.nonNullable.control('Draft', AngularValidators.required),
      ContactEmail: fb.nonNullable.control('', [AngularValidators.required, emailFormat()]),
      PhoneNumber: fb.nonNullable.control('', phoneDigits()),
      Amount: fb.control<number | null>(null, [AngularValidators.required, minNumber(SHOWCASE_MIN_AMOUNT)]),
      StartDate: fb.nonNullable.control('', AngularValidators.required),
      EndDate: fb.nonNullable.control('', AngularValidators.required),
      IsUrgent: fb.nonNullable.control(false),
      Description: fb.nonNullable.control('', AngularValidators.maxLength(SHOWCASE_DESCRIPTION_MAX)),
    },
    { validators: dateRangeOrder('StartDate', 'EndDate') }
  );
}

export type ShowcaseForm = ReturnType<typeof buildShowcaseForm>;

/** `yyyy-MM-dd` for `<input type="date">`. */
export function toDateInput(iso: string | null | undefined): string {
  return iso ? iso.slice(0, 10) : '';
}

export function showcaseFormValue(item: ShowcaseItemDto) {
  return {
    Title_en: item.Title_en,
    Title_ar: item.Title_ar,
    Category: item.Category,
    Status: item.Status,
    ContactEmail: item.ContactEmail,
    PhoneNumber: item.PhoneNumber ?? '',
    Amount: item.Amount,
    StartDate: toDateInput(item.StartDate),
    EndDate: toDateInput(item.EndDate),
    IsUrgent: item.IsUrgent,
    Description: item.Description ?? '',
  };
}

export function showcaseRequest(form: ShowcaseForm): UpsertShowcaseItemRequestDto {
  const v = form.getRawValue();
  return {
    Title_en: v.Title_en.trim(),
    Title_ar: v.Title_ar.trim(),
    Category: v.Category,
    Status: v.Status,
    ContactEmail: v.ContactEmail.trim(),
    PhoneNumber: v.PhoneNumber.trim(),
    Amount: v.Amount == null ? null : Number(v.Amount),
    StartDate: v.StartDate || null,
    EndDate: v.EndDate || null,
    IsUrgent: v.IsUrgent,
    Description: v.Description.trim(),
  };
}
