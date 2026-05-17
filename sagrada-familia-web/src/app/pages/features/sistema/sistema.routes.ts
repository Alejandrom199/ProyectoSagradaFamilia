import { Routes } from "@angular/router";
import { adminGuard } from "../../../core/guards/admin-guard";

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
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./listar-auditoria/listar-auditoria')
                .then(m => m.ListarAuditoria),
    }
];
