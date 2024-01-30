import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IResponse } from '../models/shared/response';
import { ApiCallConstant } from '../constants/api-call/apis';

@Injectable({
  providedIn: 'root',
})
export class HeaderService {
  baseUrl: string = environment.baseUrl;

  constructor(private http: HttpClient) {}

  getAvatar(): Observable<IResponse<string>> {
    return this.http.get<IResponse<string>>(ApiCallConstant.GET_AVATAR);
  }
}
