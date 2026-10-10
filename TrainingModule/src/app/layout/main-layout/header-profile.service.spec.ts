import { TestBed } from '@angular/core/testing';
import { provideState, provideStore } from '@ngrx/store';
import { of } from 'rxjs';

import { ACCESS_TOKEN_KEY } from '../../core/auth/auth-token.storage';
import { JOBSEEKER_PROFILE_ACTIONS } from '../../core/profile/jobseeker-profile-actions.token';
import type { MeResponse } from '../../core/profile/jobseeker-profile.models';
import { jobseekerProfileFeature } from '../../core/store/jobseeker-profile/jobseeker-profile.reducer';
import { HeaderProfileService } from './header-profile.service';

function jwtWithProfileId(profileId: number): string {
  const payload = btoa(JSON.stringify({ ProfileId: profileId }))
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=+$/, '');
  return `hdr.${payload}.sig`;
}

function meRow(overrides: Partial<MeResponse> = {}): MeResponse {
  return {
    ProfileId: 2522,
    CompanyName: 'Salim Alismaili',
    Address: '',
    PhoneNumber: '',
    EmailAddress: '',
    CrNumber: '',
    Website: '',
    About: '',
    ServicesOffered: [],
    ProfileImage: 'avatar-1',
    ...overrides,
  };
}

describe('HeaderProfileService', () => {
  let getMe: jasmine.Spy;
  let getProfileImage: jasmine.Spy;
  let service: HeaderProfileService;

  beforeEach(() => {
    sessionStorage.clear();
    localStorage.clear();
    sessionStorage.setItem(ACCESS_TOKEN_KEY, jwtWithProfileId(2522));
    getMe = jasmine.createSpy('getMe').and.returnValue(of(meRow()));
    getProfileImage = jasmine.createSpy('getProfileImage').and.returnValue(of('data:image/png;base64,abc'));
    TestBed.configureTestingModule({
      providers: [
        provideStore(),
        provideState(jobseekerProfileFeature),
        { provide: JOBSEEKER_PROFILE_ACTIONS, useValue: { getMe, getProfileImage } },
        HeaderProfileService,
      ],
    });
    service = TestBed.inject(HeaderProfileService);
  });

  afterEach(() => {
    sessionStorage.clear();
    localStorage.clear();
  });

  it('shows CompanyName and loads the uploaded image once', async () => {
    await service.ensureLoaded();
    await service.ensureLoaded();

    expect(service.displayName()).toBe('Salim Alismaili');
    expect(service.avatarUrl()).toBe('data:image/png;base64,abc');
    expect(getMe).toHaveBeenCalledTimes(1);
    expect(getProfileImage).toHaveBeenCalledOnceWith('avatar-1');
  });

  it('skips the image request when no profile image is uploaded', async () => {
    getMe.and.returnValue(of(meRow({ ProfileImage: null })));

    await service.ensureLoaded();

    expect(service.displayName()).toBe('Salim Alismaili');
    expect(service.avatarUrl()).toBeNull();
    expect(getProfileImage).not.toHaveBeenCalled();
  });

  it('reloads after reset for a different profile', async () => {
    await service.ensureLoaded();
    service.reset();
    sessionStorage.setItem(ACCESS_TOKEN_KEY, jwtWithProfileId(99));
    getMe.and.returnValue(of(meRow({ ProfileId: 99, CompanyName: 'Other User', ProfileImage: null })));

    await service.ensureLoaded();

    expect(service.displayName()).toBe('Other User');
    expect(getMe).toHaveBeenCalledTimes(2);
    expect(getProfileImage).toHaveBeenCalledTimes(1);
  });
});
