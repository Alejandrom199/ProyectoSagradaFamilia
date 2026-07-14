import { Component, inject, OnInit, signal } from '@angular/core';
import { NgIcon } from '@ng-icons/core';

import { AuthService } from '../../core/services/auth';
import { MedicosService } from '../../core/services/medicos';
import { PadresService } from '../../core/services/padres';
import { UsuariosService } from '../../core/services/usuarios';
import { LoadingBar } from '../../core/services/loading-bar';
import { Breadcrumb, BreadcrumbItem } from '../../shared/components/breadcrumb/breadcrumb';
import { FirmaPad } from '../../shared/components/firma-pad/firma-pad';
import { ConfirmModal } from '../../shared/components/confirm-modal/confirm-modal';
import { Button } from '../../shared/components/button/button';
import { UsuarioDetailResponse } from '../../shared/interfaces/usuario.interface';
import { MedicoDetailResponse } from '../../shared/interfaces/medico.interface';
import { PadreDetailResponse } from '../../shared/interfaces/padre.interface';
import { formatearFecha } from '../../shared/utils/date.utils';

@Component({
  selector: 'app-perfil',
  imports: [NgIcon, Breadcrumb, FirmaPad, ConfirmModal, Button],
  templateUrl: './perfil.html',
  styleUrl: './perfil.css',
})
export class Perfil implements OnInit {
  readonly authService = inject(AuthService);
  private readonly medicosService = inject(MedicosService);
  private readonly padresService = inject(PadresService);
  private readonly usuariosService = inject(UsuariosService);
  private readonly loadingBar = inject(LoadingBar);

  readonly formatearFecha = formatearFecha;
  readonly migajas: BreadcrumbItem[] = [{ label: 'Mi Perfil' }];

  readonly cargandoPerfil = signal(true);
  readonly perfilUsuario = signal<UsuarioDetailResponse | null>(null);
  readonly infoMedico = signal<MedicoDetailResponse | null>(null);
  readonly infoPadre = signal<PadreDetailResponse | null>(null);

  readonly cargandoFirma = signal(true);
  readonly firmaActual = signal<string | null>(null);
  readonly editandoFirma = signal(false);
  readonly guardandoFirma = signal(false);
  readonly errorFirma = signal<string | null>(null);
  readonly exitoFirma = signal(false);
  readonly mostrarConfirmEliminar = signal(false);
  readonly eliminandoFirma = signal(false);

  ngOnInit(): void {
    this.cargarPerfil();
    if (this.authService.esMedico()) this.cargarFirma();
  }

  private cargarPerfil(): void {
    this.cargandoPerfil.set(true);
    this.usuariosService.obtenerPerfilActual().subscribe({
      next: (res) => {
        if (!res.success) return;
        this.perfilUsuario.set(res.data);

        if (this.authService.esMedico() && res.data.medicoId) {
          this.medicosService.obtenerPorId(res.data.medicoId).subscribe(r => {
            if (r.success) this.infoMedico.set(r.data);
          });
        }

        if (this.authService.esPadre() && res.data.padreId) {
          this.padresService.obtenerPorId(res.data.padreId).subscribe(r => {
            if (r.success) this.infoPadre.set(r.data);
          });
        }
      },
      complete: () => this.cargandoPerfil.set(false)
    });
  }

  private cargarFirma(): void {
    this.cargandoFirma.set(true);
    this.medicosService.obtenerMiFirma().subscribe({
      next: (res) => { if (res.success) this.firmaActual.set(res.data); },
      complete: () => this.cargandoFirma.set(false)
    });
  }

  iniciarEdicion(): void {
    this.editandoFirma.set(true);
    this.errorFirma.set(null);
  }

  cancelarEdicion(): void {
    this.editandoFirma.set(false);
  }

  guardarFirma(imagenBase64: string): void {
    this.guardandoFirma.set(true);
    this.errorFirma.set(null);
    this.loadingBar.show();

    this.medicosService.actualizarMiFirma(imagenBase64).subscribe({
      next: (res) => {
        if (res.success) {
          this.firmaActual.set(imagenBase64);
          this.editandoFirma.set(false);
          this.exitoFirma.set(true);
          setTimeout(() => this.exitoFirma.set(false), 4000);
        } else {
          this.errorFirma.set(res.message);
        }
      },
      error: (err) => this.errorFirma.set(err.error?.message ?? 'No se pudo guardar la firma.'),
      complete: () => { this.guardandoFirma.set(false); this.loadingBar.complete(); }
    });
  }

  confirmarEliminarFirma(): void {
    this.eliminandoFirma.set(true);
    this.loadingBar.show();

    this.medicosService.eliminarMiFirma().subscribe({
      next: (res) => {
        if (res.success) this.firmaActual.set(null);
        else this.errorFirma.set(res.message);
      },
      error: (err) => this.errorFirma.set(err.error?.message ?? 'No se pudo eliminar la firma.'),
      complete: () => {
        this.eliminandoFirma.set(false);
        this.mostrarConfirmEliminar.set(false);
        this.loadingBar.complete();
      }
    });
  }
}
