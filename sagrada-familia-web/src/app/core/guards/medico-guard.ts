import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';
import { inject } from '@angular/core';

export const medicoGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.esMedico()) return true;

  return router.createUrlTree([auth.esAdmin() ? '/dashboard' : '/mis-pequenos']);
};
