import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";

export const pacientesRoutes: Routes = [
    {
        path: '',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./listar-pacientes/listar-pacientes').then(m => m.ListarPacientes),
    },
    {
        path: 'crear',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./crear-paciente/crear-paciente').then(m => m.CrearPaciente),
    },
    {
        path: ':id/editar',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./editar-paciente/editar-paciente').then(m => m.EditarPaciente),
    }
]