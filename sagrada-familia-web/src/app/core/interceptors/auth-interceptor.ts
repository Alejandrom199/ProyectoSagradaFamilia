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
        auth.logout();
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
