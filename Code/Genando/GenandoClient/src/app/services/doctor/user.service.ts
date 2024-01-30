import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiCallConstant } from 'src/app/constants/api-call/apis';
import { IPatientDetailInterface } from 'src/app/models/doctor/patient-detail.interface';
import { DoctorPatient } from 'src/app/models/doctor-patient';
import { IPatientPageRequest } from 'src/app/models/doctor/patient-page-request.interface';
import { IUserDetailsFormRequest } from 'src/app/models/doctor/register-user-request.interface';
import { IUserListingResponse } from 'src/app/models/doctor/user-listing-response.interface';
import { PageListRequest } from 'src/app/models/page-list-request';
import { IPageInfoResponse } from 'src/app/models/shared/page-info-response.interface';
import { IResponse } from 'src/app/models/shared/response';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly http: HttpClient = inject(HttpClient);

  private readonly getPatientDetailsUrl = ApiCallConstant.GET_PATIENT_DETAILS;
  private readonly searchUrl = ApiCallConstant.SEARCH_PATIENT;
  private readonly getUserUrl = ApiCallConstant.GET_USER;
  private readonly registerUrl = ApiCallConstant.REGISTER_USER;
  private readonly editUrl = ApiCallConstant.UPDATE_USER;
  private readonly deleteUrl = ApiCallConstant.DELETE_USER;
  private readonly getPatients = ApiCallConstant.GET_PATIENT;
  private readonly getLabUserUrl = ApiCallConstant.GET_LAB_USER;

  public registerUser(
    request: IUserDetailsFormRequest
  ): Observable<IResponse<IUserDetailsFormRequest>> {
    return this.http.post<IResponse<IUserDetailsFormRequest>>(
      this.registerUrl,
      request
    );
  }

  public updateUser(
    id: number,
    request: IUserDetailsFormRequest
  ): Observable<IResponse<null>> {
    return this.http.put<IResponse<null>>(this.editUrl + '/' + id, request);
  }

  public deleteUser(id: number): Observable<IResponse<null>> {
    return this.http.delete<IResponse<null>>(this.deleteUrl + '/' + id);
  }

  public loadPatients(
    pageRequest?: IPatientPageRequest
  ): Observable<IResponse<IPageInfoResponse<IUserListingResponse>>> {
    return this.http.post<IResponse<IPageInfoResponse<IUserListingResponse>>>(
      this.searchUrl,
      pageRequest ?? {}
    );
  }

  getPatientForDashboard(
    id: number,
    pageListRequest?: PageListRequest
  ): Observable<DoctorPatient[]> {
    return this.http.post<DoctorPatient[]>(
      `${this.getPatients}/${id}`,
      pageListRequest
    );
  }

  public loadUserFormDetails(
    id: number
  ): Observable<IResponse<IUserDetailsFormRequest>> {
    return this.http.get<IResponse<IUserDetailsFormRequest>>(
      this.getUserUrl + '/' + id
    );
  }

  public getPatientDetails(id: number)
    : Observable<IPatientDetailInterface> {
    return this.http
      .get<IResponse<IPatientDetailInterface>>(this.getPatientDetailsUrl + '/' + id)
      .pipe(map(response => response.data));
  }
  public loadLabDetails(): Observable<IResponse<IUserListingResponse>> {
    return this.http.get<IResponse<IUserListingResponse>>(this.getLabUserUrl);
  }
}
