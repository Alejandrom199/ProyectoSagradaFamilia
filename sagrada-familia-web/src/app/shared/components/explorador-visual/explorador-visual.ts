import { Component, input, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroFaceSmile, heroSparkles, heroMagnifyingGlass, heroExclamationTriangle } from '@ng-icons/heroicons/outline';

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
}

interface CategoriaEstilizada extends CategoriaFiltro {
  claseActiva: string;
  claseInactiva: string;
}

@Component({
  selector: 'explorador-visual',
  standalone: true,
  imports: [CommonModule, NgIcon],
  viewProviders: [provideIcons({ heroFaceSmile, heroSparkles, heroMagnifyingGlass, heroExclamationTriangle })],
  templateUrl: './explorador-visual.html'
})
export class ExploradorVisual {
  items = input.required<ItemExplorador[]>();
  categorias = input.required<CategoriaFiltro[]>();

  categoriaSeleccionadaId = signal<number | null>(null);
  busqueda = signal<string>('');

  categoriasEstilizadas = computed<CategoriaEstilizada[]>(() => {
    return this.categorias().map(cat => ({
      ...cat,
      claseActiva: this.obtenerClaseActiva(cat.nombre),
      claseInactiva: this.obtenerClaseInactiva(cat.nombre)
    }));
  });

  itemsFiltrados = computed<ItemExplorador[]>(() => {
    const texto = this.busqueda().toLowerCase().trim();
    const catId = this.categoriaSeleccionadaId();

    return this.items().filter(item => {
      const cumpleCategoria = catId === null || item.categoriaId === catId;
      const cumpleBusqueda = item.nombre.toLowerCase().includes(texto) ||
        item.descripcion.toLowerCase().includes(texto);
      return cumpleCategoria && cumpleBusqueda;
    });
  });

  filtrarCategoria(id: number | null) {
    this.categoriaSeleccionadaId.set(id);
  }

  actualizarBusqueda(event: Event) {
    const input = event.target as HTMLInputElement;
    this.busqueda.set(input.value);
  }

  private obtenerClaseActiva(nombre: string): string {
    const n = nombre.toLowerCase();
    if (n.includes('fruta')) return 'bg-orange-500 text-white border-orange-500 font-bold scale-105';
    if (n.includes('verdura')) return 'bg-green-500 text-white border-green-500 font-bold scale-105';
    if (n.includes('prote')) return 'bg-red-500 text-white border-red-500 font-bold scale-105';
    if (n.includes('cereal') || n.includes('tubérculo')) return 'bg-amber-600 text-white border-amber-600 font-bold scale-105';
    if (n.includes('lácteo') || n.includes('lacteo')) return 'bg-blue-500 text-white border-blue-500 font-bold scale-105';
    return 'bg-gray-800 text-white border-gray-800 font-bold scale-105';
  }

  private obtenerClaseInactiva(nombre: string): string {
    const n = nombre.toLowerCase();
    if (n.includes('fruta')) return 'bg-orange-50 text-orange-600 border-orange-200 hover:bg-orange-100';
    if (n.includes('verdura')) return 'bg-green-50 text-green-600 border-green-200 hover:bg-green-100';
    if (n.includes('prote')) return 'bg-red-50 text-red-600 border-red-200 hover:bg-red-100';
    if (n.includes('cereal') || n.includes('tubérculo')) return 'bg-amber-50 text-amber-700 border-amber-200 hover:bg-amber-100';
    if (n.includes('lácteo') || n.includes('lacteo')) return 'bg-blue-50 text-blue-600 border-blue-200 hover:bg-blue-100';
    return 'bg-gray-50 text-gray-700 border-gray-200 hover:bg-gray-100';
  }

  obtenerClaseBadge(nombre: string): string {
    const n = nombre.toLowerCase();
    if (n.includes('fruta')) return 'bg-orange-50 text-orange-600 border-orange-200';
    if (n.includes('verdura')) return 'bg-green-50 text-green-600 border-green-200';
    if (n.includes('prote')) return 'bg-red-50 text-red-600 border-red-200';
    if (n.includes('cereal') || n.includes('tubérculo')) return 'bg-amber-50 text-amber-700 border-amber-200';
    if (n.includes('lácteo') || n.includes('lacteo')) return 'bg-blue-50 text-blue-600 border-blue-200';
    return 'bg-gray-50 text-gray-700 border-gray-200';
  }
}