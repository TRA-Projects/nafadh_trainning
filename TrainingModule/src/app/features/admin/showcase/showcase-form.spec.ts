import { FormBuilder } from '@angular/forms';

import { buildShowcaseForm, showcaseRequest } from './showcase-form';

describe('buildShowcaseForm', () => {
  function validForm() {
    const form = buildShowcaseForm(new FormBuilder());
    form.setValue({
      Title_en: ' Workshop ',
      Title_ar: 'ورشة',
      Category: 'Training',
      Status: 'Draft',
      ContactEmail: 'user@example.com',
      PhoneNumber: '+96891234567',
      Amount: 10,
      StartDate: '2026-01-01',
      EndDate: '2026-01-05',
      IsUrgent: false,
      Description: '',
    });
    return form;
  }

  it('CompleteForm_IsValid_AndTrimsTitle', () => {
    const form = validForm();
    expect(form.valid).toBeTrue();
    expect(showcaseRequest(form).Title_en).toBe('Workshop');
  });

  it('EndBeforeStart_IsInvalid', () => {
    const form = validForm();
    form.controls.EndDate.setValue('2025-12-31');
    expect(form.hasError('dateRangeOrder')).toBeTrue();
  });

  it('AmountBelowMinimum_AndBadEmail_AreInvalid', () => {
    const form = validForm();
    form.controls.Amount.setValue(0);
    form.controls.ContactEmail.setValue('not-an-email');
    expect(form.controls.Amount.hasError('numberBelowRange')).toBeTrue();
    expect(form.controls.ContactEmail.hasError('invalidEmail')).toBeTrue();
  });
});
