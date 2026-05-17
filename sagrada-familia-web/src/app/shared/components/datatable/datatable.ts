import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, signal, Output, EventEmitter, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import {
  heroChevronLeft,
  heroChevronRight,
  heroChevronDoubleLeft,
  heroChevronDoubleRight,
  heroXMark,
  heroFunnel,
  heroArrowUp,
  heroArrowDown,
  heroCircleStack,
  heroUsers,
  heroEye,
  heroTrash,
  heroPencil,
  heroCog8Tooth,
  heroArrowDownTray,
  heroDocumentText,
  heroTableCells,
  heroDocumentArrowDown,
  heroEllipsisVertical,
  heroMagnifyingGlass,
  heroArrowPath,
  heroViewColumns
} from '@ng-icons/heroicons/outline';
import { Tooltip } from "../../directives/tooltip/tooltip";

export type ActionType = 'ver' | 'editar' | 'eliminar' | 'medidas';

export interface DatatableColumn<T> {
  key: Extract<keyof T, string> | string;
  label: string;
  sortable?: boolean;
  filterable?: boolean;
  render?: (row: T) => string;
  class?: string;
  hidden?: boolean;
  exportValue?: (row: T) => string;
}

export interface DatatableAction<T> {
  type?: ActionType;
  label?: string;
  icon?: string;
  class?: string;
  visible?: (row: T) => boolean;
  onClick: (row: T) => void;
}

@Component({
  selector: 'datatable',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIcon, Tooltip],
  viewProviders: [provideIcons({
    heroChevronLeft, heroChevronRight, heroChevronDoubleLeft, heroChevronDoubleRight,
    heroXMark, heroFunnel, heroArrowUp, heroArrowDown, heroCircleStack,
    heroPencil, heroTrash, heroEye, heroUsers, heroCog8Tooth,
    heroArrowDownTray, heroDocumentText, heroTableCells, heroDocumentArrowDown,
    heroEllipsisVertical, heroMagnifyingGlass, heroArrowPath, heroViewColumns
  })],
  templateUrl: './datatable.html',
  styleUrl: './datatable.css',
  host: {
    'class': 'block w-full'
  }
})
export class Datatable<T extends object> implements OnChanges {
  @Input() title = '';
  @Input() columns: DatatableColumn<T>[] = [];
  @Input() actions: DatatableAction<T>[] = [];
  @Input() data: T[] = [];
  @Input() pageSize = 10;
  @Input() pageSizes = [5, 10, 25, 50];
  @Input() emptyMessage = 'No hay registros para mostrar.';

  @Input() exportExcel = true;
  @Input() exportPdf = true;
  @Input() exportFileName = 'reporte';

  @Input() showActualizar = true;
  @Input() showColumnas = true;
  @Input() showExportar = true;
  @Input() showConfiguracion = true;

  @Output() onActualizar = new EventEmitter<void>();
  @Output() onConfiguracion = new EventEmitter<void>();
  @Output() onExportPdf = new EventEmitter<Record<string | number | symbol, string>>();

  filtrosSeleccion: Record<string, Set<string>> = {};
  busquedaFiltro: Record<string, string> = {};

  sortKey = '';
  sortDir: 'asc' | 'desc' = 'asc';
  filtros: Record<string | number | symbol, string> = {};
  busquedaGlobal = '';

  menuActivo = signal<string | null>(null);

  columnasPosicion = {
    top: '0px',
    left: '0px'
  };

  exportarPosicion = {
    top: '0px',
    left: '0px'
  };

  menuPosicion = {
    top: '0px',
    left: '0px'
  };

  filtroPosicion = {
    top: '0px',
    left: '0px'
  };

  private readonly paginaActualSignal = signal(1);
  readonly paginaActual = this.paginaActualSignal.asReadonly();

  @HostListener('document:click')
  onClickOutside() {
    this.menuActivo.set(null);
  }

  @HostListener('window:scroll')
  onWindowScroll() {
    this.menuActivo.set(null);
  }

  toggleMenuAcciones(
    index: number,
    event: Event,
    btnElement: HTMLElement
  ) {
    event.stopPropagation();

    const menuKey = `accion-${index}`;

    if (this.menuActivo() === menuKey) {
      this.menuActivo.set(null);
      return;
    }

    const rect = btnElement.getBoundingClientRect();

    this.menuPosicion = {
      top: `${rect.bottom + 4}px`,
      left: `${rect.left}px`
    };

    this.menuActivo.set(menuKey);
  }

