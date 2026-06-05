import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { AuthService } from '../../../core/services/auth';
import { passwordValidator } from '../../../shared/validators/password.validator';

@Component({
  selector: 'app-activar-cuenta',
  standalone: true,
  imports: [ReactiveFormsModule, NgIcon],
  templateUrl: './activar-cuenta.html',
})
export class ActivarCuenta implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  form!: FormGroup;
  token = signal('');
  mostrarClave = signal(false);
  mostrarConfirmar = signal(false);
  cargando = signal(false);
  exito = signal(false);
  error = signal('');

  ngOnInit() {
    const token = this.route.snapshot.queryParamMap.get('token');
    if (!token) {
      this.error.set('El enlace de activación no es válido o está incompleto.');
      return;
    }
    this.token.set(token);

    this.form = this.fb.group({
      nuevaClave: ['', [Validators.required, passwordValidator]],
      confirmarClave: ['', Validators.required]
    }, { validators: this.clavesIgualesValidator });
  }

  private clavesIgualesValidator(group: FormGroup) {
    const a = group.get('nuevaClave')?.value;
    const b = group.get('confirmarClave')?.value;
    return a === b ? null : { noCoinciden: true };
  }

  get f() { return this.form.controls; }
  get pErr() { return this.f['nuevaClave'].errors; }

  activar() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.cargando.set(true);
    this.error.set('');

    this.authService.activarCuenta(this.token(), this.f['nuevaClave'].value).subscribe({
      next: () => {
        this.exito.set(true);
        this.cargando.set(false);
        setTimeout(() => this.router.navigate(['/login']), 3000);
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'No se pudo activar la cuenta. El enlace puede haber expirado.');
        this.cargando.set(false);
      }
    });
  }
}
