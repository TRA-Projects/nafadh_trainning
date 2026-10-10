import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';

import { PAGE_ACTION_FLAGS } from '../../../core/api/models/identity-access.models';
import { IdentityAccessApiService } from '../../../core/api/services/identity-access-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { AdminIdentityGroupPagesPageComponent } from './admin-identity-group-pages-page.component';

describe('AdminIdentityGroupPagesPageComponent', () => {
  let navigateByUrl: jasmine.Spy;

  async function setup(navState: { roleId?: string; groupId?: number; source?: 'groups' | 'roles' } | null) {
    navigateByUrl = jasmine.createSpy('navigateByUrl');
    const getCurrentNavigation = jasmine
      .createSpy('getCurrentNavigation')
      .and.returnValue(navState ? { extras: { state: navState } } : null);

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
            Id: 2,
            UserTypeId: 1,
            Code: 'OPS',
            NameEn: 'Operations',
            NameAr: 'عمليات',
            SortOrder: 1,
            IsActive: true,
            TemplateRoleId: 'role-tpl',
            TemplateRoleName: 'Admin.OPS',
          },
        ],
      },
    ];

    await TestBed.configureTestingModule({
      imports: [AdminIdentityGroupPagesPageComponent],
      providers: [
        {
          provide: IdentityAccessApiService,
          useValue: {
            getGroups: () => of(catalog),
            getRolePages: () =>
              of({
                RoleId: 'role-tpl',
                RoleName: 'Admin.OPS',
                Pages: [{ Id: 10, Code: 'DASH', NameEn: 'Dashboard', NameAr: 'لوحة', SystemCode: 'Core' }],
                SelectedPageIds: [10],
                Grants: [{ AppPageId: 10, ActionFlags: PAGE_ACTION_FLAGS.View, DataScope: 'mine' }],
              }),
            updateRolePages: () => of({}),
          },
        },
        { provide: Store, useValue: { select: () => of(catalog) } },
        { provide: Router, useValue: { navigateByUrl, getCurrentNavigation } },
        {
          provide: AsyncExecutorService,
          useValue: { execute: <T>(work: () => Promise<T>) => work() },
        },
        { provide: LocaleService, useValue: { locale: () => 'en' as const } },
      ],
    })
      .overrideComponent(AdminIdentityGroupPagesPageComponent, { set: { template: '', styles: [] } })
      .compileComponents();

    return TestBed.createComponent(AdminIdentityGroupPagesPageComponent).componentInstance;
  }

  afterEach(() => {
    TestBed.resetTestingModule();
  });

  it('ClaimsEnabled_UnselectedPage_IsFalse', async () => {
    const component = await setup({ roleId: 'role-1', source: 'roles' });
    component.loadState.set('ready');
    expect(component.claimsEnabled(10)).toBeFalse();
  });

  it('ClaimsEnabled_SelectedPage_IsTrue', async () => {
    const component = await setup({ roleId: 'role-1', source: 'roles' });
    component.loadState.set('ready');
    component.togglePage(10, true);
    expect(component.claimsEnabled(10)).toBeTrue();
  });

  it('ToggleAction_UnselectedPage_DoesNotMutateGrants', async () => {
    const component = await setup({ roleId: 'role-1', source: 'roles' });
    component.loadState.set('ready');
    component.toggleAction(10, PAGE_ACTION_FLAGS.Create, true);
    expect(component.pageGrants()[10]).toBeUndefined();
    expect(component.hasAction(10, PAGE_ACTION_FLAGS.Create)).toBeFalse();
  });

  it('Load_RoleIdFromNavState_SetsReady', async () => {
    const component = await setup({ roleId: 'role-tpl', source: 'roles' });
    await component.ngOnInit();
    expect(component.loadState()).toBe('ready');
    expect(component.roleId()).toBe('role-tpl');
    expect(component.backToList()).toBe('/admin/roles');
  });

  it('Load_GroupIdFromNavState_ResolvesTemplateRole', async () => {
    const component = await setup({ groupId: 2, source: 'groups' });
    await component.ngOnInit();
    expect(component.loadState()).toBe('ready');
    expect(component.roleId()).toBe('role-tpl');
    expect(component.backToList()).toBe('/admin/groups');
  });

  it('Load_MissingNavState_SetsError', async () => {
    const component = await setup(null);
    await component.ngOnInit();
    expect(component.loadState()).toBe('error');
  });

  it('Cancel_NavigatesBackUsingSource', async () => {
    const component = await setup({ roleId: 'role-tpl', source: 'roles' });
    await component.ngOnInit();
    component.cancel();
    expect(navigateByUrl).toHaveBeenCalledWith('/admin/roles');
  });
});
