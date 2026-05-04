import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'status-badge',
  imports: [CommonModule],
  templateUrl: './status-badge.html',
  styleUrl: './status-badge.css',
})
export class StatusBadge {
  status = input<'success' | 'warning' | 'danger' | 'info'>('info');
  text = input<string>('Estado');
}
