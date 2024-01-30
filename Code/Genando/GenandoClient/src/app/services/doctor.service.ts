import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, retry } from 'rxjs';
import { ApiCallConstant } from '../constants/api-call/apis';
import { IResponse } from '../models/shared/response';
import { PatientTestDetail } from '../models/patient-test-detail';
import { ClinicalAnswerRequest } from '../models/clinical-answer-request';
import { ClinicalPath } from '../models/patient/clinical-path';
import { AddClinicalQuestions } from '../models/doctor/add-clinical-questions';

@Injectable({
  providedIn: 'root',
})
export class DoctorService {
  constructor(private http: HttpClient) {}

  getLabResult(id: number): Observable<any> {
    return this.http.get<any>(`${ApiCallConstant.GET_LAB_RESULT}/${id}`);
  }

  downloadLabResult(id: number): Observable<ArrayBuffer> {
    return this.http.get(`${ApiCallConstant.DOWNLOAD_LAB_RESULT}/${id}`, {
      responseType: 'arraybuffer',
    });
  }

  publishResult(params: any): Observable<any> {
    return this.http.put<any>(`${ApiCallConstant.PUBLISH_RESULT}`, params);
  }

  getPatientTestDetails(id: number): Observable<IResponse<PatientTestDetail>> {
    return this.http.get<IResponse<PatientTestDetail>>(
      `${ApiCallConstant.GET_PATIENT_TEST_DETAILS}/${id}`
    );
  }

  prescribeTest(prescribeTest: any): Observable<IResponse<null>> {
    return this.http.put<IResponse<null>>(
      `${ApiCallConstant.PRESCRIBE_TEST}`,
      prescribeTest
    );
  }

  markAsCollectSample(id: number) {
    return this.http.put(`${ApiCallConstant.COLLECT_SAMPLE}/` + id, {});
  }

  markAsShipSample(id: number) {
    return this.http.put(`${ApiCallConstant.SHIP_SAMPLE}/` + id, {});
  }

  clinicalAnswers(
    clinicalAnswersRequest: ClinicalAnswerRequest
  ): Observable<IResponse<ClinicalPath[]>> {
    return this.http.post<IResponse<ClinicalPath[]>>(
      `${ApiCallConstant.SEE_CLINICAL_ANSWER}`,
      clinicalAnswersRequest
    );
  }

  addClinicalQuestions(
    patientId: number,
    addClinicalQuestions: AddClinicalQuestions[]
  ): Observable<IResponse<null>> {
    return this.http.post<IResponse<null>>(
      `${ApiCallConstant.ADD_CLINICAL_QUESTIONS}/${patientId}`,
      addClinicalQuestions
    );
  }

  getClinicalQuestion(patientId:number){
    return this.http.get(`${ApiCallConstant.GET_CLINICAL_QUESTIONS_FOR_DOCTOR}/${patientId}`);
  }

  deleteclinicalQuestion(questionId:number){
    return this.http.delete(`${ApiCallConstant.DELETE_CLINICAL_QUESTION}/${questionId}`);
  }
}
