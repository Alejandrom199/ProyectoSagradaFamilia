import { Component, inject } from '@angular/core';
import { ListarPacientes } from "./listar-pacientes/listar-pacientes";
import { MisPequenos } from "./mis-pequenos/mis-pequenos";
import { Auth } from '../../../core/services/auth';

@Component({
  selector: 'pacientes',
  imports: [ListarPacientes, MisPequenos],
  templateUrl: './pacientes.html',
  styleUrl: './pacientes.css',
})
export class Pacientes {
  readonly auth = inject(Auth);
}
