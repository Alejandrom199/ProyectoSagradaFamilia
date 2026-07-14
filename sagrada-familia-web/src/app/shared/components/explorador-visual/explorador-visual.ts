import { Component, input, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgIcon } from '@ng-icons/core';

export interface ItemExplorador {
  id: number;
  nombre: string;
  descripcion: string;
  categoriaId: number;
  categoriaNombre: string;
  badge?: string;
  edadMaxima?: number;
  imagenUrl?: string;
}

export interface CategoriaFiltro {
  id: number;
  nombre: string;
  icono?: string;
  color?: string;
}

interface CategoriaEstilizada extends CategoriaFiltro {
  icono: string;
  color: string;
  claseActiva: string;
  claseInactiva: string;
}

@Component({
  selector: 'explorador-visual',
  standalone: true,
  imports: [CommonModule, NgIcon],
  templateUrl: './explorador-visual.html'
})
export class ExploradorVisual {
  items = input.required<ItemExplorador[]>();
  categorias = input.required<CategoriaFiltro[]>();

  categoriaSeleccionadaId = signal<number | null>(null);
  busqueda = signal<string>('');

  categoriasEstilizadas = computed<CategoriaEstilizada[]>(() =>
    this.categorias().map(cat => ({
      ...cat,
      icono: cat.icono ?? 'matAutoAwesomeOutline',
      color: cat.color ?? 'bg-slate-100 text-slate-600 dark:bg-zinc-800 dark:text-zinc-300',
      claseActiva: this.claseActivaPor(cat.nombre),
      claseInactiva: this.claseInactivaPor(cat.nombre),
    }))
  );

  private categoriaMapByName = computed<Map<string, CategoriaEstilizada>>(() => {
    const m = new Map<string, CategoriaEstilizada>();
    this.categoriasEstilizadas().forEach(c => m.set(c.nombre, c));
    return m;
  });

  itemsFiltrados = computed<ItemExplorador[]>(() => {
    const texto = this.busqueda().toLowerCase().trim();
    const catId = this.categoriaSeleccionadaId();
    return this.items().filter(item => {
      const cumpleCategoria = catId === null || item.categoriaId === catId;
      const cumpleBusqueda = !texto ||
        item.nombre.toLowerCase().includes(texto) ||
        item.descripcion.toLowerCase().includes(texto);
      return cumpleCategoria && cumpleBusqueda;
    });
  });

  filtrarCategoria(id: number | null) { this.categoriaSeleccionadaId.set(id); }
  actualizarBusqueda(e: Event) { this.busqueda.set((e.target as HTMLInputElement).value); }

  iconoDeCategoria(nombre: string): string {
    return this.categoriaMapByName().get(nombre)?.icono ?? 'matAutoAwesomeOutline';
  }

  colorDeCategoria(nombre: string): string {
    return this.categoriaMapByName().get(nombre)?.color ?? 'bg-slate-100 text-slate-600 dark:bg-zinc-800 dark:text-zinc-300';
  }

  claseTarjeta(nombre: string): string {
    const n = nombre.toLowerCase();
    if (n.includes('fruta')) return 'bg-orange-50 dark:bg-orange-500/10 border-orange-100 dark:border-orange-500/20';
    if (n.includes('verdura')) return 'bg-green-50 dark:bg-green-500/10 border-green-100 dark:border-green-500/20';
    if (n.includes('prote')) return 'bg-red-50 dark:bg-red-500/10 border-red-100 dark:border-red-500/20';
    if (n.includes('cereal') || n.includes('tubérculo')) return 'bg-amber-50 dark:bg-amber-500/10 border-amber-100 dark:border-amber-500/20';
    if (n.includes('lácteo') || n.includes('lacteo')) return 'bg-blue-50 dark:bg-blue-500/10 border-blue-100 dark:border-blue-500/20';
    return 'bg-slate-50 border-slate-100 dark:bg-zinc-800 dark:border-zinc-700';
  }

  obtenerClaseBadge(nombre: string): string {
    const n = nombre.toLowerCase();
    if (n.includes('fruta')) return 'bg-orange-50 text-orange-600 border-orange-200 dark:bg-orange-500/15 dark:text-orange-400 dark:border-orange-500/25';
    if (n.includes('verdura')) return 'bg-green-50 text-green-600 border-green-200 dark:bg-green-500/15 dark:text-green-400 dark:border-green-500/25';
    if (n.includes('prote')) return 'bg-red-50 text-red-600 border-red-200 dark:bg-red-500/15 dark:text-red-400 dark:border-red-500/25';
    if (n.includes('cereal') || n.includes('tubérculo')) return 'bg-amber-50 text-amber-700 border-amber-200 dark:bg-amber-500/15 dark:text-amber-400 dark:border-amber-500/25';
    if (n.includes('lácteo') || n.includes('lacteo')) return 'bg-blue-50 text-blue-600 border-blue-200 dark:bg-blue-500/15 dark:text-blue-400 dark:border-blue-500/25';
    return 'bg-slate-50 text-slate-600 border-slate-200 dark:bg-zinc-800 dark:text-zinc-300 dark:border-zinc-700';
  }

  private claseActivaPor(nombre: string): string {
    const n = nombre.toLowerCase();
    if (n.includes('fruta')) return 'bg-orange-500 text-white border-orange-500 font-bold';
    if (n.includes('verdura')) return 'bg-green-600 text-white border-green-600 font-bold';
    if (n.includes('prote')) return 'bg-red-500 text-white border-red-500 font-bold';
    if (n.includes('cereal') || n.includes('tubérculo')) return 'bg-amber-600 text-white border-amber-600 font-bold';
    if (n.includes('lácteo') || n.includes('lacteo')) return 'bg-blue-500 text-white border-blue-500 font-bold';
    return 'bg-slate-700 text-white border-slate-700 font-bold';
  }

  private claseInactivaPor(nombre: string): string {
    const n = nombre.toLowerCase();
    if (n.includes('fruta')) return 'bg-white dark:bg-zinc-900 text-orange-600 dark:text-orange-400 border-slate-200 dark:border-zinc-700 hover:border-orange-300 hover:bg-orange-50 dark:hover:bg-orange-500/10';
    if (n.includes('verdura')) return 'bg-white dark:bg-zinc-900 text-green-600 dark:text-green-400 border-slate-200 dark:border-zinc-700 hover:border-green-300 hover:bg-green-50 dark:hover:bg-green-500/10';
    if (n.includes('prote')) return 'bg-white dark:bg-zinc-900 text-red-600 dark:text-red-400 border-slate-200 dark:border-zinc-700 hover:border-red-300 hover:bg-red-50 dark:hover:bg-red-500/10';
    if (n.includes('cereal') || n.includes('tubérculo')) return 'bg-white dark:bg-zinc-900 text-amber-700 dark:text-amber-400 border-slate-200 dark:border-zinc-700 hover:border-amber-300 hover:bg-amber-50 dark:hover:bg-amber-500/10';
    if (n.includes('lácteo') || n.includes('lacteo')) return 'bg-white dark:bg-zinc-900 text-blue-600 dark:text-blue-400 border-slate-200 dark:border-zinc-700 hover:border-blue-300 hover:bg-blue-50 dark:hover:bg-blue-500/10';
    return 'bg-white dark:bg-zinc-900 text-slate-600 dark:text-zinc-300 border-slate-200 dark:border-zinc-700 hover:bg-slate-50 dark:hover:bg-zinc-800';
  }
}
