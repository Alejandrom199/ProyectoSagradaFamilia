import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";

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
        path: 'nino/:ninoId',
        canActivate: [medicoGuard],
        loadComponent: () => import('./listar-citas/listar-citas').then(m => m.ListarCitas),
    },
    {
        path: ':citaId',
        canActivate: [medicoGuard],
        loadComponent: () => import('./detalle-cita/detalle-cita').then(m => m.DetalleCita),
    },
];