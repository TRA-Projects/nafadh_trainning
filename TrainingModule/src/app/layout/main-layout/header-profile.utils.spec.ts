import {
  headerProfileImageId,
  resolveHeaderDisplayName,
  shouldLoadHeaderProfileImage,
} from './header-profile.utils';

describe('header-profile.utils', () => {
  it('uses CompanyName as the header display name', () => {
    expect(resolveHeaderDisplayName({ CompanyName: '  Salim Alismaili  ' })).toBe('Salim Alismaili');
  });

  it('reads camelCase companyName when PascalCase is absent', () => {
    expect(resolveHeaderDisplayName({ companyName: 'Nafadh Operator' })).toBe('Nafadh Operator');
    expect(resolveHeaderDisplayName(null)).toBe('');
    expect(resolveHeaderDisplayName({})).toBe('');
  });

  it('loads a profile image only when an id is uploaded', () => {
    expect(headerProfileImageId({ ProfileImage: 'abc-guid' })).toBe('abc-guid');
    expect(shouldLoadHeaderProfileImage({ ProfileImage: 'abc-guid' })).toBeTrue();
    expect(headerProfileImageId({ ProfileImage: '' })).toBeNull();
    expect(headerProfileImageId({ profileImage: null })).toBeNull();
    expect(shouldLoadHeaderProfileImage({ CompanyName: 'X' })).toBeFalse();
  });
});
