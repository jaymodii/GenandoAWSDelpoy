import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MessageConstant } from 'src/app/constants/message-constant';
import { ClinicalAnswerRequest } from 'src/app/models/clinical-answer-request';
import { ClinicalPath } from 'src/app/models/patient/clinical-path';
import { IResponse } from 'src/app/models/shared/response';
import { DoctorService } from 'src/app/services/doctor.service';

@Component({
  selector: 'app-clinical-answer',
  templateUrl: './clinical-answer.component.html',
  styleUrls: ['./clinical-answer.component.scss'],
})
export class ClinicalAnswerComponent implements OnInit {
  clinicalQA: ClinicalPath[] = [];
  pageTitle: string = '';
  clinicalAnswerRequest: ClinicalAnswerRequest = {
    patientId: 0,
    isRequestedAnswer: false,
  };

  constructor(
    private route: ActivatedRoute,
    private doctorService: DoctorService
  ) {}

  ngOnInit(): void {
    this.route.url.subscribe((segments) => {
      const urlSegments = segments.map((segment) => segment.path);
      this.route.params.subscribe((params) => {
        this.clinicalAnswerRequest.patientId = parseInt(params['patientId']);
        if (urlSegments.includes('see-clinical-path')) {
          this.pageTitle = MessageConstant.clinicalPathAnswerTitle;
        } else {
          this.pageTitle = MessageConstant.requestedInformationTitle;
          this.clinicalAnswerRequest.isRequestedAnswer = true;
        }
        this.doctorService
          .clinicalAnswers(this.clinicalAnswerRequest)
          .subscribe({
            next: (response: IResponse<ClinicalPath[]>) => {
              this.clinicalQA = response.data.filter((item) => item.answer);
            },
          });
      });
    });
  }
}
