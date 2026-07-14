export function avatarColorClase(sexo?: string): string {
  if (sexo === 'M') return 'badge-primary';
  if (sexo === 'F') return 'badge-pink';
  return 'avatar-neutral';
}
