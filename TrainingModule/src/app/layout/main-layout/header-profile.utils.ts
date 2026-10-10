import type { MeResponse } from '../../core/profile/jobseeker-profile.models';

/** Display name for the header menu (`auth/me` CompanyName, camelCase-safe). */
export function resolveHeaderDisplayName(
  me: Partial<MeResponse> | Record<string, unknown> | null | undefined
): string {
  if (!me || typeof me !== 'object') {
    return '';
  }
  const row = me as Record<string, unknown>;
  const raw = row['CompanyName'] ?? row['companyName'];
  if (raw == null) {
    return '';
  }
  return String(raw).trim();
}

/** Profile image id from `auth/me`, or null when none is uploaded. */
export function headerProfileImageId(
  me: Partial<MeResponse> | Record<string, unknown> | null | undefined
): string | number | null {
  if (!me || typeof me !== 'object') {
    return null;
  }
  const row = me as Record<string, unknown>;
  const raw = row['ProfileImage'] ?? row['profileImage'];
  if (raw == null || raw === '') {
    return null;
  }
  if (typeof raw === 'string' && !raw.trim()) {
    return null;
  }
  return raw as string | number;
}

/** True when `getProfileImage` should run (uploaded id present). */
export function shouldLoadHeaderProfileImage(
  me: Partial<MeResponse> | Record<string, unknown> | null | undefined
): boolean {
  return headerProfileImageId(me) != null;
}
