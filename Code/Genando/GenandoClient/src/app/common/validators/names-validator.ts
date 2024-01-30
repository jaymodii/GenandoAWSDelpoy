import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { ValidationPattern } from 'src/app/constants/validation/validation-pattern';

export const namesValidator = (): ValidatorFn => {
  return (control: AbstractControl): ValidationErrors | null => {
    const nameRegex = ValidationPattern.names;
    return nameRegex.test(control.value) ? null : { invalidName: true };
  };
};
