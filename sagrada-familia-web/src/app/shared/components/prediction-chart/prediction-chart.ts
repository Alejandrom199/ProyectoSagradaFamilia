import { Component, input, computed, viewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule, ChartComponent } from 'ng-apexcharts';

import { ThemeService } from '../../../core/services/theme';
import type {
  ApexAxisChartSeries,
  ApexChart,
  ApexXAxis,
  ApexYAxis,
  ApexStroke,
  ApexFill,
  ApexMarkers,
  ApexTooltip,
  ApexLegend,
  ApexDataLabels,
  ApexAnnotations,
  ApexTheme
} from 'ng-apexcharts';

import { PuntoPrediccion, CurvasOmsResponse } from '../../interfaces/prediccion.interface';

export type PredictionChartOptions = {
  series:      ApexAxisChartSeries;
  chart:       ApexChart;
  theme:       ApexTheme;
  xaxis:       ApexXAxis;
  yaxis:       ApexYAxis;
  stroke:      ApexStroke;
  fill:        ApexFill;
  markers:     ApexMarkers;
  tooltip:     ApexTooltip;
  legend:      ApexLegend;
  dataLabels:  ApexDataLabels;
  annotations: ApexAnnotations;
  colors:      string[];
};

@Component({
  selector: 'prediction-chart',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule],
  templateUrl: './prediction-chart.html',
  styleUrl: './prediction-chart.css',
})
export class PredictionChart {
  private readonly chartRef = viewChild<ChartComponent>('chartRef');
  private readonly themeService = inject(ThemeService);

  predicciones = input.required<PuntoPrediccion[]>();
  curvasOms    = input<CurvasOmsResponse | null>(null);
  title        = input<string>('Curva de Crecimiento - Peso');

  async capturarImagen(): Promise<string | null> {
    const chart = this.chartRef();
    if (!chart) return null;
    try {
      const resultado = await chart.dataURI() as { imgURI?: string };
      return resultado.imgURI ?? null;
    } catch {
      return null;
    }
  }

  chartOptions = computed<Partial<PredictionChartOptions> | null>(() => {
    const pts = this.predicciones();
    if (!pts || pts.length === 0) return null;
    return this.construirOpciones(pts, this.curvasOms(), this.themeService.oscuro());
  });

  private construirOpciones(
    pts: PuntoPrediccion[],
    oms: CurvasOmsResponse | null,
    oscuro: boolean
  ): Partial<PredictionChartOptions> {
    const colorEje = oscuro ? '#a1a1aa' : '#64748b';
    const ts = (fecha: string) => new Date(fecha + 'T00:00:00').getTime();

    // ── Series del modelo Prophet ──────────────────────────────────────────────
    const bandaConfianza = pts.map(p => ({
      x: ts(p.fechaObjetivo),
      y: [p.pesoMinimo, p.pesoMaximo]
    }));

    const lineaPredicha = pts.map(p => ({
      x: ts(p.fechaObjetivo),
      y: p.pesoPredicho
    }));

    const puntosReales = pts
      .filter(p => p.pesoReal !== null)
      .map(p => ({ x: ts(p.fechaObjetivo), y: p.pesoReal as number }));

    const series: ApexAxisChartSeries = [
      { name: 'Rango de confianza (80%)', type: 'rangeArea', data: bandaConfianza as any },
      { name: 'Peso predicho',             type: 'line',      data: lineaPredicha },
      ...(puntosReales.length > 0
        ? [{ name: 'Peso real medido', type: 'scatter', data: puntosReales }]
        : [])
    ];

    const colors = ['#bfdbfe', '#2563eb', '#16a34a'];
    const fillOpacity = [0.3, 1, 1];
    const strokeWidth = [0, 2.5, 0];
    const strokeDash  = [0, 5, 0];
    const markerSize  = [0, 3, 7];

    // ── Curvas OMS como líneas de referencia ──────────────────────────────────
    if (oms && oms.curvas.length > 0) {
      // Construimos la fecha base desde la fecha objetivo más próxima
      // Para curvas OMS usamos edad en meses → convertimos a timestamp
      // Tomamos la primera fecha objetivo como referencia de "hoy"
      const hoy = new Date(pts[0].fechaObjetivo + 'T00:00:00');

      const omsTs = (edadMeses: number) => {
        const d = new Date(hoy);
        d.setMonth(d.getMonth() + edadMeses);
        return d.getTime();
      };

      const p3  = oms.curvas.map(c => ({ x: omsTs(c.edadMeses - oms.curvas[0].edadMeses), y: Number(c.percentil3) }));
      const p15 = oms.curvas.map(c => ({ x: omsTs(c.edadMeses - oms.curvas[0].edadMeses), y: Number(c.percentil15) }));
      const p50 = oms.curvas.map(c => ({ x: omsTs(c.edadMeses - oms.curvas[0].edadMeses), y: Number(c.percentil50) }));
      const p85 = oms.curvas.map(c => ({ x: omsTs(c.edadMeses - oms.curvas[0].edadMeses), y: Number(c.percentil85) }));
      const p97 = oms.curvas.map(c => ({ x: omsTs(c.edadMeses - oms.curvas[0].edadMeses), y: Number(c.percentil97) }));

      series.push(
        { name: 'P3  (OMS)',  type: 'line', data: p3  },
        { name: 'P15 (OMS)',  type: 'line', data: p15 },
        { name: 'P50 (OMS)', type: 'line', data: p50 },
        { name: 'P85 (OMS)',  type: 'line', data: p85 },
        { name: 'P97 (OMS)',  type: 'line', data: p97 },
      );

      colors.push('#ef4444', '#f97316', '#10b981', '#f97316', '#ef4444');
      fillOpacity.push(1, 1, 1, 1, 1);
      strokeWidth.push(1, 1, 1.5, 1, 1);
      strokeDash.push(4, 3, 0, 3, 4);
      markerSize.push(0, 0, 0, 0, 0);
    }

    return {
      series,
      chart: {
        type: 'rangeArea',
        height: 380,
        toolbar: { show: false },
        fontFamily: 'inherit',
        zoom: { enabled: false },
        background: 'transparent',
        foreColor: colorEje
      },
      theme: { mode: oscuro ? 'dark' : 'light' },
      colors,
      fill:    { opacity: fillOpacity },
      stroke:  { width: strokeWidth, curve: 'smooth', dashArray: strokeDash },
      markers: {
        size:         markerSize,
        colors:       ['transparent', '#2563eb', '#16a34a'],
        strokeColors: ['transparent', '#2563eb', '#15803d'],
        strokeWidth:  [0, 0, 2]
      },
      dataLabels: { enabled: false },
      xaxis: {
        type: 'datetime',
        labels: { datetimeUTC: false, style: { colors: colorEje, fontSize: '11px' } }
      },
      yaxis: {
        title: { text: 'Peso (kg)', style: { color: colorEje, fontWeight: 600 } },
        labels: { style: { colors: colorEje, fontSize: '11px' } }
      },
      tooltip: { shared: false, theme: oscuro ? 'dark' : 'light', x: { format: 'MMM yyyy' } },
      legend: {
        show:       true,
        position:   'top',
        fontSize:   '11px',
        fontFamily: 'inherit',
        markers:    { size: 8 },
        labels:     { colors: colorEje }
      },
      annotations: {}
    };
  }
}
