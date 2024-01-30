import { Pipe, PipeTransform } from '@angular/core';
import { GenderType } from '../constants/shared/gender-type';

@Pipe({
    name: 'genderTitle'
})

export class GenderTitlePipe implements PipeTransform {
    transform(value: number): string {
        switch(value){
            case GenderType.Male : return 'Male';
            case GenderType.Female : return 'Female';   
            default: return 'N/A';
        }
    }
}