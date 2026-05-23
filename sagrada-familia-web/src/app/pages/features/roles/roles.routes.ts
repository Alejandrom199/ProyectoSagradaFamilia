import { Routes } from '@angular/router';
import { adminGuard } from '../../../core/guards/admin-guard';

export const rolesRoutes: Routes = [
    {
        path: '',
        canActivate: [adminGuard],
        loadComponent: () => import('./listar-roles/listar-roles').then(m => m.ListarRoles),
    },
    {
        path: ':rolId',
        canActivate: [adminGuard],
        loadComponent: () => import('./permisos-rol/permisos-rol').then(m => m.PermisosRol),
    },
];