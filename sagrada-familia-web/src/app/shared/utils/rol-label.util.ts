export function rolLabel(rol: string | null | undefined): string {
  if (rol === 'Medico') return 'Médico';
  return rol ?? '';
}
