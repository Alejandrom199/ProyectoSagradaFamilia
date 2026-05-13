import { Routes } from "@angular/router";
import { medicoGuard } from "../../../core/guards/medico-guard";

export const prediccionesRoutes: Routes = [
    {
        path: '',
        canActivate: [medicoGuard],
        loadComponent: () =>
            import('./listar-predicciones/listar-predicciones')
                .then(m => m.ListarPredicciones),
    }
]
