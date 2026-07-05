import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";
import { padreGuard } from "../../../core/guards/padre-guard";

export const prescripcionesRoutes: Routes = [
    {
        path: 'nino/:ninoId',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./listar-prescripciones/listar-prescripciones')
                .then(m => m.ListarPrescripciones),
    },
    {
        path: 'hijo',
        canActivate: [padreGuard],
        loadComponent: () =>
            import('./prescripciones-hijo/prescripciones-hijo')
                .then(m => m.PrescripcionesHijo),
    },
    {
        path: 'consulta/:consultaId/crear',
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