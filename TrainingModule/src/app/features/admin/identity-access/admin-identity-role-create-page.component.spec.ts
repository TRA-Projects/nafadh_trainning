import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { IdentityAccessApiService } from '../../../core/api/services/identity-access-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { FormPipelineService } from '../../../core/ui/form-pipeline.service';
import { AdminIdentityRoleCreatePageComponent } from './admin-identity-role-create-page.component';

describe('AdminIdentityRoleCreatePageComponent', () => {
  let navigateByUrl: jasmine.Spy;
  let createRole: jasmine.Spy;

  async function setup(opts?: { failLoad?: boolean }): Promise<AdminIdentityRoleCreatePageComponent> {
    navigateByUrl = jasmine.createSpy('navigateByUrl');
    createRole = jasmine.createSpy('createRole').and.returnValue(of({ Id: 'new-role' }));
    await TestBed.configureTestingModule({
      imports: [AdminIdentityRoleCreatePageComponent],
      providers: [
        {
          provide: IdentityAccessApiService,
          useValue: {
            getRolesPage: () =>
              opts?.failLoad
                ? throwError(() => new Error('fail'))
                : of({
                    rows: [
                      {
                        Id: 'tpl-1',
                        Name: 'Corporate.SME',
                        DisplayNameEn: 'Small and medium enterprise',
                        DisplayNameAr: 'منشأة صغيرة ومتوسطة',
                        IsTemplate: true,
                        CanDelete: false,
                      },
                    ],
                    total: 1,
                    pageIndex: 0,
                    pageSize: 15,
                  }),
            createRole,
          },
        },
        { provide: Router, useValue: { navigateByUrl } },
        {
          provide: AsyncExecutorService,
          useValue: { execute: <T>(work: () => Promise<T>) => work() },
        },
        {
          provide: FormPipelineService,
          useValue: {
            run: async (opts: { work: () => Promise<unknown> }) => {
              await opts.work();
              return { ok: true };
            },
          },
        },
        { provide: LocaleService, useValue: { locale: () => 'en' as const } },
      ],
    })
      .overrideComponent(AdminIdentityRoleCreatePageComponent, { set: { template: '', styles: [] } })
      .compileComponents();

    return TestBed.createComponent(AdminIdentityRoleCreatePageComponent).componentInstance;
  }

  afterEach(() => {
    TestBed.resetTestingModule();
  });

  it('Load_Catalog_SetsFormReady', async () => {
    const component = await setup();
    await component.ngOnInit();
    expect(component.loadState()).toBe('ready');
    expect(component.templateRoles().length).toBe(1);
  });

  it('Load_CatalogFails_SetsError', async () => {
    const component = await setup({ failLoad: true });
    await component.ngOnInit();
    expect(component.loadState()).toBe('error');
    expect(component.loadError()).toBeTruthy();
  });

  it('Cancel_NavigatesBackToRolesList', async () => {
    const component = await setup();
    component.cancel();
    expect(navigateByUrl).toHaveBeenCalledWith('/admin/roles');
  });

  it('Save_OmitsOrganizationId_UsesCallerOrgOnApi', async () => {
    const component = await setup();
    await component.ngOnInit();
    component.form.setValue({ name: 'Ops Custom', parentRoleId: 'tpl-1' });
    await component.save();
    expect(createRole).toHaveBeenCalledWith({
      Name: 'Ops Custom',
      ParentRoleId: 'tpl-1',
    });
    expect(navigateByUrl).toHaveBeenCalledWith('/admin/roles');
  });

  it('Form_HasNoOrganizationControl', async () => {
    const component = await setup();
    expect(component.form.contains('organizationId')).toBeFalse();
    expect(component.form.controls.name.touched).toBeFalse();
    expect(component.form.controls.parentRoleId.touched).toBeFalse();
  });

  it('TemplateLabel_UsesDisplayNameNotInternalCode', async () => {
    const component = await setup();
    await component.ngOnInit();
    const label = component.templateLabel(component.templateRoles()[0]);
    expect(label).toBe('Small and medium enterprise');
    expect(label).not.toContain('Corporate.SME');
  });
});
