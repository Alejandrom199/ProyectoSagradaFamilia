import { Component, inject, signal } from '@angular/core';
import { Auth } from '../../../core/services/auth';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroEye, heroEyeSlash, heroHeart } from '@ng-icons/heroicons/outline';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, NgIcon],
  viewProviders: [provideIcons({ heroEye, heroEyeSlash, heroHeart })],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private fb = inject(FormBuilder);
  private auth = inject(Auth);
  private router = inject(Router);

  cargando = signal(false);
  error = signal('');
  mostrarPassword = signal(false);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

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
      next: () => {
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Ocurrió un error al iniciar sesión.');
        this.cargando.set(false);
      }
    });
  }

}
