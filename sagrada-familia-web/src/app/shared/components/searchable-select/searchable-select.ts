import {
  Component, Input, Output, EventEmitter,
  HostListener, ElementRef, inject, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

/**
 * Combobox con búsqueda integrada.
 *
 * Uso básico:
 *   <searchable-select
 *       [options]="lista"
 *       valueKey="id"
 *       [labelFn]="fn"
 *       [value]="seleccionado"
 *       placeholder="Elegir..."
 *       (valueChange)="onCambio($event)">
 *   </searchable-select>
 *
 * Si todas las opciones tienen una sola propiedad de texto, se puede usar
 * [labelKey]="'nombre'" en lugar de [labelFn].
 */
@Component({
  selector: 'searchable-select',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIcon],
  templateUrl: './searchable-select.html',
})
export class SearchableSelect {
  private el = inject(ElementRef);

  /** Array de objetos que representan las opciones */
  @Input() options: any[] = [];

  /** Clave del objeto que contiene el valor a emitir (ej: 'id') */
  @Input() valueKey = 'id';

  /**
   * Clave del objeto para mostrar el texto de la opción.
   * Se usa cuando el label es una sola propiedad.
   */
  @Input() labelKey = 'nombre';

  /**
   * Función personalizada para obtener el texto de una opción.
   * Tiene prioridad sobre labelKey cuando está definida.
   */
  @Input() labelFn?: (item: any) => string;

  /** Texto del placeholder cuando no hay nada seleccionado */
  @Input() placeholder = 'Seleccionar...';

  /** Placeholder del input de búsqueda interno */
  @Input() searchPlaceholder = 'Buscar...';

  /** Valor actualmente seleccionado (compatible con binding one-way) */
  @Input() value: any = null;

  /** Emite el valor de la opción seleccionada */
  @Output() valueChange = new EventEmitter<any>();

  abierto      = signal(false);
  busqueda     = '';
  dropdownPos  = { top: '0px', left: '0px', width: '0px' };

  getLabel(item: any): string {
    return this.labelFn ? this.labelFn(item) : String(item[this.labelKey] ?? '');
  }

  get etiquetaSeleccionada(): string {
    if (this.value === null || this.value === undefined || this.value === 0) return '';
    const found = this.options.find(o => o[this.valueKey] === this.value);
    return found ? this.getLabel(found) : '';
  }

  get opcionesFiltradas(): any[] {
    if (!this.busqueda.trim()) return this.options;
    const q = this.busqueda.toLowerCase();
    return this.options.filter(o => this.getLabel(o).toLowerCase().includes(q));
  }

  toggle(btnEl: HTMLElement): void {
    if (this.abierto()) { this.abierto.set(false); this.busqueda = ''; return; }
    const rect = btnEl.getBoundingClientRect();
    this.dropdownPos = {
      top:   `${rect.bottom + 4}px`,
      left:  `${rect.left}px`,
      width: `${rect.width}px`
    };
    this.abierto.set(true);
  }

  seleccionar(opt: any): void {
    this.valueChange.emit(opt[this.valueKey]);
    this.abierto.set(false);
    this.busqueda = '';
  }

  limpiar(e: Event): void {
    e.stopPropagation();
    this.valueChange.emit(null);
    this.abierto.set(false);
    this.busqueda = '';
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(e: Event): void {
    // El dropdown está en fixed fuera del host → verificar por clase además de containment
    const target = e.target as HTMLElement;
    if (!this.el.nativeElement.contains(target) &&
        !target.closest('.searchable-select-dropdown')) {
      this.abierto.set(false);
      this.busqueda = '';
    }
  }

  @HostListener('window:scroll')
  onScroll(): void {
    this.abierto.set(false);
    this.busqueda = '';
  }
}
