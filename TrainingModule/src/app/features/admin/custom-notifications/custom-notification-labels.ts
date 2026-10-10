/// <reference types="@angular/localize" />

export function customNotificationStatusLabel(status: string): string {
  switch (status) {
    case 'Scheduled':
      return $localize`:@@jobseeker.customNotifications.statusScheduled:Scheduled`;
    case 'Processing':
      return $localize`:@@jobseeker.customNotifications.statusProcessing:Sending`;
    case 'Sent':
      return $localize`:@@jobseeker.customNotifications.statusSent:Sent`;
    case 'PartiallyFailed':
      return $localize`:@@jobseeker.customNotifications.statusPartiallyFailed:Partially failed`;
    case 'Failed':
      return $localize`:@@jobseeker.customNotifications.statusFailed:Failed`;
    case 'Cancelled':
      return $localize`:@@jobseeker.customNotifications.statusCancelled:Cancelled`;
    default:
      return status || '—';
  }
}

export function customNotificationRegistrationFilterLabel(filter: string | null | undefined): string {
  switch (filter) {
    case 'Registered':
      return $localize`:@@jobseeker.customNotifications.filterRegistered:Registered`;
    case 'NotRegistered':
      return $localize`:@@jobseeker.customNotifications.filterNotRegistered:Not registered`;
    default:
      return $localize`:@@jobseeker.customNotifications.filterAll:All`;
  }
}

export function customNotificationChannelsLabel(row: { SendEmail: boolean; SendSms: boolean; SendSystem: boolean }): string {
  const parts: string[] = [];
  if (row.SendEmail) parts.push($localize`:@@jobseeker.customNotifications.channelEmail:Email`);
  if (row.SendSms) parts.push($localize`:@@jobseeker.customNotifications.channelSms:SMS`);
  if (row.SendSystem) parts.push($localize`:@@jobseeker.customNotifications.channelSystem:System`);
  return parts.join(', ') || '—';
}
