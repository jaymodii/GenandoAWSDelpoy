import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RoutingPathConstant } from "src/app/constants/routing/routing-path";
import { LoginComponent } from "./login/login.component";
import { VerifyOtpComponent } from "./verify-otp/verify-otp.component";
import { ResetPasswordComponent } from "./reset-password/reset-password.component";
import { ForgotPasswordComponent } from "./forgot-password/forgot-password.component";
import { AuthenticationLayoutComponent } from "../layout/authentication-layout/authentication-layout.component";

const routes: Routes = [
    {
        path: '',
        component: AuthenticationLayoutComponent,
        children: [
            {
                path: '',
                redirectTo: RoutingPathConstant.loginUrl,
                pathMatch: 'full',
            },
            {
                path: RoutingPathConstant.login,
                component: LoginComponent,
            },
            {
                path: RoutingPathConstant.verifyOtp,
                component: VerifyOtpComponent,
            },
            {
                path: RoutingPathConstant.resetPassword,
                component: ResetPasswordComponent
            },
            {
                path: RoutingPathConstant.forgotPassword,
                component: ForgotPasswordComponent
            },
            {
                path: "",
                redirectTo: RoutingPathConstant.loginUrl,
                pathMatch: 'full',
            },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class AuthenticationRoutingModule { }
