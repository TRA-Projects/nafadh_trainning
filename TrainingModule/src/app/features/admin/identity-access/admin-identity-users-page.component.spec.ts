import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';

import type { IdentityAccessUserRowDto } from '../../../core/api/models/identity-access.models';
import { IdentityAccessApiService } from '../../../core/api/services/identity-access-api.service';
import { AuthService } from '../../../core/auth/auth.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { FormPipelineService } from '../../../core/ui/form-pipeline.service';
import { AdminIdentityUsersPageComponent } from './admin-identity-users-page.component';

function user(overrides: Partial<IdentityAccessUserRowDto> = {}): IdentityAccessUserRowDto {
  return {
    Id: 'acc-1',
    ProfileId: 101,
    NameEn: 'Ada Lovelace',
    NameAr: 'آدا',
    Email: 'ada@example.com',
    UserTypeId: 1,
    UserTypeCode: 'ADMIN',
    UserTypeNameEn: 'Admin',
    UserTypeNameAr: 'مشرف',
    UserGroupId: 2,
    UserGroupCode: 'OPS',
    UserGroupNameEn: 'Ops',
    UserGroupNameAr: 'عمليات',
    IsActive: true,
    IsPrimary: false,
    HasRequesterCapability: false,
    CanRemove: true,
    IsSelf: false,
    ...overrides,
  };
}

describe('AdminIdentityUsersPageComponent row actions menu', () => {
  let fixture: ComponentFixture<AdminIdentityUsersPageComponent>;
  let component: AdminIdentityUsersPageComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminIdentityUsersPageComponent],
      providers: [
        {
          provide: IdentityAccessApiService,
          useValue: {
            getGroups: () => of([]),
            getUsersPage: () => of({ rows: [], total: 0, pageIndex: 0, pageSize: 5 }),
            createAdminUser: () => of({}),
            updateAdminUser: () => of({}),
            changeUserGroup: () => of({}),
            removeAccount: () => of({}),
          },
        },
        {
          provide: Store,
          useValue: {
            select: () => of(null),
          },
        },
        {
          provide: AsyncExecutorService,
          useValue: {
            execute: <T>(work: () => Promise<T>) => work(),
          },
        },
        {
          provide: FormPipelineService,
          useValue: {
            run: async () => ({ ok: true }),
          },
        },
        {
          provide: LocaleService,
          useValue: { locale: () => 'en' as const },
        },
        {
          provide: AuthService,
          useValue: { homePath: () => '/admin/dashboard' },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminIdentityUsersPageComponent);
    component = fixture.componentInstance;
  });

  it('ToggleRowActionsMenu_OpensAndClosesSameRow', () => {
    const event = { stopPropagation: jasmine.createSpy('stopPropagation') } as unknown as Event;
    component.toggleRowActionsMenu('acc-1', event);
    expect(component.actionsMenuRowId()).toBe('acc-1');
    component.toggleRowActionsMenu('acc-1', event);
    expect(component.actionsMenuRowId()).toBeNull();
  });

  it('OpenEditUser_ClosesMenuAndOpensModal', () => {
    component.actionsMenuRowId.set('acc-1');
    component.openEditUser(user({ Id: 'acc-1', NameEn: 'Ada Lovelace', Email: 'ada@example.com', IsActive: true }));
    expect(component.actionsMenuRowId()).toBeNull();
    expect(component.editingUser()?.Id).toBe('acc-1');
    expect(component.createUserOpen()).toBeFalse();
    expect(component.editForm.controls.name.value).toBe('Ada Lovelace');
    expect(component.editForm.controls.email.value).toBe('ada@example.com');
    expect(component.editForm.controls.isActive.value).toBeTrue();
  });

  it('OpenEditUser_DoesNotExposeGroupControl', () => {
    component.openEditUser(user({ UserGroupId: 99 }));
    expect((component.editForm.controls as { userGroupId?: unknown }).userGroupId).toBeUndefined();
  });

  it('OpenAddFromHeader_OpensCreateUserModal', () => {
    component.actionsMenuRowId.set('acc-1');
    component.openAddFromHeader();
    expect(component.actionsMenuRowId()).toBeNull();
    expect(component.editingUser()).toBeNull();
    expect(component.createUserOpen()).toBeTrue();
  });

  it('AdminGroups_OnlySystemTypeActiveGroups', () => {
    Object.defineProperty(component, 'types', {
      configurable: true,
      value: () => [
        {
          Id: 1,
          Code: 'System',
          NameEn: 'System',
          NameAr: 'نظام',
          Groups: [
            { Id: 10, NameEn: 'Admin', NameAr: 'مشرف', IsActive: true },
            { Id: 11, NameEn: 'Inactive', NameAr: 'معطل', IsActive: false },
          ],
        },
        {
          Id: 2,
          Code: 'Corporate',
          NameEn: 'Corporate',
          NameAr: 'شركات',
          Groups: [{ Id: 20, NameEn: 'SME', NameAr: 'منشآت', IsActive: true }],
        },
      ],
    });
    expect(component.adminGroups().map((g) => g.Id)).toEqual([10]);
  });

  it('ClearFilters_ResetsDraftAndAppliedFilters', () => {
    component.draftQuery.set('ada');
    component.query.set('ada');
    component.draftTypeId.set(1);
    component.typeId.set(1);
    component.draftActive.set('active');
    component.activeFilter.set('active');
    component.clearFilters();
    expect(component.draftQuery()).toBe('');
    expect(component.query()).toBe('');
    expect(component.typeId()).toBeNull();
    expect(component.activeFilter()).toBe('all');
    expect(component.hasActiveFilters()).toBeFalse();
    expect(component.drawerOpen()).toBeFalse();
  });

  it('RemoveAccount_BlockedWhenCanRemoveFalse', async () => {
    const confirmSpy = spyOn(window, 'confirm').and.returnValue(true);
    component.actionsMenuRowId.set('acc-1');
    await component.removeAccount(user({ CanRemove: false }));
    expect(confirmSpy).not.toHaveBeenCalled();
    expect(component.actionsMenuRowId()).toBeNull();
  });
});
