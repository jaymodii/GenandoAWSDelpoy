import {
  AfterViewChecked,
  ChangeDetectorRef,
  Component,
  OnInit,
} from '@angular/core';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PatientLabResult } from 'src/app/models/patient-lab-result';
import { DoctorService } from 'src/app/services/doctor.service';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';

@Component({
  selector: 'app-send-result',
  templateUrl: './send-result.component.html',
  styleUrls: ['./send-result.component.scss'],
})
export class SendResultComponent implements OnInit, AfterViewChecked {
  clinicalProcessId!: number;
  labResultForm!: FormGroup;
  patientLabResult!: PatientLabResult;
  public Editor = ClassicEditor;

  constructor(
    private doctorService: DoctorService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}
  ngAfterViewChecked(): void {
    this.cdr.detectChanges();
  }

  ngOnInit(): void {
    this.builForm();
    this.route.params.subscribe((params) => {
      this.clinicalProcessId = params['clinicalProcessId'];
      this.doctorService.getLabResult(this.clinicalProcessId).subscribe({
        next: (data: any) => {
          this.patientLabResult = data.data;
          this.labResultForm
            .get('name')
            ?.setValue(
              this.patientLabResult.firstName +
                ' ' +
                this.patientLabResult.lastName
            );
          this.labResultForm
            .get('email')
            ?.setValue(this.patientLabResult.email);
          this.addResultToForm();
        },
      });
    });
  }

  builForm() {
    this.labResultForm = new FormGroup({
      name: new FormControl(),
      email: new FormControl(),
      doctorResultArray: new FormArray([]),
    });
  }

  get doctorResultArray(): FormArray {
    return this.labResultForm.get('doctorResultArray') as FormArray;
  }

  addResultToForm() {
    for (let item of this.patientLabResult.testResults) {
      const result = new FormGroup({
        testTitle: new FormControl(item.testTitle),
        doctorNotes: new FormControl(),
        clinicalProcessTestId: new FormControl(item.clinicalProcessTestId),
        reportAttachmentTitle: new FormControl(item.reportAttachmentTitle),
      });
      this.doctorResultArray.push(result);
    }
  }

  parentForm(index: number): FormGroup {
    return this.doctorResultArray.at(index) as FormGroup;
  }

  downloadResult(clinicalProcessTestId: number, fileName: string) {
    this.doctorService.downloadLabResult(clinicalProcessTestId).subscribe({
      next: (response: ArrayBuffer) => {
        const blob = new Blob([response], { type: 'application/pdf' });
        const blobUrl = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = blobUrl;
        link.download = fileName;
        link.click();
        URL.revokeObjectURL(blobUrl);
      },
    });
  }

  cancel() {
    this.router.navigate([RoutingPathConstant.doctorDashboardUrl]);
  }

  publishResult() {
    this.labResultForm.markAllAsTouched();
    if (this.labResultForm.valid) {
      this.doctorService.publishResult(this.doctorResultArray.value).subscribe({
        next: (res: any) => {
          this.router.navigate([RoutingPathConstant.doctorDashboardUrl]);
        },
      });
    }
  }
}
