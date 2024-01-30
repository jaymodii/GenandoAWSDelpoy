import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiCallConstant } from 'src/app/constants/api-call/apis';
import { IResponse } from 'src/app/models/shared/response';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  constructor(protected http: HttpClient) {}

  getGenders() {
    return this.http.get<any>(`${ApiCallConstant.GET_GENDER_DATA}`);
  }

  getProfileDetails(userId: number) {
    return this.http.get<any>(
      `${ApiCallConstant.GET_PROFILE_DETAILS}/${userId}`
    );
  }

  updateProfileDetails(userId: number, updatedProfileData: any) {
    const headers = new HttpHeaders();
    return this.http.put(
      `${ApiCallConstant.UPDATE_PROFILE_DETAILS}/${userId}`,
      updatedProfileData,
      { headers }
    );
  }

  sendOtp(email: any): Observable<IResponse<any>> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const options = { headers: headers };

    return this.http.post<IResponse<any>>(
      ApiCallConstant.SEND_PROFILE_OTP,
      JSON.stringify(email),
      options
    );
  }

  verifyOtp(otp: any): Observable<IResponse<any>> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const options = { headers: headers };

    return this.http.post<IResponse<any>>(
      ApiCallConstant.VERIFY_PROFILE_OTP,
      JSON.stringify(otp),
      options
    );
  }
}
