import { BADGE_CLASE_POR_COLOR, BadgeColorKey } from '../constants/badge-color.constants';

export function badgeHtml(texto: string, color: BadgeColorKey): string {
  return `<span class="badge ${BADGE_CLASE_POR_COLOR[color]}">${texto}</span>`;
}
