import { NgModule } from "@angular/core";
import { LoginComponent } from "./login/login.component";
import { VerifyOtpComponent } from "./verify-otp/verify-otp.component";
import { ForgotPasswordComponent } from "./forgot-password/forgot-password.component";
import { ResetPasswordComponent } from "./reset-password/reset-password.component";
import { AuthenticationRoutingModule } from "./authentication-routing.module";
import { SharedModule } from "src/app/shared/shared.module";
import { AuthenticationLayoutComponent } from "../layout/authentication-layout/authentication-layout.component";
import { CommonModule } from "@angular/common";

const components = [
    AuthenticationLayoutComponent,
    LoginComponent,
    VerifyOtpComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent
];

@NgModule({
    declarations: [...components],
    imports: [
        AuthenticationRoutingModule,
        SharedModule,
        CommonModule
    ],
    exports: [],
    providers: [],
})
export class AuthenticationModule { }