import { CommonModule } from '@angular/common';
import { Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Tooltip } from '../../directives/tooltip/tooltip';
import { rolLabel } from '../../utils/rol-label.util';
import { avatarColorClase } from '../../utils/avatar-color.util';

@Component({
  selector: 'avatar',
  standalone: true,
  imports: [CommonModule, RouterLink, Tooltip],
  templateUrl: './avatar.html',
  styleUrl: './avatar.css',
})
export class Avatar {
  nombre = input.required<string>();
  apellido = input.required<string>();
  sexo = input<string>();
  rol = input<string>();
  sidebarExpandido = input<boolean>(false);
  tooltipTexto = input<string>('');
  tooltipPosicion = input<'top' | 'bottom' | 'left' | 'right'>('top');
  enlacePerfil = input<boolean>(false);

  iniciales = computed(() => {
    return `${this.nombre().charAt(0)}${this.apellido().charAt(0)}`.toUpperCase();
  });

  colorClase = computed(() => avatarColorClase(this.sexo()));

  textoSexo = computed(() => {
    const s = this.sexo();
    if (s === 'M') return 'Niño';
    if (s === 'F') return 'Niña';

    return '';
  });

  textoRol = computed(() => rolLabel(this.rol()));
}