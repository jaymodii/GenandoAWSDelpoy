import { KeyValue } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { Checkbox } from 'src/app/models/checkbox';
import { DropdownItem } from 'src/app/models/drop-down-item';
import { ClinicalPathUpdate } from 'src/app/models/patient/clinical-path-update';
import { IResponse } from 'src/app/models/shared/response';
import { PatientService } from 'src/app/services/patient.service';

@Component({
  selector: 'app-clinical-path-update',
  templateUrl: './clinical-path-update.component.html',
  styleUrls: ['./clinical-path-update.component.scss'],
})
export class ClinicalPathUpdateComponent {
  clinicalPath: ClinicalPathUpdate[] = [];
  clinicalForm!: FormGroup;
  clinicalAnswerControls: FormControl[] = [];
  clinicalPathArray!: FormArray;
  items: FormArray = this.formBuilder.array([]);
  clinicalOptions: string[] = [];
  dropdownItems: DropdownItem[] = [];
  clinicalOptionsArray: DropdownItem[][] = [];
  checkboxAnswersArray: Checkbox[][] = [];

  constructor(
    private patientService: PatientService,
    private formBuilder: FormBuilder,
    private router: Router,
  ) {
    this.clinicalForm = new FormGroup({
      questions: new FormArray([]),
      answers: new FormArray([]),
    });
  }

  ngOnInit(): void {}

  ngAfterViewInit() {
    this.patientService
    .getClinicalPathUpdate()
    .subscribe((response: IResponse<ClinicalPathUpdate[]>) => {
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
  }

  getQuestions() {
    const questionControlArray = [];
    for (let item of this.clinicalPath) {
      var optionsArray: string[] = [];
      var dropdown;
      if (item.options != null) {
        optionsArray = item.options.split(',').map((option) => option.trim());
        const dropdownItemss: DropdownItem[] = optionsArray.map((option) => ({
          value: option,
          viewValue: option,
        }));
        this.clinicalOptionsArray.push(dropdownItemss);
        dropdown = optionsArray.map((option, index) => ({
          value: index + 1,
          viewValue: option,
        }));
      } else {
        this.clinicalOptionsArray.push([]);
      }
      const validators = [];
      if (item.isQuestionMandatory) {
        validators.push(Validators.required);
      }
      questionControlArray.push({
        id: item.id,
        question: item.question,
        typeOfQuestion: item.typeOfQuestion,
        isQuestionMandatory: item.isQuestionMandatory,
        answer: [item.answer, validators],
      });
    }
    this.clinicalOptionsArray[2];

    return questionControlArray;
  }

  onSubmit() {
    this.clinicalForm.markAllAsTouched();
    if (this.clinicalForm.invalid) {
      return;
    }
    this.patientService
      .clinicalEnhancementAnswers(this.clinicalForm.value.questions)
      .subscribe({
        next: (response: any) => {
          this.router.navigate([RoutingPathConstant.patientDashboardUrl]);
        },
      });
  }
  onClear() {
    const questionsArray = this.getQuestionFormArray();
    while (questionsArray.length !== 0) {
      questionsArray.removeAt(0);
    }
    this.buildForm();
  }
  getQuestionFormArray(): FormArray {
    return this.clinicalForm?.get('questions') as FormArray;
  }

  getAnswerFormArray(): FormArray {
    return this.clinicalForm?.get('answer') as FormArray;
  }

  getDropdownValues(
    dropdownItems: KeyValue<string, DropdownItem>[]
  ): DropdownItem[] {
    return dropdownItems.map((keyValue) => keyValue.value);
  }

  getParentForm(index: number): FormGroup {
    const typeGroup = this.getQuestionFormArray().at(index) as FormGroup;
    return typeGroup;
  }

  getDropdownItems(index: number): DropdownItem[] {
    return this.clinicalOptionsArray[index];
  }

  handleCheckboxSelectionChange(selectedValues: string, i: number) {
    this.getParentForm(i).get('answer')?.setValue(selectedValues);
  }
  public navigateBack(): void {
    history.back();
  }
}
