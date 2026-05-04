import { Component, Input, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroHome, heroChevronRight } from '@ng-icons/heroicons/outline';
import { CommonModule } from '@angular/common';

export interface BreadcrumbItem {
  label: string;
  ruta?: string | any[];
}
@Component({
  selector: 'breadcrumb',
  imports: [CommonModule, RouterLink, NgIcon],
  viewProviders: [provideIcons({ heroHome, heroChevronRight })],
  templateUrl: './breadcrumb.html',
  styleUrl: './breadcrumb.css',
})
export class Breadcrumb {
  @Input() items: BreadcrumbItem[] = [];

  // Puedes recibir una ruta base para el icono de la casita (ej: '/dashboard' o '/pacientes')
  @Input() homeRoute: string | any[] = '/dashboard';
}
