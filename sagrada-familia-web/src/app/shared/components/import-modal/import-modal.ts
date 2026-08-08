import { Component, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgIcon } from '@ng-icons/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../../interfaces/api.interface';
import { ImportResult } from '../../interfaces/import.interface';
import { Button } from '../button/button';

type ImportEstado = 'seleccion' | 'cargando' | 'resultado';

@Component({
  selector: 'import-modal',
  standalone: true,
  imports: [CommonModule, NgIcon, Button],
  templateUrl: './import-modal.html'
})
export class ImportModal {
  titulo    = input<string>('Importación masiva');
  subtitulo = input<string>('Use la plantilla oficial para garantizar el formato correcto.');
  importFn  = input.required<(file: File) => Observable<ApiResponse<ImportResult>>>();

  cerrado            = output<void>();
  importacionExitosa = output<void>();

  estado            = signal<ImportEstado>('seleccion');
  resultado         = signal<ImportResult | null>(null);
  archivoSeleccionado = signal<File | null>(null);
  dragging          = signal(false);
  detalleExpandido  = signal(false);

  cerrar() {
    this.estado.set('seleccion');
    this.resultado.set(null);
    this.archivoSeleccionado.set(null);
    this.detalleExpandido.set(false);
    this.cerrado.emit();
  }

  onArchivoSeleccionado(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) this.archivoSeleccionado.set(input.files[0]);
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.dragging.set(true);
  }

  onDragLeave() { this.dragging.set(false); }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.dragging.set(false);
    const file = event.dataTransfer?.files[0];
    if (file?.name.toLowerCase().endsWith('.xlsx')) this.archivoSeleccionado.set(file);
  }

  ejecutarImport() {
    const archivo = this.archivoSeleccionado();
    if (!archivo) return;

    this.estado.set('cargando');
    this.importFn()(archivo).subscribe({
      next: (r) => {
        if (r.success) {
          this.resultado.set(r.data);
          if (r.data.importados > 0 || r.data.actualizados > 0) {
            this.importacionExitosa.emit();
          }
        }
        this.estado.set('resultado');
      },
      error: () => this.estado.set('seleccion')
    });
  }

  volverASeleccion() {
    this.estado.set('seleccion');
    this.archivoSeleccionado.set(null);
    this.resultado.set(null);
    this.detalleExpandido.set(false);
  }

  toggleDetalle() {
    this.detalleExpandido.update(v => !v);
  }
}
