import { Component, inject } from '@angular/core';
import { BuildInfoService } from '../../../core/services/build-info';

@Component({
  selector: 'build-badge',
  standalone: true,
  imports: [],
  templateUrl: './build-badge.html',
})
export class BuildBadge {
  readonly buildInfo = inject(BuildInfoService);
}
