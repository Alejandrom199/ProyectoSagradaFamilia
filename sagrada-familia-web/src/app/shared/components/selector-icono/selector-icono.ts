import {
  Component, Input, Output, EventEmitter,
  HostListener, ElementRef, Renderer2, inject, signal, viewChild,
  AfterViewInit, OnDestroy
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { SIDEBAR_ICON_MAP } from '../../../core/icons/app-icons';

interface OpcionIcono {
  clave: string;
  ngIcon: string;
}

// Universo de íconos seleccionables: las claves de negocio (backend) mapeadas
// a su ícono real, para que el admin elija visualmente en vez de escribir texto libre.
const OPCIONES_ICONO: OpcionIcono[] = Object.entries(SIDEBAR_ICON_MAP)
  .map(([clave, ngIcon]) => ({ clave, ngIcon }));

@Component({
  selector: 'selector-icono',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIcon],
  templateUrl: './selector-icono.html',
})
export class SelectorIcono implements AfterViewInit, OnDestroy {
  private el       = inject(ElementRef<HTMLElement>);
  private renderer = inject(Renderer2);

  @Input() value: string | null = null;
  @Input() placeholder = 'Seleccionar ícono...';
  @Output() valueChange = new EventEmitter<string>();

  readonly opciones = OPCIONES_ICONO;

  abierto  = signal(false);
  busqueda = '';

  // Panel portado a <body> para escapar de cualquier stacking context/overflow
  // de contenedores ancestros — mismo patrón que searchable-select.
  readonly panel = viewChild.required<ElementRef<HTMLElement>>('panel');

  private static readonly ALTURA_ESTIMADA_PANEL = 320;

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

    const noCabeAbajo = espacioAbajo < SelectorIcono.ALTURA_ESTIMADA_PANEL
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

  get ngIconSeleccionado(): string | null {
    return this.opciones.find(o => o.clave === this.value)?.ngIcon ?? null;
  }

  get opcionesFiltradas(): OpcionIcono[] {
    if (!this.busqueda.trim()) return this.opciones;
    const q = this.busqueda.toLowerCase();
    return this.opciones.filter(o => o.clave.toLowerCase().includes(q));
  }

  toggle(): void {
    const abrir = !this.abierto();
    this.abierto.set(abrir);
    if (abrir) this.actualizarPosicion();
    else this.busqueda = '';
  }

  seleccionar(opt: OpcionIcono): void {
    this.valueChange.emit(opt.clave);
    this.abierto.set(false);
    this.busqueda = '';
  }

  limpiar(e: Event): void {
    e.stopPropagation();
    this.valueChange.emit('');
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
