import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { provideIcons, NgIcon } from '@ng-icons/core';
import {
  heroEnvelope, heroEye, heroEyeSlash, heroLockClosed,
  heroPlus, heroUserCircle, heroShieldCheck
} from '@ng-icons/heroicons/outline';
import { finalize } from 'rxjs';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { UsuariosService } from '../../../../core/services/usuarios';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { UsuarioCreate } from '../../../../shared/interfaces/usuario.interface';
import { passwordValidator } from '../../../../shared/validators/password.validator';

// Roles del sistema (deben coincidir con SagradaFamilia.Domain.Enums.Rol)
const ROLES = [
  { id: 1, nombre: 'Administrador' },
  { id: 2, nombre: 'Medico' },
  { id: 3, nombre: 'Padre' }
];

@Component({
  selector: 'app-crear-usuario',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, ReactiveFormsModule],
  viewProviders: [provideIcons({
    heroEnvelope, heroLockClosed, heroEye, heroEyeSlash,
    heroPlus, heroUserCircle, heroShieldCheck
  })],
  templateUrl: './crear-usuario.html',
  styleUrl: './crear-usuario.css',
})
export class CrearUsuario implements OnInit {
  private fb = inject(FormBuilder);
  private usuariosService = inject(UsuariosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  formUsuario!: FormGroup;
  roles = ROLES;

  guardando = signal(false);
  error = signal<string | null>(null);
  verPassword = signal(false);

  migajas: BreadcrumbItem[] = [
    { label: 'Usuarios', ruta: '/usuarios' },
    { label: 'Crear usuario' },
  ];

  ngOnInit(): void {
    this.formUsuario = this.initForm();
  }

  private initForm(): FormGroup {
    return this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, passwordValidator]],
      rolId: [null, Validators.required]
    });
  }

  get f() {
    return this.formUsuario.controls;
  }

  guardar(): void {
    if (this.formUsuario.invalid) {
      this.formUsuario.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.error.set(null);
    this.loadingBar.show();

    const data = this.formUsuario.value as UsuarioCreate;

    this.usuariosService.crearAdmin(data)
      .pipe(
        finalize(() => {
          this.guardando.set(false);
          this.loadingBar.complete();
        })
      )
      .subscribe({
        next: (res) => {
          if (res.success) this.router.navigate(['/usuarios']);
          else this.error.set(res.message);
        },
        error: () => this.error.set('Ocurrió un error al crear el usuario. Intente más tarde.')
      });
  }
}
