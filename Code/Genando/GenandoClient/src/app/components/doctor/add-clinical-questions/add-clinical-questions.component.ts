import {
  ChangeDetectorRef,
  Component,
  OnInit,
} from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageConstant } from 'src/app/constants/message-constant';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { QuestionTypeConstant } from 'src/app/constants/system-constant';
import { AddClinicalQuestions } from 'src/app/models/doctor/add-clinical-questions';
import { DropdownItem } from 'src/app/models/drop-down-item';
import { IResponse } from 'src/app/models/shared/response';
import { QuestionTypePipe } from 'src/app/pipes/question-type.pipe';
import { DoctorService } from 'src/app/services/doctor.service';

@Component({
  selector: 'app-add-clinical-questions',
  templateUrl: './add-clinical-questions.component.html',
  styleUrls: ['./add-clinical-questions.component.scss'],
})
export class AddClinicalQuestionsComponent implements OnInit {
  clinicalQuestionHeader: string = "";
  clinicalForm: FormGroup;
  selectedQuestionType: (string | number)[] = [];
  questionTypeOptions: DropdownItem[] = [
    { value: QuestionTypeConstant.textValue, viewValue: QuestionTypeConstant.textViewValue },
    { value: QuestionTypeConstant.radioValue, viewValue: QuestionTypeConstant.radioViewValue },
    { value: QuestionTypeConstant.checkboxValue, viewValue: QuestionTypeConstant.checkboxViewValue },
    { value: QuestionTypeConstant.selectValue, viewValue: QuestionTypeConstant.selectViewValue },
  ];
  patientId!: number;
  addClinicalQuestions: AddClinicalQuestions[] = [];
  questionTypePipe = new QuestionTypePipe();
  existingOptionArray: string[] = [];
  constructor(
    private formBuilder: FormBuilder,
    private changeDetectorRef: ChangeDetectorRef,
    private doctorService: DoctorService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.clinicalForm = this.formBuilder.group({
      questions: this.formBuilder.array([]),
    });
  }

  ngOnInit(): void {
    this.patientId = parseInt(
      this.route.snapshot.paramMap.get('patientId') || '0'
    );
    this.clinicalQuestionHeader = this.patientId == 0 ? MessageConstant.addCommonClinicalQuestion : MessageConstant.addClinicalQuestionForPatient;
    this.doctorService.getClinicalQuestion(this.patientId).subscribe(
      (response: any) => {
        response.data.forEach((element: any) => {
          this.addQuestion(element.questionId, element.question, <string>this.questionTypePipe.transform(element.typeOfQuestion), !element.options ? [] : (element.options).split(","), element.isQuestionMandatory);
        });
      }
    );
  }

  ngAfterViewChecked(): void {
    this.changeDetectorRef.detectChanges();
  }

  get questionArray() {
    return this.clinicalForm.get('questions') as FormArray;
  }

  getParentForm(index: number): FormGroup {
    return this.questionArray.at(index) as FormGroup;
  }

  getOptionParentForm(i: number, j: number): FormGroup {
    const optionArray = this.getParentForm(i).get('options') as FormArray;
    return optionArray.at(j) as FormGroup;
  }

  onQuestionTypeSelected(
    selectedValue: string | number,
    index: number,
    question: AbstractControl
  ) {
    this.selectedQuestionType[index] = selectedValue;
    const optionsArray = this.getParentForm(index).get('options') as FormArray;
    if (selectedValue === QuestionTypeConstant.textValue || selectedValue === '') {
      optionsArray.clear();
    } else if (optionsArray.length === 0) {
      this.addOption(question, index);
      this.addOption(question, index);
    }
  }

  getQuestionControl(index: number): AbstractControl {
    return this.questionArray.at(index);
  }

  getOptionIsDisabled(index: number): boolean {
    const typeControl = this.getQuestionControl(index).get(
      'questionType'
    ) as FormControl;
    if (typeControl.value === QuestionTypeConstant.textValue || typeControl.value === '') {
      return true;
    }
    return false;
  }

  addQuestion(questionId?: number, addQuestion?: string, questionType?: string, options?: string[], required?: boolean) {

    options ??= [];
    const question = this.formBuilder.group({
      questionId: new FormControl(questionId),
      addQuestion: new FormControl(addQuestion, Validators.compose([Validators.required])),
      questionType: new FormControl(questionType, Validators.compose([Validators.required])),
      options: this.formBuilder.array(
        options!.map(option => this.formBuilder.group({
          option: new FormControl(option)
        }))
      ),
      required: new FormControl(required ?? false),
    });
    this.questionArray.push(question);
  }

  removeQuestion(questionId: number, index: number) {
    this.questionArray.removeAt(index);
    if (questionId) {
      this.doctorService.deleteclinicalQuestion(questionId).subscribe();
    }
  }

  getOptionsArray(question: AbstractControl): FormArray {
    return (question as FormGroup).get('options') as FormArray;
  }

  addOption(question: AbstractControl, index: number) {
    if (!this.getOptionIsDisabled(index)) {
      this.getOptionsArray(question).push(
        this.formBuilder.group({
          option: this.formBuilder.control('', Validators.required),
        })
      );
    }
  }

  hasOptions(i: number): boolean {
    if (this.getQuestionControl(i).get('questionType')?.value !== QuestionTypeConstant.textValue) {
      return true;
    }
    return false;
  }

  removeOption(question: AbstractControl, index: number) {
    this.getOptionsArray(question).removeAt(index);
  }

  submitForm() {
    this.addClinicalQuestions = [];
    for (let item of this.clinicalForm.value.questions) {
      if (item.options && item.options.length > 0) {
        const optionsArray = item.options
          .filter((option: any) => option && option.option)
          .map((option: any) => option.option);
        const allOption: string = optionsArray.join(',');
        const clinicalQuestion: AddClinicalQuestions = {
          questionId: item.questionId,
          question: item.addQuestion,
          typeOfQuestion: <number>this.questionTypePipe.transform(item.questionType),
          options: allOption,
          isQuestionMandatory: item.required,
        };
        this.addClinicalQuestions.push(clinicalQuestion);
      } else {
        const clinicalQuestion: AddClinicalQuestions = {
          questionId: item.questionId,
          question: item.addQuestion,
          typeOfQuestion: <number>this.questionTypePipe.transform(item.questionType),
          options: '',
          isQuestionMandatory: item.required,
        };
        this.addClinicalQuestions.push(clinicalQuestion);
      }
    }
    this.doctorService
      .addClinicalQuestions(this.patientId, this.addClinicalQuestions)
      .subscribe({
        next: (response: IResponse<null>) => {
          this.router.navigate([RoutingPathConstant.doctorDashboard]);
        },
      });
  }

  public navigateBack(): void {
    history.back();
  }
}
