import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { of, throwError } from 'rxjs';

import { IdentityAccessApiService } from '../../../core/api/services/identity-access-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { FormPipelineService } from '../../../core/ui/form-pipeline.service';
import { AdminIdentityGroupCreatePageComponent } from './admin-identity-group-create-page.component';

const catalog = [
  {
    Id: 1,
    Code: 'ADMIN',
    NameEn: 'Admin',
    NameAr: 'مشرف',
    SortOrder: 1,
    HasRequesterCapability: false,
    Groups: [],
  },
];

describe('AdminIdentityGroupCreatePageComponent', () => {
  let navigateByUrl: jasmine.Spy;
  let createGroup: jasmine.Spy;
  let formPipelineRun: jasmine.Spy;

  async function setup(opts?: { failLoad?: boolean }): Promise<AdminIdentityGroupCreatePageComponent> {
    navigateByUrl = jasmine.createSpy('navigateByUrl');
    createGroup = jasmine.createSpy('createGroup').and.returnValue(of({ Id: 99 }));
    formPipelineRun = jasmine.createSpy('run').and.callFake(async (opts: { work: () => Promise<unknown> }) => {
      await opts.work();
      return { ok: true };
    });

    await TestBed.configureTestingModule({
      imports: [AdminIdentityGroupCreatePageComponent],
      providers: [
        {
          provide: IdentityAccessApiService,
          useValue: {
            getGroups: () => (opts?.failLoad ? throwError(() => new Error('fail')) : of(catalog)),
            createGroup,
          },
        },
        { provide: Store, useValue: { select: () => of(catalog) } },
        { provide: Router, useValue: { navigateByUrl } },
        {
          provide: AsyncExecutorService,
          useValue: { execute: <T>(work: () => Promise<T>) => work() },
        },
        { provide: FormPipelineService, useValue: { run: formPipelineRun } },
        { provide: LocaleService, useValue: { locale: () => 'en' as const } },
      ],
    })
      .overrideComponent(AdminIdentityGroupCreatePageComponent, { set: { template: '', styles: [] } })
      .compileComponents();

    return TestBed.createComponent(AdminIdentityGroupCreatePageComponent).componentInstance;
  }

  afterEach(() => {
    TestBed.resetTestingModule();
  });

  it('Load_Catalog_SetsFormReady', async () => {
    const component = await setup();
    await component.ngOnInit();
    expect(component.loadState()).toBe('ready');
    expect(component.types().length).toBe(1);
  });

  it('Load_CatalogFails_SetsError', async () => {
    const component = await setup({ failLoad: true });
    await component.ngOnInit();
    expect(component.loadState()).toBe('error');
  });

  it('Save_UsesFormPipelineAndNavigatesBack', async () => {
    const component = await setup();
    await component.ngOnInit();
    component.form.setValue({
      userTypeId: 1,
      code: 'OPS',
      nameEn: 'Operations',
      nameAr: 'عمليات',
      sortOrder: 0,
      isActive: true,
    });
    await component.save();
    expect(formPipelineRun).toHaveBeenCalled();
    expect(createGroup).toHaveBeenCalled();
    expect(navigateByUrl).toHaveBeenCalledWith('/admin/groups');
  });

  it('Form_ControlsStartUntouched', async () => {
    const component = await setup();
    expect(component.form.controls.nameEn.touched).toBeFalse();
    expect(component.form.controls.userTypeId.touched).toBeFalse();
    expect(component.form.controls.code.touched).toBeFalse();
  });
});
