import { BadgeColorKey } from './badge-color.constants';

export type EstadoNutricional = 'Normal' | 'BajoPeso' | 'BajoPesoSevero' | 'Sobrepeso' | 'Obesidad';

export const ESTADO_NUTRICIONAL_COLOR: Record<EstadoNutricional, BadgeColorKey> = {
  Normal:         'success',
  BajoPeso:       'amber',
  BajoPesoSevero: 'danger',
  Sobrepeso:      'orange',
  Obesidad:       'danger',
};

export const ESTADO_NUTRICIONAL_LABEL: Record<EstadoNutricional, string> = {
  Normal:         'Normal',
  BajoPeso:       'Bajo peso',
  BajoPesoSevero: 'Bajo peso severo',
  Sobrepeso:      'Sobrepeso',
  Obesidad:       'Obesidad',
};

/** Fondo/texto sutiles (shade -50) para el banner de progreso-hijos — más claro que
 * el shade -100 de los badges de tabla, mismo criterio de color por estado. */
export const ESTADO_NUTRICIONAL_FONDO: Record<EstadoNutricional, string> = {
  Normal:         'bg-green-50 dark:bg-green-500/10',
  BajoPeso:       'bg-amber-50 dark:bg-amber-500/10',
  BajoPesoSevero: 'bg-red-50 dark:bg-red-500/10',
  Sobrepeso:      'bg-orange-50 dark:bg-orange-500/10',
  Obesidad:       'bg-red-50 dark:bg-red-500/10',
};

export const ESTADO_NUTRICIONAL_TEXTO: Record<EstadoNutricional, string> = {
  Normal:         'text-green-700 dark:text-green-400',
  BajoPeso:       'text-amber-700 dark:text-amber-400',
  BajoPesoSevero: 'text-red-700 dark:text-red-400',
  Sobrepeso:      'text-orange-700 dark:text-orange-400',
  Obesidad:       'text-red-700 dark:text-red-400',
};

export const ESTADO_NUTRICIONAL_BORDE: Record<EstadoNutricional, string> = {
  Normal:         'border-green-200 dark:border-green-500/20',
  BajoPeso:       'border-amber-200 dark:border-amber-500/20',
  BajoPesoSevero: 'border-red-200 dark:border-red-500/20',
  Sobrepeso:      'border-orange-200 dark:border-orange-500/20',
  Obesidad:       'border-red-200 dark:border-red-500/20',
};
