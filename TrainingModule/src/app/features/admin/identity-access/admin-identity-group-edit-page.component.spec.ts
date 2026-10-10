import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';

import { IdentityAccessApiService } from '../../../core/api/services/identity-access-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { FormPipelineService } from '../../../core/ui/form-pipeline.service';
import { AdminIdentityGroupEditPageComponent } from './admin-identity-group-edit-page.component';

const catalog = [
  {
    Id: 1,
    Code: 'ADMIN',
    NameEn: 'Admin',
    NameAr: 'مشرف',
    SortOrder: 1,
    HasRequesterCapability: false,
    Groups: [
      {
        Id: 11,
        UserTypeId: 1,
        Code: 'OPS',
        NameEn: 'Operations',
        NameAr: 'عمليات',
        SortOrder: 1,
        IsActive: true,
      },
    ],
  },
];

describe('AdminIdentityGroupEditPageComponent', () => {
  let navigateByUrl: jasmine.Spy;

  async function setup(
    groupId: number | null
  ): Promise<{ component: AdminIdentityGroupEditPageComponent; fixture: ComponentFixture<AdminIdentityGroupEditPageComponent> }> {
    navigateByUrl = jasmine.createSpy('navigateByUrl');
    const getCurrentNavigation = jasmine
      .createSpy('getCurrentNavigation')
      .and.returnValue(groupId == null ? null : { extras: { state: { groupId } } });

    await TestBed.configureTestingModule({
      imports: [AdminIdentityGroupEditPageComponent],
      providers: [
        {
          provide: IdentityAccessApiService,
          useValue: {
            getGroups: () => of(catalog),
            updateGroup: () => of({}),
          },
        },
        { provide: Store, useValue: { select: () => of(catalog) } },
        { provide: Router, useValue: { navigateByUrl, getCurrentNavigation } },
        {
          provide: AsyncExecutorService,
          useValue: { execute: <T>(work: () => Promise<T>) => work() },
        },
        { provide: FormPipelineService, useValue: { run: async () => ({ ok: true }) } },
        { provide: LocaleService, useValue: { locale: () => 'en' as const } },
      ],
    })
      .overrideComponent(AdminIdentityGroupEditPageComponent, { set: { template: '', styles: [] } })
      .compileComponents();

    const fixture = TestBed.createComponent(AdminIdentityGroupEditPageComponent);
    return { component: fixture.componentInstance, fixture };
  }

  afterEach(() => {
    TestBed.resetTestingModule();
  });

  it('Load_ValidGroupIdFromNavState_SetsFormReady', async () => {
    const { component } = await setup(11);
    await component.ngOnInit();
    expect(component.loadState()).toBe('ready');
    expect(component.group()?.Id).toBe(11);
    expect(component.form.controls.code.value).toBe('OPS');
  });

  it('Load_MissingGroupId_SetsError', async () => {
    const { component } = await setup(null);
    await component.ngOnInit();
    expect(component.loadState()).toBe('error');
    expect(component.loadError()).toBeTruthy();
  });

  it('Cancel_NavigatesBackToGroupsList', async () => {
    const { component } = await setup(11);
    component.cancel();
    expect(navigateByUrl).toHaveBeenCalledWith('/admin/groups');
  });
});
