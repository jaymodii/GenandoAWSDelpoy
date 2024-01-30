import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Faq } from '../models/faq';
import { IResponse } from '../models/shared/response';
import { ApiCallConstant } from '../constants/api-call/apis';

@Injectable({
  providedIn: 'root',
})
export class FaqService {
  constructor(private http: HttpClient) {}

  getFaq(): Observable<IResponse<Faq[]>> {
    return this.http.get<IResponse<Faq[]>>(ApiCallConstant.GET_FAQ);
  }
}
