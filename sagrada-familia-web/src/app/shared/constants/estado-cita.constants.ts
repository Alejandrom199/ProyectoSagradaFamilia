import { EstadoCita } from '../interfaces/cita.interface';
import { BadgeColorKey } from './badge-color.constants';

export const ESTADO_CITA_COLOR: Record<EstadoCita, BadgeColorKey> = {
  [EstadoCita.Pendiente]:  'warning',
  [EstadoCita.EnCurso]:    'primary',
  [EstadoCita.Completada]: 'success',
  [EstadoCita.Cancelada]:  'danger',
  [EstadoCita.NoAsistio]:  'muted',
  [EstadoCita.Reagendada]: 'violet',
};

export const ESTADO_CITA_LABEL: Record<EstadoCita, string> = {
  [EstadoCita.Pendiente]:  'Pendiente',
  [EstadoCita.EnCurso]:    'En curso',
  [EstadoCita.Completada]: 'Completada',
  [EstadoCita.Cancelada]:  'Cancelada',
  [EstadoCita.NoAsistio]:  'No asistió',
  [EstadoCita.Reagendada]: 'Reagendada',
};

/** Variante de chip con punto y borde (mis-citas-hoy, citas-hijo). */
export const ESTADO_CITA_CHIP: Record<EstadoCita, { fondo: string; texto: string; punto: string; borde: string }> = {
  [EstadoCita.Pendiente]:  { fondo: 'bg-amber-50 dark:bg-amber-500/10',     texto: 'text-amber-700 dark:text-amber-400',     punto: 'bg-amber-400',   borde: 'border-amber-200 dark:border-amber-500/20' },
  [EstadoCita.EnCurso]:    { fondo: 'bg-blue-50 dark:bg-blue-500/10',       texto: 'text-blue-700 dark:text-blue-400',       punto: 'bg-blue-500',    borde: 'border-blue-200 dark:border-blue-500/20' },
  [EstadoCita.Completada]: { fondo: 'bg-emerald-50 dark:bg-emerald-500/10', texto: 'text-emerald-700 dark:text-emerald-400', punto: 'bg-emerald-400', borde: 'border-emerald-200 dark:border-emerald-500/20' },
  [EstadoCita.Cancelada]:  { fondo: 'bg-red-50 dark:bg-red-500/10',         texto: 'text-red-600 dark:text-red-400',         punto: 'bg-red-400',     borde: 'border-red-200 dark:border-red-500/20' },
  [EstadoCita.NoAsistio]:  { fondo: 'bg-gray-100 dark:bg-zinc-800',         texto: 'text-gray-500 dark:text-zinc-400',       punto: 'bg-gray-400',    borde: 'border-gray-200 dark:border-zinc-700' },
  [EstadoCita.Reagendada]: { fondo: 'bg-violet-50 dark:bg-violet-500/10',   texto: 'text-violet-700 dark:text-violet-400',   punto: 'bg-violet-400',  borde: 'border-violet-200 dark:border-violet-500/20' },
};
