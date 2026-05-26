import { Component, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { Breadcrumb, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb';
import { Button } from '../../../../shared/components/button/button';

interface RolItem {
  id: number;
  nombre: string;
  descripcion: string;
  editable: boolean;
}

@Component({
  selector: 'app-listar-roles',
  standalone: true,
  imports: [Breadcrumb, NgIcon],

  templateUrl: './listar-roles.html',
})
export class ListarRoles {
  private router = new Router();

  readonly roles = signal<RolItem[]>([
    { id: 1, nombre: 'Administrador', descripcion: 'Acceso total al sistema', editable: false },
    { id: 2, nombre: 'Médico', descripcion: 'Gestión de citas, pacientes y prescripciones', editable: true },
    { id: 3, nombre: 'Padre', descripcion: 'Acceso a información de sus hijos', editable: true },
  ]);

  readonly migajas: BreadcrumbItem[] = [
    { label: 'Roles y Permisos' }
  ];

  gestionarPermisos(rolId: number): void {
    this.router.navigate(['/roles', rolId]);
  }
}