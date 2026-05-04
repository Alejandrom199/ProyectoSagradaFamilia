import { computed, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { SessionUser } from '../../shared/interfaces/session-user.interface';
import { LoginRequest } from '../../shared/interfaces/requests/login.request';
import { ApiResponse } from '../../shared/interfaces/responses/api-response';
import { LoginResponse } from '../../shared/interfaces/responses/login.response';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class Auth {

  private readonly _currentUser = signal<SessionUser | null>(
    this.cargarDesdeStorage()
  );

  readonly currentUser = this._currentUser.asReadonly();
  readonly isAuthenticated = computed(() => !!this._currentUser());
  readonly rol = computed(() => this._currentUser()?.rol ?? null);
  readonly esMedico = computed(() => this.rol() === "Medico");
  readonly esPadre = computed(() => this.rol() === "Padre");

  constructor(private router: Router, private http: HttpClient) { }

  login(request: LoginRequest) {
    return this.http

      .post<ApiResponse<LoginResponse>>(
        `${environment.apiUrl}/auth/login`,
        request,
        { withCredentials: true }
      )
      .pipe(
        tap(response => {

          if (!response.success) {
            throw new Error(response.message || 'Error en el login');
          }
          const user: SessionUser = {
            nombre: response.data.nombre,
            rol: response.data.rol,
            accessToken: response.data.accessToken
          };

          this._currentUser.set(user);
          localStorage.setItem('user', JSON.stringify(user));
        })
      )
  }

  logout() {
    this.http
      .post(`${environment.apiUrl}/auth/logout`, {}, { withCredentials: true })
      .subscribe();

    this._currentUser.set(null);
    localStorage.removeItem('user');
    this.router.navigate(['/login']);
  }

  private cargarDesdeStorage(): SessionUser | null {
    try {
      const raw = localStorage.getItem('user');
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;
    }
  }
}
