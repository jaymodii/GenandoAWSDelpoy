import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { IResponse } from '../models/shared/response';
import { PatientTimeline } from '../models/patient/patient-timeline';
import { ClinicalPath } from '../models/patient/clinical-path';
import {
  IPatientMoreInfoRequest,
  IPatientMoreInfoResponse,
} from '../models/patient/patient-more-info.interface';
import { ApiCallConstant } from '../constants/api-call/apis';
import { QuestionPublishStatus } from '../constants/shared/question-publish-status';
import { DoctorDetails } from '../models/patient/doctor-details.interface';
import { ClinicalPathUpdate } from '../models/patient/clinical-path-update';


@Injectable({
  providedIn: 'root',
})
export class PatientService {
  private readonly moreQuestionsUrl = ApiCallConstant.LOAD_MORE_QUESTIONS;

  private readonly doctorDetailsUrl = ApiCallConstant.GET_DOCTOR_DETAILS;

  constructor(private http: HttpClient) { }

  getClinicalStatus(id: number): Observable<IResponse<PatientTimeline>> {
    return this.http.get<IResponse<PatientTimeline>>(
      ApiCallConstant.GET_STATUS_BY_ID_PATIENT + '/' + id
    );
  }

  getClinicalPath(): Observable<IResponse<ClinicalPath[]>> {
    return this.http.get<IResponse<ClinicalPath[]>>(
      ApiCallConstant.GET_CLINICAL_PATH
    );
  }

  clinicalAnswers(
    clinicalPath: ClinicalPath
  ): Observable<IResponse<ClinicalPath[]>> {
    return this.http.put<IResponse<ClinicalPath[]>>(
      ApiCallConstant.UPDATE_CLINICAL_ANSWER,
      clinicalPath
    );
  }

  //Request more info services
  public loadRequestedQuestions(): Observable<IPatientMoreInfoResponse[]> {
    return this.http
      .get<IResponse<IPatientMoreInfoResponse[]>>(this.moreQuestionsUrl)
      .pipe(map((response) => response.data));
  }

  public savePatientInformation(
    patientMoreInfo: IPatientMoreInfoResponse[],
    status: QuestionPublishStatus
  ): Observable<IResponse<any>> {
    const request: IPatientMoreInfoRequest = { patientMoreInfo, status };
    return this.http.post<IResponse<any>>(this.moreQuestionsUrl, request);
  }

  public getDoctorDetails(): Observable<IResponse<DoctorDetails>> {
    return this.http.get<IResponse<DoctorDetails>>(this.doctorDetailsUrl);
  }

  getClinicalPathUpdate(): Observable<IResponse<ClinicalPathUpdate[]>> {
    return this.http.get<IResponse<ClinicalPathUpdate[]>>(
      ApiCallConstant.GET_CLINICAL_ENHANCEMENT
    );
  }

  clinicalEnhancementAnswers(
    clinicalPath: ClinicalPath[]
  ): Observable<IResponse<null>> {
    return this.http.post<IResponse<null>>(
      ApiCallConstant.POST_CLINICAL_ENHANCEMENT,
      clinicalPath
    );
  }
}
