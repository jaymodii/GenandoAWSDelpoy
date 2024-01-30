import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { passwordMatchValidator } from 'src/app/common/password-match';
import { TabTitleConstant } from 'src/app/constants/routing/tab-title';
import { ValidationMessageConstant } from 'src/app/constants/validation/validation-message';
import { ValidationPattern } from 'src/app/constants/validation/validation-pattern';
import { IResetPasswordInterface } from 'src/app/models/authentication/reset-password.interface';
import { ResetPasswordService } from 'src/app/services/authentication/reset-password.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class ResetPasswordComponent {
  passwordValidationMsg: string = ValidationMessageConstant.password;
  resetPasswordForm = new FormGroup({
    password: new FormControl('', [
      Validators.required,
      Validators.pattern(ValidationPattern.password),
    ]),
    confirmPassword: new FormControl(
      '',
      [Validators.required],
      [passwordMatchValidator()]
    ),
  });

  constructor(
    private service: ResetPasswordService,
    private titleService: Title,
    private router: Router
  ) {}

  ngOnInit() {
    this.router.events.subscribe(() => {
      this.titleService.setTitle(TabTitleConstant.resetPassword);
    });
  }

  onSubmit() {
    this.resetPasswordForm.markAllAsTouched();
    if (this.resetPasswordForm.valid) {
      this.service.resetPassword(
        <IResetPasswordInterface>this.resetPasswordForm.value
      );
    }
  }
}
