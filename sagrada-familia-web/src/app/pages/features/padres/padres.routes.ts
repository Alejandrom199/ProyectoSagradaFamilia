import { Routes } from "@angular/router";
import { authGuard } from "../../../core/guards/auth-guard";

export const padresRoutes: Routes = [
    {
        path: '',
        loadComponent: () =>
            import('./listar-padres/listar-padres')
                .then(m => m.ListarPadres),
        canActivate: [authGuard],
    },
    {
        path: 'crear',
        loadComponent: () =>
            import('./crear-padre/crear-padre')
                .then(m => m.CrearPadre),
        canActivate: [authGuard],
    },
    {
        path: ':id/editar',
        loadComponent: () =>
            import('./editar-padre/editar-padre')
                .then(m => m.EditarPadre),
        canActivate: [authGuard],
    },
    {
        path: ':id/hijos',
        loadComponent: () =>
            import('./detalle-padre/detalle-padre')
                .then(m => m.DetallePadre),
        canActivate: [authGuard],
    },
    {
        path: ':id/hijos/crear',
        loadComponent: () =>
            import('./crear-hijo/crear-hijo')
                .then(m => m.CrearHijo),
        canActivate: [authGuard],
    },
    {
        path: ':id/hijos/:hijoId/editar',
        loadComponent: () =>
            import('./editar-hijo/editar-hijo')
                .then(m => m.EditarHijo),
        canActivate: [authGuard],
    }
]
