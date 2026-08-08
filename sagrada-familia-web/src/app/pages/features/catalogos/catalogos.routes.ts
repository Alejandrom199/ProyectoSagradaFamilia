import { Routes } from "@angular/router";
import { adminGuard } from "../../../core/guards/admin-guard";

export const catalogosRoutes: Routes = [
    {
        path: '',
        canActivate: [adminGuard],
        loadComponent: () => import('./listar-catalogos/listar-catalogos').then(m => m.ListarCatalogos)
    }
];
