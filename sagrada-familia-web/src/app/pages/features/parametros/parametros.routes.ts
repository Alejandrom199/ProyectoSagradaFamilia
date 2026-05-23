import { Routes } from "@angular/router";
import { adminGuard } from "../../../core/guards/admin-guard";

export const parametrosRoutes: Routes = [
    {
        path: '',
        canActivate: [adminGuard],
        loadComponent: () => import('./listar-parametros/listar-parametros').then(m => m.ListarParametros)
    }
];
