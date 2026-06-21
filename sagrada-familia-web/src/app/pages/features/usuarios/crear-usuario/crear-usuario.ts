import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { UsuariosService } from '../../../../core/services/usuarios';
import { MedicosService } from '../../../../core/services/medicos';
import { PadresService } from '../../../../core/services/padres';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { MedicoResponse } from '../../../../shared/interfaces/medico.interface';
import { SearchableSelect } from '../../../../shared/components/searchable-select/searchable-select';

const ROLES = [
  { id: 1, nombre: 'Administrador', icono: 'matAdminPanelSettingsOutline', descripcion: 'Acceso total al sistema' },
  { id: 2, nombre: 'Médico',        icono: 'matSchoolOutline',             descripcion: 'Gestión de pacientes y citas' },
  { id: 3, nombre: 'Representante', icono: 'matPersonAddOutline',          descripcion: 'Seguimiento de sus hijos' },
];

@Component({
  selector: 'app-crear-usuario',
  standalone: true,
  imports: [RouterLink, FormsModule, CommonModule, Breadcrumb, NgIcon, ReactiveFormsModule, SearchableSelect],
  templateUrl: './crear-usuario.html',
  styleUrl: './crear-usuario.css',
})
export class CrearUsuario implements OnInit {
  private fb              = inject(FormBuilder);
  private usuariosService = inject(UsuariosService);
  private medicosService  = inject(MedicosService);
  private padresService   = inject(PadresService);
  private router          = inject(Router);
  private loadingBar      = inject(LoadingBar);

  form!: FormGroup;
  roles = ROLES;
  medicos = signal<MedicoResponse[]>([]);
  readonly medicoLabelFn = (m: MedicoResponse) => `${m.nombre} ${m.apellido}`;

  guardando       = signal(false);
  error           = signal<string | null>(null);
  rolSeleccionado = signal<number | null>(null);

  migajas: BreadcrumbItem[] = [
    { label: 'Usuarios', ruta: '/usuarios' },
    { label: 'Crear usuario' },
  ];

  ngOnInit(): void {
    this.form = this.initForm();
    this.medicosService.obtenerTodos().subscribe(res => {
      if (res.success) this.medicos.set(res.data);
    });
  }

  private initForm(): FormGroup {
    return this.fb.group({
      email:        ['', [Validators.required, Validators.email]],
      nombre:       [''],
      apellido:     [''],
      especialidad: [''],
      telefono:     [''],
      medicoId:     [null],
    });
  }

  seleccionarRol(rolId: number): void {
    if (this.rolSeleccionado() === rolId) return;
    this.rolSeleccionado.set(rolId);
    this.form.reset();
    this.error.set(null);
    this.actualizarValidadores(rolId);
  }

  private actualizarValidadores(rolId: number): void {
    const nombre   = this.form.get('nombre')!;
    const apellido = this.form.get('apellido')!;
    const telefono = this.form.get('telefono')!;
    const medicoId = this.form.get('medicoId')!;

    [nombre, apellido, telefono, medicoId].forEach(c => c.clearValidators());

    if (rolId === 2) {
      nombre.setValidators([Validators.required]);
      apellido.setValidators([Validators.required]);
    } else if (rolId === 3) {
      nombre.setValidators([Validators.required]);
      apellido.setValidators([Validators.required]);
      telefono.setValidators([Validators.required, Validators.pattern(/^\d{10}$/)]);
      medicoId.setValidators([Validators.required]);
    }

    [nombre, apellido, telefono, medicoId].forEach(c => c.updateValueAndValidity());
  }

  get f()        { return this.form.controls; }
  get esAdmin()  { return this.rolSeleccionado() === 1; }
  get esMedico() { return this.rolSeleccionado() === 2; }
  get esPadre()  { return this.rolSeleccionado() === 3; }

  guardar(): void {
    if (this.form.invalid || !this.rolSeleccionado()) {
      this.form.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.error.set(null);
    this.loadingBar.show();

    const v = this.form.value;
    let obs$: any;

    if (this.esMedico) {
      obs$ = this.medicosService.crear({
        email:        v.email,
        nombre:       v.nombre,
        apellido:     v.apellido,
        especialidad: v.especialidad || undefined,
        telefono:     v.telefono     || undefined,
      });
    } else if (this.esPadre) {
      obs$ = this.padresService.crear({
        email:    v.email,
        nombre:   v.nombre,
        apellido: v.apellido,
        telefono: v.telefono,
        medicoId: v.medicoId,
      });
    } else {
      obs$ = this.usuariosService.crearAdmin({ email: v.email, rolId: this.rolSeleccionado()! });
    }

    obs$.pipe(finalize(() => { this.guardando.set(false); this.loadingBar.complete(); }))
      .subscribe({
        next:  (res: any) => {
          if (res.success) this.router.navigate(['/usuarios']);
          else this.error.set(res.message);
        },
        error: () => this.error.set('Ocurrió un error al crear el usuario. Intente más tarde.'),
      });
  }
}
