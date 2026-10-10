import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';

import type { IdentityAccessUserGroupDto, IdentityAccessUserTypeDto } from '../../../core/api/models/identity-access.models';
import { IdentityAccessApiService } from '../../../core/api/services/identity-access-api.service';
import { AuthService } from '../../../core/auth/auth.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { AdminIdentityGroupsPageComponent } from './admin-identity-groups-page.component';

function typeWithGroups(): IdentityAccessUserTypeDto {
  return {
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
        CanDeactivate: true,
      } as IdentityAccessUserGroupDto,
    ],
  };
}

describe('AdminIdentityGroupsPageComponent navigation', () => {
  let fixture: ComponentFixture<AdminIdentityGroupsPageComponent>;
  let component: AdminIdentityGroupsPageComponent;
  let navigateSpy: jasmine.Spy;

  beforeEach(async () => {
    const catalog = [typeWithGroups()];
    navigateSpy = jasmine.createSpy('navigate');
    await TestBed.configureTestingModule({
      imports: [AdminIdentityGroupsPageComponent],
      providers: [
        {
          provide: IdentityAccessApiService,
          useValue: {
            getGroups: () => of(catalog),
            deactivateGroup: () => of({}),
          },
        },
        { provide: Store, useValue: { select: () => of(catalog) } },
        {
          provide: AsyncExecutorService,
          useValue: { execute: <T>(work: () => Promise<T>) => work() },
        },
        { provide: LocaleService, useValue: { locale: () => 'en' as const } },
        { provide: AuthService, useValue: { homePath: () => '/admin' } },
        { provide: Router, useValue: { navigate: navigateSpy } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminIdentityGroupsPageComponent);
    component = fixture.componentInstance;
  });

  it('OpenEdit_NavigatesToEditPageWithoutIdInUrl', () => {
    const type = typeWithGroups();
    const row = { type, group: type.Groups[0] };
    component.actionsMenuRowId.set(11);
    component.openEdit(row);
    expect(component.actionsMenuRowId()).toBeNull();
    expect(navigateSpy).toHaveBeenCalledWith(['/admin/groups/edit'], { state: { groupId: 11 } });
  });

  it('OpenPages_NavigatesToPagesPage', () => {
    const type = typeWithGroups();
    const row = { type, group: type.Groups[0] };
    component.openPages(row);
    expect(navigateSpy).toHaveBeenCalledWith(['/admin/groups/pages'], {
      state: { groupId: 11, source: 'groups' },
    });
  });

  it('GoToCreate_NavigatesToCreatePage', () => {
    component.actionsMenuRowId.set(11);
    component.goToCreate();
    expect(component.actionsMenuRowId()).toBeNull();
    expect(navigateSpy).toHaveBeenCalledWith(['/admin/groups/create']);
  });

  it('CreateModal_RemovedFromListPage', () => {
    expect((component as unknown as { creating?: unknown }).creating).toBeUndefined();
    expect((component as unknown as { openCreate?: unknown }).openCreate).toBeUndefined();
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).querySelector('form')).toBeNull();
  });

  it('ListColumns_OrderIsGroupNameCodeTypeRole', () => {
    component.listLoadState.set('ready');
    fixture.detectChanges();
    const headers = Array.from(
      (fixture.nativeElement as HTMLElement).querySelectorAll('.ia-table thead th')
    ).map((th) => (th.textContent ?? '').trim());
    expect(headers.slice(1, 5)).toEqual(['Group name', 'Code', 'Type', 'Role']);
  });

  it('MobileCard_ShowsGroupNameThenLighterCode', () => {
    component.listLoadState.set('ready');
    fixture.detectChanges();
    const card = (fixture.nativeElement as HTMLElement).querySelector('.ia-card') as HTMLElement | null;
    expect(card).toBeTruthy();
    expect(card!.querySelector('.ia-card__label')?.textContent?.trim()).toBe('Group name');
    expect(card!.querySelector('.ia-card__name')?.textContent?.trim()).toBe('Operations');
    expect(card!.querySelector('.ia-card__code')?.textContent?.trim()).toBe('OPS');
  });

  it('MobileCard_MetaOrderIsTypeThenRole', () => {
    component.listLoadState.set('ready');
    fixture.detectChanges();
    const labels = Array.from(
      (fixture.nativeElement as HTMLElement).querySelectorAll('.ia-card__meta .ia-card__label')
    ).map((el) => (el.textContent ?? '').trim());
    expect(labels).toEqual(['Type', 'Role']);
  });
});
