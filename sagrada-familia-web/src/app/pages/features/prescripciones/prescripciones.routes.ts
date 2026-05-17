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
        path: 'crear',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./crear-prescripcion/crear-prescripcion')
                .then(m => m.CrearPrescripcion),
    }
];
