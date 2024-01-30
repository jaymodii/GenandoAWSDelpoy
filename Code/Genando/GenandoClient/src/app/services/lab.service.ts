import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LabPatient } from '../models/lab-patient';
import { PageListRequest } from '../models/page-list-request';
import { ApiCallConstant } from '../constants/api-call/apis';
import { LabResult } from '../models/lab-result';
import { IResponse } from '../models/shared/response';
import { LabPatientInfo } from '../models/lab-patient-info';

@Injectable({
  providedIn: 'root',
})
export class LabService {
  private resultUploadedStatus: { [clinicalProcessId: number]: boolean } = {};
  constructor(private http: HttpClient) {}
  setResultUploaded(clinicalProcessId: number): void {
    this.resultUploadedStatus[clinicalProcessId] = true;
  }

  isResultUploaded(clinicalProcessId: number): boolean {
    return this.resultUploadedStatus[clinicalProcessId] || false;
  }

  getData(
    Id: number,
    pageListRequest: PageListRequest
  ): Observable<LabPatient[]> {
    return this.http.post<LabPatient[]>(
      `${ApiCallConstant.GET_PATIENT_LIST_LAB}/` + Id,
      pageListRequest
    );
  }

  getPatientInfo(
    clinicalProcessId: number
  ): Observable<IResponse<LabPatientInfo>> {
    return this.http.get<IResponse<LabPatientInfo>>(
      `${ApiCallConstant.GET_USER_BY_ID_LAB}/${clinicalProcessId}`
    );
  }

  uploadResults(formData: FormData): Observable<any> {
    return this.http.post(ApiCallConstant.UPLOAD_RESULT_LAB, formData);
  }

  markAsRecieve(id: number) {
    return this.http.put(
      `${ApiCallConstant.RECEIVED_SAMPLE_STATUS_UPDATE}/` + id,
      {}
    );
  }
}
