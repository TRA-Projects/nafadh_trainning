/// <reference types="@angular/localize" />

export function showcaseCategoryLabel(category: string | null | undefined): string {
  switch (category) {
    case 'Training':
      return $localize`:@@jobseeker.showcase.categoryTraining:Training`;
    case 'Equipment':
      return $localize`:@@jobseeker.showcase.categoryEquipment:Equipment`;
    case 'Travel':
      return $localize`:@@jobseeker.showcase.categoryTravel:Travel`;
    default:
      return category || '—';
  }
}

export function showcaseStatusLabel(status: string | null | undefined): string {
  switch (status) {
    case 'Draft':
      return $localize`:@@jobseeker.showcase.statusDraft:Draft`;
    case 'Submitted':
      return $localize`:@@jobseeker.showcase.statusSubmitted:Submitted`;
    case 'Approved':
      return $localize`:@@jobseeker.showcase.statusApproved:Approved`;
    default:
      return status || '—';
  }
}

export function showcaseStatusPillClass(status: string | null | undefined): string {
  const base = 'inline-flex items-center rounded border px-2 py-0.5 text-xs font-semibold ';
  switch (status) {
    case 'Approved':
      return base + 'border-success/30 bg-success/10 text-success';
    case 'Submitted':
      return base + 'border-warning/30 bg-warning/10 text-warning';
    default:
      return base + 'border-primary/30 bg-primary/10 text-primary';
  }
}

export function formatShowcaseDate(iso: string | null | undefined): string {
  if (!iso) return '—';
  const d = new Date(iso);
  return Number.isNaN(d.getTime()) ? iso : d.toLocaleDateString();
}
