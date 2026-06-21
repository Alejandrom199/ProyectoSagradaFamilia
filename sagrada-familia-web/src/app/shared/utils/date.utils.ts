export function calcularEdadMeses(fechaNacimiento: string): number {
    const hoy = new Date();
    const nac = parsearFechaLocal(fechaNacimiento);
    let meses = (hoy.getFullYear() - nac.getFullYear()) * 12;
    meses += hoy.getMonth() - nac.getMonth();
    if (hoy.getDate() < nac.getDate()) meses--;
    return Math.max(0, meses);
}

/**
 * Convierte un string de fecha/datetime a Date tratándolo siempre como hora LOCAL,
 * sin importar si el navegador o el runtime lo interpreten como UTC.
 *
 * Formato aceptado: "2026-06-29", "2026-06-29T13:00:00", "2026-06-29T18:00:00Z"
 * Para strings con Z (UTC del servidor antiguo) convierte normalmente vía Date.
 * Para strings sin Z los parsea como hora local para evitar desfases.
 */
function parsearFechaLocal(fecha: string): Date {
    if (!fecha) return new Date();
    // Si tiene Z o offset explícito (+/-HH:MM) → ya tiene zona → usar Date directo
    if (fecha.endsWith('Z') || /[+-]\d{2}:\d{2}$/.test(fecha)) {
        return new Date(fecha);
    }
    // Sin offset: parsear manualmente para evitar que el runtime asuma UTC
    const partes = fecha.split('T');
    const [anio, mes, dia] = partes[0].split('-').map(Number);
    if (partes.length === 1) return new Date(anio, mes - 1, dia);
    const [hora, minuto, segundo] = partes[1].split(':').map(Number);
    return new Date(anio, mes - 1, dia, hora, minuto, segundo ?? 0);
}

export function formatearFecha(fecha: string): string {
    return parsearFechaLocal(fecha).toLocaleDateString('es-EC', {
        day: '2-digit', month: 'short', year: 'numeric'
    });
}

export function formatearHora(fecha: string): string {
    return parsearFechaLocal(fecha).toLocaleTimeString('es-EC', {
        hour: '2-digit', minute: '2-digit', hour12: false
    });
}

export function formatearEdad(meses: number): string {
    if (meses < 12) return `${meses} meses`;
    const años = Math.floor(meses / 12);
    const m = meses % 12;
    return m > 0 ? `${años} año${años > 1 ? 's' : ''} y ${m} mes${m > 1 ? 'es' : ''}` : `${años} año${años > 1 ? 's' : ''}`;
}