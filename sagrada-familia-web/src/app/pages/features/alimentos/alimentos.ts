import { Component, inject } from '@angular/core';
import { Auth } from '../../../core/services/auth';
import { CommonModule } from '@angular/common';
import { ListarAlimentos } from './listar-alimentos/listar-alimentos';
import { OrientacionPadres } from './orientacion-padres/orientacion-padres';

@Component({
  selector: 'app-alimentos',
  imports: [CommonModule, ListarAlimentos, OrientacionPadres],
  templateUrl: './alimentos.html',
  styleUrl: './alimentos.css',
})
export class Alimentos {
  readonly auth = inject(Auth);
}
