/** Production build: real API base URL only (set at deploy time). */
export const environment = {
  production: true,
  useMockBackend: false,
  useMockLogin: false,
  useAdminSystemMock: false,
  useJobseekerProfileMock: false,
  apiBaseUrl: 'https://your-api-host.example.com/',
  freelancerMvcOrigin: '',
  authModuleOrigin: 'https://nafadhpki.tra.gov.om',
};
