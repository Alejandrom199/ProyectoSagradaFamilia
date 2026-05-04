import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(Auth);
  
  let request = req.clone({ withCredentials: true });
  const token = auth.currentUser()?.accessToken;
  
  if (token) {
    request = request.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  return next(request).pipe(
    catchError(error => {
      if (error.status === 401 && !req.url.includes('/auth/login') && !req.url.includes('/auth/logout')) {
        auth.logout();
      }
      return throwError(() => error);
    })
  );
};
