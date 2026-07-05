import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";
import { roleGuard } from "../../../core/guards/role-guard";

export const pacientesRoutes: Routes = [
    {
        path: '',
        canActivate: [roleGuard],
        loadComponent: () => import('./listar-pacientes/listar-pacientes').then(m => m.ListarPacientes),
    },
    {
        path: 'crear',
        canActivate: [medicoGuard],
        loadComponent: () => import('./crear-paciente/crear-paciente').then(m => m.CrearPaciente),
    },
    {
        path: ':id/editar',
        canActivate: [roleGuard],
        loadComponent: () => import('./editar-paciente/editar-paciente').then(m => m.EditarPaciente),
    },
    {
        path: ':id',
        canActivate: [roleGuard],
        loadComponent: () => import('./detalle-paciente/detalle-paciente').then(m => m.DetallePaciente),
    },
];