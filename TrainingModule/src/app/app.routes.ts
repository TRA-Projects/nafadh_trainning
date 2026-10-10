import { Routes } from '@angular/router';
import {
  adminOnlyGuard,
  authGuard,
  completeProfileGuard,
  loginGuard,
  registrationProfileCompletionGuard,
  registrationVerifyEmailGuard,
} from './core/auth/auth.guard';

const placeholder = () => import('./pages/placeholder/placeholder.component').then((m) => m.PlaceholderComponent);
const sectionRouter = () => import('./layout/section-router/section-router.component').then((m) => m.SectionRouterComponent);
const adminIdentityUsersPage = () =>
  import('./features/admin/identity-access/admin-identity-users-page.component').then(
    (m) => m.AdminIdentityUsersPageComponent
  );
const adminIdentityRolesPage = () =>
  import('./features/admin/identity-access/admin-identity-roles-page.component').then(
    (m) => m.AdminIdentityRolesPageComponent
  );
const adminIdentityRoleCreatePage = () =>
  import('./features/admin/identity-access/admin-identity-role-create-page.component').then(
    (m) => m.AdminIdentityRoleCreatePageComponent
  );
const adminIdentityGroupsPage = () =>
  import('./features/admin/identity-access/admin-identity-groups-page.component').then(
    (m) => m.AdminIdentityGroupsPageComponent
  );
const adminIdentityGroupCreatePage = () =>
  import('./features/admin/identity-access/admin-identity-group-create-page.component').then(
    (m) => m.AdminIdentityGroupCreatePageComponent
  );
const adminIdentityGroupEditPage = () =>
  import('./features/admin/identity-access/admin-identity-group-edit-page.component').then(
    (m) => m.AdminIdentityGroupEditPageComponent
  );
const adminIdentityGroupPagesPage = () =>
  import('./features/admin/identity-access/admin-identity-group-pages-page.component').then(
    (m) => m.AdminIdentityGroupPagesPageComponent
  );
const systemConfigurationPage = () =>
  import('./features/admin/system-administration/system-configuration-page.component').then(
    (m) => m.SystemConfigurationPageComponent
  );
const constantsManagePage = () =>
  import('./features/admin/system-administration/constants-manage-page.component').then(
    (m) => m.ConstantsManagePageComponent
  );
const notificationTemplatesListPage = () =>
  import('./features/admin/system-administration/notification-templates-list-page.component').then(
    (m) => m.NotificationTemplatesListPageComponent
  );
const notificationTemplateManagePage = () =>
  import('./features/admin/system-administration/notification-template-manage-page.component').then(
    (m) => m.NotificationTemplateManagePageComponent
  );
const auditTrailPage = () =>
  import('./features/admin/system-administration/audit-trail-page.component').then((m) => m.AuditTrailPageComponent);
const notificationTransactionsPage = () =>
  import('./features/admin/system-administration/notification-transactions-page.component').then(
    (m) => m.NotificationTransactionsPageComponent
  );
const customNotificationsListPage = () =>
  import('./features/admin/custom-notifications/custom-notifications-list-page.component').then(
    (m) => m.CustomNotificationsListPageComponent
  );
const customNotificationManagePage = () =>
  import('./features/admin/custom-notifications/custom-notification-manage-page.component').then(
    (m) => m.CustomNotificationManagePageComponent
  );
const showcaseListPage = () =>
  import('./features/admin/showcase/showcase-list-page.component').then((m) => m.ShowcaseListPageComponent);
const showcaseFormPage = () =>
  import('./features/admin/showcase/showcase-form-page.component').then((m) => m.ShowcaseFormPageComponent);
