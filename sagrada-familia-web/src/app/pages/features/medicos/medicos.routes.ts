import { Routes } from "@angular/router";
import { adminGuard } from "../../../core/guards/admin-guard";

export const medicosRoutes: Routes = [
    {
        path: '',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./listar-medicos/listar-medicos')
                .then(m => m.ListarMedicos),
    },
    {
        path: 'crear',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./crear-medico/crear-medico')
                .then(m => m.CrearMedico),
    },
    {
        path: ':id/editar',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./editar-medico/editar-medico')
                .then(m => m.EditarMedico),
    },
    {
        path: ':id',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./detalle-medico/detalle-medico')
                .then(m => m.DetalleMedico),
    }
];
