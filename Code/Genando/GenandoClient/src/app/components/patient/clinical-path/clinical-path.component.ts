import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { ClinicalPath } from 'src/app/models/patient/clinical-path';
import { IResponse } from 'src/app/models/shared/response';
import { PatientService } from 'src/app/services/patient.service';

@Component({
  selector: 'app-clinical-path',
  templateUrl: './clinical-path.component.html',
  styleUrls: ['./clinical-path.component.scss'],
})
export class ClinicalPathComponent implements OnInit, AfterViewInit {
  clinicalPath: ClinicalPath[] = [];
  clinicalForm!: FormGroup;
  clinicalAnswerControls: FormControl[] = [];
  clinicalPathArray!: FormArray;
  items: FormArray = this.formBuilder.array([]);

  constructor(
    private patientService: PatientService,
    private formBuilder: FormBuilder,
    private router: Router,
  ) {
    this.clinicalForm = new FormGroup({
      questions: new FormArray([]),
    });
  }

  ngOnInit(): void {}

  ngAfterViewInit() {
    this.patientService
      .getClinicalPath()
      .subscribe((response: IResponse<ClinicalPath[]>) => {
        this.clinicalPath = response.data;
        this.buildForm();
      });
  }

  buildForm() {
    this.items = this.clinicalForm?.get('questions') as FormArray;
    this.clinicalForm.get('questions');
    this.getQuestions().map((question) =>
      this.items.push(this.formBuilder.group(question))
    );

    console.warn(this.clinicalForm);
  }

  getQuestions() {
    const questionControlArray = [];
    let i = 0;
    for (let item of this.clinicalPath) {
      questionControlArray.push({
        id: [item.id],
        question: [item.question],
        answer: [item.answer],
      });
    }
    return questionControlArray;
  }

  onSubmit() {
    this.patientService
      .clinicalAnswers(this.clinicalForm.value.questions)
      .subscribe(
        (response: any) => {
          this.router.navigate([RoutingPathConstant.patientDashboardUrl]);
        },
        (error: any) => {
          console.log(error);
        }
      );
  }

  onCancel() {
    this.router.navigate([RoutingPathConstant.patientDashboardUrl]);
  }

  getQuestionFormArray(): FormArray {
    return this.clinicalForm?.get('questions') as FormArray;
  }
}
