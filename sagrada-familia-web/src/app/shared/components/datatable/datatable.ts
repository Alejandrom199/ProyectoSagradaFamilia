import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, signal, computed, Output, EventEmitter, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

import { Tooltip } from "../../directives/tooltip/tooltip";

export type ActionType = 'ver' | 'editar' | 'eliminar' | 'medidas';
export type Densidad = 'compacto' | 'normal' | 'espacioso';

export interface ServerQuery {
  page: number;
  pageSize: number;
  search: string;
  sortBy: string;
  sortDir: 'asc' | 'desc';
  columnFilters: Record<string, string[]>;
}

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
  @Input() pageSizes = [5, 10, 25, 50, 100];
  @Input() emptyMessage = 'No hay registros para mostrar.';

  @Input() exportExcel = true;
  @Input() exportPdf = true;
  @Input() exportFileName = 'reporte';

  @Input() showActualizar = true;
  @Input() showColumnas = true;
  @Input() showExportar = true;
  @Input() showConfiguracion = true;
  @Input() showPlantilla = false;
  @Input() showImportar = false;

  @Input() serverSide = false;
  @Input() serverTotalItems = 0;
  @Input() tooltipThreshold = 55;
  @Input() filterOptions: Record<string, string[]> = {};

  @Output() onActualizar = new EventEmitter<void>();
  @Output() onConfiguracion = new EventEmitter<void>();
  @Output() onExportPdf = new EventEmitter<Record<string | number | symbol, string>>();
  @Output() onExportExcel = new EventEmitter<void>();
  @Output() onPlantilla = new EventEmitter<void>();
  @Output() onImportar = new EventEmitter<void>();
  @Output() onServerQuery = new EventEmitter<ServerQuery>();

  filtrosSeleccion: Record<string, Set<string>> = {};
  busquedaFiltro: Record<string, string> = {};

  sortKey = '';
  sortDir: 'asc' | 'desc' = 'asc';
  filtros: Record<string | number | symbol, string> = {};
  busquedaGlobal = '';

  menuActivo = signal<string | null>(null);

  densidad = signal<Densidad>(
    (localStorage.getItem('datatable-density') as Densidad | null) ?? 'normal'
  );

  columnWidths = signal<Record<string, number>>({});
  readonly hasCustomWidths = computed(() => Object.keys(this.columnWidths()).length > 0);

  private resizingKey: string | null = null;
  private resizeStartX = 0;
  private resizeStartWidth = 0;

  readonly densidadOpciones: { valor: Densidad; label: string; desc: string }[] = [
    { valor: 'compacto', label: 'Compacto', desc: 'Más filas visibles' },
    { valor: 'normal', label: 'Normal', desc: 'Vista estándar' },
    { valor: 'espacioso', label: 'Espacioso', desc: 'Mayor legibilidad' },
  ];

  columnasPosicion = { top: '0px', left: '0px' };
  exportarPosicion = { top: '0px', left: '0px' };
  menuPosicion = { top: '0px', left: '0px' };
  filtroPosicion = { top: '0px', left: '0px' };
  configuracionPosicion = { top: '0px', left: '0px' };

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

  private searchTimer: ReturnType<typeof setTimeout> | null = null;

  private emitServerQuery() {
    this.onServerQuery.emit({
      page: this.paginaActual(),
      pageSize: this.pageSize,
      search: this.busquedaGlobal,
      sortBy: this.sortKey,
      sortDir: this.sortDir,
      columnFilters: this.buildColumnFilters(),
    });
  }

  private buildColumnFilters(): Record<string, string[]> {
    const filtrosActivos: Record<string, string[]> = {};

    Object.entries(this.filtrosSeleccion).forEach(([columna, valoresSeleccionados]) => {
      const tieneValores = valoresSeleccionados.size > 0;
      if (tieneValores) filtrosActivos[columna] = [...valoresSeleccionados];
    });

    return filtrosActivos;
  }

  onSearchChange(value: string) {
    this.busquedaGlobal = value;
    if (this.serverSide) {
      if (this.searchTimer) clearTimeout(this.searchTimer);
      this.searchTimer = setTimeout(() => {
        this.paginaActualSignal.set(1);
        this.emitServerQuery();
      }, 350);
    } else {
      this.onFilterChange();
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

  cellTooltip(row: T, col: DatatableColumn<T>): string {
    const text = col.render
      ? this.limpiarHtml(col.render(row))
      : String(this.getCellValue(row, col.key) ?? '');
    return text.length > this.tooltipThreshold ? text : '';
  }

  private limpiarHtml(html: string): string {
    const div = document.createElement('div');
    div.innerHTML = html;
    return (div.textContent || div.innerText || '').trim();
  }

  exportarExcel() {
    if (this.onExportExcel.observed) {
      this.onExportExcel.emit();
      return;
    }

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
    const columna = String(key);
    const opcionesServidor = this.filterOptions[columna];
    if (this.serverSide && opcionesServidor?.length) return opcionesServidor;
    const valores = this.data.map(row => String(this.getCellValue(row, columna) ?? ''));
    return [...new Set(valores)].filter(valor => valor.trim() !== '');
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
    if (this.serverSide) {
      this.paginaActualSignal.set(1);
      this.emitServerQuery();
    }
  }

  cambiarPagina(p: number) {
    if (p >= 1 && p <= this.totalPaginas) {
      this.paginaActualSignal.set(p);
      if (this.serverSide) this.emitServerQuery();
    }
  }

  cambiarTamanoPagina(nuevoTamano: number) {
    this.pageSize = nuevoTamano;
    this.paginaActualSignal.set(1);
    if (this.serverSide) this.emitServerQuery();
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
      ver: { label: 'Ver', icon: 'matVisibilityOutline', class: 'text-blue-600' },
      editar: { label: 'Editar', icon: 'matEditOutline', class: 'text-amber-600' },
      eliminar: { label: 'Eliminar', icon: 'matDeleteOutline', class: 'text-red-600' },
      medidas: { label: 'Medidas', icon: 'matBarChartOutline', class: 'text-purple-600' },
    };

    return this.actions.map((action: DatatableAction<T>) => {
      let preset: Partial<DatatableAction<T>> = {};
      if (action.type) { preset = defaults[action.type]; }
      return { ...preset, ...action };
    });
  }

  get datosFiltrados() {
    if (this.hayFiltroVacio) return [];

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

  get totalPaginas() {
    if (this.hayFiltroVacio) return 1;
    if (this.serverSide) return Math.ceil(this.serverTotalItems / this.pageSize) || 1;
    return Math.ceil(this.datosFiltrados.length / this.pageSize) || 1;
  }
  get inicio() { return (this.paginaActual() - 1) * this.pageSize; }
  get fin() {
    if (this.hayFiltroVacio) return 0;
    if (this.serverSide) return Math.min(this.inicio + this.pageSize, this.serverTotalItems);
    return Math.min(this.inicio + this.pageSize, this.datosFiltrados.length);
  }
  get datosPaginados() {
    if (this.hayFiltroVacio) return [];
    if (this.serverSide) return this.data;
    return this.datosFiltrados.slice(this.inicio, this.fin);
  }

  onResizeStart(event: MouseEvent, key: string, th: HTMLTableCellElement): void {
    event.preventDefault();
    event.stopPropagation();

    if (!this.hasCustomWidths()) {
      const table = th.closest('table');
      if (table) {
        const allThs = table.querySelectorAll<HTMLTableCellElement>('thead tr th');
        const offset = this.accionesProcesadas.length > 0 ? 1 : 0;
        const widths: Record<string, number> = {};
        this.columnasVisibles.forEach((col, i) => {
          const el = allThs[i + offset];
          if (el) widths[String(col.key)] = el.offsetWidth;
        });
        this.columnWidths.set(widths);
      }
    }

    this.resizingKey = String(key);
    this.resizeStartX = event.clientX;
    this.resizeStartWidth = this.columnWidths()[String(key)] ?? th.offsetWidth;

    document.body.style.cursor = 'col-resize';
    document.body.style.userSelect = 'none';
  }

  @HostListener('document:mousemove', ['$event'])
  onResizeMove(event: MouseEvent): void {
    if (!this.resizingKey) return;
    const delta = event.clientX - this.resizeStartX;
    const newWidth = Math.max(60, this.resizeStartWidth + delta);
    this.columnWidths.update(w => ({ ...w, [this.resizingKey!]: newWidth }));
  }

  @HostListener('document:mouseup')
  onResizeEnd(): void {
    if (!this.resizingKey) return;
    this.resizingKey = null;
    document.body.style.cursor = '';
    document.body.style.userSelect = '';
  }

  setDensidad(d: Densidad) {
    this.densidad.set(d);
    localStorage.setItem('datatable-density', d);
  }

  toggleMenuConfiguracion(event: Event, btnElement: HTMLElement) {
    event.stopPropagation();
    if (this.menuActivo() === 'configuracion') {
      this.menuActivo.set(null);
      return;
    }
    const rect = btnElement.getBoundingClientRect();
    this.configuracionPosicion = {
      top: `${rect.bottom + 8}px`,
      left: `${rect.right - 208}px`,
    };
    this.menuActivo.set('configuracion');
  }

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

    if (this.serverSide) {
      this.paginaActualSignal.set(1);
      if (!this.hayFiltroVacio) this.emitServerQuery();
    } else {
      this.onFilterChange();
    }
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

    if (this.serverSide) {
      this.paginaActualSignal.set(1);
      if (!this.hayFiltroVacio) this.emitServerQuery();
    } else {
      this.onFilterChange();
    }
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

  limpiarFiltrosColumna() {
    this.filtrosSeleccion = {};
    this.busquedaFiltro = {};
    this.menuActivo.set(null);
  }

  limpiarBusqueda() {
    this.busquedaGlobal = '';
    this.onSearchChange('');
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

  // Cuando cualquier filtro de columna tiene el set vacío → tabla sin datos
  get hayFiltroVacio(): boolean {
    return Object.values(this.filtrosSeleccion).some(set => set.size === 0);
  }
}