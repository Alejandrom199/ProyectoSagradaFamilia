import { Component, input, output } from '@angular/core';
import { NgIcon } from '@ng-icons/core';
import { Button } from '../button/button';
import { formatearFecha, formatearHora } from '../../utils/date.utils';
import { EstadoHistorialResponse } from '../../interfaces/usuario.interface';

@Component({
  selector: 'historial-estado-modal',
  imports: [NgIcon, Button],
  templateUrl: './historial-estado-modal.html',
})
export class HistorialEstadoModal {
  nombreUsuario = input<string>('');
  historial = input<EstadoHistorialResponse[]>([]);
  cargando = input<boolean>(false);

  cerrar = output<void>();

  formatearFecha = formatearFecha;
  formatearHora = formatearHora;
}
