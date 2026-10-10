import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import type { NotificationTransactionLogRowDto } from '../../../core/api/models/admin-system.models';
import { AdminSystemApiService } from '../../../core/api/services/admin-system-api.service';
import { LocaleService } from '../../../core/locale/locale.service';
import { AsyncExecutorService } from '../../../core/ui/async-executor.service';
import { NotificationTransactionsPageComponent } from './notification-transactions-page.component';

function row(
  overrides: Partial<NotificationTransactionLogRowDto> = {}
): NotificationTransactionLogRowDto {
  return {
    Id: 1,
    ChannelType: 'Email',
    Receiver: 'user@example.com',
    Subject: null,
    MessageBody: null,
    PreferredLanguage: null,
    ActionCode: 'TEST',
    ProfileId: null,
    IsSuccess: false,
    FailureReason: 'SMTP timeout',
    CreatedAtUtc: '2026-09-13T10:00:00Z',
    ...overrides,
  };
}

describe('NotificationTransactionsPageComponent failure status', () => {
  let fixture: ComponentFixture<NotificationTransactionsPageComponent>;
  let component: NotificationTransactionsPageComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotificationTransactionsPageComponent],
      providers: [
        {
          provide: AdminSystemApiService,
          useValue: {
            getNotificationTransactionsPage: () =>
              of({ rows: [], total: 0, pageIndex: 0, pageSize: 5 }),
          },
        },
        {
          provide: AsyncExecutorService,
          useValue: {
            execute: <T>(work: () => Promise<T>) => work(),
          },
        },
        {
          provide: LocaleService,
          useValue: { locale: () => 'en' as const },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(NotificationTransactionsPageComponent);
    component = fixture.componentInstance;
  });

  it('HasFailureReason_FailedWithReason_IsTrue', () => {
    expect(component.hasFailureReason(row({ IsSuccess: false, FailureReason: 'boom' }))).toBeTrue();
  });

  it('HasFailureReason_SuccessOrBlank_IsFalse', () => {
    expect(component.hasFailureReason(row({ IsSuccess: true, FailureReason: 'ignored' }))).toBeFalse();
    expect(component.hasFailureReason(row({ IsSuccess: false, FailureReason: '   ' }))).toBeFalse();
    expect(component.hasFailureReason(row({ IsSuccess: false, FailureReason: null }))).toBeFalse();
  });

  it('OpenFailureReason_SetsModalFromFailedLabelClick', () => {
    component.openFailureReason(row({ FailureReason: '  Delivery rejected  ' }));
    expect(component.failureReasonOpen()).toBeTrue();
    expect(component.failureReasonText()).toBe('Delivery rejected');
  });
});
