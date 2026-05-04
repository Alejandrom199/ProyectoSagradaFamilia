import { Component, input, output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import {
  heroHome, heroUsers, heroHeart, heroChartBar,
  heroCake, heroArrowRightOnRectangle, heroBars3,
  heroUserCircle, heroChevronLeft, heroChevronRight
} from '@ng-icons/heroicons/outline';

import { MenuResponse } from '../../interfaces/responses/menu.response';
import { Auth } from '../../../core/services/auth';
import { Button } from "../button/button";
import { Tooltip } from "../../directives/tooltip/tooltip";

@Component({
  selector: 'sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, NgIcon, Button, Tooltip],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
  viewProviders: [provideIcons({
    heroHome, heroUsers, heroHeart, heroChartBar,
    heroCake, heroArrowRightOnRectangle, heroBars3,
    heroUserCircle, heroChevronLeft, heroChevronRight
  })],
})
export class Sidebar {
  readonly auth = inject(Auth);

  // Inputs desde AdminLayout
  sidebarExpandido = input.required<boolean>();
  sidebarMovilAbierto = input.required<boolean>();
  menu = input.required<MenuResponse[]>();

  // Notificamos eventos al Layout
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

  resolverIcono(icono: string): string {
    return this.iconoMap[icono] ?? 'heroHome';
  }

  iniciales(): string {
    const nombre = this.auth.currentUser()?.nombre ?? '';
    return nombre.split(' ').slice(0, 2).map(p => p[0]).join('').toUpperCase();
  }
}