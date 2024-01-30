import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { TabTitleConstant } from 'src/app/constants/routing/tab-title';
import { ValidationMessageConstant } from 'src/app/constants/validation/validation-message';
import { ValidationPattern } from 'src/app/constants/validation/validation-pattern';
import { ILoginInterface } from 'src/app/models/authentication/login.interface';
import { LoginService } from 'src/app/services/authentication/login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit{
  emailValidationMsg: string = ValidationMessageConstant.email;
  passwordValidationMsg: string = ValidationMessageConstant.password;
  forgotPasswordUrl: string = RoutingPathConstant.forgotPasswordUrl;
  loginForm = new FormGroup({
    email: new FormControl(
      '',
      Validators.compose([
        Validators.required,
        Validators.pattern(ValidationPattern.email),
      ])
    ),
    password: new FormControl(
      '',
      Validators.compose([
        Validators.required,
        Validators.pattern(ValidationPattern.password),
      ])
    ),
    rememberMe : new FormControl(false)
  });

  constructor(
    private service: LoginService,
    private titleService: Title,
    private router: Router,
  ) { }

  ngOnInit() {
    this.router.events.subscribe(() => {
      this.titleService.setTitle(TabTitleConstant.login);
    });
  }

  rememberMeClick(checkbox: any) {
    this.loginForm.value.rememberMe = checkbox.target.checked;
  }

  onSubmit() {
    this.loginForm.markAllAsTouched();
    if (this.loginForm.valid) {
      this.service.login(<ILoginInterface>this.loginForm.value);
    }
  }
}
