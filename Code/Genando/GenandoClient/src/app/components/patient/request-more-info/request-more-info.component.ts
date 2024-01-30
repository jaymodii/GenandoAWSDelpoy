import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { QuestionPublishStatus } from 'src/app/constants/shared/question-publish-status';
import { IPatientMoreInfoResponse } from 'src/app/models/patient/patient-more-info.interface';
import { PatientService } from 'src/app/services/patient.service';

@Component({
  selector: 'app-request-more-info',
  templateUrl: './request-more-info.component.html',
  styleUrls: ['./request-more-info.component.scss'],
})
export class RequestMoreInfoComponent {
  private patientService = inject(PatientService);
  private router = inject(Router);

  public require = false;

  public moreInfo$: Observable<IPatientMoreInfoResponse[]> =
    this.patientService.loadRequestedQuestions();

  public onSave(patientMoreInfo: IPatientMoreInfoResponse[]): void {
    if (this.validateAnswer(patientMoreInfo, false)) return;
    this.savePatientMoreInfo(
      patientMoreInfo,
      QuestionPublishStatus.DraftByPatient
    );
  }

  public onSend(patientMoreInfo: IPatientMoreInfoResponse[]): void {
    if (this.validateAnswer(patientMoreInfo)) return;
    this.savePatientMoreInfo(
      patientMoreInfo,
      QuestionPublishStatus.PublishByPatient
    );
  }

  public navigateBack(): void {
    history.back();
  }

  private savePatientMoreInfo(
    patientMoreInfo: IPatientMoreInfoResponse[],
    status: QuestionPublishStatus
  ): void {
    this.patientService
      .savePatientInformation(patientMoreInfo, status)
      .subscribe((res) => {
        if (status === QuestionPublishStatus.PublishByPatient)
          this.router.navigate([RoutingPathConstant.patientDashboardUrl]);
      });
  }

  private validateAnswer(
    moreInfo: IPatientMoreInfoResponse[],
    allRequire: boolean = true
  ): boolean {
    this.require = moreInfo.some(
      (info) => !info.answer || info.answer.trim() === ''
    );

    return this.require;
  }
}
