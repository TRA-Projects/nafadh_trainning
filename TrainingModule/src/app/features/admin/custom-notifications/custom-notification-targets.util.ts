import type {
  CustomNotificationRegistrationFilter,
  CustomNotificationTargetDto,
} from '../../../core/api/models/custom-notifications.models';

export function targetKey(t: CustomNotificationTargetDto): string {
  switch (t.TargetType) {
    case 'User':
      return `u:${(t.UserId ?? '').trim().toLowerCase()}`;
    case 'UserGroup':
      return `g:${t.UserGroupId ?? ''}`;
    case 'Organization':
      return `o:${t.OrganizationId ?? ''}`;
    case 'JobSeekers':
      return 'all-jobseekers';
    default:
      return 'all-orgs';
  }
}

export function normalizeRegistrationFilter(
  value: string | null | undefined
): CustomNotificationRegistrationFilter {
  const v = (value ?? '').trim().toLowerCase();
  if (v === 'registered') return 'Registered';
  if (v === 'notregistered') return 'NotRegistered';
  return 'All';
}

export function hasAllOrganizations(targets: readonly CustomNotificationTargetDto[]): boolean {
  return targets.some((t) => t.TargetType === 'AllOrganizations');
}

/** "All organizations" with no registration filter already covers every individual organization. */
export function coversEveryOrganization(targets: readonly CustomNotificationTargetDto[]): boolean {
  return targets.some(
    (t) => t.TargetType === 'AllOrganizations' && normalizeRegistrationFilter(t.RegistrationFilter) === 'All'
  );
}

export function registrationFilterOf(
  targets: readonly CustomNotificationTargetDto[],
  type: 'AllOrganizations' | 'JobSeekers'
): CustomNotificationRegistrationFilter | null {
  const t = targets.find((x) => x.TargetType === type);
  return t ? normalizeRegistrationFilter(t.RegistrationFilter) : null;
}

/** Adds a target once; individual organizations are ignored while unfiltered "All organizations" is selected. */
export function addTarget(
  targets: readonly CustomNotificationTargetDto[],
  next: CustomNotificationTargetDto
): CustomNotificationTargetDto[] {
  if (next.TargetType === 'Organization' && coversEveryOrganization(targets)) {
    return [...targets];
  }
  const key = targetKey(next);
  if (targets.some((t) => targetKey(t) === key)) {
    return [...targets];
  }
  return [...targets, next];
}

export function removeTarget(
  targets: readonly CustomNotificationTargetDto[],
  target: CustomNotificationTargetDto
): CustomNotificationTargetDto[] {
  const key = targetKey(target);
  return targets.filter((t) => targetKey(t) !== key);
}

/** Unfiltered "All organizations" replaces individually picked organizations; a registration filter keeps them. */
export function setAllOrganizations(
  targets: readonly CustomNotificationTargetDto[],
  enabled: boolean,
  label: string,
  filter: CustomNotificationRegistrationFilter = 'All'
): CustomNotificationTargetDto[] {
  const dropOrganizations = enabled && filter === 'All';
  const rest = targets.filter(
    (t) => t.TargetType !== 'AllOrganizations' && (!dropOrganizations || t.TargetType !== 'Organization')
  );
  return enabled ? [...rest, { TargetType: 'AllOrganizations', Label: label, RegistrationFilter: filter }] : rest;
}

export function setJobSeekers(
  targets: readonly CustomNotificationTargetDto[],
  enabled: boolean,
  label: string,
  filter: CustomNotificationRegistrationFilter = 'All'
): CustomNotificationTargetDto[] {
  const rest = targets.filter((t) => t.TargetType !== 'JobSeekers');
  return enabled ? [...rest, { TargetType: 'JobSeekers', Label: label, RegistrationFilter: filter }] : rest;
}

/** `datetime-local` value (local time, no zone) → ISO UTC string. */
export function localInputToIso(value: string | null | undefined): string | null {
  const raw = (value ?? '').trim();
  if (!raw) return null;
  const d = new Date(raw);
  return Number.isNaN(d.getTime()) ? null : d.toISOString();
}

/** ISO string → `datetime-local` value in the browser's time zone. */
export function isoToLocalInput(iso: string | null | undefined): string {
  if (!iso) return '';
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return '';
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}
