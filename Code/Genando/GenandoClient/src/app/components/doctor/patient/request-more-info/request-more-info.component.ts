import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, of, switchMap } from 'rxjs';
import { MessageConstant } from 'src/app/constants/message-constant';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { QuestionPublishStatus } from 'src/app/constants/shared/question-publish-status';
import { IPatientMoreQuestionRequest } from 'src/app/models/doctor/patient-more-question.interface';
import { IPatientTestInfoResponse } from 'src/app/models/doctor/user-listing-response.interface';
import { IPatientQuestion } from 'src/app/models/patient/patient-question.interface';
import { IResponse } from 'src/app/models/shared/response';
import { PatientService } from 'src/app/services/doctor/patient.service';
import { MessageService } from 'src/app/shared/services/message.service';

@Component({
  selector: 'app-request-more-info',
  templateUrl: './request-more-info.component.html',
  styleUrls: ['./request-more-info.component.scss'],
})
export class RequestMoreInfoComponent implements OnInit, OnDestroy {
  private subscription!: Subscription;
  private preloadQuestions: IPatientQuestion[] = [];
  private patientId!: number;
  public questions: IPatientQuestion[] = [];
  public deletedQuestions: number[] = [];
  public patientClinicalProfile: IPatientTestInfoResponse | null = null;
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private patientService: PatientService,
    private messageService: MessageService
  ) { }
  public ngOnInit(): void {
    this.loadPatientQuestions();
  }

  private loadPatientQuestions() {
    this.route.paramMap
      .pipe(
        switchMap((params) => {
          this.patientId = Number(params.get('id'));
          this.loadPatientClinicalProfile(this.patientId);
          this.loadPatientQuestionsRequest(this.patientId);
          return of(params);
        })
      )
      .subscribe();
  }

  private loadPatientClinicalProfile(patientId: number) {
    this.subscription = this.patientService
      .getPatientTestProfile(this.patientId)
      .subscribe((response) => {
        this.patientClinicalProfile = response.data;
      });
  }

  private loadPatientQuestionsRequest(patientId: number) {
    this.subscription = this.patientService
      .getPatientInformationQuestions(patientId)
      .subscribe({
        next: (data) => {
          this.questions = [...data.data];
          this.preloadQuestions = [...data.data];
        },
      });
  }

  public addNewQuestion(): void {
    const newQuestion: IPatientQuestion = {
      id: this.questions.length + 1,
      question: '',
      status: QuestionPublishStatus.New,
    };

    this.questions.push(newQuestion);
  }

  public deleteQuestion(id: number): void {
    this.questions = this.questions.filter((question) => {
      if (
        question.id === id &&
        question.status === QuestionPublishStatus.DraftByDoctor
      )
        this.deletedQuestions.push(question.id);
      return question.id !== id;
    });
  }

  public onSave(): void {
    if (this.validateQuestion()) return;
    const request = this.convertToQuestionModel(
      QuestionPublishStatus.DraftByDoctor
    );
    this.sendRequest(request);
  }

  private sendRequest(request: IPatientMoreQuestionRequest): void {
    this.subscription = this.patientService
      .addPatientInformationQuestion(this.patientId, request)
      .subscribe({
        next: (response: IResponse<IPatientMoreQuestionRequest>) =>
          {
            this.loadPatientQuestionsRequest(this.patientId);
            this.navigateBack();
          }
      });
  }

  public onSend(): void {
    if (this.validateQuestion()) return;
    const request = this.convertToQuestionModel(
      QuestionPublishStatus.PublishByDoctor
    );
    this.sendRequest(request);
  }

  private validateQuestion(): boolean {
    if (this.questions.some((que) => que.question.trim() === '')) {
      this.messageService.error(MessageConstant.enterQuestion);
      return true;
    }
    return this.questions.length === 0;
  }

  private convertToQuestionModel(
    type: QuestionPublishStatus
  ): IPatientMoreQuestionRequest {
    const patientQuestionRequest: IPatientMoreQuestionRequest = {
      questions: [...this.questions],
      status: type,
      deletedQuestions: [...this.deletedQuestions],
      questionIds: this.preloadQuestions
        .filter((que) => !this.deletedQuestions.includes(que.id))
        .map((que) => que.id),
    };
    return patientQuestionRequest;
  }

  public navigateBack(): void {
    this.router.navigate([RoutingPathConstant.doctorDashboardUrl]);
  }

  public ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
