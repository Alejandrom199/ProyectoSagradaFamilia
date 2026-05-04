import { Component, computed, OnInit, signal } from '@angular/core';
import { Auth } from '../../core/services/auth';
import { Ninos } from '../../core/services/ninos';
import { RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroCake, heroChartBar, heroHandRaised, heroHeart, heroUsers, heroSun, heroCloud, heroMoon } from '@ng-icons/heroicons/outline';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, NgIcon],
  viewProviders: [provideIcons({ heroUsers, heroHeart, heroChartBar, heroCake, heroHandRaised, heroSun, heroCloud, heroMoon })],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  stats = signal([
    { label: 'Pacientes', valor: '—', icon: 'heroUsers', color: '#2563eb', bgColor: '#dbeafe' },
    { label: 'Medidas', valor: '—', icon: 'heroChartBar', color: '#16a34a', bgColor: '#dcfce7' },
    { label: 'Alimentos', valor: '—', icon: 'heroCake', color: '#d97706', bgColor: '#fef3c7' },
    { label: 'Predicciones activas', valor: '—', icon: 'heroHeart', color: '#dc2626', bgColor: '#fee2e2' },
  ]);


  constructor(
    readonly auth: Auth,
    private ninosService: Ninos
  ) { }

  ngOnInit() {
    const esMedico = this.auth.esMedico();

    if (esMedico) {
      // Cargar estadísticas básicas
      this.ninosService.obtenerTodos().subscribe(response => {
        if (response.success) {
          this.stats.update(s => {
            s[0].valor = String(response.data.length);
            return [...s];
          });
        }
      });
    }
  }

  readonly saludoConfig = computed(() => {
    const hora = new Date().getHours();

    if (hora >= 6 && hora < 12) {
      return {
        texto: 'Buenos días',
        icono: 'heroSun',
        color: 'text-orange-500'
      };
    } else if (hora >= 12 && hora < 19) {
      return {
        texto: 'Buenas tardes',
        icono: 'heroCloud',
        color: 'text-blue-500'
      };
    } else {
      return {
        texto: 'Buenas noches',
        icono: 'heroMoon',
        color: 'text-indigo-600'
      };
    }
  });
}