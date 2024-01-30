import { Component, Input, inject } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PatientConsultationStatusType } from 'src/app/constants/shared/patient-consultation-status-type';

@Component({
  selector: 'filter-patient',
  templateUrl: './filter-patient.component.html',
  styleUrls: ['./filter-patient.component.scss'],
})
export class FilterPatientComponent {
  constructor(private router: Router, private route: ActivatedRoute) {}

  @Input() filterForm!: FormGroup;
  public consultationStatus: number[] = [
    PatientConsultationStatusType.New,
    PatientConsultationStatusType.OnGoing,
    PatientConsultationStatusType.Done,
  ];

  public navigateToRegisterPatient(): void {
    this.router.navigate([`register`], { relativeTo: this.route });
  }
}
