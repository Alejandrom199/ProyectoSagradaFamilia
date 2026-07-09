import { Component, input } from '@angular/core';
import { BuildBadge } from '../build-badge/build-badge';

@Component({
  selector: 'header',
  standalone: true,
  imports: [BuildBadge],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  title = input<string>('Sistema de Monitoreo Pediátrico');
}