import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'base64ToImage',
})
export class Base64ToImagePipe implements PipeTransform {
  private readonly base64regex =
    /^([0-9a-zA-Z+/]{4})*(([0-9a-zA-Z+/]{2}==)|([0-9a-zA-Z+/]{3}=))?$/;

  public transform(value: string) {
    return this.base64regex.test(value) ? `data:img/*;base64,${value}` : value;
  }
}
