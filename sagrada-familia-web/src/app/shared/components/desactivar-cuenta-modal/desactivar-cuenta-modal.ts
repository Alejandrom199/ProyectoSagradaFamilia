import { Component, computed, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Button } from '../button/button';

@Component({
  selector: 'desactivar-cuenta-modal',
  imports: [FormsModule, Button],
  templateUrl: './desactivar-cuenta-modal.html',
})
export class DesactivarCuentaModal {
  static readonly LONGITUD_MINIMA_MOTIVO = 10;

  nombreUsuario = input<string>('');
  errorMessage = input<string | null>(null);
  procesando = input<boolean>(false);

  readonly motivo = signal('');
  readonly motivoValido = computed(() => this.motivo().trim().length >= DesactivarCuentaModal.LONGITUD_MINIMA_MOTIVO);

  confirm = output<string>();
  cancel = output<void>();

  onConfirmar(): void {
    if (!this.motivoValido()) return;
    this.confirm.emit(this.motivo().trim());
  }
}
