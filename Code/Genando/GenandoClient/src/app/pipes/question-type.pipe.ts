import { Pipe, PipeTransform } from '@angular/core';
import { QuestionTypeConstant } from '../constants/system-constant';

@Pipe({
    name: 'questionTypePipe'
})

export class QuestionTypePipe implements PipeTransform {
    transform(value: number | string,): string | number {
        if (typeof value !== 'number') {
            switch (value) {
                case QuestionTypeConstant.textValue:
                    return 1;
                case QuestionTypeConstant.checkboxValue:
                    return 2;
                case QuestionTypeConstant.radioValue:
                    return 3;
                case QuestionTypeConstant.selectValue:
                    return 4;
                default:
                    return 1;
            }
        }
        else {
            switch (value) {
                case 1:
                    return QuestionTypeConstant.textValue;
                case 2:
                    return QuestionTypeConstant.checkboxValue;
                case 3:
                    return QuestionTypeConstant.radioValue;
                case 4:
                    return QuestionTypeConstant.selectValue;
                default:
                    return QuestionTypeConstant.textValue;
            }
        }
    }
}