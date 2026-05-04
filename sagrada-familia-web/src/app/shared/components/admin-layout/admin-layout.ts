import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgIcon, provideIcons } from "@ng-icons/core";
import {
  heroBars3, heroChevronLeft, heroChevronRight
} from '@ng-icons/heroicons/outline';

import { MenuResponse } from '../../interfaces/responses/menu.response';
import { LoadingBar } from '../loading-bar/loading-bar';
import { Sidebar } from '../sidebar/sidebar';
import { Header } from '../header/header'; // <-- Importación agregada
import { Auth } from '../../../core/services/auth';
import { Menu } from '../../../core/services/menu';

@Component({
  selector: 'admin-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    NgIcon,
    CommonModule,
    Sidebar,
    LoadingBar,
    Header
  ],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css',
  viewProviders: [provideIcons({
    heroBars3, heroChevronLeft, heroChevronRight
  })],
})
export class AdminLayout {
  readonly auth = inject(Auth);
  private readonly menuService = inject(Menu);

  readonly sidebarExpandido = signal(true);
  readonly sidebarMovilAbierto = signal(false);
  readonly menu = signal<MenuResponse[]>([]);

  constructor() {
    this.cargarMenu();
  }

  private cargarMenu() {
    this.menuService.obtenerMenu().subscribe({
      next: (response) => {
        if (response.success) this.menu.set(response.data);
      },
      error: (err) => console.error('Error al cargar el menú:', err)
    });
  }

  toggleExpandido() {
    this.sidebarExpandido.update(v => !v);
  }

  toggleMovil() {
    this.sidebarMovilAbierto.update(v => !v);
  }

  cerrarMovil() {
    this.sidebarMovilAbierto.set(false);
  }

  isMovil(): boolean {
    return window.innerWidth < 1024;
  }

  getMarginLeft(): string {
    if (this.isMovil()) return '0';
    return this.sidebarExpandido() ? '240px' : '56px';
  }

  logout() {
    this.auth.logout();
  }
}