import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";

export const prescripcionesRoutes: Routes = [
    {
        path: 'nino/:ninoId',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./listar-prescripciones/listar-prescripciones')
                .then(m => m.ListarPrescripciones),
    },
    {
        path: 'cita/:citaId/crear',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./crear-prescripcion/crear-prescripcion')
                .then(m => m.CrearPrescripcion),
    },

    {
        path: '',
        canActivate: [medicoGuard],
        loadComponent: () => import('./historial-prescripciones/historial-prescripciones').then(m => m.HistorialPrescripciones),
    },
];