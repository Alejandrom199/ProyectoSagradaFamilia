import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroPencil, heroCheck, heroXMark, heroPlus, heroTrash, heroCog6Tooth } from '@ng-icons/heroicons/outline';
import { ParametrosService } from '../../../../core/services/parametros';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Breadcrumb, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { ParametroResponse, ParametroCreate, ParametroUpdate } from '../../../../shared/interfaces/parametro.interface';

interface ParametroEditando {
    id: number;
    valor: string;
    descripcion: string;
    activo: boolean;
}

@Component({
    selector: 'listar-parametros',
    standalone: true,
    imports: [CommonModule, FormsModule, NgIcon, Breadcrumb, ConfirmModal],
    viewProviders: [provideIcons({ heroPencil, heroCheck, heroXMark, heroPlus, heroTrash, heroCog6Tooth })],
    templateUrl: './listar-parametros.html'
})
export class ListarParametros implements OnInit {
    private readonly parametrosService = inject(ParametrosService);
    private readonly loadingBar = inject(LoadingBar);

    parametros = signal<ParametroResponse[]>([]);
    editando = signal<ParametroEditando | null>(null);
    guardando = signal(false);
    errorGuardado = signal('');
    parametroAEliminar = signal<ParametroResponse | null>(null);
    mostrarFormNuevo = signal(false);

    nuevoParametro: ParametroCreate = { grupo: '', codigo: '', valor: '', descripcion: '' };
    errorNuevo = signal('');
    creando = signal(false);

    grupos = computed(() => {
        const mapa = new Map<string, ParametroResponse[]>();
        for (const p of this.parametros()) {
            if (!mapa.has(p.grupo)) mapa.set(p.grupo, []);
            mapa.get(p.grupo)!.push(p);
        }
        return Array.from(mapa.entries()).map(([grupo, items]) => ({ grupo, items }));
    });

    migajas: BreadcrumbItem[] = [{ label: 'Parámetros del Sistema' }];

    ngOnInit() { this.cargarDatos(); }

    cargarDatos() {
        this.loadingBar.show();
        this.parametrosService.obtenerTodos().subscribe({
            next: (r) => { if (r.success) this.parametros.set(r.data); this.loadingBar.complete(); },
            error: () => this.loadingBar.complete()
        });
    }

    iniciarEdicion(p: ParametroResponse) {
        this.errorGuardado.set('');
        this.editando.set({ id: p.id, valor: p.valor, descripcion: p.descripcion ?? '', activo: p.activo });
    }

    cancelarEdicion() { this.editando.set(null); this.errorGuardado.set(''); }

    guardar() {
        const e = this.editando();
        if (!e) return;
        if (!e.valor.trim()) { this.errorGuardado.set('El valor no puede estar vacío.'); return; }

        this.guardando.set(true);
        const request: ParametroUpdate = { valor: e.valor.trim(), descripcion: e.descripcion || undefined, activo: e.activo };

        this.parametrosService.actualizar(e.id, request).subscribe({
            next: (r) => {
                if (r.success) { this.cargarDatos(); this.editando.set(null); }
                this.guardando.set(false);
            },
            error: (err) => {
                this.errorGuardado.set(err.error?.message ?? 'Error al guardar.');
                this.guardando.set(false);
            }
        });
    }

    confirmarEliminar() {
        const p = this.parametroAEliminar();
        if (!p) return;
        this.loadingBar.show();
        this.parametrosService.eliminar(p.id).subscribe({
            next: (r) => { if (r.success) this.cargarDatos(); this.parametroAEliminar.set(null); },
            error: () => { this.parametroAEliminar.set(null); this.loadingBar.complete(); }
        });
    }

    crearParametro() {
        this.errorNuevo.set('');
        const n = this.nuevoParametro;
        if (!n.grupo.trim() || !n.codigo.trim() || !n.valor.trim()) {
            this.errorNuevo.set('Grupo, código y valor son obligatorios.');
            return;
        }
        this.creando.set(true);
        const request: ParametroCreate = {
            grupo: n.grupo.trim().toUpperCase(),
            codigo: n.codigo.trim().toUpperCase(),
            valor: n.valor.trim(),
            descripcion: n.descripcion?.trim() || undefined
        };
        this.parametrosService.crear(request).subscribe({
            next: (r) => {
                if (r.success) {
                    this.cargarDatos();
                    this.nuevoParametro = { grupo: '', codigo: '', valor: '', descripcion: '' };
                    this.mostrarFormNuevo.set(false);
                }
                this.creando.set(false);
            },
            error: (err) => {
                this.errorNuevo.set(err.error?.message ?? 'Error al crear.');
                this.creando.set(false);
            }
        });
    }

    estaEditando(id: number) { return this.editando()?.id === id; }
}
