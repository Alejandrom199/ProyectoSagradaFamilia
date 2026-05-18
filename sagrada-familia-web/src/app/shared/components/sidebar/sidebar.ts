import { Component, input, output, inject, signal, effect, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import {
  heroHome, heroUsers, heroHeart, heroChartBar,
  heroCake, heroArrowRightOnRectangle, heroBars3,
  heroUserCircle, heroChevronLeft, heroChevronRight, heroChevronDown
} from '@ng-icons/heroicons/outline';

import { Avatar } from '../avatar/avatar';
import { Tooltip } from '../../directives/tooltip/tooltip';
import { AuthService } from '../../../core/services/auth';
import { MenuResponse } from '../../interfaces/menu.interface';

@Component({
  selector: 'sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, NgIcon, Avatar, Tooltip],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
  viewProviders: [provideIcons({
    heroHome, heroUsers, heroHeart, heroChartBar,
    heroCake, heroArrowRightOnRectangle, heroBars3,
    heroUserCircle, heroChevronLeft, heroChevronRight, heroChevronDown
  })],
})
export class Sidebar implements OnInit {
  readonly auth = inject(AuthService);
  readonly router = inject(Router);

  sidebarExpandido = input.required<boolean>();
  sidebarMovilAbierto = input.required<boolean>();
  menu = input.required<MenuResponse[]>();

  nombre = signal<string>('');
  apellido = signal<string>('');

  // IDs de módulos abiertos
  modulosAbiertos = signal<Set<number>>(new Set());

  cerrarMovil = output<void>();
  logout = output<void>();

  private readonly iconoMap: Record<string, string> = {
    'home': 'heroHome',
    'users': 'heroUsers',
    'baby': 'heroUsers',
    'child': 'heroUsers',
    'chart-bar': 'heroChartBar',
    'chart-line': 'heroChartBar',
    'trending-up': 'heroChartBar',
    'apple': 'heroCake',
    'food': 'heroCake',
    'tag': 'heroCake',
    'user-group': 'heroUsers',
  };

  constructor() {
    // Auto-abrir el módulo cuya ruta coincide con la URL actual
    effect(() => {
      const menu = this.menu();
      const url = this.router.url;
      menu.forEach(modulo => {
        if (modulo.opciones.some(op => op.ruta && url.startsWith(op.ruta))) {
          this.modulosAbiertos.update(s => new Set(s).add(modulo.id));
        }
      });
    }, { allowSignalWrites: true });
  }

  ngOnInit(): void {
    const partes = this.auth.currentUser()!.nombre.split(' ');
    this.nombre.set(partes[0] ?? '');
    this.apellido.set(partes[1] ?? '');
  }

  toggleModulo(id: number): void {
    this.modulosAbiertos.update(set => {
      const next = new Set(set);
      next.has(id) ? next.delete(id) : next.add(id);
      return next;
    });
  }

  moduloEstaAbierto(id: number): boolean {
    return this.modulosAbiertos().has(id);
  }

  resolverIcono(icono: string): string {
    return this.iconoMap[icono] ?? 'heroHome';
  }

  iniciales(): string {
    return (this.auth.currentUser()?.nombre ?? '')
      .split(' ').slice(0, 2).map(p => p[0]).join('').toUpperCase();
  }
}