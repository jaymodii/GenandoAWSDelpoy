import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map, retry } from 'rxjs';
import { ApiCallConstant } from 'src/app/constants/api-call/apis';
import { IPatientMoreQuestionRequest } from 'src/app/models/doctor/patient-more-question.interface';
import { IPatientTestInfoResponse } from 'src/app/models/doctor/user-listing-response.interface';
import { IPatientQuestion } from 'src/app/models/patient/patient-question.interface';
import { IResponse } from 'src/app/models/shared/response';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private readonly http = inject(HttpClient);

  private readonly PATIENT_MORE_INFO_REQUEST =
    ApiCallConstant.PATIENT_INFO_REQUEST;
  private readonly PATIENT_CLINICAL_PROFILE =
    ApiCallConstant.PATIENT_CLINICAL_PROFILE;

  public addPatientInformationQuestion(
    patientId: number,
    request: IPatientMoreQuestionRequest
  ): Observable<IResponse<IPatientMoreQuestionRequest>> {
    return this.http.post<IResponse<IPatientMoreQuestionRequest>>(
      `${this.PATIENT_MORE_INFO_REQUEST}/${patientId}`,
      request
    );
  }

  public getPatientInformationQuestions(
    patientId: number
  ): Observable<IResponse<IPatientQuestion[]>> {
    return this.http.get<IResponse<IPatientQuestion[]>>(
      `${this.PATIENT_MORE_INFO_REQUEST}/${patientId}`
    );
  }

  public getPatientTestProfile(
    patientId: number
  ): Observable<IResponse<IPatientTestInfoResponse>> {
    return this.http.get<IResponse<IPatientTestInfoResponse>>(
      `${this.PATIENT_CLINICAL_PROFILE}/${patientId}`
    );
  }
}
