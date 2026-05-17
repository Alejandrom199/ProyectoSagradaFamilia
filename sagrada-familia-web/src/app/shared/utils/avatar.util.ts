export function generarAvatarHtml(nombre: string = '', apellido: string = '', sexo?: string): string {
  const nom = nombre || '';
  const ape = apellido || '';

  let iniciales = `${nom.charAt(0)}${ape.charAt(0)}`.toUpperCase();
  if (!iniciales) {
    iniciales = '?';
  }

  const esVaron = sexo === 'M';
  const esMujer = sexo === 'F';
  const colorClass = esVaron
    ? 'bg-blue-100 text-blue-600'
    : esMujer
      ? 'bg-pink-100 text-pink-600'
      : 'bg-gray-100 text-gray-600';

  const textoSexo = esVaron ? 'Niño' : esMujer ? 'Niña' : '';
  const parrafoSexo = textoSexo ? `<p class="text-xs text-gray-500">${textoSexo}</p>` : '';

  const nombreCompleto = `${nom} ${ape}`.trim() || '<span class="text-gray-400 italic">Sin registrar</span>';

  return `
    <div class="flex items-center gap-3">
      <div class="w-10 h-10 ${colorClass} flex items-center justify-center text-sm font-bold border border-none shrink-0 rounded-lg">
        ${iniciales}
      </div>
      <div>
        <p class="font-bold text-gray-800">${nombreCompleto}</p>
        ${parrafoSexo}
      </div>
    </div>
  `;
}