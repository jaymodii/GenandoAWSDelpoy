import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ApiCallConstant } from "src/app/constants/api-call/apis";

@Injectable({
    providedIn: 'root',
})
export class ForgotPasswordService {

    forgotPasswordApi = ApiCallConstant.FORGOT_PASSWORD_URL;

    constructor(private http: HttpClient) { }

    forgotPassword(email: string) {
        let body = {
            email: email,
        };
        this.http.post(this.forgotPasswordApi, body).subscribe();
    }

}