import { Component, inject } from '@angular/core';
import { AppVersionService } from '../../../core/services/app-version';

@Component({
  selector: 'app-version-badge',
  standalone: true,
  imports: [],
  templateUrl: './app-version-badge.html',
})
export class AppVersionBadge {
  readonly appVersion = inject(AppVersionService);
}
