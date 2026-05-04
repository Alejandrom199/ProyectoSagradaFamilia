import { Routes } from "@angular/router";
import { authGuard } from "../../../core/guards/auth-guard";

export const pacientesRoutes: Routes = [
    {
        path: '',
        loadComponent: () =>
            import('./pacientes').then(m => m.Pacientes), // Apunta al orquestador
        canActivate: [authGuard],
    },
    {
        path: 'crear',
        loadComponent: () =>
            import('./crear-paciente/crear-paciente').then(m => m.CrearPaciente),
        canActivate: [authGuard],
    },
    {
        path: ':id/editar',
        loadComponent: () =>
            import('./editar-paciente/editar-paciente').then(m => m.EditarPaciente),
        canActivate: [authGuard],
    }
]