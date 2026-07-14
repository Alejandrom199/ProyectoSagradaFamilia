export type BadgeColorKey =
  | 'success' | 'warning' | 'danger' | 'primary'
  | 'pink' | 'purple' | 'violet' | 'amber' | 'orange' | 'indigo' | 'emerald' | 'muted';

export const BADGE_CLASE_POR_COLOR: Record<BadgeColorKey, string> = {
  success: 'badge-success',
  warning: 'badge-warning',
  danger:  'badge-danger',
  primary: 'badge-primary',
  pink:    'badge-pink',
  purple:  'badge-purple',
  violet:  'badge-violet',
  amber:   'badge-amber',
  orange:  'badge-orange',
  indigo:  'badge-indigo',
  emerald: 'badge-emerald',
  muted:   'bg-[var(--color-surface-alt)] text-muted',
};

/** Solo utilidades de color (bg+text), sin el wrapper `.badge` — para contenedores
 * que ya definen su propio padding/borde/radio (ej. chips con borde propio). */
export const BADGE_UTILITY_POR_COLOR: Record<BadgeColorKey, string> = {
  success: 'bg-success-soft text-success',
  warning: 'bg-warning-soft text-warning',
  danger:  'bg-danger-soft text-danger',
  primary: 'bg-blue-100 dark:bg-blue-500/15 text-blue-700 dark:text-blue-400',
  pink:    'bg-pink-100 dark:bg-pink-500/15 text-pink-700 dark:text-pink-400',
  purple:  'bg-purple-100 dark:bg-purple-500/15 text-purple-700 dark:text-purple-400',
  violet:  'bg-violet-100 dark:bg-violet-500/15 text-violet-700 dark:text-violet-400',
  amber:   'bg-amber-100 dark:bg-amber-500/15 text-amber-700 dark:text-amber-400',
  orange:  'bg-orange-100 dark:bg-orange-500/15 text-orange-700 dark:text-orange-400',
  indigo:  'bg-indigo-100 dark:bg-indigo-500/15 text-indigo-700 dark:text-indigo-400',
  emerald: 'bg-emerald-100 dark:bg-emerald-500/15 text-emerald-700 dark:text-emerald-400',
  muted:   'bg-[var(--color-surface-alt)] text-muted',
};
