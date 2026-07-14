import { avatarColorClase } from './avatar-color.util';

export function generarAvatarHtml(nombre: string = '', apellido: string = '', sexo?: string): string {
  const nom = nombre || '';
  const ape = apellido || '';

  let iniciales = `${nom.charAt(0)}${ape.charAt(0)}`.toUpperCase();
  if (!iniciales) {
    iniciales = '?';
  }

  const textoSexo = sexo === 'M' ? 'Niño' : sexo === 'F' ? 'Niña' : '';
  const parrafoSexo = textoSexo ? `<p class="text-xs text-gray-500 dark:text-zinc-400">${textoSexo}</p>` : '';

  const nombreCompleto = `${nom} ${ape}`.trim() || '<span class="text-gray-400 dark:text-zinc-500 italic">Sin registrar</span>';

  return `
    <div class="flex items-center gap-3">
      <div class="w-10 h-10 ${avatarColorClase(sexo)} flex items-center justify-center text-sm font-bold shrink-0 rounded-md">
        ${iniciales}
      </div>
      <div>
        <p class="font-bold text-gray-800 dark:text-zinc-100">${nombreCompleto}</p>
        ${parrafoSexo}
      </div>
    </div>
  `;
}