import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { SearchableSelect } from '../../../../shared/components/searchable-select/searchable-select';
import { SEXO_OPTIONS } from '../../../../shared/constants/sexo.constants';
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { NinosService } from '../../../../core/services/ninos';
import { PadresService } from '../../../../core/services/padres';
import { PadreDetailResponse, PadreResponse } from '../../../../shared/interfaces/padre.interface';

@Component({
  selector: 'app-editar-hijo',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, SearchableSelect],

  templateUrl: './editar-hijo.html',
})
export class EditarHijo implements OnInit {
  @Input() id!: string;       // padreId
  @Input() hijoId!: string;

  private ninosService = inject(NinosService);
  private padresService = inject(PadresService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private loadingBar = inject(LoadingBar);

  padre = signal<PadreDetailResponse | null>(null);
  hijo = signal<NinoResponse | null>(null);
  guardando = signal(false);
  error = signal('');

  // 'padres' | 'pacientes'
  origen = 'padres';

  form: { nombre: string; apellido: string; fechaNacimiento: string; sexo: 'M' | 'F' | '' } = {
    nombre: '', apellido: '', fechaNacimiento: '', sexo: ''
  };

  migajas: BreadcrumbItem[] = [];

  readonly sexoOptions = SEXO_OPTIONS;

  ngOnInit(): void {
    this.origen = this.route.snapshot.queryParamMap.get('origen') ?? 'padres';
    this.loadingBar.show();

    // Cargar hijo
    this.ninosService.obtenerPorId(parseInt(this.hijoId)).subscribe({
      next: (r) => {
        if (r.success) {
          // NinosService devuelve DetailResponse, casteamos a NinoResponse para la signal
          const data = r.data as any;
          this.hijo.set(data);
          this.form = {
            nombre: data.nombre,
            apellido: data.apellido,
            fechaNacimiento: data.fechaNacimiento,
            sexo: data.sexo,
          };
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });

    // Cargar padre para las migas
    this.padresService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r) => {
        if (r.success) {
          this.padre.set(r.data);
          this.construirMigas(r.data);
        }
      }
    });
  }

  private construirMigas(padre: PadreDetailResponse): void {
    if (this.origen === 'pacientes') {
      this.migajas = [
        { label: 'Pacientes', ruta: '/pacientes' },
        {
          label: `${this.hijo()?.nombre ?? ''} ${this.hijo()?.apellido ?? ''}`,
          ruta: `/pacientes/${this.hijoId}`
        },
        { label: 'Editar' },
      ];
    } else {
      this.migajas = [
        { label: 'Padres', ruta: '/padres' },
        { label: `${padre.nombre} ${padre.apellido}`, ruta: `/padres/${this.id}/hijos` },
        { label: 'Editar hijo' },
      ];
    }
  }

  rutaCancelar(): string[] {
    return this.origen === 'pacientes'
      ? ['/pacientes', this.hijoId]
      : ['/padres', this.id, 'hijos'];
  }

  guardar(): void {
    if (!this.form.nombre || !this.form.apellido || !this.form.fechaNacimiento || !this.form.sexo) {
      this.error.set('Completá todos los campos obligatorios.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    this.ninosService.actualizar(parseInt(this.hijoId), {
      nombre: this.form.nombre,
      apellido: this.form.apellido,
      fechaNacimiento: this.form.fechaNacimiento,
      sexo: this.form.sexo as 'M' | 'F',
    }).subscribe({
      next: (r) => {
        if (r.success) this.router.navigate(this.rutaCancelar());
        this.guardando.set(false);
        this.loadingBar.complete();
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Error al actualizar.');
        this.guardando.set(false);
        this.loadingBar.complete();
      }
    });
  }
}