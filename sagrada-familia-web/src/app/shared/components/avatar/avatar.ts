import { CommonModule } from '@angular/common';
import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'avatar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './avatar.html',
  styleUrl: './avatar.css',
})
export class Avatar {
  nombre = input.required<string>();
  apellido = input.required<string>();
  sexo = input<string>();
  rol = input<string>();
  sidebarExpandido = input<boolean>(false);

  iniciales = computed(() => {
    return `${this.nombre().charAt(0)}${this.apellido().charAt(0)}`.toUpperCase();
  });

  colorClase = computed(() => {
    const s = this.sexo();
    if (s === 'M') return 'bg-blue-100 text-blue-600';
    if (s === 'F') return 'bg-pink-100 text-pink-600';

    return 'bg-white text-gray-600';
  });

  textoSexo = computed(() => {
    const s = this.sexo();
    if (s === 'M') return 'Niño';
    if (s === 'F') return 'Niña';

    return '';
  });

  textoRol = computed(() => {
    const r = this.rol();
    if (r === 'Medico') return 'Médico';
    if (r === 'Padre') return 'Padre';
    else return 'Administrador';

    return null;
  });
}