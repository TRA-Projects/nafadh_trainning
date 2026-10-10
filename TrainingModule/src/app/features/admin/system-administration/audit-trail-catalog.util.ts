/** Catalog of audit modules and their entity types (filter dropdowns). */

export interface AuditTrailEntityTypeOption {
  value: string;
  labelEn: string;
  labelAr: string;
}

export interface AuditTrailModuleOption {
  value: string;
  labelEn: string;
  labelAr: string;
  entityTypes: readonly AuditTrailEntityTypeOption[];
}

export const AUDIT_TRAIL_MODULES: readonly AuditTrailModuleOption[] = [
  {
    value: 'InterviewManagement',
    labelEn: 'Interview management',
    labelAr: 'إدارة المقابلات',
    entityTypes: [
      {
        value: 'InterviewRecord',
        labelEn: 'Interview record',
        labelAr: 'سجل مقابلة',
      },
    ],
  },
] as const;

export function auditTrailEntityTypesForModule(module: string): readonly AuditTrailEntityTypeOption[] {
  const m = AUDIT_TRAIL_MODULES.find((x) => x.value === module);
  return m?.entityTypes ?? [];
}

export function defaultEntityTypeForModule(module: string): string {
  return auditTrailEntityTypesForModule(module)[0]?.value ?? '';
}
