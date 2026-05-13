import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";
import { padreGuard } from "../../../core/guards/padre-guard";

export const medidasRoutes: Routes = [
    {
        path: 'progreso',
        canActivate: [padreGuard],
        loadComponent: () =>
            import('./progreso-hijos/progreso-hijos')
                .then(m => m.ProgresoHijos),
    },
    {
        path: ':id',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./listar-medidas/listar-medidas')
                .then(m => m.ListarMedidas),
    }
];