import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgIcon } from "@ng-icons/core";


import { LoadingBar } from '../loading-bar/loading-bar';
import { Sidebar } from '../sidebar/sidebar';
import { Header } from '../header/header';
import { AuthService } from '../../../core/services/auth';
import { MenuService } from '../../../core/services/menu';

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

  readonly sidebarExpandido = signal<boolean>(true);
  readonly sidebarMovilAbierto = signal<boolean>(false);

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

  isMovil(): boolean {
    return window.innerWidth < 1024;
  }

  getMarginLeft(): string {
    if (this.isMovil()) return '0';
    return this.sidebarExpandido() ? '280px' : '64px';
  }

  logout(): void {
    this.auth.logout();
  }
}