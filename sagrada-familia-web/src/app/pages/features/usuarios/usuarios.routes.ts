import { Routes } from "@angular/router";
import { adminGuard } from "../../../core/guards/admin-guard";

export const usuariosRoutes: Routes = [
    {
        path: '',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./listar-usuarios/listar-usuarios')
                .then(m => m.ListarUsuarios),
    },
    {
        path: 'crear',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./crear-usuario/crear-usuario')
                .then(m => m.CrearUsuario),
    },
    {
        path: ':id',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./detalle-usuario/detalle-usuario')
                .then(m => m.DetalleUsuario),
    }
];
