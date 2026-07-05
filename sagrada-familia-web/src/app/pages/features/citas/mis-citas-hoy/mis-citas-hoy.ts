import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';


import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { CitasService } from '../../../../core/services/citas';
import { CitaResponse, EstadoCita } from '../../../../shared/interfaces/cita.interface';
import { finalize, forkJoin } from 'rxjs';

@Component({
  selector: 'app-mis-citas-hoy',
  standalone: true,
  imports: [CommonModule, NgIcon, RouterLink, Button, Breadcrumb],

  templateUrl: './mis-citas-hoy.html',
  styleUrl: './mis-citas-hoy.css',
})
export class MisCitasHoy implements OnInit {
  private citasService = inject(CitasService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  readonly EstadoCita = EstadoCita;

  citasHoy = signal<CitaResponse[]>([]);
  citasProximas = signal<CitaResponse[]>([]);
  cargando = signal(false);

  mesActual = signal(new Date());
  fechaSeleccionada = signal(new Date());

  todasLasCitas = computed(() => {
    const mapa = new Map<number, CitaResponse>();
    [...this.citasHoy(), ...this.citasProximas()].forEach(c => mapa.set(c.id, c));
    return Array.from(mapa.values());
  });

  indicePorFecha = computed(() => {
    const mapa = new Map<string, CitaResponse[]>();
    this.todasLasCitas().forEach(c => {
      const key = this.keyFecha(new Date(c.fechaHora));
      if (!mapa.has(key)) mapa.set(key, []);
      mapa.get(key)!.push(c);
    });
    return mapa;
  });

  citasDelDiaSeleccionado = computed(() => {
    const key = this.keyFecha(this.fechaSeleccionada());
    return (this.indicePorFecha().get(key) ?? [])
      .slice()
      .sort((a, b) => new Date(a.fechaHora).getTime() - new Date(b.fechaHora).getTime());
  });

  diasDelMes = computed(() => {
    const ref = this.mesActual();
    const año = ref.getFullYear();
    const mes = ref.getMonth();
    const primer = new Date(año, mes, 1);
    const ultimo = new Date(año, mes + 1, 0).getDate();

    let dow = primer.getDay();
    dow = dow === 0 ? 6 : dow - 1;

    const dias: (Date | null)[] = Array(dow).fill(null);
    for (let d = 1; d <= ultimo; d++) dias.push(new Date(año, mes, d));
    return dias;
  });

  // Stats
  totalHoy = computed(() => this.citasHoy().length);
  pendientesHoy = computed(() => this.citasHoy().filter(c => c.estado === EstadoCita.Pendiente).length);
  completadasHoy = computed(() => this.citasHoy().filter(c => c.estado === EstadoCita.Completada).length);
  proximasTotal = computed(() => this.citasProximas().length);

  migajas: BreadcrumbItem[] = [{ label: 'Mi Agenda' }];

  ngOnInit(): void { this.cargarTodo(); }

  cargarTodo(): void {
    this.cargando.set(true);
    this.loadingBar.show();

    forkJoin({
      hoy: this.citasService.obtenerMisCitasHoy(),
      proximas: this.citasService.obtenerProximas()
    }).pipe(finalize(() => { this.cargando.set(false); this.loadingBar.complete(); }))
      .subscribe({
        next: ({ hoy, proximas }) => {
          if (hoy.success) this.citasHoy.set(hoy.data);
          if (proximas.success) this.citasProximas.set(proximas.data);
        }
      });
  }

  cambiarEstado(citaId: number, estado: EstadoCita, event: MouseEvent): void {
    event.stopPropagation();
    this.loadingBar.show();
    this.citasService.cambiarEstado(citaId, estado).subscribe({
      next: (res) => { if (res.success) this.cargarTodo(); else this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  verCita(id: number): void { this.router.navigate(['/citas', id]); }

  // Navegación calendario
  mesAnterior(): void {
    const m = this.mesActual();
    this.mesActual.set(new Date(m.getFullYear(), m.getMonth() - 1, 1));
  }
  mesSiguiente(): void {
    const m = this.mesActual();
    this.mesActual.set(new Date(m.getFullYear(), m.getMonth() + 1, 1));
  }
  seleccionarDia(dia: Date): void { this.fechaSeleccionada.set(dia); }

  // Helpers
  keyFecha(d: Date): string {
    return `${d.getFullYear()}-${d.getMonth()}-${d.getDate()}`;
  }
  tieneCitas(dia: Date): CitaResponse[] {
    return this.indicePorFecha().get(this.keyFecha(dia)) ?? [];
  }
  esDiaSeleccionado(dia: Date): boolean {
    const s = this.fechaSeleccionada();
    return dia.toDateString() === s.toDateString();
  }
  esHoy(dia: Date): boolean {
    return dia.toDateString() === new Date().toDateString();
  }
  nombreMes(): string {
    return this.mesActual().toLocaleDateString('es-EC', { month: 'long', year: 'numeric' });
  }
  formatearHora(fechaHora: string): string {
    return new Date(fechaHora).toLocaleTimeString('es-EC', { hour: '2-digit', minute: '2-digit', hour12: false });
  }
  formatearFechaSel(): string {
    return this.fechaSeleccionada().toLocaleDateString('es-EC', {
      weekday: 'long', day: 'numeric', month: 'long'
    });
  }
  esHoySeleccionado(): boolean {
    return this.fechaSeleccionada().toDateString() === new Date().toDateString();
  }

  colorEstado(estado: string): { fondo: string; texto: string; punto: string; borde: string } {
    const mapa: Record<string, { fondo: string; texto: string; punto: string; borde: string }> = {
      Pendiente: { fondo: 'bg-amber-50', texto: 'text-amber-700', punto: 'bg-amber-400', borde: 'border-amber-200' },
      EnCurso: { fondo: 'bg-blue-50', texto: 'text-blue-700', punto: 'bg-blue-500', borde: 'border-blue-200' },
      Completada: { fondo: 'bg-emerald-50', texto: 'text-emerald-700', punto: 'bg-emerald-400', borde: 'border-emerald-200' },
      Cancelada: { fondo: 'bg-red-50', texto: 'text-red-600', punto: 'bg-red-400', borde: 'border-red-200' },
      NoAsistio: { fondo: 'bg-gray-100', texto: 'text-gray-500', punto: 'bg-gray-400', borde: 'border-gray-200' },
    };
    return mapa[estado] ?? { fondo: 'bg-gray-100', texto: 'text-gray-500', punto: 'bg-gray-400', borde: 'border-gray-200' };
  }
}