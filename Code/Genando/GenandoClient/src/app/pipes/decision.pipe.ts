import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'decision',
})
export class DecisionPipe implements PipeTransform {
  transform(value: string | boolean): string | boolean {
    if (typeof value === 'boolean') {
      if (value === true) {
        return 'Yes';
      } else {
        return 'No';
      }
    }
    return value;
  }
}
