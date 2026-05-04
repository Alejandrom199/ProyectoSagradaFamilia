import { Routes } from "@angular/router";

export const medidasRoutes: Routes = [
    {
        path: 'progreso',
        loadComponent: () =>
            import('./progreso-hijos/progreso-hijos')
                .then(m => m.ProgresoHijos),
    },
    {
        path: ':id',
        loadComponent: () =>
            import('./listar-medidas/listar-medidas')
                .then(m => m.ListarMedidas),
    }
];