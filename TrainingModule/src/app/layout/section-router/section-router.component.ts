import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/** Empty shell so `/applications/applied` style nested routes work under the main layout. */
@Component({
  selector: 'app-section-router',
  standalone: true,
  imports: [RouterOutlet],
  template: '<router-outlet />',
})
export class SectionRouterComponent {}
