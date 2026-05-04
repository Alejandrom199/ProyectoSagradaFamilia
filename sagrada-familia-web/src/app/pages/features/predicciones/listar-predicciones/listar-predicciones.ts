import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroChartBar, heroExclamationTriangle, heroUser } from '@ng-icons/heroicons/outline'; // Añadí heroUser
import { NinoResponse } from '../../../../shared/interfaces/responses/nino.response';
import { PrediccionResponse, PuntoPrediccion } from '../../../../shared/interfaces/responses/prediccion.response';
import { Predicciones as PrediccionesService } from '../../../../core/services/predicciones';
import { Auth } from '../../../../core/services/auth';
import { formatearFecha } from '../../../../shared/utils/date.utils';
import { Datatable, DatatableColumn } from '../../../../shared/components/datatable/datatable';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Ninos } from '../../../../core/services/ninos';

// NUEVOS IMPORTES COMPARTIDOS
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { SpinnerModal } from '../../../../shared/components/spinner-modal/spinner-modal';
import { PredictionChart } from '../../../../shared/components/prediction-chart/prediction-chart';
import { Breadcrumb, BreadcrumbItem } from "../../../../shared/components/breadcrumb/breadcrumb";

@Component({
  selector: 'app-listar-predicciones',
  imports: [
    CommonModule,
    FormsModule,
    NgIcon,
    Datatable,
    StatusBadge,
    SpinnerModal,
    PredictionChart,
    Breadcrumb
  ],
  viewProviders: [provideIcons({ heroChartBar, heroExclamationTriangle, heroUser })],
  templateUrl: './listar-predicciones.html',
  styleUrl: './listar-predicciones.css',
})
export class ListarPredicciones implements OnInit {
  private ninosService = inject(Ninos);
  private prediccionesService = inject(PrediccionesService);
  private loadingBar = inject(LoadingBar);
  readonly auth = inject(Auth);

  ninos = signal<NinoResponse[]>([]);
  ninoSeleccionado = signal<NinoResponse | null>(null);
  prediccion = signal<PrediccionResponse | null>(null);
  cargando = signal(false);

  estadoMotor = signal<'comprobando' | 'online' | 'offline'>('comprobando');
  versionMotor = signal<string>('');

  readonly formatearFecha = formatearFecha;

  // Actualización de columnas usando el componente visual en vez de HTML puro en string
  columnas: DatatableColumn<PuntoPrediccion>[] = [
    {
      key: 'meses', label: 'Mes', sortable: true,
      render: (r) => `Mes ${r.meses}`
    },
    {
      key: 'fechaObjetivo', label: 'Fecha objetivo', sortable: true,
      render: (r) => formatearFecha(r.fechaObjetivo)
    },
    {
      key: 'pesoPredicho', label: 'Peso predicho (kg)', sortable: true,
      render: (r) => `<span class="font-bold text-blue-600">${r.pesoPredicho.toFixed(2)} kg</span>`
    },
    {
      key: 'pesoMinimo', label: 'Rango de confianza',
      render: (r) => `<span class="text-xs text-gray-500">${r.pesoMinimo.toFixed(2)} — ${r.pesoMaximo.toFixed(2)} kg</span>`
    },
    {
      key: 'pesoReal', label: 'Peso real',
      // Mantenemos string HTML para la datatable, pero usamos clases de Tailwind consistentes
      render: (r) => r.pesoReal !== null
        ? `<span class="px-2.5 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800">${r.pesoReal.toFixed(2)} kg</span>`
        : '<span class="text-gray-400 font-medium">Aún sin medir</span>'
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Predicciones' },
  ];

  ngOnInit() {
    this.cargarNinos();
    this.comprobarEstadoMotor();
  }

  comprobarEstadoMotor() {
    this.prediccionesService.obtenerEstadoServicio().subscribe({
      next: (response) => {
        if (response.success && response.data.status === 'ok') {
          this.estadoMotor.set('online');
          this.versionMotor.set(response.data.version);
        } else {
          this.estadoMotor.set('offline');
        }
      },
      error: () => {
        this.estadoMotor.set('offline');
      }
    });
  }

  cargarNinos() {
    this.loadingBar.show();
    const obs = this.auth.esMedico()
      ? this.ninosService.obtenerTodos()
      : this.ninosService.obtenerMisNinos();

    obs.subscribe({
      next: (r) => { if (r.success) this.ninos.set(r.data); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  seleccionarNino(ninoId: number) {
    const nino = this.ninos().find(n => n.id === ninoId);
    if (!nino) { this.prediccion.set(null); this.ninoSeleccionado.set(null); return; }

    this.ninoSeleccionado.set(nino);
    this.cargando.set(true);
    this.loadingBar.show();

    this.prediccionesService.obtenerPorNino(ninoId).subscribe({
      next: (r) => {
        if (r.success) this.prediccion.set(r.data);
        this.cargando.set(false);
        this.loadingBar.complete();
      },
      error: () => { this.cargando.set(false); this.loadingBar.complete(); }
    });
  }
}