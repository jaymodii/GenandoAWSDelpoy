import {
  Component,
  ElementRef,
  OnInit,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DropdownItem } from 'src/app/models/drop-down-item';
import { Profile } from 'src/app/models/profile/profile';
import { ProfileService } from 'src/app/services/profile/profile.service';
import { NgbDateStruct, NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { MessageService } from 'src/app/shared/services/message.service';
import { ValidationPattern } from 'src/app/constants/validation/validation-pattern';
import { ValidationMessageConstant } from 'src/app/constants/validation/validation-message';
import { AuthService } from 'src/app/services/auth.service';
import { MessageConstant } from 'src/app/constants/message-constant';
import { IResponse } from 'src/app/models/shared/response';
import { DialogBoxComponent } from 'src/app/shared/components/dialog-box/dialog-box.component';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  notifications: string[] = [];
  @ViewChild('profilePicture') profilePicture!: ElementRef;
  @ViewChild('profileHeadline') profileHeadline!: ElementRef;
  @ViewChild('modalContent') modalContent!: TemplateRef<any>;
  @ViewChild('pictureFileInput') pictureFileInput!: ElementRef;
  profilePictureSrc = '';
  userId = parseInt(this.authService.getUserId() || '');
  submitted = false;
  selectValue: number | undefined;
  selectedDate: NgbDateStruct | null = null;
  genderOptions: DropdownItem[] = [];
  isProfilePictureChanged: boolean = false;

  profile: Profile = {
    id: 0,
    firstName: '',
    lastName: '',
    headline: '',
    dob: '',
    gender: 0,
    phoneNumber: '',
    email: '',
    address: '',
    avatar: '',
  };

  verifyOtpForm = new FormGroup({
    otp: new FormControl('', [
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(6),
    ]),
  });

  profileForm = new FormGroup({
    firstName: new FormControl('', [
      Validators.required,
      Validators.maxLength(16),
    ]),
    lastName: new FormControl('', [
      Validators.required,
      Validators.maxLength(16),
    ]),
    headline: new FormControl('', [Validators.maxLength(50)]),
    genderDropdown: new FormControl('', [Validators.required]),
    phoneNumber: new FormControl('', [
      Validators.required,
      Validators.pattern(ValidationPattern.phoneNumber),
    ]),
    email: new FormControl('', [
      Validators.required,
      Validators.pattern(ValidationPattern.email),
      Validators.maxLength(128),
    ]),
    address: new FormControl('', [
      Validators.required,
      Validators.maxLength(512),
    ]),
    dob: new FormControl('', [Validators.required]),
  });

  emailValidationMsg = ValidationMessageConstant.email;
  phoneNumberValidationMsg = ValidationMessageConstant.phoneNumber;

  profileImage = { avatar: null };
  isAvatarChanged = false;
  isPatient = false;
  constructor(
    private profileService: ProfileService,
    private messageService: MessageService,
    private authService: AuthService,
    private modalService: NgbModal
  ) { }

  ngOnInit(): void {
    this.userId = parseInt(this.authService.getUserId()!);
    this.isPatient = this.authService.getUserType() === '2';
    this.getGenders();
    this.getProfileDetails();
  }

  getGenders() {
    this.profileService.getGenders().subscribe((response) => {
      if (response.success) {
        this.genderOptions = response.data.map((item: any) => ({
          value: item.id,
          viewValue: item.title,
        }));
      } else {
        this.messageService.error(
          MessageConstant.validProfile,
          MessageConstant.error
        );
      }
    });
  }

  getProfileDetails() {
    if (this.userId != null) {
      this.profileService
        .getProfileDetails(this.userId)
        .subscribe((response: IResponse<Profile>) => {
          this.profile = response.data;
          this.profile.dob = new Date(this.profile.dob)
            .toISOString()
            .split('T')[0];
          this.profileForm.patchValue(this.profile);
          this.profileForm
            .get('genderDropdown')
            ?.setValue(this.profile.gender.toString());
        });
    }
  }

  updateProfileDetails() {
    this.profileForm.markAllAsTouched();
    const oldEmail = this.profile.email;
    const newEmail = this.profileForm.value.email;
    if (!this.profileForm.dirty && !this.isProfilePictureChanged) {
      this.messageService.error(MessageConstant.editDetailsFirst);
      return;
    }

    if (oldEmail !== newEmail) {
      if (this.profileForm.valid) {
        this.profileService
          .sendOtp(newEmail)
          .subscribe((response: IResponse<any>) => {
            if (response.success) {
              this.openConfirmationModal();
            } else {
              this.messageService.error(
                MessageConstant.error,
                MessageConstant.error
              );
            }
          });
      } else {
        this.messageService.error(
          MessageConstant.validProfile,
          MessageConstant.error
        );
      }
    } else if (this.profileForm.valid) {
      this.performProfileUpdate();
    } else {
      this.messageService.error(
        MessageConstant.validProfile,
        MessageConstant.error
      );
    }
  }

  openConfirmationModal() {
    const modalRef = this.modalService.open(DialogBoxComponent, {
      centered: true,
    });
    modalRef.componentInstance.modalTitle = MessageConstant.emailChangeModalTitle;
    modalRef.componentInstance.modalContent = this.modalContent;
    modalRef.componentInstance.modalBtnLabel2 = MessageConstant.verify;
    modalRef.componentInstance.clickBtn2.subscribe(() => {
      if (this.verifyOtpForm.valid) {
        this.verifyOTP(modalRef);

        this.verifyOtpForm.reset();
      }
    });
    modalRef.componentInstance.closeBtn.subscribe(() => {
      this.verifyOtpForm.reset();
      this.getProfileDetails();
    });
  }

  verifyOTP(modalRef:NgbModalRef) {
    this.verifyOtpForm.markAllAsTouched();
    if (this.verifyOtpForm.valid) {
      this.profileService.verifyOtp(this.verifyOtpForm.value.otp).subscribe({
        next: (response) => {
          this.performProfileUpdate();
          this.getProfileDetails();
          modalRef.close();
        },
        error: (error) => { },
      });
    }
  }

  performProfileUpdate() {
    this.isProfilePictureChanged = false;
    this.submitted = true;
    this.profileForm.markAllAsTouched();

    const currentProfile = { ...this.profile };

    if (
      !this.areProfileFieldsEqual(currentProfile, this.profileForm.value) ||
      this.isAvatarChanged
    ) {
      const formData = new FormData();
      formData.append('firstName', this.profileForm.value.firstName ?? '');
      formData.append('lastName', this.profileForm.value.lastName ?? '');
      formData.append('headline', this.profileForm.value.headline ?? '');
      formData.append('gender', this.profileForm.value.genderDropdown ?? '');
      formData.append('phoneNumber', this.profileForm.value.phoneNumber ?? '');
      formData.append('email', this.profileForm.value.email ?? '');
      formData.append('address', this.profileForm.value.address ?? '');

      const dobValue = this.profileForm.value.dob;
      const formattedDob = dobValue ? new Date(dobValue).toISOString() : '';
      formData.append('dob', formattedDob);

      if (this.profile.avatar) {
        const avatarBlob = this.dataURItoBlob(this.profile.avatar);
        formData.append('avatar', avatarBlob, 'avatar.png');
      }

      this.profileService
        .updateProfileDetails(this.userId, formData)
        .subscribe((response: any) => {
          this.getProfileDetails();
          this.isAvatarChanged = false;
        });
    }
  }

  areProfileFieldsEqual(profile1: Profile, profile2: any): boolean {
    return (
      profile1.firstName === profile2.firstName &&
      profile1.lastName === profile2.lastName &&
      profile1.headline === profile2.headline &&
      profile1.gender.toString() === profile2.genderDropdown &&
      profile1.phoneNumber === profile2.phoneNumber &&
      profile1.email === profile2.email &&
      profile1.address === profile2.address &&
      profile1.dob === profile2.dob
    );
  }

  dataURItoBlob(dataURI: string): Blob {
    const base64String = dataURI.split(',')[1];
    const mimeType = dataURI.split(',')[0].split(':')[1].split(';')[0];
    const byteCharacters = atob(base64String);
    const byteArrays = new Uint8Array(byteCharacters.length);

    for (let i = 0; i < byteCharacters.length; i++) {
      byteArrays[i] = byteCharacters.charCodeAt(i);
    }

    return new Blob([byteArrays], { type: mimeType });
  }

  openPictureFileDialog() {
    this.isAvatarChanged = false;
    this.pictureFileInput.nativeElement.click();
  }

  handlePictureFileChange(event: Event) {
    this.isProfilePictureChanged = true;
    const inputElement = event.target as HTMLInputElement;
    const file = inputElement.files?.[0];

    if (file) {
      const reader = new FileReader();
      reader.onload = (event: ProgressEvent<FileReader>) => {
        this.profile.avatar = event.target?.result as string;
        this.profilePicture.nativeElement.setAttribute(
          'src',
          event.target?.result as string
        );
      };
      reader.readAsDataURL(file);
      this.isAvatarChanged = true;
    }
  }

  restrictInput(event: KeyboardEvent): void {
    const pattern = ValidationPattern.number;
    const allowedKeys = ['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight'];
    if (!pattern.test(event.key) && !allowedKeys.includes(event.key)) {
      event.preventDefault();
    }
  }

  cancelProfile() {
    this.profileForm.reset(this.profile);
  }
}
