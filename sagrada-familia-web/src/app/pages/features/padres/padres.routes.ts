import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";
import { roleGuard } from "../../../core/guards/role-guard";

export const padresRoutes: Routes = [
    {
        // El Administrador gestiona padres desde /usuarios, no desde este listado.
        path: '',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./listar-padres/listar-padres')
                .then(m => m.ListarPadres),
    },
    {
        path: 'crear',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./crear-padre/crear-padre')
                .then(m => m.CrearPadre),
    },
    {
        // roleGuard: el Administrador llega aquí solo desde /usuarios (reasignar médico);
        // el Médico llega desde el listado de Padres (edición completa).
        path: ':id/editar',
        canActivate: [roleGuard],
        loadComponent: () =>
            import('./editar-padre/editar-padre')
                .then(m => m.EditarPadre),
    },
    {
        path: ':id/hijos',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./detalle-padre/detalle-padre')
                .then(m => m.DetallePadre),
    },
    {
        path: ':id/hijos/crear',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./crear-hijo/crear-hijo')
                .then(m => m.CrearHijo),
    },
    {
        path: ':id/hijos/:hijoId/editar',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./editar-hijo/editar-hijo')
                .then(m => m.EditarHijo),
    }
]
