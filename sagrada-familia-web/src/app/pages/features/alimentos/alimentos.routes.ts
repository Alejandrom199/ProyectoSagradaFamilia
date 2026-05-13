import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";
import { inject } from "@angular/core";
import { AuthService } from "../../../core/services/auth";

export const alimentosRoutes: Routes = [
    {
        path: '',
        canMatch: [() => inject(AuthService).esMedico()],
        loadComponent: () => import('./listar-alimentos/listar-alimentos').then(m => m.ListarAlimentos)
    },
    {
        path: '',
        canMatch: [() => inject(AuthService).esPadre()],
        loadComponent: () =>
            import('./orientacion-padres/orientacion-padres').then(m => m.OrientacionPadres)
    },
    {
        path: 'crear',
        canActivate: [medicoGuard],
        loadComponent: () => import('./crear-alimento/crear-alimento').then(m => m.CrearAlimento),
    },
    {
        path: ':id/editar',
        canActivate: [medicoGuard],
        loadComponent: () => import('./editar-alimento/editar-alimento').then(m => m.EditarAlimento),
    },

]
