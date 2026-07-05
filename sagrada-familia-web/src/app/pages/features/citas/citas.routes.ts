import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";
import { padreGuard } from "../../../core/guards/padre-guard";

export const citasRoutes: Routes = [
    {
        path: '',
        canActivate: [medicoGuard],
        loadComponent: () => import('./mis-citas-hoy/mis-citas-hoy').then(m => m.MisCitasHoy),
    },
    {
        path: 'crear',
        canActivate: [medicoGuard],
        loadComponent: () => import('./crear-cita/crear-cita').then(m => m.CrearCita),
    },
    {
        path: 'historial',
        canActivate: [medicoGuard],
        loadComponent: () => import('./historial-citas/historial-citas').then(m => m.HistorialCitas),
    },
    {
        path: 'hijo',
        canActivate: [padreGuard],
        loadComponent: () => import('./citas-hijo/citas-hijo').then(m => m.CitasHijo),
    },
    {
        path: 'nino/:ninoId',
        canActivate: [medicoGuard],
        loadComponent: () => import('./listar-citas/listar-citas').then(m => m.ListarCitas),
    },
    {
        path: ':citaId/editar',
        canActivate: [medicoGuard],
        loadComponent: () => import('./editar-cita/editar-cita').then(m => m.EditarCita),
    },
    {
        path: ':citaId',
        canActivate: [medicoGuard],
        loadComponent: () => import('./detalle-cita/detalle-cita').then(m => m.DetalleCita),
    },
];