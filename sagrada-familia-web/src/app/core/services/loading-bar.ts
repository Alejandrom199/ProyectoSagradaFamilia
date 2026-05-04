import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoadingBar {
  private readonly _loading = signal(false);
  private readonly _progress = signal(0);
  private intervalId: any = null;

  readonly loading = this._loading.asReadonly();
  readonly progress = this._progress.asReadonly();

  show() {
    this._loading.set(true);
    this._progress.set(15);

    // Simular progreso gradual
    this.clearInterval();
    this.intervalId = setInterval(() => {
      const current = this._progress();
      if (current < 90) {
        this._progress.set(current + Math.random() * 10);
      }
    }, 300);
  }

  complete() {
    this.clearInterval();
    this._progress.set(100);

    setTimeout(() => {
      this._loading.set(false);
      this._progress.set(0);
    }, 300);
  }

  private clearInterval() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }
  }
}
