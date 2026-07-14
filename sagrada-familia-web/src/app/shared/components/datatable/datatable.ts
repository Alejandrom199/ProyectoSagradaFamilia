import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, signal, computed, Output, EventEmitter, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

import { Tooltip } from "../../directives/tooltip/tooltip";
import { formatearFecha } from "../../utils/date.utils";
import * as XLSX from 'xlsx';

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
  @Input() mobileTitleKey?: string;

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

  columnasPosicion: Record<string, string> = { top: '0px', left: '0px' };
  exportarPosicion: Record<string, string> = { top: '0px', left: '0px' };
  menuPosicion: Record<string, string> = { top: '0px', left: '0px' };
  filtroPosicion: Record<string, string> = { top: '0px', left: '0px' };
  configuracionPosicion: Record<string, string> = { top: '0px', left: '0px' };
  ordenarFiltrarPosicion: Record<string, string> = { top: '0px', left: '0px' };

  // Calcula la posición de un menú flotante (fixed) evitando que se salga de la ventana:
  // en pantallas angostas (móvil), un menú anclado con "left"/"right" sin límites puede
  // quedar parcialmente fuera del viewport, y uno anclado con "top: rect.bottom" puede
  // quedar cortado contra el borde inferior si se abre cerca del final de la pantalla
  // (por eso se voltea hacia arriba, con "bottom", cuando no hay espacio suficiente debajo).
  private posicionFlotante(rect: DOMRect, anchoMenu: number, alinearDerecha = false, altoEstimado = 300): Record<string, string> {
    const margen = 8;
    let left = alinearDerecha ? rect.right - anchoMenu : rect.left;
    left = Math.min(Math.max(left, margen), window.innerWidth - anchoMenu - margen);

    const espacioAbajo = window.innerHeight - rect.bottom;
    if (espacioAbajo < altoEstimado && rect.top > espacioAbajo) {
      return { bottom: `${window.innerHeight - rect.top + 4}px`, left: `${left}px` };
    }
    return { top: `${rect.bottom + 4}px`, left: `${left}px` };
  }

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
    this.menuPosicion = this.posicionFlotante(rect, 160, false, 220);
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
    this.filtroPosicion = this.posicionFlotante(rect, 256, false, 380);
    this.menuActivo.set(menuKey);
  }

  toggleMenuExportar(event: Event, btnElement: HTMLElement) {
    event.stopPropagation();

    if (this.menuActivo() === 'exportar') {
      this.menuActivo.set(null);
      return;
    }

    const rect = btnElement.getBoundingClientRect();
    this.exportarPosicion = this.posicionFlotante(rect, 176, true, 220);
    this.menuActivo.set('exportar');
  }

  toggleMenuColumnas(event: Event, btnElement: HTMLElement) {
    event.stopPropagation();

    if (this.menuActivo() === 'columnas') {
      this.menuActivo.set(null);
      return;
    }

    const rect = btnElement.getBoundingClientRect();
    this.columnasPosicion = this.posicionFlotante(rect, 192, true, 300);
    this.menuActivo.set('columnas');
  }

  toggleMenuOrdenarFiltrar(event: Event, btnElement: HTMLElement) {
    event.stopPropagation();

    if (this.menuActivo() === 'ordenar-filtrar') {
      this.menuActivo.set(null);
      return;
    }

    const rect = btnElement.getBoundingClientRect();
    this.ordenarFiltrarPosicion = this.posicionFlotante(rect, 260, true, 380);
    this.menuActivo.set('ordenar-filtrar');
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

  // Los valores tipo fecha/datetime (ISO) se truncan al día para filtrar: evita que cada
  // fila con hora/minuto/segundo distinto aparezca como un valor único en el desplegable.
  private static readonly REGEX_FECHA_ISO = /^\d{4}-\d{2}-\d{2}(T|$)/;

  private valorParaFiltro(row: T, key: string | number | symbol): string {
    // Si la columna define exportValue, es la representación "humana" del dato
    // (ej. true/false -> "Activo"/"Inactivo") — hay que filtrar sobre esa, no sobre el
    // valor crudo, porque las opciones del desplegable (filterOptions) están en ese formato.
    const col = this.columns.find(c => c.key === key);
    if (col?.exportValue) return col.exportValue(row);

    const raw = this.getCellValue(row, key);
    if (raw == null) return '';
    const str = String(raw);
    return Datatable.REGEX_FECHA_ISO.test(str) ? formatearFecha(str) : str;
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

    const headers = this.columns.map(c => c.label);
    const filas = this.datosFiltrados.map(row => this.columns.map(col => {
      if (col.exportValue) return col.exportValue(row);
      if (col.render) return this.limpiarHtml(col.render(row));
      return String(this.getCellValue(row, col.key) ?? '');
    }));

    // Excel real (.xlsx), no un CSV renombrado: se arma con SheetJS en el navegador.
    const hoja = XLSX.utils.aoa_to_sheet([headers, ...filas]);
    hoja['!cols'] = headers.map((_, i) => ({
      wch: Math.min(60, Math.max(10, ...filas.map(f => String(f[i] ?? '').length), headers[i].length) + 2)
    }));

    // Los nombres de hoja de Excel no aceptan \ / * ? : [ ] y están limitados a 31 caracteres.
    const nombreHoja = (this.title || 'Datos').replace(/[\\/*?:[\]]/g, '').slice(0, 31) || 'Datos';

    const libro = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(libro, hoja, nombreHoja);
    XLSX.writeFile(libro, `${this.exportFileName}_${new Date().toISOString().split('T')[0]}.xlsx`);
  }

  exportarPdf() {
    this.onExportPdf.emit(this.filtros);
  }

  obtenerValoresUnicos(key: string | number | symbol): string[] {
    const columna = String(key);
    const opcionesServidor = this.filterOptions[columna];
    if (this.serverSide && opcionesServidor?.length) return opcionesServidor;
    const valores = this.data.map(row => this.valorParaFiltro(row, columna));
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

  get columnaTituloMovil(): DatatableColumn<T> | undefined {
    const visibles = this.columnasVisibles;
    if (this.mobileTitleKey) {
      const porKey = visibles.find(c => c.key === this.mobileTitleKey);
      if (porKey) return porKey;
    }
    return visibles[0];
  }

  get columnasCard(): DatatableColumn<T>[] {
    const titulo = this.columnaTituloMovil;
    return this.columnasVisibles.filter(c => c !== titulo);
  }

  get columnasOrdenables(): DatatableColumn<T>[] {
    return this.columnasVisibles.filter(c => c.sortable);
  }

  tieneFiltroDisponible(col: DatatableColumn<T>): boolean {
    return !!col.filterable && (!this.serverSide || !!this.filterOptions[col.key]?.length || this.obtenerValoresUnicos(col.key).length > 0);
  }

  get columnasFiltrables(): DatatableColumn<T>[] {
    return this.columnasVisibles.filter(c => this.tieneFiltroDisponible(c));
  }

  get columnaFiltroActiva(): DatatableColumn<T> | undefined {
    const activo = this.menuActivo();
    if (!activo?.startsWith('filtro-')) return undefined;
    const key = activo.slice('filtro-'.length);
    return this.columns.find(c => String(c.key) === key);
  }

  get densidadPaddingCard(): string {
    return this.densidad() === 'compacto' ? 'p-2' : this.densidad() === 'espacioso' ? 'p-6' : 'p-4';
  }

  get accionesProcesadas(): DatatableAction<T>[] {
    const defaults: Record<ActionType, Partial<DatatableAction<T>>> = {
      ver: { label: 'Ver', icon: 'matVisibilityOutline', class: 'text-blue-600 dark:text-blue-400' },
      editar: { label: 'Editar', icon: 'matEditOutline', class: 'text-amber-600 dark:text-amber-400' },
      eliminar: { label: 'Eliminar', icon: 'matDeleteOutline', class: 'text-red-600 dark:text-red-400' },
      medidas: { label: 'Medidas', icon: 'matBarChartOutline', class: 'text-purple-600 dark:text-purple-400' },
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
        const val = this.valorParaFiltro(row, key);
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
    // En modo servidor, this.data ya es solo la página actual (traída del backend):
    // igual aplicamos el filtro de columna localmente sobre esa página, porque
    // columnFilters no siempre llega/se usa en el backend de cada listado.
    if (this.serverSide) return this.datosFiltrados;
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
    this.configuracionPosicion = this.posicionFlotante(rect, 208, true, 220);
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