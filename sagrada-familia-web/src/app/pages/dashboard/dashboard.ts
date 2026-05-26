import { Component, computed, OnInit, signal, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';
import { CommonModule } from '@angular/common';


import { AuthService } from '../../core/services/auth';
import { NinosService } from '../../core/services/ninos';
import { AlimentosService } from '../../core/services/alimentos';
import { CitasService } from '../../core/services/citas';

@Component({
  selector: 'dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, NgIcon],

  templateUrl: './dashboard.html',
})
export class Dashboard implements OnInit {
  readonly auth = inject(AuthService);
  private ninosService = inject(NinosService);
  private alimentosService = inject(AlimentosService);
  private citasService = inject(CitasService);

  stats = signal([
    { label: 'Pacientes', valor: '—', icon: 'matPeopleOutline', color: '#2563eb', bgColor: '#dbeafe', ruta: '/pacientes' },
    { label: 'Citas hoy', valor: '—', icon: 'matCalendarMonthOutline', color: '#7c3aed', bgColor: '#ede9fe', ruta: '/citas' },
    { label: 'Alimentos', valor: '—', icon: 'matCakeOutline', color: '#d97706', bgColor: '#fef3c7', ruta: '/alimentos' },
    { label: 'Predicciones', valor: '—', icon: 'matBarChartOutline', color: '#16a34a', bgColor: '#dcfce7', ruta: '/predicciones' },
  ]);

  ngOnInit() {
    if (!this.auth.esMedico()) return;

    // Pacientes
    this.ninosService.obtenerTodos().subscribe(r => {
      if (r.success) this.actualizarStat(0, String(r.data.length));
    });

    // Citas de hoy
    this.citasService.obtenerMisCitasHoy().subscribe(r => {
      if (r.success) this.actualizarStat(1, String(r.data.length));
    });

    // Alimentos
    this.alimentosService.obtenerTodos().subscribe(r => {
      if (r.success) this.actualizarStat(2, String(r.data.length));
    });
  }

  private actualizarStat(index: number, valor: string): void {
    this.stats.update(s => {
      const copia = [...s];
      copia[index] = { ...copia[index], valor };
      return copia;
    });
  }

  readonly saludoConfig = computed(() => {
    const hora = new Date().getHours();
    if (hora >= 6 && hora < 12) return { texto: 'Buenos días', icono: 'heroSun', color: 'text-orange-500' };
    if (hora >= 12 && hora < 19) return { texto: 'Buenas tardes', icono: 'heroCloud', color: 'text-blue-500' };
    return { texto: 'Buenas noches', icono: 'heroMoon', color: 'text-indigo-600' };
  });
}