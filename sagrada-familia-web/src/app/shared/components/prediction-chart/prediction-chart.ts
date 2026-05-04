import { Component, input } from '@angular/core';

@Component({
  selector: 'prediction-chart',
  imports: [],
  templateUrl: './prediction-chart.html',
  styleUrl: './prediction-chart.css',
})
export class PredictionChart {
  title = input<string>('Curva de Crecimiento OMS');
}
