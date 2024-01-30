import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ApiCallConstant } from 'src/app/constants/api-call/apis';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { IVerifyOtpInterface } from 'src/app/models/authentication/verify-otp.interface';
import { AuthService } from '../auth.service';
import { StorageHelperConstant } from 'src/app/constants/storage-helper/storage-helper';
import { UserRole } from 'src/app/constants/system-constant';
import { StorageHelperService } from '../storage-helper.service';

@Injectable({
  providedIn: 'root',
})
export class VerifyOtpService {
  verifyOtpApi = ApiCallConstant.VERIFY_OTP_URL;
  resendOtpApi = ApiCallConstant.RESEND_OTP_URL;

  constructor(
    private router: Router,
    private http: HttpClient,
    private authService: AuthService,
    private storageHelper: StorageHelperService
  ) { }

  verifyOtp(id: number, otpData: IVerifyOtpInterface) {
    let body = {
      email: this.storageHelper.getFromSession(StorageHelperConstant.email),
      otp: otpData.otp,
      id: id
    };
    this.http.post(this.verifyOtpApi, body, {
      withCredentials: true,
      headers: {
        credentials: 'include'
      }
    }).subscribe({
      next: (res: any) => {
        this.authService.decodeToken(res.data.accessToken);
        this.storageHelper.setAsLocal(
          StorageHelperConstant.refreshToken,
          res.data.refreshToken
        );
        if (this.authService.getUserType() == UserRole.doctorRoleId) {
          this.router.navigate([RoutingPathConstant.doctorDashboardUrl]);
        } else if (this.authService.getUserType() == UserRole.patientRoleId) {
          this.router.navigate([RoutingPathConstant.patientDashboardUrl]);
        } else {
          this.router.navigate([RoutingPathConstant.labDashboardUrl]);
        }
      },
    });
  }

  resendOtp() {
    let body = {
      email: this.storageHelper.getFromSession(StorageHelperConstant.email),
    };
    this.http.post(this.resendOtpApi, body).subscribe();
  }
}
