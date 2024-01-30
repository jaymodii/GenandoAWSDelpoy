import { Component, OnInit, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { VALIDATION_CONSTANTS } from 'src/app/constants/validation/validation-constants';
import { IUserDetailsFormRequest } from 'src/app/models/doctor/register-user-request.interface';
import { DropdownItem } from 'src/app/models/drop-down-item';
import { UserService } from 'src/app/services/doctor/user.service';
import { emailValidator } from 'src/app/common/validators/email-validator';
import { maxDateValidator } from 'src/app/common/validators/max-date-validator';
import { minDateValidator } from 'src/app/common/validators/min-date-validator';
import { namesValidator } from 'src/app/common/validators/names-validator';
import { MessageService } from 'src/app/shared/services/message.service';
import { setInputDate } from 'src/app/utils/date-util';
import { DATE_CONST } from 'src/app/constants/shared/date-constant';
import { IResponse } from 'src/app/models/shared/response';
import { MessageConstant } from 'src/app/constants/message-constant';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { GenderType } from 'src/app/constants/shared/gender-type';
import { GenderTitlePipe } from 'src/app/pipes/gender-title.pipe';
import { ValidationPattern } from 'src/app/constants/validation/validation-pattern';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly userService = inject(UserService);
  private readonly messageService = inject(MessageService);
  private readonly router = inject(Router);

  public formTitle: string = '';
  public submitButtonText: string = '';

  private user!: IUserDetailsFormRequest;
  private userId!: number;
  public isAddMode: boolean = true;

  private isPatient: boolean = true;

  public userForm!: FormGroup;

  public firstName!: FormControl;

  public lastName!: FormControl;

  public email!: FormControl;

  public dateOfBirth!: FormControl;

  public phoneNumber!: FormControl;

  public gender!: FormControl;

  public address!: FormControl;

  private genderPipe = new GenderTitlePipe();

  public genderOptions: DropdownItem[] = [
    {
      value: GenderType.Male,
      viewValue: this.genderPipe.transform(GenderType.Male),
    },
    {
      value: GenderType.Female,
      viewValue: this.genderPipe.transform(GenderType.Female),
    },
  ];

  public ngOnInit(): void {
    this.setFormDetails();
    this.createForm();
  }

  private setFormDetails(): void {
    this.isPatient = this.route.snapshot.data['isPatient'];

    this.route.paramMap.subscribe((params) => {
      const id = params.get('id') ? +params.get('id')! : null;
      if (id === null || Number.isNaN(id)) {
        this.formTitle = this.isPatient
          ? MessageConstant.createNewPatient
          : MessageConstant.createNewLabUser;
        this.submitButtonText = this.isPatient
          ? MessageConstant.saveAndreqClinicalPath
          : MessageConstant.save;
        this.isAddMode = true;
      } else {
        this.formTitle = this.isPatient
          ? MessageConstant.editPatient
          : MessageConstant.editLabUser;
        this.submitButtonText = MessageConstant.saveChanges;
        this.isAddMode = false;
        this.userId = id;
      }
    });
  }

  private createForm(): void {
    this.firstName = new FormControl('', [
      Validators.required,
      Validators.minLength(VALIDATION_CONSTANTS.MIN_NAME_LENGTH),
      Validators.maxLength(VALIDATION_CONSTANTS.MAX_NAME_LENGTH),
      namesValidator(),
    ]);
    this.lastName = new FormControl('', [
      Validators.required,
      Validators.minLength(VALIDATION_CONSTANTS.MIN_NAME_LENGTH),
      Validators.maxLength(VALIDATION_CONSTANTS.MAX_NAME_LENGTH),
      namesValidator(),
    ]);
    this.email = new FormControl('', [Validators.required, emailValidator()]);
    this.dateOfBirth = new FormControl(null, [
      Validators.required,
      minDateValidator(DATE_CONST.minDate),
      maxDateValidator(DATE_CONST.maxDate),
    ]);
    this.phoneNumber = new FormControl('', [Validators.required]);
    this.gender = new FormControl(0, [Validators.required]);
    this.address = new FormControl('', [Validators.required,Validators.minLength(VALIDATION_CONSTANTS.MIN_ADDRESS_LENGTH)]);

    this.userForm = new FormGroup({
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      dateOfBirth: this.dateOfBirth,
      phoneNumber: this.phoneNumber,
      gender: this.gender,
      address: this.address,
    });

    if (!this.isAddMode) {
      this.userService
        .loadUserFormDetails(this.userId)
        .subscribe((response) => {
          this.user = response.data;
          this.patchUserForm(response.data);
        });
    }
  }

  private patchUserForm(response: IUserDetailsFormRequest): void {
    this.userForm.patchValue(response);
    this.dateOfBirth.setValue(
      response.dateOfBirth ? setInputDate(response.dateOfBirth) : null
    );
  }

  public onSubmit(): void {
    this.userForm.markAllAsTouched();
    if (!this.userForm.dirty){
      this.messageService.error(MessageConstant.editDetailsFirst);
      return;
    }
    if (this.userForm.invalid) return;

    const request: IUserDetailsFormRequest = this.GetRegisterRequest();
    if (this.isAddMode) {
      this.userService.registerUser(request).subscribe({
        next: (response: IResponse<IUserDetailsFormRequest>) => {
          this.navigateBack();
        },
      });
    } else {
      this.userService
        .updateUser(this.userId, request)
        .subscribe((response: IResponse<null>) => {
          this.navigateBack();
        });
    }
  }

  public navigateBack(): void {
    history.back();
  }

  private GetRegisterRequest(): IUserDetailsFormRequest {
    const registerRequest: IUserDetailsFormRequest = this.userForm.value;
    registerRequest.isPatient = this.isPatient;
    registerRequest.dateOfBirth = new Date(this.dateOfBirth.value);
    registerRequest.gender = parseInt(this.gender.value);
    return registerRequest;
  }

  public resetForm(): void {
    this.router.navigate([RoutingPathConstant.patientListUrl]);
  }
}
