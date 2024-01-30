import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { ValidationPattern } from 'src/app/constants/validation/validation-pattern';

export const emailValidator = (): ValidatorFn => {
  return (control: AbstractControl): ValidationErrors | null => {
    const emailRegex = ValidationPattern.email;
    return emailRegex.test(control.value) ? null : { invalidEmail: true };
  };
};
