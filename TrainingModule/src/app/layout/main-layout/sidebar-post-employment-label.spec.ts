import arMessages from '../../core/translations/layout/messages.ar.json';

describe('sidebar post-employment label', () => {
  const label = 'إجراءات ما بعد التوظيف والتدريب';

  it('renames the admin sidebar item', () => {
    expect(arMessages.translations['jobseeker.layout.nav.admin.postEmploymentProcedures']).toBe(label);
  });

  it('renames the operator sidebar item', () => {
    expect(arMessages.translations['jobseeker.layout.nav.operator.postEmploymentProcedures']).toBe(label);
  });

  it('keeps the employment management group name unchanged', () => {
    expect(arMessages.translations['jobseeker.layout.nav.admin.employmentManagement']).toBe('إدارة التوظيف');
    expect(arMessages.translations['jobseeker.layout.nav.operator.employmentManagement']).toBe('إدارة التوظيف');
  });
});
