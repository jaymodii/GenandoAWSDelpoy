import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { TestExplanation } from '../models/test-explanation';
import { IResponse } from '../models/shared/response';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TestExplanationService {
  baseUrl: string = environment.baseUrl;
  constructor(private http: HttpClient) {}

  getTestExplanatipon(): Observable<IResponse<TestExplanation[]>> {
    return this.http.get<IResponse<TestExplanation[]>>(
      this.baseUrl + 'testexplanation/testinfo'
    );
  }
}
