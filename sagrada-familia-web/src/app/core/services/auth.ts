import { computed, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { tap, finalize, shareReplay, Observable } from 'rxjs';
import { SessionUser, LoginRequest, LoginResponse } from '../../shared/interfaces/auth.interface';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _currentUser = signal<SessionUser | null>(this.cargarDesdeStorage());

  readonly currentUser = this._currentUser.asReadonly();
  readonly isAuthenticated = computed(() => !!this._currentUser());
  readonly rol = computed(() => this._currentUser()?.rol ?? null);
  readonly esMedico = computed(() => this.rol() === "Medico");
  readonly esPadre = computed(() => this.rol() === "Padre");
  readonly esAdmin = computed(() => this.rol() === 'Administrador');

  constructor(private router: Router, private http: HttpClient) { }

  login(request: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${environment.apiUrl}/auth/login`, request, { withCredentials: true }
    ).pipe(
      tap(response => {
        if (!response.success || !response.data) {
          throw new Error(response.message || 'Error en el login');
        }
        this.establecerSesion(response.data);
      })
    );
  }

  private refreshEnCurso$: Observable<ApiResponse<LoginResponse>> | null = null;

  // Comparte una única llamada en curso: si varias peticiones reciben 401 al mismo tiempo,
  // todas reutilizan el mismo refresh en vez de disparar uno cada una (el backend rota el token).
  refreshToken(): Observable<ApiResponse<LoginResponse>> {
    if (!this.refreshEnCurso$) {
      this.refreshEnCurso$ = this.http.post<ApiResponse<LoginResponse>>(
        `${environment.apiUrl}/auth/refresh-token`,
        {},
        { withCredentials: true }
      ).pipe(
        tap(response => {
          if (!response.success || !response.data) {
            this.limpiarSesion();
            throw new Error(response.message || 'Error al refrescar el token');
          }
          this.establecerSesion(response.data);
        }),
        finalize(() => this.refreshEnCurso$ = null),
        shareReplay(1)
      );
    }
    return this.refreshEnCurso$;
  }

  nuevaClave(token: string, nuevaClave: string): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(
      `${environment.apiUrl}/auth/nueva-clave`,
      { token, nuevaClave }
    );
  }

  activarCuenta(token: string, nuevaClave: string): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(
      `${environment.apiUrl}/auth/activar-cuenta`,
      { token, nuevaClave }
    );
  }

  solicitarReset(email: string): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(
      `${environment.apiUrl}/auth/solicitar-reset`,
      { email }
    );
  }

  logout(): void {
    this.http.post(`${environment.apiUrl}/auth/logout`, {}, { withCredentials: true }).subscribe({
      next: () => this.limpiarSesion(),
      error: () => this.limpiarSesion()
    });
  }

  private establecerSesion(data: LoginResponse): void {
    const user: SessionUser = {
      id: data.id,
      medicoId: data.medicoId,
      nombre: data.nombre,
      apellido: data.apellido,
      rol: data.rol
    };
    this._currentUser.set(user);
    localStorage.setItem('user', JSON.stringify(user));
  }

  private limpiarSesion(): void {
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