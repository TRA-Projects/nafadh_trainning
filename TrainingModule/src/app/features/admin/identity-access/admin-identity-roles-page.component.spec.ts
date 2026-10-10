import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';

import type { IdentityAccessRoleRowDto } from '../../../core/api/models/identity-access.models';
import { IdentityAccessApiService } from '../../../core/api/services/identity-access-api.service';
import { AuthService } from '../../../core/auth/auth.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { AdminIdentityRolesPageComponent } from './admin-identity-roles-page.component';

function role(overrides: Partial<IdentityAccessRoleRowDto> = {}): IdentityAccessRoleRowDto {
  return {
    Id: 'role-1',
    Name: 'Ops Admin',
    UserTypeId: 1,
    UserTypeCode: 'ADMIN',
    UserTypeNameEn: 'Admin',
    UserTypeNameAr: 'مشرف',
    UserGroupId: 2,
    UserGroupCode: 'OPS',
    UserGroupNameEn: 'Ops',
    UserGroupNameAr: 'عمليات',
    IsTemplate: false,
    CanDelete: true,
    ...overrides,
  };
}

describe('AdminIdentityRolesPageComponent', () => {
  let fixture: ComponentFixture<AdminIdentityRolesPageComponent>;
  let component: AdminIdentityRolesPageComponent;
  let navigateSpy: jasmine.Spy;
  let getRolesPage: jasmine.Spy;

  async function setup(rolesMode: 'custom' | 'template' = 'custom'): Promise<void> {
    navigateSpy = jasmine.createSpy('navigate');
    getRolesPage = jasmine
      .createSpy('getRolesPage')
      .and.returnValue(of({ rows: [], total: 0, pageIndex: 0, pageSize: 5 }));
    await TestBed.configureTestingModule({
      imports: [AdminIdentityRolesPageComponent],
      providers: [
        {
          provide: IdentityAccessApiService,
          useValue: {
            getGroups: () => of([]),
            getRolesPage,
            deleteRole: () => of({}),
          },
        },
        { provide: Store, useValue: { select: () => of(null) } },
        {
          provide: AsyncExecutorService,
          useValue: { execute: <T>(work: () => Promise<T>) => work() },
        },
        { provide: Router, useValue: { navigate: navigateSpy } },
        { provide: ActivatedRoute, useValue: { snapshot: { data: { rolesMode } } } },
        { provide: LocaleService, useValue: { locale: () => 'en' as const } },
        { provide: AuthService, useValue: { homePath: () => '/admin' } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminIdentityRolesPageComponent);
    component = fixture.componentInstance;
  }

  afterEach(() => {
    TestBed.resetTestingModule();
  });

  it('CustomMode_ShowsCreateAndRequestsNonTemplates', async () => {
    await setup('custom');
    await component.ngOnInit();
    expect(component.isTemplateMode).toBeFalse();
    expect(component.pageTitle.toLowerCase()).toContain('custom');
    expect(getRolesPage).toHaveBeenCalledWith(
      jasmine.objectContaining({ templatesOnly: false })
    );
    component.goToCreate();
    expect(navigateSpy).toHaveBeenCalledWith(['/admin/roles/create']);
  });

  it('TemplateMode_HidesCreateAndRequestsTemplatesOnly', async () => {
    await setup('template');
    await component.ngOnInit();
    expect(component.isTemplateMode).toBeTrue();
    expect(component.pageTitle.toLowerCase()).toContain('template');
    expect(getRolesPage).toHaveBeenCalledWith(
      jasmine.objectContaining({ templatesOnly: true })
    );
    fixture.detectChanges();
    const html = fixture.nativeElement as HTMLElement;
    expect(html.textContent).not.toContain('Create role');
  });

  it('OpenPages_FromTemplateMode_UsesTemplateRolesSource', async () => {
    await setup('template');
    component.openPages(role({ Id: 'tpl-9', IsTemplate: true }));
    expect(navigateSpy).toHaveBeenCalledWith(['/admin/roles/pages'], {
      state: { roleId: 'tpl-9', source: 'template-roles' },
    });
  });

  it('OpenPages_FromCustomMode_UsesRolesSource', async () => {
    await setup('custom');
    component.openPages(role({ Id: 'role-9' }));
    expect(navigateSpy).toHaveBeenCalledWith(['/admin/roles/pages'], {
      state: { roleId: 'role-9', source: 'roles' },
    });
  });

  it('DeleteRole_BlockedWhenCanDeleteFalse', async () => {
    await setup('custom');
    const confirmSpy = spyOn(window, 'confirm').and.returnValue(true);
    component.actionsMenuRowId.set('role-1');
    await component.deleteRole(role({ CanDelete: false }));
    expect(confirmSpy).not.toHaveBeenCalled();
    expect(component.actionsMenuRowId()).toBeNull();
  });

  it('RoleDisplayName_Template_UsesDisplayName', async () => {
    await setup('template');
    const label = component.roleDisplayName(
      role({
        Name: 'Corporate.SME',
        IsTemplate: true,
        DisplayNameEn: 'Small and medium enterprise',
        DisplayNameAr: 'منشأة صغيرة ومتوسطة',
        UserGroupNameEn: 'Small and medium enterprise',
        UserGroupNameAr: 'منشأة صغيرة ومتوسطة',
      })
    );
    expect(label).toBe('Small and medium enterprise');
    expect(label).not.toContain('.');
  });
});
