import { Component, OnInit, computed, signal, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

import { DatatableAction, DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { SistemaService } from '../../../../core/services/sistema';
import { AuthService } from '../../../../core/services/auth';
import { AuditoriaResponse } from '../../../../shared/interfaces/sistema.interface';
import { formatearFecha } from '../../../../shared/utils/date.utils';
import { JsonFormatPipe } from '../../../../shared/pipes/json-format.pipe';

const TABLAS_AUDITABLES = [
  'Padres', 'Medicos', 'Ninos', 'Medidas',
  'Citas', 'Prescripciones', 'Usuarios', 'Alimentos'
];

const TABLA_LABELS: Record<string, string> = {
  Ninos: 'Paciente',
  Citas: 'Consulta',
  Prescripciones: 'Receta médica',
  Medidas: 'Medición',
  Alimentos: 'Alimento',
  Medicos: 'Médico',
  Padres: 'Padre/Tutor',
  Usuarios: 'Usuario',
};

const ETIQUETAS_CAMPOS: Record<string, string> = {
  Nombre: 'Nombre',
  Apellido: 'Apellido',
  FechaNacimiento: 'Fecha de nacimiento',
  Genero: 'Género',
  Sangre: 'Tipo de sangre',
  Alergias: 'Alergias',
  FechaHora: 'Fecha y hora',
  Motivo: 'Motivo',
  Estado: 'Estado',
  NotasConsulta: 'Notas de consulta',
  DetalleMedicamentos: 'Medicamentos',
  Indicaciones: 'Indicaciones',
  Diagnostico: 'Diagnóstico',
  Peso: 'Peso (kg)',
  Talla: 'Talla (cm)',
  Temperatura: 'Temperatura (°C)',
  PerimetroCefalico: 'Perímetro cefálico (cm)',
  FechaMedicion: 'Fecha de medición',
  FechaCreacion: 'Fecha de registro',
  FechaModificacion: 'Última modificación',
  Descripcion: 'Descripción',
  Categoria: 'Categoría',
  EdadMinMeses: 'Edad mínima (meses)',
  EdadMaxMeses: 'Edad máxima (meses)',
  Recomendacion: 'Recomendación',
  Email: 'Correo electrónico',
  Activo: 'Activo',
};

const CAMPOS_OCULTOS = new Set([
  'Id', 'NinoId', 'MedicoId', 'PadreId', 'CitaId', 'UsuarioId',
  'UsuarioCreacionId', 'UsuarioModificacionId', 'Eliminado',
  'PasswordHash', 'PasswordSalt', 'RefreshToken', 'RefreshTokenExpiry',
]);

interface CampoModificado {
  clave: string;
  etiqueta: string;
  antes: string;
  despues: string;
}

@Component({
  selector: 'app-listar-auditoria',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, Breadcrumb, ReactiveFormsModule, JsonFormatPipe],
  templateUrl: './listar-auditoria.html',
  styleUrl: './listar-auditoria.css',
})
export class ListarAuditoria implements OnInit {
  private fb = inject(FormBuilder);
  private sistemaService = inject(SistemaService);
  private loadingBar = inject(LoadingBar);
  protected auth = inject(AuthService);

  formFiltro: FormGroup;
  registros = signal<AuditoriaResponse[]>([]);
  totalRegistros = signal(0);
  registroDetalle = signal<AuditoriaResponse | null>(null);
  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'desc' };

  tablas = TABLAS_AUDITABLES;

  // Columnas para administrador (técnicas)
  columnas: DatatableColumn<AuditoriaResponse>[] = [
    {
      key: 'fecha', label: 'Fecha', sortable: true,
      render: (row) => `<span class="text-sm text-slate-700">${formatearFecha(row.fecha)}</span>`
    },
    {
      key: 'accion', label: 'Acción', sortable: true, filterable: true,
      render: (row) => this.badgeAccion(row.accion)
    },
    {
      key: 'usuarioEmail', label: 'Usuario', sortable: true, filterable: true,
      render: (row) => `<span class="text-sm text-slate-700">${row.usuarioEmail || 'Sistema'}</span>`
    },
    {
      key: 'clavePrimaria', label: 'Registro',
      render: (row) => `<code class="text-xs bg-slate-100 px-2 py-1 rounded">${row.tabla} #${row.clavePrimaria}</code>`
    },
    {
      key: 'ipAddress', label: 'IP',
      render: (row) => row.ipAddress
        ? `<span class="text-xs text-slate-500">${row.ipAddress}</span>`
        : '<span class="text-slate-400">—</span>'
    }
  ];

  // Columnas para médico (sin tecnicismos)
  columnasMedico: DatatableColumn<AuditoriaResponse>[] = [
    {
      key: 'fecha', label: 'Fecha', sortable: true,
      render: (row) => `<span class="text-sm text-slate-700">${formatearFecha(row.fecha)}</span>`
    },
    {
      key: 'accion', label: 'Acción', sortable: true, filterable: true,
      render: (row) => this.badgeAccion(row.accion)
    },
    {
      key: 'tabla', label: 'Módulo',
      render: (row) => `<span class="text-sm text-slate-700">${this.tablaAmigable(row.tabla)}</span>`
    }
  ];

  get columnasActivas(): DatatableColumn<AuditoriaResponse>[] {
    return this.auth.esAdmin() ? this.columnas : this.columnasMedico;
  }

  // Campos procesados para la vista de médico
  camposModificados = computed<CampoModificado[]>(() => {
    const d = this.registroDetalle();
    if (!d) return [];

    const antes = this.parsearJson(d.valoresAntiguos);
    const despues = this.parsearJson(d.valoresNuevos);

    const fmtVal = (v: unknown): string => {
      if (v == null) return '—';
      if (typeof v === 'boolean') return v ? 'Sí' : 'No';
      return String(v);
    };

    if (!antes && despues) {
      return Object.entries(despues)
        .filter(([k]) => !CAMPOS_OCULTOS.has(k))
        .map(([k, v]) => ({ clave: k, etiqueta: ETIQUETAS_CAMPOS[k] ?? k, antes: '—', despues: fmtVal(v) }));
    }

    if (antes && !despues) {
      return Object.entries(antes)
        .filter(([k]) => !CAMPOS_OCULTOS.has(k))
        .map(([k, v]) => ({ clave: k, etiqueta: ETIQUETAS_CAMPOS[k] ?? k, antes: fmtVal(v), despues: '—' }));
    }

    if (antes && despues) {
      const claves = new Set([...Object.keys(antes), ...Object.keys(despues)]);
      return [...claves]
        .filter(k => !CAMPOS_OCULTOS.has(k))
        .map(k => ({
          clave: k,
          etiqueta: ETIQUETAS_CAMPOS[k] ?? k,
          antes: fmtVal(antes[k]),
          despues: fmtVal(despues[k])
        }))
        .filter(c => c.antes !== c.despues);
    }

    return [];
  });

  acciones: DatatableAction<AuditoriaResponse>[] = [
    { type: 'ver', label: 'Ver cambio', onClick: (row) => this.abrirDetalle(row) }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Sistema' },
    { label: 'Auditoría' },
  ];

  constructor() {
    this.formFiltro = this.fb.group({
      tabla: ['Padres', Validators.required],
      pk: ['']
    });
  }

  get f() { return this.formFiltro.controls; }

  ngOnInit(): void {
    this.cargarInicial();
  }

  cargarInicial(): void {
    this.loadingBar.show();

    if (this.auth.esAdmin()) {
      const { page, pageSize, search, sortBy, sortDir } = this.queryActual;
      this.sistemaService.obtenerAuditoriaPaginado(page, pageSize, search, sortBy, sortDir === 'asc').subscribe({
        next: (r) => {
          if (r.success) {
            this.registros.set(r.data);
            this.totalRegistros.set(r.totalItems);
          }
          this.loadingBar.complete();
        },
        error: () => this.loadingBar.complete()
      });
    } else {
      this.sistemaService.obtenerMiActividad().subscribe({
        next: (r) => {
          if (r.success) this.registros.set(r.data);
          this.loadingBar.complete();
        },
        error: () => this.loadingBar.complete()
      });
    }
  }

  onServerQuery(query: ServerQuery): void {
    this.queryActual = query;
    this.cargarInicial();
  }

  buscar(): void {
    if (this.formFiltro.invalid) {
      this.formFiltro.markAllAsTouched();
      return;
    }
    const { tabla, pk } = this.formFiltro.value;
    this.loadingBar.show();
    this.sistemaService.obtenerAuditoriaPorTabla(tabla, pk || undefined).subscribe({
      next: (r) => {
        if (r.success) this.registros.set(r.data);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  abrirDetalle(row: AuditoriaResponse): void {
    this.registroDetalle.set(row);
  }

  tablaAmigable(tabla: string): string {
    return TABLA_LABELS[tabla] ?? tabla;
  }

  private parsearJson(json?: string): Record<string, unknown> | null {
    if (!json) return null;
    try { return JSON.parse(json); } catch { return null; }
  }

  private badgeAccion(accion: string): string {
    const mapa: Record<string, string> = {
      'Creación': 'bg-green-100 text-green-700',
      'Actualización': 'bg-blue-100 text-blue-700',
      'Eliminación': 'bg-red-100 text-red-700',
    };
    const clase = mapa[accion] || 'bg-slate-100 text-slate-700';
    return `<span class="px-2 py-1 rounded-md text-[10px] font-bold uppercase ${clase}">${accion}</span>`;
  }
}
