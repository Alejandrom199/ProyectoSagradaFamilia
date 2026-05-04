import { Component, input } from '@angular/core';

@Component({
  selector: 'header',
  standalone: true,
  imports: [],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  title = input<string>('Sistema de Monitoreo Pediátrico');
}