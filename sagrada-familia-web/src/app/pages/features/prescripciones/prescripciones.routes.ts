import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";

// ✅ Así debería quedar
export const prescripcionesRoutes: Routes = [
    {
        path: 'nino/:ninoId',           // historial — solo lectura
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./listar-prescripciones/listar-prescripciones')
                .then(m => m.ListarPrescripciones),
    },
    {
        path: 'cita/:citaId/crear',     // crear CON contexto de cita
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./crear-prescripcion/crear-prescripcion')
                .then(m => m.CrearPrescripcion),
    },
    // {
    //     path: ':id',                    // detalle/PDF de una prescripción
    //     canActivate: [medicoGuard],
    //     loadComponent: () =>
    //         import('./detalle-prescripcion/detalle-prescripcion')
    //             .then(m => m.DetallePrescripcion),
    // }
];