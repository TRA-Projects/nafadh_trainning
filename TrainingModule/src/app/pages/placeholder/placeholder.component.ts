import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { LocaleService } from '../../core/locale/locale.service';

@Component({
  selector: 'app-placeholder',
  standalone: true,
  templateUrl: './placeholder.component.html',
})
export class PlaceholderComponent {
  private readonly route = inject(ActivatedRoute);
  readonly locale = inject(LocaleService);

  readonly title = this.route.snapshot.data['title'] as string;
  readonly titleAr = this.route.snapshot.data['titleAr'] as string;
}
