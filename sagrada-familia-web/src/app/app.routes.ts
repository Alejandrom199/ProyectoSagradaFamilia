import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { medicoGuard } from './core/guards/medico-guard';
import { padreGuard } from './core/guards/padre-guard';
import { noPadreGuard } from './core/guards/no-padre-guard';

export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () => import('./pages/auth/login/login').then(m => m.Login)
    },
    {
        path: 'nueva-clave',
        loadComponent: () => import('./pages/auth/nueva-clave/nueva-clave').then(m => m.NuevaClave)
    },
    {
        path: 'activar-cuenta',
        loadComponent: () => import('./pages/auth/activar-cuenta/activar-cuenta').then(m => m.ActivarCuenta)
    },
    {
        path: '',
        canActivate: [authGuard],
        loadComponent: () => import('./shared/components/admin-layout/admin-layout').then(m => m.AdminLayout),
        children: [
            {
                path: 'dashboard',
                canActivate: [authGuard, noPadreGuard],
                loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard)
            },
            {
                path: 'perfil',
                canActivate: [authGuard],
                loadComponent: () => import('./pages/perfil/perfil').then(m => m.Perfil)
            },
            {
                path: 'mis-pequenos',
                canActivate: [padreGuard],
                loadComponent: () => import('./pages/features/pacientes/mis-pequenos/mis-pequenos').then(m => m.MisPequenos)
            },
            {
                path: 'roles',
                loadChildren: () => import('./pages/features/roles/roles.routes').then(m => m.rolesRoutes)
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
                path: 'medicos',
                loadChildren: () => import('./pages/features/medicos/medicos.routes').then(m => m.medicosRoutes)
            },
            {
                path: 'citas',
                loadChildren: () => import('./pages/features/citas/citas.routes').then(m => m.citasRoutes)
            },
            {
                path: 'prescripciones',
                loadChildren: () => import('./pages/features/prescripciones/prescripciones.routes').then(m => m.prescripcionesRoutes)
            },
            {
                path: 'usuarios',
                loadChildren: () => import('./pages/features/usuarios/usuarios.routes').then(m => m.usuariosRoutes)
            },
            {
                path: 'sistema',
                loadChildren: () => import('./pages/features/sistema/sistema.routes').then(m => m.sistemaRoutes)
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
                path: 'parametros',
                loadChildren: () => import('./pages/features/parametros/parametros.routes').then(m => m.parametrosRoutes)
            },
            {
                path: 'catalogos',
                loadChildren: () => import('./pages/features/catalogos/catalogos.routes').then(m => m.catalogosRoutes)
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