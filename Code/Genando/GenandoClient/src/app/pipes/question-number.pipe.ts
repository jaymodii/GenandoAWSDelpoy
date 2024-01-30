import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'questionNumberPipe'
})

export class QuestionNumberPipe implements PipeTransform {
    transform(value: number,): string {
       

       
       
        return `Q-${value}`;
    }
}