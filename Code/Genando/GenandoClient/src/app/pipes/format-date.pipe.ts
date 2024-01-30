import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatDate',
})
export class FormatDatePipe implements PipeTransform {
  transform(value: any): string {
    const datePipe = new DatePipe('en-US');
    const transformedDate = datePipe.transform(value, 'dd MMM yyyy');
    return transformedDate || value;
  }
}
