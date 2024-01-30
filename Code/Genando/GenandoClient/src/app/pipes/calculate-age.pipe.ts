import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'calculateAge',
})
export class CalculateAgePipe implements PipeTransform {
  transform(value: Date | string): number {
    if (typeof value === 'string') {
      value = new Date(value);
    }

    if (!(value instanceof Date) || isNaN(value.getTime())) {
      return 0;
    }

    const today = new Date();
    const years = today.getFullYear() - value.getFullYear();
    const months = today.getMonth() - value.getMonth();

    if (months < 0 || (months === 0 && today.getDate() < value.getDate())) {
      return years - 1;
    }

    return years;
  }
}