const myNotificationsPage = () =>
  import('./features/notifications/my-notifications-page.component').then((m) => m.MyNotificationsPageComponent);

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  {
    path: 'auth/pki-callback',
    loadComponent: () =>
      import('./pages/auth/jobseeker-pki-callback-page.component').then((m) => m.JobseekerPkiCallbackPageComponent),
  },
  {
    path: 'register/jobseeker/pki',
    loadComponent: () =>
      import('./pages/register/jobseeker-register-pki-page.component').then((m) => m.JobseekerRegisterPkiPageComponent),
    canActivate: [loginGuard],
  },
  {
    path: 'register/jobseeker',
    loadComponent: () =>
      import('./pages/register/jobseeker-register-page.component').then((m) => m.JobseekerRegisterPageComponent),
    canActivate: [loginGuard],
  },
  {
    path: 'register/jobseeker/verify-email',
    loadComponent: () =>
      import('./pages/register/jobseeker-email-verification-page.component').then(
        (m) => m.JobseekerEmailVerificationPageComponent
      ),
    canActivate: [registrationVerifyEmailGuard],
  },
  {
    path: 'register/jobseeker/complete-profile',
    loadComponent: placeholder,
    canActivate: [registrationProfileCompletionGuard],
    data: { title: 'Complete profile', titleAr: 'إكمال الملف' },
  },
  {
    path: 'complete-profile',
    loadComponent: placeholder,
    canActivate: [completeProfileGuard],
    data: { title: 'Complete profile', titleAr: 'إكمال الملف' },
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then((m) => m.LoginComponent),
    canActivate: [loginGuard],
  },
  /** Internal staff password login — not linked from public UI. */
  {
    path: 'staff/login',
    loadComponent: () => import('./pages/staff-login/staff-login.component').then((m) => m.StaffLoginComponent),
    canActivate: [loginGuard],
  },
  {
    path: '',
    loadComponent: () => import('./layout/main-layout/main-layout.component').then((m) => m.MainLayoutComponent),
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: placeholder,
        data: { title: 'Dashboard', titleAr: 'لوحة التحكم' },
      },
      {
        path: 'notifications',
        loadComponent: myNotificationsPage,
        data: { title: 'My notifications', titleAr: 'إشعاراتي' },
      },
      {
        path: 'admin',
        loadComponent: sectionRouter,
        canActivate: [adminOnlyGuard],
        canActivateChild: [adminOnlyGuard],
        children: [
          { path: '', pathMatch: 'full', redirectTo: 'users' },
          {
            path: 'users',
            loadComponent: adminIdentityUsersPage,
            data: { title: 'Users', titleAr: 'المستخدمون' },
          },
          {
            path: 'roles',
            loadComponent: adminIdentityRolesPage,
            data: { title: 'Custom roles', titleAr: 'الأدوار المخصصة', rolesMode: 'custom' },
          },
          {
            path: 'template-roles',
            loadComponent: adminIdentityRolesPage,
            data: { title: 'Template roles', titleAr: 'أدوار القوالب', rolesMode: 'template' },
          },
          {
            path: 'roles/create',
            loadComponent: adminIdentityRoleCreatePage,
            data: { title: 'Create role', titleAr: 'إنشاء دور' },
          },
          {
            path: 'groups',
            loadComponent: adminIdentityGroupsPage,
            data: { title: 'Groups', titleAr: 'المجموعات' },
          },
          {
            path: 'groups/create',
            loadComponent: adminIdentityGroupCreatePage,
            data: { title: 'Create group', titleAr: 'إنشاء مجموعة' },
          },
          {
            path: 'groups/edit',
            loadComponent: adminIdentityGroupEditPage,
            data: { title: 'Edit group', titleAr: 'تعديل المجموعة' },
          },
          {
            path: 'groups/pages',
            loadComponent: adminIdentityGroupPagesPage,
            data: { title: 'Group pages', titleAr: 'صفحات المجموعة' },
          },
          {
            path: 'roles/pages',
            loadComponent: adminIdentityGroupPagesPage,
            data: { title: 'Role pages', titleAr: 'صفحات الدور' },
          },
          {
            path: 'system-configuration',
            loadComponent: systemConfigurationPage,
            data: { title: 'System configuration', titleAr: 'إعدادات النظام' },
          },
          {
            path: 'constants-management',
            loadComponent: constantsManagePage,
            data: { title: 'Constants management', titleAr: 'إدارة الثوابت' },
          },
          {
            path: 'notification-templates',
            loadComponent: notificationTemplatesListPage,
            data: { title: 'Notification templates', titleAr: 'قوالب الإشعارات' },
          },
          {
            path: 'notification-templates/:actionCode',
            loadComponent: notificationTemplateManagePage,
            data: { title: 'Notification template', titleAr: 'قالب إشعار' },
          },
          {
            path: 'audit-trail',
            loadComponent: auditTrailPage,
            data: { title: 'Audit trail', titleAr: 'سجل التدقيق' },
          },
          {
            path: 'notification-transactions',
            loadComponent: notificationTransactionsPage,
            data: { title: 'Notification transactions', titleAr: 'سجل الإشعارات' },
          },
          {
            path: 'custom-notifications',
            loadComponent: customNotificationsListPage,
            data: { title: 'Custom notifications', titleAr: 'الإشعارات المخصصة' },
          },
          {
            path: 'custom-notifications/new',
            loadComponent: customNotificationManagePage,
            data: { title: 'New notification', titleAr: 'إشعار جديد' },
          },
          {
            path: 'custom-notifications/:id',
            loadComponent: customNotificationManagePage,
            data: { title: 'Custom notification', titleAr: 'إشعار مخصص' },
          },
          {
            path: 'showcase',
            loadComponent: showcaseListPage,
            data: { title: 'UI showcase', titleAr: 'نماذج الواجهة' },
          },
          {
            path: 'showcase/new',
            loadComponent: showcaseFormPage,
            data: { title: 'New record', titleAr: 'سجل جديد' },
          },
          {
            path: 'showcase/:id',
            loadComponent: showcaseFormPage,
            data: { title: 'UI showcase', titleAr: 'نماذج الواجهة' },
          },
        ],
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
