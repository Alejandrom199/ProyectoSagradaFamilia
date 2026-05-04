import { Component, input } from '@angular/core';

@Component({
  selector: 'spinner-modal',
  imports: [],
  templateUrl: './spinner-modal.html',
  styleUrl: './spinner-modal.css',
})
export class SpinnerModal {
  message = input<string>('Procesando...');
}
