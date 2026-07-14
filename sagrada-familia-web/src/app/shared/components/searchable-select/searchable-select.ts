import {
  Component, Input, Output, EventEmitter,
  HostListener, ElementRef, Renderer2, inject, signal, viewChild,
  AfterViewInit, OnDestroy
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
export class SearchableSelect implements AfterViewInit, OnDestroy {
  private el       = inject(ElementRef<HTMLElement>);
  private renderer = inject(Renderer2);

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

  // Panel del dropdown: se porta a <body> (ver ngAfterViewInit) para escapar de
  // cualquier stacking context/overflow de tarjetas o contenedores ancestros —
  // el mismo problema que ya resuelve la directiva Tooltip de la misma forma.
  readonly panel = viewChild.required<ElementRef<HTMLElement>>('panel');

  // Altura estimada del panel (buscador + lista con max-h-52). No se mide el DOM
  // real: en vez de eso, cuando no cabe abajo se ancla desde `bottom` (no `top`),
  // así el panel crece hacia arriba sin necesitar saber su altura exacta.
  private static readonly ALTURA_ESTIMADA_PANEL = 280;

  readonly panelTop    = signal<number | null>(0);
  readonly panelBottom = signal<number | null>(null);
  readonly panelLeft   = signal(0);
  readonly panelWidth  = signal(0);

  private readonly actualizarPosicion = (): void => {
    if (!this.abierto()) return;

    const rect = this.el.nativeElement.getBoundingClientRect();
    const espacioAbajo  = window.innerHeight - rect.bottom;
    const espacioArriba = rect.top;

    this.panelLeft.set(rect.left);
    this.panelWidth.set(rect.width);

    const noCabeAbajo = espacioAbajo < SearchableSelect.ALTURA_ESTIMADA_PANEL
      && espacioArriba > espacioAbajo;

    if (noCabeAbajo) {
      this.panelTop.set(null);
      this.panelBottom.set(window.innerHeight - rect.top + 4);
    } else {
      this.panelBottom.set(null);
      this.panelTop.set(rect.bottom + 4);
    }
  };

  ngAfterViewInit(): void {
    this.renderer.appendChild(document.body, this.panel().nativeElement);
    window.addEventListener('scroll', this.actualizarPosicion, true);
    window.addEventListener('resize', this.actualizarPosicion);
  }

  ngOnDestroy(): void {
    window.removeEventListener('scroll', this.actualizarPosicion, true);
    window.removeEventListener('resize', this.actualizarPosicion);
    this.panel().nativeElement.remove();
  }

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
    const abrir = !this.abierto();
    this.abierto.set(abrir);
    if (abrir) this.actualizarPosicion();
    else this.busqueda = '';
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
    const target = e.target as Node;
    const dentroDelHost  = this.el.nativeElement.contains(target);
    const dentroDelPanel = this.panel().nativeElement.contains(target);
    if (!dentroDelHost && !dentroDelPanel) {
      this.abierto.set(false);
      this.busqueda = '';
    }
  }
}
