export function generarAvatarHtml(nombre: string, apellido: string, sexo?: string): string {
  const iniciales = `${nombre.charAt(0)}${apellido.charAt(0)}`.toUpperCase();
  const esVaron = sexo === 'M';
  const esMujer = sexo === 'F';
  const colorClass = esVaron ? 'bg-blue-100 text-blue-600' : esMujer ? 'bg-pink-100 text-pink-600' : 'bg-gray-100 text-gray-600';

  const textoSexo = esVaron ? 'Varón' : esMujer ? 'Niña' : '';

  const parrafoSexo = textoSexo ? `<p class="text-xs text-gray-500">${textoSexo}</p>` : '';

  return `
    <div class="flex items-center gap-3">
      <div class="w-10 h-10 rounded-full ${colorClass} flex items-center justify-center text-sm font-bold shadow-sm border border-white">
        ${iniciales}
      </div>
      <div>
        <p class="font-bold text-gray-800">${nombre} ${apellido}</p>
        ${parrafoSexo}
      </div>
    </div>
  `;
}