import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { provideIcons, NgIcon } from '@ng-icons/core';
import { heroChevronLeft, heroEnvelope, heroEye, heroEyeSlash, heroLockClosed, heroPhone, heroPlus, heroUserPlus } from '@ng-icons/heroicons/outline';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { finalize } from 'rxjs';
import { PadresService } from '../../../../core/services/padres';
import { AuthService } from '../../../../core/services/auth';
import { PadreCreate } from '../../../../shared/interfaces/padre.interface';

@Component({
  selector: 'crear-padre',
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, ReactiveFormsModule],
  viewProviders: [provideIcons({
    heroUserPlus, heroEnvelope, heroLockClosed,
    heroPhone, heroChevronLeft, heroEye, heroEyeSlash, heroPlus
  })],
  templateUrl: './crear-padre.html',
  styleUrl: './crear-padre.css',
})
export class CrearPadre implements OnInit {
  private fb = inject(FormBuilder);
  private padresService = inject(PadresService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  medicos = signal<any[]>([]);

  formPadre!: FormGroup;

  guardando = signal(false);
  error = signal<string | null>(null);
  verPassword = signal(false);

  migajas: BreadcrumbItem[] = [
    { label: 'Padres', ruta: '/padres' },
    { label: 'Crear Padre' },
  ];

  ngOnInit(): void {
    this.formPadre = this.initForm();
    const user = this.authService.currentUser();

    if (this.authService.esMedico() && user?.id) {
      this.f['medicoId'].patchValue(user.id);
      this.f['medicoId'].disable();
    }
  }

  private initForm(): FormGroup {
    return this.fb.group({
      medicoId: [this.authService.currentUser()?.id, Validators.required],
      nombre: ['', [Validators.required, Validators.minLength(3)]],
      apellido: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      telefono: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]]
    });
  }

  get f() {
    return this.formPadre.controls;
  }

  guardar(): void {
    if (this.formPadre.invalid) {
      this.formPadre.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.error.set(null);
    this.loadingBar.show();

    const data = this.formPadre.getRawValue() as PadreCreate;

    this.padresService.crear(data)
      .pipe(
        finalize(() => {
          this.guardando.set(false);
          this.loadingBar.complete();
        })
      )
      .subscribe({
        next: (res) => {
          if (res.success) this.router.navigate(['/padres']);
          else this.error.set(res.message);
        },
        error: () => this.error.set('Ocurrió un error crítico en el servidor. Intente más tarde.')
      });
  }
}