  toggleFiltro(
    key: string | number | symbol,
    event: Event,
    btnElement: HTMLElement
  ) {
    event.stopPropagation();

    const menuKey = `filtro-${String(key)}`;

    if (this.menuActivo() === menuKey) {
      this.menuActivo.set(null);
      return;
    }

    const rect = btnElement.getBoundingClientRect();

    let leftPos = rect.left;

    if (leftPos + 224 > window.innerWidth) {
      leftPos = window.innerWidth - 240;
    }

    this.filtroPosicion = {
      top: `${rect.bottom + 8}px`,
      left: `${leftPos}px`
    };

    this.menuActivo.set(menuKey);
  }

  toggleMenuExportar(event: Event, btnElement: HTMLElement) {
    event.stopPropagation();

    if (this.menuActivo() === 'exportar') {
      this.menuActivo.set(null);
      return;
    }

    const rect = btnElement.getBoundingClientRect();

    this.exportarPosicion = {
      top: `${rect.bottom + 8}px`,
      left: `${rect.right - 160}px`
    };

    this.menuActivo.set('exportar');
  }

  toggleMenuColumnas(event: Event, btnElement: HTMLElement) {
    event.stopPropagation();

    if (this.menuActivo() === 'columnas') {
      this.menuActivo.set(null);
      return;
    }

    const rect = btnElement.getBoundingClientRect();

    this.columnasPosicion = {
      top: `${rect.bottom + 8}px`,
      left: `${rect.right - 200}px`
    };

    this.menuActivo.set('columnas');
  }

  toggleVisibilidadColumna(key: string | number | symbol) {
    const col = this.columns.find(c => c.key === key);
    if (col) {
      col.hidden = !col.hidden;
    }
  }

  ngOnChanges() {
    this.paginaActualSignal.set(1);
  }

  accionesVisibles(row: T): DatatableAction<T>[] {
    return this.accionesProcesadas.filter(a => !a.visible || a.visible(row));
  }


  getCellValue(row: T, key: string | number | symbol): unknown {
    const path = String(key);
    return path.split('.').reduce((obj: unknown, k) => {
      if (obj && typeof obj === 'object') {
        return (obj as Record<string, unknown>)[k];
      }
      return null;
    }, row);
  }

  private limpiarHtml(html: string): string {
    const div = document.createElement('div');
    div.innerHTML = html;
    return (div.textContent || div.innerText || '').trim();
  }

