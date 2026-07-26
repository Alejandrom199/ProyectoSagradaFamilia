import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth';
import { catchError, switchMap, throwError } from 'rxjs';

const RUTAS_AUTH = ['/auth/login', '/auth/logout', '/auth/refresh-token'];

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);

  const request = req.clone({ withCredentials: true });
  const esRutaAuth = RUTAS_AUTH.some(ruta => req.url.includes(ruta));

  return next(request).pipe(
    catchError(error => {
      if (error.status !== 401) return throwError(() => error);

      if (esRutaAuth) {
        // No llamar a auth.logout() acá: si la propia petición fallida YA es
        // /auth/logout, volver a invocarlo dispara otro POST a /auth/logout,
        // que vuelve a fallar con 401 y reentra en este mismo bloque — un loop
        // infinito. Alcanza con limpiar el estado local.
        auth.limpiarSesion();
        return throwError(() => error);
      }

      // El access token expiró: intenta renovarlo con el refresh token y reintenta la petición original.
      return auth.refreshToken().pipe(
        switchMap(() => next(req.clone({ withCredentials: true }))),
        catchError(errorRefresh => {
          auth.logout();
          return throwError(() => errorRefresh);
        })
      );
    })
  );
};
