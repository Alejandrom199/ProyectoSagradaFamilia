import { Routes } from "@angular/router";
import { adminGuard } from "../../../core/guards/admin-guard";
import { medicoGuard } from "../../../core/guards/medico-guard";

export const medicosRoutes: Routes = [
    {
        // El Administrador gestiona médicos desde /usuarios, no desde este listado.
        path: '',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./listar-medicos/listar-medicos')
                .then(m => m.ListarMedicos),
    },
    {
        path: 'crear',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./crear-medico/crear-medico')
                .then(m => m.CrearMedico),
    },
    {
        // Único punto de entrada del Administrador: llega aquí desde /usuarios.
        path: ':id/editar',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./editar-medico/editar-medico')
                .then(m => m.EditarMedico),
    },
    {
        path: ':id',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./detalle-medico/detalle-medico')
                .then(m => m.DetalleMedico),
    }
];
