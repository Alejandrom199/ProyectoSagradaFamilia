export function calcularEdadMeses(fechaNacimiento: string): number {
    const hoy = new Date();
    const nac = new Date(fechaNacimiento);
    let meses = (hoy.getFullYear() - nac.getFullYear()) * 12;
    meses += hoy.getMonth() - nac.getMonth();
    if (hoy.getDate() < nac.getDate()) meses--;
    return Math.max(0, meses);
}

export function formatearFecha(fecha: string): string {
    return new Date(fecha).toLocaleDateString('es-EC', {
        day: '2-digit', month: 'short', year: 'numeric'
    });
}

export function formatearHora(fecha: string): string {
    return new Date(fecha).toLocaleTimeString('es-EC', {
        hour: '2-digit', minute: '2-digit', hour12: false
    });
}

export function formatearEdad(meses: number): string {
    if (meses < 12) return `${meses} meses`;
    const años = Math.floor(meses / 12);
    const m = meses % 12;
    return m > 0 ? `${años} año${años > 1 ? 's' : ''} y ${m} mes${m > 1 ? 'es' : ''}` : `${años} año${años > 1 ? 's' : ''}`;
}