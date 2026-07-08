import { Component, inject, signal, OnInit, HostListener } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgIcon } from "@ng-icons/core";


import { LoadingBar } from '../loading-bar/loading-bar';
import { Sidebar } from '../sidebar/sidebar';
import { Header } from '../header/header';
import { AuthService } from '../../../core/services/auth';
import { MenuService } from '../../../core/services/menu';
import { ServerTimeService } from '../../../core/services/server-time';

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

})
export class AdminLayout implements OnInit {
  readonly auth = inject(AuthService);
  private readonly menuService = inject(MenuService);
  readonly serverTime = inject(ServerTimeService);

  readonly sidebarExpandido = signal<boolean>(true);
  readonly sidebarMovilAbierto = signal<boolean>(false);
  readonly esMovil = signal<boolean>(window.innerWidth < 1024);

  @HostListener('window:resize')
  onResize() {
    this.esMovil.set(window.innerWidth < 1024);
  }

  readonly menu = this.menuService.menuItems;

  ngOnInit(): void {
    this.menuService.cargarMenu();
  }

  toggleExpandido(): void {
    this.sidebarExpandido.update(v => !v);
  }

  toggleMovil(): void {
    this.sidebarMovilAbierto.update(v => !v);
  }

  cerrarMovil(): void {
    this.sidebarMovilAbierto.set(false);
  }

  getMarginLeft(): string {
    if (this.esMovil()) return '0';
    return this.sidebarExpandido() ? '280px' : '64px';
  }

  logout(): void {
    this.auth.logout();
  }
}