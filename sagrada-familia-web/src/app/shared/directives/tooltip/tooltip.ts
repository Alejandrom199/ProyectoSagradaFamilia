import { Directive, ElementRef, HostListener, Input, Renderer2, OnDestroy } from '@angular/core';

@Directive({
  selector: '[tooltip]',
  standalone: true,
})
export class Tooltip implements OnDestroy {
  @Input('tooltip') texto = '';
  @Input() tooltipPosition: 'top' | 'bottom' | 'left' | 'right' = 'top';

  private tooltipEl: HTMLElement | null = null;

  constructor(private el: ElementRef, private renderer: Renderer2) { }

  @HostListener('mouseenter')
  onMouseEnter() {
    if (!this.texto) return;
    this.crear();
  }

  @HostListener('mouseleave')
  onMouseLeave() {
    this.destruir();
  }

  // NUEVO 1: Destruir el tooltip apenas el usuario haga clic en el botón
  @HostListener('click')
  onClick() {
    this.destruir();
  }

  // NUEVO 2: Destruir el tooltip si Angular destruye el componente (ej. al cambiar de ruta)
  ngOnDestroy() {
    this.destruir();
  }

  private crear() {
    // Evita crear múltiples tooltips si el usuario mueve el mouse rápido
    if (this.tooltipEl) this.destruir();

    this.tooltipEl = this.renderer.createElement('div');

    const clasesTailwind = [
      'absolute', 'z-[9999]', 'px-2.5', 'py-1.5', 'text-xs', 'font-medium',
      'text-white', 'bg-[var(--color-primary)]', 'rounded-md', 'shadow-md',
      'pointer-events-none', 'transition-opacity', 'duration-200', 'opacity-0',
      'whitespace-nowrap'
    ];

    clasesTailwind.forEach(clase => this.renderer.addClass(this.tooltipEl, clase));

    this.tooltipEl!.textContent = this.texto;

    this.renderer.appendChild(document.body, this.tooltipEl);
    this.posicionar();

    requestAnimationFrame(() => {
      if (this.tooltipEl) {
        this.renderer.removeClass(this.tooltipEl, 'opacity-0');
        this.renderer.addClass(this.tooltipEl, 'opacity-100');
      }
    });
  }

  private posicionar() {
    const hostRect = this.el.nativeElement.getBoundingClientRect();
    const tooltipRect = this.tooltipEl!.getBoundingClientRect();

    let top = 0;
    let left = 0;

    switch (this.tooltipPosition) {
      case 'top':
        top = hostRect.top - tooltipRect.height - 10;
        left = hostRect.left + (hostRect.width - tooltipRect.width) / 2;
        break;
      case 'bottom':
        top = hostRect.bottom + 10;
        left = hostRect.left + (hostRect.width - tooltipRect.width) / 2;
        break;
      case 'left':
        top = hostRect.top + (hostRect.height - tooltipRect.height) / 2;
        left = hostRect.left - tooltipRect.width - 10;
        break;
      case 'right':
        top = hostRect.top + (hostRect.height - tooltipRect.height) / 2;
        left = hostRect.right + 10;
        break;
    }

    this.renderer.setStyle(this.tooltipEl, 'top', `${top}px`);
    this.renderer.setStyle(this.tooltipEl, 'left', `${left}px`);
  }

  private destruir() {
    if (this.tooltipEl) {
      this.renderer.removeChild(document.body, this.tooltipEl);
      this.tooltipEl = null;
    }
  }
}