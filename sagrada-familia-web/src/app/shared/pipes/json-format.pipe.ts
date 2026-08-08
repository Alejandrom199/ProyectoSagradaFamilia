import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'jsonFormat', standalone: true, pure: true })
export class JsonFormatPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) return '-';
    try {
      return JSON.stringify(JSON.parse(value), null, 2);
    } catch {
      return value;
    }
  }
}
