import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PatientComponent } from './patient.component';
import { RouterModule } from '@angular/router';
import { PatientDashboardComponent } from './patient-dashboard/patient-dashboard.component';
import { PatientRoutingModule } from './patient-routing.module';
import { ClinicalPathComponent } from './clinical-path/clinical-path.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { LayoutModule } from '../layout/layout.module';
import { RequestMoreInfoComponent } from './request-more-info/request-more-info.component';
import { ClinicalPathUpdateComponent } from './clinical-path-update/clinical-path-update.component';
import { PipeModule } from 'src/app/pipes/pipe.module';

const components = [
  PatientComponent,
  PatientDashboardComponent,
  ClinicalPathComponent,
  RequestMoreInfoComponent,
  ClinicalPathUpdateComponent,
];

@NgModule({
  declarations: [...components],
  exports: [...components],
  imports: [
    CommonModule,
    RouterModule,
    PatientRoutingModule,
    LayoutModule,
    NgbModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule,
    PipeModule,
  ],
})
export class PatientModule {}
