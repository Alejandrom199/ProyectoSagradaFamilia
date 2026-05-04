import { Component, input, output } from '@angular/core';
import { Button } from "../button/button";

@Component({
  selector: 'confirm-modal',
  imports: [Button],
  templateUrl: './confirm-modal.html',
  styleUrl: './confirm-modal.css',
})
export class ConfirmModal {
  title = input<string>('¿Estás seguro?');
  message = input<string>('Esta acción no se puede deshacer.');

  confirm = output<void>();
  cancel = output<void>();
}
