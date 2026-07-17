import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";
import { roleGuard } from "../../../core/guards/role-guard";

export const pacientesRoutes: Routes = [
    {
        path: '',
        canActivate: [medicoGuard],
        loadComponent: () => import('./listar-pacientes/listar-pacientes').then(m => m.ListarPacientes),
    },
    {
        path: 'crear',
        canActivate: [medicoGuard],
        loadComponent: () => import('./crear-paciente/crear-paciente').then(m => m.CrearPaciente),
    },
    {
        // roleGuard: el Administrador llega aquí solo desde /usuarios (reasignar médico);
        // el Médico llega desde el listado de Pacientes (edición completa).
        path: ':id/editar',
        canActivate: [roleGuard],
        loadComponent: () => import('./editar-paciente/editar-paciente').then(m => m.EditarPaciente),
    },
    {
        path: ':id',
        canActivate: [medicoGuard],
        loadComponent: () => import('./detalle-paciente/detalle-paciente').then(m => m.DetallePaciente),
    },
];