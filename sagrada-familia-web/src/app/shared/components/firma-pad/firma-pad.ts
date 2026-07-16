import { Component, ElementRef, output, signal, viewChild, AfterViewInit, inject, effect } from '@angular/core';
import { Button } from '../button/button';
import { ThemeService } from '../../../core/services/theme';

@Component({
  selector: 'firma-pad',
  imports: [Button],
  templateUrl: './firma-pad.html',
  styleUrl: './firma-pad.css',
})
export class FirmaPad implements AfterViewInit {
  private readonly canvasRef = viewChild.required<ElementRef<HTMLCanvasElement>>('canvas');
  private readonly themeService = inject(ThemeService);

  private static readonly COLOR_CLARO = '#1B355A';
  private static readonly COLOR_OSCURO = '#FFFFFF';

  readonly guardar = output<string>();
  readonly cancelar = output<void>();

  readonly tieneTrazo = signal(false);

  private contexto!: CanvasRenderingContext2D;
  private dibujando = false;

  constructor() {
    effect(() => {
      const color = this.themeService.oscuro() ? FirmaPad.COLOR_OSCURO : FirmaPad.COLOR_CLARO;
      if (this.contexto) this.contexto.strokeStyle = color;
    });
  }

  ngAfterViewInit(): void {
    const canvas = this.canvasRef().nativeElement;
    const contexto = canvas.getContext('2d');
    if (!contexto) return;

    this.contexto = contexto;
    this.contexto.lineWidth = 2.5;
    this.contexto.lineCap = 'round';
    this.contexto.strokeStyle = this.themeService.oscuro() ? FirmaPad.COLOR_OSCURO : FirmaPad.COLOR_CLARO;
  }

  iniciarTrazo(evento: MouseEvent | TouchEvent): void {
    evento.preventDefault();
    this.dibujando = true;
    const { x, y } = this.obtenerPosicion(evento);
    this.contexto.beginPath();
    this.contexto.moveTo(x, y);
  }

  dibujar(evento: MouseEvent | TouchEvent): void {
    if (!this.dibujando) return;
    evento.preventDefault();
    const { x, y } = this.obtenerPosicion(evento);
    this.contexto.lineTo(x, y);
    this.contexto.stroke();
    this.tieneTrazo.set(true);
  }

  terminarTrazo(): void {
    this.dibujando = false;
  }

  limpiar(): void {
    const canvas = this.canvasRef().nativeElement;
    this.contexto.clearRect(0, 0, canvas.width, canvas.height);
    this.tieneTrazo.set(false);
  }

  onGuardarClick(): void {
    if (!this.tieneTrazo()) return;
    const canvas = this.canvasRef().nativeElement;
    this.guardar.emit(canvas.toDataURL('image/png'));
  }

  private obtenerPosicion(evento: MouseEvent | TouchEvent): { x: number; y: number } {
    const canvas = this.canvasRef().nativeElement;
    const rect = canvas.getBoundingClientRect();
    const escalaX = canvas.width / rect.width;
    const escalaY = canvas.height / rect.height;

    const punto = 'touches' in evento ? evento.touches[0] : evento;

    return {
      x: (punto.clientX - rect.left) * escalaX,
      y: (punto.clientY - rect.top) * escalaY,
    };
  }
}
