import { HttpEventType, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { tap } from 'rxjs';
import { ServerTimeService } from '../services/server-time';

export const serverTimeInterceptor: HttpInterceptorFn = (req, next) => {
  const serverTime = inject(ServerTimeService);

  return next(req).pipe(
    tap(event => {
      if (event.type !== HttpEventType.Response) return;

      const fechaServidor = event.headers.get('date');
      if (fechaServidor) serverTime.registrarFechaServidor(fechaServidor);
    })
  );
};
