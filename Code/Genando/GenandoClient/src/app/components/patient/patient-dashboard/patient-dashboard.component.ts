import { Component, OnInit } from '@angular/core';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { LinksTitleConstant } from 'src/app/constants/system-constant';
import { PatientTimeline } from 'src/app/models/patient/patient-timeline';
import { IResponse } from 'src/app/models/shared/response';
import { AuthService } from 'src/app/services/auth.service';
import { PatientService } from 'src/app/services/patient.service';

@Component({
  selector: 'app-patient-dashboard',
  templateUrl: './patient-dashboard.component.html',
  styleUrls: ['./patient-dashboard.component.scss'],
})
export class PatientDashboardComponent implements OnInit {
  clinicalProcessStatus: number = 0;
  id!: number;
  sampleCollectionInProgress: number[] = [2, 3, 4];
  analysisInProgress: number[] = [5, 6];
  otherLinks: { text: string; route: string }[] = [
    { text: LinksTitleConstant.testMeans, route: RoutingPathConstant.testExplainationUrl},
    { text: LinksTitleConstant.faq, route: RoutingPathConstant.patientFaqUrl },
  ];
  constructor(
    private patientService: PatientService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.id = parseInt(this.authService.getUserId()!);
    this.patientService.getClinicalStatus(this.id).subscribe({
      next: (response: IResponse<PatientTimeline>) => {
        this.clinicalProcessStatus = response.data.status;
      },
    });
  }
}
