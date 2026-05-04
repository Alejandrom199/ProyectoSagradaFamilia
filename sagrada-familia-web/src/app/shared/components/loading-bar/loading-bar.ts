import { Component, inject } from '@angular/core';
import { LoadingBar as LoadingBarService } from '../../../core/services/loading-bar';
@Component({
  selector: 'loading-bar',
  imports: [],
  templateUrl: './loading-bar.html',
  styleUrl: './loading-bar.css',
})
export class LoadingBar {
  readonly loadingBar = inject(LoadingBarService);
}
