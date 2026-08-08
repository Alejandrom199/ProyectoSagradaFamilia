import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { CatalogoValoresService } from '../../../../core/services/catalogo-valores';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Breadcrumb, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { Tooltip } from '../../../../shared/directives/tooltip/tooltip';
import { CatalogoValorResponse, CatalogoValorCreate, CatalogoValorUpdate } from '../../../../shared/interfaces/catalogo-valor.interface';

interface CatalogoValorEditando {
    id: number;
    valor: string;
    descripcion: string;
    activo: boolean;
}

@Component({
    selector: 'listar-catalogos',
    standalone: true,
    imports: [CommonModule, FormsModule, NgIcon, Breadcrumb, ConfirmModal, Tooltip],
    templateUrl: './listar-catalogos.html'
})
export class ListarCatalogos implements OnInit {
    private readonly catalogoValoresService = inject(CatalogoValoresService);
    private readonly loadingBar = inject(LoadingBar);

    valores = signal<CatalogoValorResponse[]>([]);
    editando = signal<CatalogoValorEditando | null>(null);
    guardando = signal(false);
    errorGuardado = signal('');
    valorAEliminar = signal<CatalogoValorResponse | null>(null);
    mostrarFormNuevo = signal(false);

    nuevoValor: CatalogoValorCreate = { tipo: '', codigo: '', valor: '', descripcion: '' };
    errorNuevo = signal('');
    creando = signal(false);

    tipos = computed(() => {
        const mapa = new Map<string, CatalogoValorResponse[]>();
        for (const v of this.valores()) {
            if (!mapa.has(v.tipo)) mapa.set(v.tipo, []);
            mapa.get(v.tipo)!.push(v);
        }
        return Array.from(mapa.entries()).map(([tipo, items]) => ({ tipo, items }));
    });

    tiposColapsados = signal<Set<string>>(new Set());

    toggleColapso(tipo: string): void {
        this.tiposColapsados.update(set => {
            const nuevo = new Set(set);
            nuevo.has(tipo) ? nuevo.delete(tipo) : nuevo.add(tipo);
            return nuevo;
        });
    }

    estaColapsado(tipo: string): boolean {
        return this.tiposColapsados().has(tipo);
    }

    migajas: BreadcrumbItem[] = [{ label: 'Catálogos' }];

    ngOnInit() { this.cargarDatos(); }

    private primeraCarga = true;

    cargarDatos() {
        this.loadingBar.show();
        this.catalogoValoresService.obtenerTodos().subscribe({
            next: (r) => {
                if (r.success) {
                    this.valores.set(r.data);
                    // Solo la primera vez: entra a la pantalla con todos los tipos recogidos.
                    if (this.primeraCarga) {
                        this.tiposColapsados.set(new Set(r.data.map(v => v.tipo)));
                        this.primeraCarga = false;
                    }
                }
                this.loadingBar.complete();
            },
            error: () => this.loadingBar.complete()
        });
    }

    toggleActivo(v: CatalogoValorResponse) {
        const activo = !v.activo;
        const request: CatalogoValorUpdate = { valor: v.valor, descripcion: v.descripcion ?? undefined, activo };

        this.catalogoValoresService.actualizar(v.id, request).subscribe({
            next: (r) => {
                if (r.success) {
                    this.valores.update(lista => lista.map(x => x.id === v.id ? { ...x, activo } : x));
                }
            }
        });
    }

    iniciarEdicion(v: CatalogoValorResponse) {
        this.errorGuardado.set('');
        this.editando.set({ id: v.id, valor: v.valor, descripcion: v.descripcion ?? '', activo: v.activo });
    }

    cancelarEdicion() { this.editando.set(null); this.errorGuardado.set(''); }

    guardar() {
        const e = this.editando();
        if (!e) return;
        if (!e.valor.trim()) { this.errorGuardado.set('El valor no puede estar vacío.'); return; }

        this.guardando.set(true);
        const request: CatalogoValorUpdate = { valor: e.valor.trim(), descripcion: e.descripcion || undefined, activo: e.activo };

        this.catalogoValoresService.actualizar(e.id, request).subscribe({
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
        const v = this.valorAEliminar();
        if (!v) return;
        this.loadingBar.show();
        this.catalogoValoresService.eliminar(v.id).subscribe({
            next: (r) => { if (r.success) this.cargarDatos(); this.valorAEliminar.set(null); },
            error: () => { this.valorAEliminar.set(null); this.loadingBar.complete(); }
        });
    }

    crearValor() {
        this.errorNuevo.set('');
        const n = this.nuevoValor;
        if (!n.tipo.trim() || !n.codigo.trim() || !n.valor.trim()) {
            this.errorNuevo.set('Tipo, código y valor son obligatorios.');
            return;
        }
        this.creando.set(true);
        const request: CatalogoValorCreate = {
            tipo: n.tipo.trim().toUpperCase(),
            codigo: n.codigo.trim().toUpperCase(),
            valor: n.valor.trim(),
            descripcion: n.descripcion?.trim() || undefined
        };
        this.catalogoValoresService.crear(request).subscribe({
            next: (r) => {
                if (r.success) {
                    this.cargarDatos();
                    this.nuevoValor = { tipo: '', codigo: '', valor: '', descripcion: '' };
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
