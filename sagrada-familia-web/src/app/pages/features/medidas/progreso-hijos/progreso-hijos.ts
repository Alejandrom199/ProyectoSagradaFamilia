import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroFaceSmile, heroChartBar, heroScale, heroCalendarDays } from '@ng-icons/heroicons/outline';

import { NgApexchartsModule } from 'ng-apexcharts';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexXAxis,
  ApexDataLabels,
  ApexStroke,
  ApexYAxis,
  ApexTooltip,
  ApexTheme
} from 'ng-apexcharts';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { formatearFecha } from '../../../../shared/utils/date.utils';
import { finalize } from 'rxjs';
import { Breadcrumb, BreadcrumbItem } from "../../../../shared/components/breadcrumb/breadcrumb";
import { NinosService } from '../../../../core/services/ninos';
import { MedidasService } from '../../../../core/services/medidas';
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { MedidaResponse } from '../../../../shared/interfaces/medida.interface';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  dataLabels: ApexDataLabels;
  stroke: ApexStroke;
  tooltip: ApexTooltip;
  theme: ApexTheme;
  colors: string[];
};

@Component({
  selector: 'progreso-hijos',
  standalone: true,
  imports: [CommonModule, NgIcon, NgApexchartsModule, Breadcrumb],
  viewProviders: [provideIcons({ heroFaceSmile, heroChartBar, heroScale, heroCalendarDays })],
  templateUrl: './progreso-hijos.html',
  styleUrl: './progreso-hijos.css'
})
export class ProgresoHijos implements OnInit {
  private ninosService = inject(NinosService);
  private medidasService = inject(MedidasService);
  private loadingBar = inject(LoadingBar);
  private route = inject(ActivatedRoute);

  ninoSeleccionado = signal<NinoResponse | null>(null);
  ultimaMedida = signal<MedidaResponse | null>(null);
  cargando = signal(false);
  hayDatos = signal(false);

  readonly formatearFecha = formatearFecha;
  chartOptions: Partial<ChartOptions> | null = null;

  migajas: BreadcrumbItem[] = [
    { label: 'Progreso del niño' },
  ];

  ngOnInit() {
    const ninoIdParam = this.route.snapshot.queryParamMap.get('ninoId');
    if (ninoIdParam) {
      const idBuscar = parseInt(ninoIdParam);
      this.ninosService.obtenerPorId(idBuscar)
        .pipe(finalize(() => this.loadingBar.complete()))
        .subscribe({
          next: (r) => {
            if (r.success && r.data) {
              this.seleccionarNino(r.data);
            }
          },
          error: () => this.loadingBar.complete()
        });
    }
  }

  seleccionarNino(nino: NinoResponse) {
    this.ninoSeleccionado.set(nino);
    this.cargando.set(true);
    this.loadingBar.show();

    this.medidasService.obtenerPorNino(nino.id)
      .pipe(finalize(() => {
        this.cargando.set(false);
        this.loadingBar.complete();
      }))
      .subscribe({
        next: (r) => {
          if (r.success && r.data.length > 0) {
            this.hayDatos.set(true);
            const ordenadasPorFechaDesc = [...r.data].sort((a, b) =>
              new Date(b.fechaMedicion).getTime() - new Date(a.fechaMedicion).getTime()
            );
            this.ultimaMedida.set(ordenadasPorFechaDesc[0]);
            this.construirGrafica(r.data);
          } else {
            this.hayDatos.set(false);
            this.ultimaMedida.set(null);
            this.chartOptions = null;
          }
        },
        error: () => {
          this.hayDatos.set(false);
          this.ultimaMedida.set(null);
          this.cargando.set(false);
          this.loadingBar.complete();
        }
      });
  }

  obtenerClaseEstado(estado: string): string {
    const clases: Record<string, string> = {
      'Normal': 'bg-green-100 text-green-700 border-green-200',
      'BajoPeso': 'bg-yellow-100 text-yellow-700 border-yellow-200',
      'BajoPesoSevero': 'bg-red-100 text-red-700 border-red-200',
      'Sobrepeso': 'bg-orange-100 text-orange-700 border-orange-200',
      'Obesidad': 'bg-red-100 text-red-700 border-red-200'
    };
    return clases[estado] || 'bg-gray-100 text-gray-700 border-gray-200';
  }

  private construirGrafica(medidas: MedidaResponse[]) {
    const medidasCronologicas = [...medidas].sort((a, b) =>
      new Date(a.fechaMedicion).getTime() - new Date(b.fechaMedicion).getTime()
    );

    const dataPeso = medidasCronologicas.map(m => {
      const [year, month, day] = m.fechaMedicion.split('-').map(Number);
      return [
        new Date(year, month - 1, day).getTime(),
        m.peso
      ];
    });

    this.chartOptions = {
      series: [
        {
          name: 'Peso (kg)',
          data: dataPeso
        }
      ],
      chart: {
        type: 'area',
        height: 380,
        fontFamily: 'inherit',
        zoom: { enabled: true },
        toolbar: { show: false }
      },
      colors: ['#3b82f6'],
      dataLabels: { enabled: false },
      stroke: { curve: 'smooth', width: 3 },
      xaxis: {
        type: 'datetime',
        labels: {
          datetimeUTC: false,
          style: { colors: '#64748b', fontSize: '12px' }
        }
      },
      yaxis: {
        title: { text: 'Peso en Kilogramos', style: { color: '#64748b', fontWeight: 600 } },
        min: 0,
        labels: { style: { colors: '#64748b' } }
      },
      tooltip: { x: { format: 'dd MMM yyyy' } },
      theme: { mode: 'light' }
    };
  }
}