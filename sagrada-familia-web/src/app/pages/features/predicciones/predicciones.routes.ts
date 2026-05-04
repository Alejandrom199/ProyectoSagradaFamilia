import { Routes } from "@angular/router";
import { authGuard } from "../../../core/guards/auth-guard";

export const prediccionesRoutes: Routes = [
    {
        path: '',
        loadComponent: () =>
            import('./listar-predicciones/listar-predicciones')
                .then(m => m.ListarPredicciones),
        canActivate: [authGuard],
    }
]
