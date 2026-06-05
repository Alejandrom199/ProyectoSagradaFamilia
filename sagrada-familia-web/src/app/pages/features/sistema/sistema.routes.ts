import { Routes } from "@angular/router";
import { adminGuard } from "../../../core/guards/admin-guard";
import { roleGuard } from "../../../core/guards/role-guard";

export const sistemaRoutes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'logs'
    },
    {
        path: 'logs',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./listar-logs/listar-logs')
                .then(m => m.ListarLogs),
    },
    {
        path: 'auditoria',
        canActivate: [roleGuard],
        loadComponent: () =>
            import('./listar-auditoria/listar-auditoria')
                .then(m => m.ListarAuditoria),
    },
    {
        path: 'plantillas',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./plantillas/listar-plantillas/listar-plantillas')
                .then(m => m.ListarPlantillas),
    },
    {
        path: 'plantillas/:id/editar',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./plantillas/editar-plantilla/editar-plantilla')
                .then(m => m.EditarPlantilla),
    }
];