  exportarExcel() {
    if (this.datosFiltrados.length === 0) return;

    const headers = this.columns.map(c => c.label).join(';');
    const rows = this.datosFiltrados.map(row => {
      return this.columns.map(col => {
        let valor = '';
        if (col.exportValue) {
          valor = col.exportValue(row);
        } else if (col.render) {
          valor = this.limpiarHtml(col.render(row));
        } else {
          valor = String(this.getCellValue(row, col.key) ?? '');
        }
        return `"${valor.replace(/"/g, '""').replace(/;/g, ',')}"`;
      }).join(';');
    });

    const csvContent = '\uFEFF' + [headers, ...rows].join('\r\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');

    link.setAttribute('href', url);
    link.setAttribute('download', `${this.exportFileName}_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  exportarPdf() {
    this.onExportPdf.emit(this.filtros);
  }

  obtenerValoresUnicos(key: string | number | symbol): string[] {
    const path = String(key);
    const valores = this.data.map(row => String(this.getCellValue(row, path) ?? ''));
    return [...new Set(valores)].filter(v => v.trim() !== '');
  }

  onFilterChange() {
    this.paginaActualSignal.set(1);
  }

  ordenarPor(key: string | number | symbol, dir?: 'asc' | 'desc') {
    const sortPath = String(key);
    if (dir) {
      this.sortKey = sortPath;
      this.sortDir = dir;
    } else {
      if (this.sortKey === sortPath) {
        this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
      } else {
        this.sortKey = sortPath;
        this.sortDir = 'asc';
      }
    }
  }

  cambiarPagina(p: number) {
    if (p >= 1 && p <= this.totalPaginas) {
      this.paginaActualSignal.set(p);
    }
  }

  cambiarTamanoPagina(nuevoTamano: number) {
    this.pageSize = nuevoTamano;
    this.paginaActualSignal.set(1);
  }

  toggleTodasLasColumnas() {
    const mostrarTodas = !this.todasLasColumnasVisibles;

    this.columns.forEach(col => {
      col.hidden = !mostrarTodas;
    });
  }

  get todasLasColumnasVisibles(): boolean {
    return this.columns.every(c => !c.hidden);
  }

  get columnasVisibles() {
    return this.columns.filter(c => !c.hidden);
  }

  get accionesProcesadas(): DatatableAction<T>[] {
    const defaults: Record<ActionType, Partial<DatatableAction<T>>> = {
      ver: { label: 'Ver', icon: 'heroEye', class: 'text-blue-600' },
      editar: { label: 'Editar', icon: 'heroPencil', class: 'text-amber-600' },
      eliminar: { label: 'Eliminar', icon: 'heroTrash', class: 'text-red-600' },
      medidas: { label: 'Medidas', icon: 'heroChartBar', class: 'text-purple-600' },
    };

    return this.actions.map((action: DatatableAction<T>) => {
      let preset: Partial<DatatableAction<T>> = {};
      if (action.type) { preset = defaults[action.type]; }
      return { ...preset, ...action };
    });
  }

  get datosFiltrados() {
    let resultado = [...this.data];

    if (this.busquedaGlobal.trim()) {
      const termino = this.busquedaGlobal.toLowerCase();
      resultado = resultado.filter(row => {
        return this.columns.some(col => {
          const val = this.getCellValue(row, col.key);
          return val != null && String(val).toLowerCase().includes(termino);
        });
      });
    }

    Object.entries(this.filtrosSeleccion).forEach(([key, set]) => {
      resultado = resultado.filter(row => {
        const val = String(this.getCellValue(row, key) ?? '');
        return set.has(val);
      });
    });

    if (this.sortKey) {
      resultado.sort((a, b) => {
        const aVal = this.getCellValue(a, this.sortKey);
        const bVal = this.getCellValue(b, this.sortKey);

        if (aVal == null && bVal == null) return 0;
        if (aVal == null) return this.sortDir === 'asc' ? -1 : 1;
        if (bVal == null) return this.sortDir === 'asc' ? 1 : -1;

        const cmp = String(aVal).localeCompare(String(bVal), undefined, { numeric: true });
        return this.sortDir === 'asc' ? cmp : -cmp;
      });
    }

    return resultado;
  }

  get tieneFilrosActivos(): boolean {
    return Object.values(this.filtros).some(v => v && v.trim() !== '') || this.busquedaGlobal.trim() !== '';
  }

  get totalPaginas() { return Math.ceil(this.datosFiltrados.length / this.pageSize) || 1; }
  get inicio() { return (this.paginaActual() - 1) * this.pageSize; }
  get fin() { return Math.min(this.inicio + this.pageSize, this.datosFiltrados.length); }
  get datosPaginados() { return this.datosFiltrados.slice(this.inicio, this.fin); }

  cerrarMenus() {
    this.menuActivo.set(null);
  }

  // filtros
  isValorMarcado(key: string | number | symbol, valor: string): boolean {
    const set = this.filtrosSeleccion[String(key)];
    return set ? set.has(valor) : true;
  }

  toggleValorFiltro(key: string | number | symbol, valor: string) {
    const k = String(key);

    if (!this.filtrosSeleccion[k]) {
      this.filtrosSeleccion[k] = new Set(this.obtenerValoresUnicos(k));
    }

    const set = this.filtrosSeleccion[k];
    set.has(valor) ? set.delete(valor) : set.add(valor);

    if (set.size === this.obtenerValoresUnicos(k).length) {
      delete this.filtrosSeleccion[k];
    }
    this.onFilterChange();
  }

  todosLosValoresMarcados(key: string | number | symbol): boolean {
    const set = this.filtrosSeleccion[String(key)];
    return !set || set.size === this.obtenerValoresUnicos(key).length;
  }

  toggleTodosLosValores(key: string | number | symbol) {
    const k = String(key);
    if (this.todosLosValoresMarcados(k)) {
      this.filtrosSeleccion[k] = new Set();
    } else {
      delete this.filtrosSeleccion[k];
    }
    this.onFilterChange();
  }

  valoresUnicosFiltrados(key: string | number | symbol): string[] {
    const k = String(key);
    const q = (this.busquedaFiltro[k] || '').toLowerCase().trim();
    const todos = this.obtenerValoresUnicos(k);
    return q ? todos.filter(v => v.toLowerCase().includes(q)) : todos;
  }

  limpiarFiltroColumna(key: string | number | symbol) {
    delete this.filtrosSeleccion[String(key)];
    delete this.busquedaFiltro[String(key)];
    this.onFilterChange();
    this.menuActivo.set(null);
  }

  limpiarFiltros() {
    this.filtrosSeleccion = {};
    this.busquedaFiltro = {};
    this.busquedaGlobal = '';
    this.onFilterChange();
    this.menuActivo.set(null);
  }

  get tieneFiltrosActivos(): boolean {
    return Object.keys(this.filtrosSeleccion).length > 0 || this.busquedaGlobal.trim() !== '';
  }
}