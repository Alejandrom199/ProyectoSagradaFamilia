import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { Router } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { CommonModule } from '@angular/common';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { finalize } from 'rxjs';
import { Button } from "../../../../shared/components/button/button";
import { Tooltip } from "../../../../shared/directives/tooltip/tooltip";
import { NinosService } from '../../../../core/services/ninos';
import { AuthService } from '../../../../core/services/auth';
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { formatearEdad } from '../../../../shared/utils/date.utils';

@Component({
  selector: 'mis-pequenos',
  standalone: true,
  imports: [CommonModule, NgIcon, Button, Tooltip],

  templateUrl: './mis-pequenos.html',
})
export class MisPequenos implements OnInit {
  private ninosService = inject(NinosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  readonly auth = inject(AuthService);

  ninos = signal<NinoResponse[]>([]);
  readonly formatearEdad = formatearEdad;

  ngOnInit() {
    this.cargarDatos();
  }

  cargarDatos() {
    this.loadingBar.show();
    this.ninosService.obtenerMisNinos()
      .pipe(
        finalize(() => this.loadingBar.complete())
      ).subscribe({
        next: (r) => {
          if (r.success) this.ninos.set(r.data);
        },
        error: () => { }
      });
  }

  verSeguimiento(id: number) {
    this.router.navigate(['/medidas/progreso'], { queryParams: { ninoId: id } });
  }

  verAlimentos(id: number) {
    this.router.navigate(['/alimentos'], { queryParams: { ninoId: id } });
  }

  readonly saludoConfig = computed(() => {
    const hora = new Date().getHours();

    if (hora >= 6 && hora < 12) {
      return { texto: 'Buenos días', icono: 'heroSun', color: 'text-orange-500' };
    } else if (hora >= 12 && hora < 18) {
      return { texto: 'Buenas tardes', icono: 'heroCloud', color: 'text-blue-500' };
    } else {
      return { texto: 'Buenas noches', icono: 'heroMoon', color: 'text-indigo-600' };
    }
  });
}