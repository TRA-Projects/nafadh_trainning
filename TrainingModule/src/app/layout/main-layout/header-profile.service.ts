import { Injectable, inject, signal } from '@angular/core';
import { Store } from '@ngrx/store';
import { firstValueFrom } from 'rxjs';

import { JOBSEEKER_PROFILE_ACTIONS } from '../../core/profile/jobseeker-profile-actions.token';
import type { MeResponse } from '../../core/profile/jobseeker-profile.models';
import { getAccessToken } from '../../core/auth/auth-token.storage';
import { profileIdFromAccessToken } from '../../core/auth/auth-jwt.util';
import { jobseekerProfileActions } from '../../core/store/jobseeker-profile/jobseeker-profile.actions';
import { jobseekerProfileFeature } from '../../core/store/jobseeker-profile/jobseeker-profile.reducer';
import { headerProfileImageId, resolveHeaderDisplayName } from './header-profile.utils';

/**
 * Header avatar + display name. `auth/me` and the image bytes are each fetched at most once
 * per signed-in profile (until {@link reset}).
 */
@Injectable({ providedIn: 'root' })
export class HeaderProfileService {
  private readonly actions = inject(JOBSEEKER_PROFILE_ACTIONS);
  private readonly store = inject(Store);

  readonly displayName = signal('');
  readonly avatarUrl = signal<string | null>(null);

  private readonly lastMe = this.store.selectSignal(jobseekerProfileFeature.selectLastMe);

  private inflight: Promise<void> | null = null;
  private loadedProfileId: number | null = null;
  private loadedImageId: string | null = null;

  /** Load name (and image if uploaded). No-ops when this profile was already loaded. */
  ensureLoaded(): Promise<void> {
    const profileId = this.currentProfileId();
    if (this.loadedProfileId != null && this.loadedProfileId === profileId) {
      return Promise.resolve();
    }
    if (this.inflight) {
      return this.inflight;
    }
    const run = this.loadOnce(profileId).finally(() => {
      if (this.inflight === run) {
        this.inflight = null;
      }
    });
    this.inflight = run;
    return run;
  }

  reset(): void {
    this.inflight = null;
    this.loadedProfileId = null;
    this.loadedImageId = null;
    this.displayName.set('');
    this.avatarUrl.set(null);
  }

  private currentProfileId(): number | null {
    const token = getAccessToken();
    return token ? profileIdFromAccessToken(token) : null;
  }

  private async loadOnce(profileId: number | null): Promise<void> {
    try {
      const me = await this.resolveMe(profileId);
      this.displayName.set(resolveHeaderDisplayName(me));
      await this.loadImageOnce(me);
      this.loadedProfileId = me.ProfileId > 0 ? me.ProfileId : profileId;
    } catch {
      this.loadedProfileId = profileId;
    }
  }

  private async resolveMe(profileId: number | null): Promise<MeResponse> {
    const cached = this.lastMe();
    if (cached && profileId != null && cached.ProfileId === profileId) {
      return cached;
    }
    const me = await firstValueFrom(this.actions.getMe());
    this.store.dispatch(jobseekerProfileActions.cacheLoadedMe({ me }));
    return me;
  }

  private async loadImageOnce(me: MeResponse): Promise<void> {
    const imageId = headerProfileImageId(me);
    if (imageId == null) {
      this.avatarUrl.set(null);
      this.loadedImageId = null;
      return;
    }
    const key = String(imageId);
    if (this.loadedImageId === key && this.avatarUrl()) {
      return;
    }
    try {
      const url = (await firstValueFrom(this.actions.getProfileImage(imageId)))?.trim() ?? '';
      this.avatarUrl.set(url || null);
      this.loadedImageId = key;
    } catch {
      this.avatarUrl.set(null);
    }
  }
}
