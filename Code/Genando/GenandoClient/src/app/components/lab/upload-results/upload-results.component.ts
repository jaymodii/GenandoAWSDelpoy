import { Component, ElementRef, ViewChild } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageConstant } from 'src/app/constants/message-constant';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { ValidationPattern } from 'src/app/constants/validation/validation-pattern';
import { LabPatientInfo } from 'src/app/models/lab-patient-info';
import { DoctorService } from 'src/app/services/doctor.service';
import { LabService } from 'src/app/services/lab.service';
import { InputComponent } from 'src/app/shared/components/input/input.component';
import { MessageService } from 'src/app/shared/services/message.service';

@Component({
  selector: 'app-upload-results',
  templateUrl: './upload-results.component.html',
  styleUrls: ['./upload-results.component.scss'],
})
export class UploadResultsComponent {
  patientInfo!: LabPatientInfo;
  clinicalProcessTestId!: number;
  errorMessage: string | null = null;
  selectedFile: any;
  uploadContent: Blob | null = null;
  profileForm!: FormGroup;
  authService: any;
  totalRecords: any;
  data: any;
  newFileUploaded: boolean = false;
  @ViewChild('fileInput') fileInput!: InputComponent;

  constructor(
    private labService: LabService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private router: Router,
    private doctorService: DoctorService,
    private fb: FormBuilder
  ) { }

  onButtonClick() {
    this.fileInput.fileInput();
  }

  ngOnInit(): void {
    this.buildForm();
    this.loadPatientInfo();
  }

  buildForm(): void {
    this.profileForm = this.fb.group({
      upload: new FormControl(),
      externalLink: new FormControl('', [
        Validators.required,
        Validators.pattern(ValidationPattern.link),
      ]),
      notes: new FormControl('', [Validators.required]),
      fileName: new FormControl('', [Validators.required]),
    });
  }


  loadPatientInfo(): void {
    this.route.url.subscribe((segments) => {
      const urlSegments = segments.map((segment) => segment.path);
      this.route.params.subscribe((params) => {
        this.clinicalProcessTestId = +params['clinicalProcessTestId'];
        this.labService.getPatientInfo(this.clinicalProcessTestId).subscribe({
          next: (data) => {
            this.patientInfo = data.data;

            if (urlSegments.includes(RoutingPathConstant.labUpdateResultName)) {
              const blob = new Blob(
                [this.patientInfo.testResults.reportAttachmentBytes],
                {
                  type: 'application/pdf',
                }
              );
              const file = new File(
                [blob],
                this.patientInfo.testResults.reportAttachmentTitle
              );
              this.selectedFile = file;
              this.profileForm
                .get('notes')
                ?.setValue(this.patientInfo.testResults.labNotes);
              this.profileForm
                .get('externalLink')
                ?.setValue(this.patientInfo.externalLink);
              this.profileForm
                .get('fileName')
                ?.setValue(this.patientInfo.testResults.reportAttachmentTitle);
            }
          },
          error: (error) => {
            console.error(error);
          },
        });
      });
    });
  }

  onFileInputChange(event: any): void {
    this.newFileUploaded = true;
    const files = event.target.files;
    if (files && files.length > 0 && files[0].type === 'application/pdf') {
      this.selectedFile = files[0];
      this.profileForm.get('fileName')?.setValue(this.selectedFile.name);
    } else {
      event.target.value = null;
      this.selectedFile = null;
      this.messageService.error(MessageConstant.pdfOnly);
    }
  }

  onSaveButtonClick(): void {
    this.profileForm.markAllAsTouched();
    this.profileForm.get('upload')?.clearValidators();
    this.profileForm.get('upload')?.updateValueAndValidity();

    if (!this.profileForm.dirty) {
      return this.messageService.error(MessageConstant.editDetailsFirst);
    }

    if (this.profileForm.invalid) {
      return;
    }

    if (this.selectedFile) {
      const formData = new FormData();
      const externalLinkValue =
        this.profileForm.get('externalLink')?.value || '';
      const notesValue = this.profileForm.get('notes')?.value || '';

      formData.append(
        'clinicalProcessTestId',
        (this.clinicalProcessTestId || 0).toString()
      );
      formData.append('reportAttachmentTitle', this.selectedFile!.name);
      formData.append('labNotes', notesValue);
      formData.append('externalLink', externalLinkValue);
      formData.append('reportAttachment', this.selectedFile);
      formData.append(
        'testResultId',
        (this.patientInfo.testResults
          ? this.patientInfo.testResults.testResultId
          : 0
        ).toString()
      );

      this.labService.uploadResults(formData).subscribe({
        next: (response) => {
          this.labService.setResultUploaded(this.clinicalProcessTestId);
        },
        error: (error) => {
          this.messageService.error(error.error.Message, MessageConstant.error);
        },
      });
      this.router.navigate([RoutingPathConstant.labDashboardUrl]);
    }
    this.profileForm.get('upload')?.addValidators(Validators.required);
    this.profileForm.get('upload')?.updateValueAndValidity();
  }

  downloadResult() {
    if (this.newFileUploaded || !this.patientInfo.testResults) {
      return;
    }
    this.doctorService.downloadLabResult(this.clinicalProcessTestId).subscribe({
      next: (response: ArrayBuffer) => {
        const blob = new Blob([response], { type: 'application/pdf' });
        const blobUrl = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = blobUrl;
        link.download = this.selectedFile.name;
        link.click();
        URL.revokeObjectURL(blobUrl);
      },
      error: (error) => {
        console.error(MessageConstant.errorDownloadFile, error);
      },
    });
  }

  onCancelButtonClick(): void {
    this.profileForm.reset();
    this.selectedFile = null;
    this.router.navigate([RoutingPathConstant.labDashboardUrl]);
  }

  onBackButtonClick() {
    this.router.navigate([RoutingPathConstant.labDashboardUrl]);
  }
}
