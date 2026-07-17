import { Component, computed, OnInit, signal, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';
import type { ApexChart, ApexAxisChartSeries, ApexXAxis, ApexStroke, ApexFill, ApexDataLabels, ApexNonAxisChartSeries, ApexPlotOptions, ApexLegend, ApexTooltip, ApexTheme } from 'ng-apexcharts';

import { AuthService } from '../../core/services/auth';
import { ThemeService } from '../../core/services/theme';
import { NinosService } from '../../core/services/ninos';
import { AlimentosService } from '../../core/services/alimentos';
import { CitasService } from '../../core/services/citas';
import { SistemaService } from '../../core/services/sistema';
import { Predicciones } from '../../core/services/predicciones';
import { DashboardAdminResponse } from '../../shared/interfaces/sistema.interface';
import { formatearFecha } from '../../shared/utils/date.utils';

@Component({
  selector: 'dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, NgIcon, NgApexchartsModule],
  templateUrl: './dashboard.html',
})
export class Dashboard implements OnInit {
  readonly auth             = inject(AuthService);
  private readonly themeService = inject(ThemeService);
  private ninosService      = inject(NinosService);
  private alimentosService  = inject(AlimentosService);
  private citasService      = inject(CitasService);
  private sistemaService    = inject(SistemaService);
  private prediccionService = inject(Predicciones);

  formatearFecha = formatearFecha;

  // ── Médico ────────────────────────────────────────────────
  stats = signal([
    { label: 'Pacientes',    valor: '—', icon: 'matPeopleOutline',        color: '#2563eb', bgColor: '#dbeafe', ruta: '/pacientes' },
    { label: 'Citas hoy',    valor: '—', icon: 'matCalendarMonthOutline', color: '#7c3aed', bgColor: '#ede9fe', ruta: '/citas' },
    { label: 'Alimentos',    valor: '—', icon: 'matCakeOutline',           color: '#d97706', bgColor: '#fef3c7', ruta: '/alimentos' },
    { label: 'Predicciones', valor: '—', icon: 'matBarChartOutline',       color: '#16a34a', bgColor: '#dcfce7', ruta: '/predicciones' },
  ]);

  // ── Admin dashboard ────────────────────────────────────────
  cargandoAdmin  = signal(true);
  dashboard      = signal<DashboardAdminResponse | null>(null);

  // Gráfico de actividad (línea de área)
  readonly actividadChart = computed(() => {
    const d = this.dashboard();
    const oscuro = this.themeService.oscuro();
    const colorEje = oscuro ? '#a1a1aa' : '#64748b';
    const labels    = d?.actividadSemana.map(a => a.fecha)    ?? [];
    const cantidades = d?.actividadSemana.map(a => a.cantidad) ?? [];
    return {
      series: [{ name: 'Acciones', data: cantidades }] as ApexAxisChartSeries,
      chart:  { type: 'area', height: 220, toolbar: { show: false }, sparkline: { enabled: false }, fontFamily: 'inherit', background: 'transparent', foreColor: colorEje } as ApexChart,
      theme:  { mode: oscuro ? 'dark' : 'light' } as ApexTheme,
      xaxis:  { categories: labels, labels: { style: { colors: colorEje, fontSize: '11px' } } } as ApexXAxis,
      stroke: { curve: 'smooth', width: 2 } as ApexStroke,
      fill:   { type: 'gradient', gradient: { shadeIntensity: 1, opacityFrom: 0.4, opacityTo: 0.05 } } as ApexFill,
      colors: ['#2563eb'],
      dataLabels: { enabled: false } as ApexDataLabels,
      tooltip: { theme: oscuro ? 'dark' : 'light', y: { formatter: (v: number) => `${v} acciones` } } as ApexTooltip,
    };
  });

  // Gráfico de usuarios por rol (dona)
  readonly rolesChart = computed(() => {
    const d = this.dashboard();
    const oscuro = this.themeService.oscuro();
    const colorEje = oscuro ? '#a1a1aa' : '#64748b';
    return {
      series: (d?.usuariosPorRol.map(r => r.cantidad) ?? []) as ApexNonAxisChartSeries,
      labels: d?.usuariosPorRol.map(r => r.rol) ?? [],
      chart:  { type: 'donut', height: 220, fontFamily: 'inherit', background: 'transparent', foreColor: colorEje } as ApexChart,
      theme:  { mode: oscuro ? 'dark' : 'light' } as ApexTheme,
      colors: ['#7c3aed', '#2563eb', '#16a34a'],
      plotOptions: { pie: { donut: { size: '65%' } } } as ApexPlotOptions,
      legend: { position: 'bottom', fontSize: '12px', labels: { colors: colorEje } } as ApexLegend,
      tooltip: { theme: oscuro ? 'dark' : 'light', y: { formatter: (v: number) => `${v} usuarios` } } as ApexTooltip,
      dataLabels: { enabled: true, formatter: (_: any, opts: any) => `${opts.w.globals.series[opts.seriesIndex]}` } as ApexDataLabels,
    };
  });

  // Gráfico de registro de usuarios por mes (barras)
  readonly registrosChart = computed(() => {
    const d = this.dashboard();
    const oscuro = this.themeService.oscuro();
    const colorEje = oscuro ? '#a1a1aa' : '#64748b';
    const labels    = d?.registrosPorMes.map(r => r.mes)      ?? [];
    const cantidades = d?.registrosPorMes.map(r => r.cantidad) ?? [];
    return {
      series: [{ name: 'Registros', data: cantidades }] as ApexAxisChartSeries,
      chart:  { type: 'bar', height: 220, toolbar: { show: false }, fontFamily: 'inherit', background: 'transparent', foreColor: colorEje } as ApexChart,
      theme:  { mode: oscuro ? 'dark' : 'light' } as ApexTheme,
      xaxis:  { categories: labels, labels: { style: { colors: colorEje, fontSize: '11px' } } } as ApexXAxis,
      colors: ['#16a34a'],
      plotOptions: { bar: { borderRadius: 4, columnWidth: '50%' } } as ApexPlotOptions,
      dataLabels: { enabled: false } as ApexDataLabels,
      tooltip: { theme: oscuro ? 'dark' : 'light', y: { formatter: (v: number) => `${v} usuarios nuevos` } } as ApexTooltip,
    };
  });

  // Salud del sistema
  readonly uptimeTexto = computed(() => {
    const inicio = this.dashboard()?.salud?.iniciadoEn;
    if (!inicio) return '—';

    const ms = Date.now() - new Date(inicio).getTime();
    const dias  = Math.floor(ms / (1000 * 60 * 60 * 24));
    const horas = Math.floor((ms / (1000 * 60 * 60)) % 24);

    if (dias > 0) return `${dias}d ${horas}h`;
    const minutos = Math.floor((ms / (1000 * 60)) % 60);
    return `${horas}h ${minutos}m`;
  });

  ngOnInit() {
    if (this.auth.esAdmin()) {
      this.cargarDashboardAdmin();
      return;
    }

    if (!this.auth.esMedico()) return;

    this.ninosService.obtenerTodos().subscribe(r => {
      if (r.success) this.actualizarStat(0, String(r.data.length));
    });

    this.citasService.obtenerMisCitasHoy().subscribe(r => {
      if (r.success) this.actualizarStat(1, String(r.data.length));
    });

    this.alimentosService.obtenerTodos().subscribe(r => {
      if (r.success) this.actualizarStat(2, String(r.data.length));
    });

    this.prediccionService.obtenerConteo().subscribe(r => {
      if (r.success) this.actualizarStat(3, String(r.data));
    });
  }

  private cargarDashboardAdmin(): void {
    this.cargandoAdmin.set(true);
    this.sistemaService.obtenerDashboardAdmin().subscribe({
      next: (r) => {
        if (r.success) this.dashboard.set(r.data);
        this.cargandoAdmin.set(false);
      },
      error: () => this.cargandoAdmin.set(false)
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
    if (hora >= 6 && hora < 12) return { texto: 'Buenos días',   icono: 'heroSun',   color: 'text-orange-500' };
    if (hora >= 12 && hora < 19) return { texto: 'Buenas tardes', icono: 'heroCloud', color: 'text-blue-500' };
    return { texto: 'Buenas noches', icono: 'heroMoon', color: 'text-indigo-600' };
  });
}
