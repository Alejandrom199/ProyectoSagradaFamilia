import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../../core/services/auth';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

type Vista = 'login' | 'reset' | 'confirmacion';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, NgIcon],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  // ── Login ─────────────────────────────────────────────────────────────────
  cargando = signal(false);
  error = signal('');
  mostrarPassword = signal(false);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  // ── Reset de contraseña ───────────────────────────────────────────────────
  vistaActual = signal<Vista>('login');
  emailReset = signal('');
  enviandoReset = signal(false);
  errorReset = signal('');

  constructor() {
    if (this.auth.isAuthenticated()) this.router.navigate(['/dashboard']);
  }

  onSubmit() {
    if (this.form.invalid) {
      this.error.set('Por favor, completa los campos correctamente.');
      return;
    }

    const { email, password } = this.form.value;

    this.cargando.set(true);
    this.error.set('');

    this.auth.login({ email: email!, password: password! }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.error.set(err.error?.message ?? 'Ocurrió un error al iniciar sesión.');
        this.cargando.set(false);
      }
    });
  }

  enviarReset() {
    const email = this.emailReset().trim();
    if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      this.errorReset.set('Ingresá un correo electrónico válido.');
      return;
    }

    this.enviandoReset.set(true);
    this.errorReset.set('');

    this.auth.solicitarReset(email).subscribe({
      next: () => {
        this.enviandoReset.set(false);
        this.vistaActual.set('confirmacion');
      },
      error: () => {
        this.enviandoReset.set(false);
        this.vistaActual.set('confirmacion');
      }
    });
  }

  irALogin() {
    this.vistaActual.set('login');
    this.emailReset.set('');
    this.errorReset.set('');
  }
}
