import { Pipe, PipeTransform } from '@angular/core';
import { AccionPermisoResponse } from '../interfaces/permiso.interface';

@Pipe({ name: 'accionPorNombre', standalone: true })
export class AccionPorNombrePipe implements PipeTransform {
  transform(acciones: AccionPermisoResponse[], nombre: string): AccionPermisoResponse | undefined {
    return acciones.find(a => a.accionNombre === nombre);
  }
}