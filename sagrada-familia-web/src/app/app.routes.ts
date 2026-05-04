import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () =>
            import('./pages/auth/login/login').then(m => m.Login)
    },
    {
        path: '',
        loadComponent: () =>
            import('./shared/components/admin-layout/admin-layout').then(m => m.AdminLayout),
        canActivate: [authGuard],
        children: [
            {
                path: 'dashboard',
                loadComponent: () =>
                    import('./pages/dashboard/dashboard').then(m => m.Dashboard)
            },
            {
                path: 'pacientes',
                loadChildren: () =>
                    import('./pages/features/pacientes/pacientes.routes')
                        .then(m => m.pacientesRoutes)
            },
            {
                path: 'medidas',
                loadChildren: () =>
                    import('./pages/features/medidas/medidas.routes')
                        .then(m => m.medidasRoutes)
            },
            {
                path: 'padres',
                loadChildren: () =>
                    import('./pages/features/padres/padres.routes')
                        .then(m => m.padresRoutes)
            },
            {
                path: 'alimentos',
                loadChildren: () =>
                    import('./pages/features/alimentos/alimentos.routes')
                        .then(m => m.alimentosRoutes)
            },
            {
                path: 'predicciones',
                loadChildren: () =>
                    import('./pages/features/predicciones/predicciones.routes')
                        .then(m => m.prediccionesRoutes)
            },
            {
                path: '',
                redirectTo: 'dashboard',
                pathMatch: 'full'
            },
            {
                path: '**',
                loadComponent: () =>
                    import('./pages/errors/not-found/not-found').then(m => m.NotFound)
            }
        ]
    },
];