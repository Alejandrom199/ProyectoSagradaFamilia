import {
  Component, Input, Output, EventEmitter,
  HostListener, ElementRef, inject, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

@Component({
  selector: 'searchable-select',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIcon],
  templateUrl: './searchable-select.html',
})
export class SearchableSelect {
  private el = inject(ElementRef);

  @Input() options: any[] = [];
  @Input() valueKey = 'id';
  @Input() labelKey = 'nombre';
  @Input() labelFn?: (item: any) => string;
  @Input() placeholder = 'Seleccionar...';
  @Input() searchPlaceholder = 'Buscar...';
  @Input() value: any = null;
  @Output() valueChange = new EventEmitter<any>();

  abierto  = signal(false);
  busqueda = '';

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

  toggle(): void {
    this.abierto.update(v => !v);
    if (!this.abierto()) this.busqueda = '';
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
    if (!this.el.nativeElement.contains(e.target)) {
      this.abierto.set(false);
      this.busqueda = '';
    }
  }
}
