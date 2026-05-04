import { Routes } from "@angular/router";
import { authGuard } from "../../../core/guards/auth-guard";
import { medicoGuard } from "../../../core/guards/medico-guard";
import { padreGuard } from "../../../core/guards/padre-guard";

export const alimentosRoutes: Routes = [
    {
        path: '',
        loadComponent: () =>
            import('../alimentos/alimentos')
                .then(m => m.Alimentos),
        canActivate: [authGuard],
    },
    {
        path: 'crear',
        loadComponent: () =>
            import('./crear-alimento/crear-alimento')
                .then(m => m.CrearAlimento),
        canActivate: [authGuard, medicoGuard],
    },
    {
        path: ':id/editar',
        loadComponent: () =>
            import('./editar-alimento/editar-alimento')
                .then(m => m.EditarAlimento),
        canActivate: [authGuard, medicoGuard],
    },
    {
        path: 'orientacion',
        loadComponent: () =>
            import('./orientacion-padres/orientacion-padres')
                .then(m => m.OrientacionPadres),
        canActivate: [authGuard, padreGuard],
    }
]
