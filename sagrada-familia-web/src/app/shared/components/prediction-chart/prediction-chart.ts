import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
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
  ApexDataLabels
} from 'ng-apexcharts';

import { PuntoPrediccion } from '../../interfaces/prediccion.interface';

export type PredictionChartOptions = {
  series:      ApexAxisChartSeries;
  chart:       ApexChart;
  xaxis:       ApexXAxis;
  yaxis:       ApexYAxis;
  stroke:      ApexStroke;
  fill:        ApexFill;
  markers:     ApexMarkers;
  tooltip:     ApexTooltip;
  legend:      ApexLegend;
  dataLabels:  ApexDataLabels;
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
  predicciones = input.required<PuntoPrediccion[]>();
  title        = input<string>('Evolución de Peso Proyectada');

  chartOptions = computed<Partial<PredictionChartOptions> | null>(() => {
    const pts = this.predicciones();
    if (!pts || pts.length === 0) return null;
    return this.construirOpciones(pts);
  });

  private construirOpciones(pts: PuntoPrediccion[]): Partial<PredictionChartOptions> {
    const ts = (fecha: string) => new Date(fecha + 'T00:00:00').getTime();

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
      { name: 'Rango de confianza', type: 'rangeArea', data: bandaConfianza as any },
      { name: 'Peso predicho',      type: 'line',      data: lineaPredicha },
      ...(puntosReales.length > 0
        ? [{ name: 'Peso real medido', type: 'scatter', data: puntosReales }]
        : [])
    ];

    return {
      series,
      chart: {
        type: 'rangeArea',
        height: 320,
        toolbar: { show: false },
        fontFamily: 'inherit',
        zoom: { enabled: false }
      },
      colors: ['#bfdbfe', '#2563eb', '#16a34a'],
      fill: { opacity: [0.35, 1, 1] },
      stroke: {
        width:      [0,  2.5, 0],
        curve:      'smooth',
        dashArray:  [0,  5,   0]
      },
      markers: {
        size:         [0, 3, 7],
        colors:       ['transparent', '#2563eb', '#16a34a'],
        strokeColors: ['transparent', '#2563eb', '#15803d'],
        strokeWidth:  [0, 0, 2]
      },
      dataLabels: { enabled: false },
      xaxis: {
        type: 'datetime',
        labels: {
          datetimeUTC: false,
          style: { colors: '#64748b', fontSize: '11px' }
        }
      },
      yaxis: {
        title: { text: 'Peso (kg)', style: { color: '#64748b', fontWeight: 600 } },
        labels: { style: { colors: '#64748b', fontSize: '11px' } }
      },
      tooltip: {
        shared:   false,
        x: { format: 'MMM yyyy' }
      },
      legend: {
        show:      true,
        position:  'top',
        fontSize:  '12px',
        fontFamily: 'inherit'
      }
    };
  }
}
