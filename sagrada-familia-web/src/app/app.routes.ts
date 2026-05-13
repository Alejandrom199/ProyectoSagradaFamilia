import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { medicoGuard } from './core/guards/medico-guard';
import { padreGuard } from './core/guards/padre-guard';

export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () => import('./pages/auth/login/login').then(m => m.Login)
    },
    {
        path: '',
        canActivate: [authGuard],
        loadComponent: () => import('./shared/components/admin-layout/admin-layout').then(m => m.AdminLayout),
        children: [
            {
                path: 'dashboard',
                canActivate: [medicoGuard],
                loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard)
            },
            {
                path: 'mis-pequenos',
                canActivate: [padreGuard],
                loadComponent: () => import('./pages/features/pacientes/mis-pequenos/mis-pequenos').then(m => m.MisPequenos)
            },
            {
                path: 'pacientes',
                loadChildren: () => import('./pages/features/pacientes/pacientes.routes').then(m => m.pacientesRoutes)
            },
            {
                path: 'medidas',
                loadChildren: () => import('./pages/features/medidas/medidas.routes').then(m => m.medidasRoutes)
            },
            {
                path: 'padres',
                loadChildren: () => import('./pages/features/padres/padres.routes').then(m => m.padresRoutes)
            },
            {
                path: 'alimentos',
                loadChildren: () => import('./pages/features/alimentos/alimentos.routes').then(m => m.alimentosRoutes)
            },
            {
                path: 'predicciones',
                loadChildren: () => import('./pages/features/predicciones/predicciones.routes').then(m => m.prediccionesRoutes)
            },
            {
                path: '',
                pathMatch: 'full',
                loadComponent: () => import('./shared/components/role-redirect/role-redirect').then(m => m.RoleRedirect)
            },
            {
                path: '**',
                loadComponent: () => import('./pages/errors/not-found/not-found').then(m => m.NotFound)
            }
        ]
    },
];