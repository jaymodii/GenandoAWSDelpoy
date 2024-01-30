import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { RegisterComponent } from './register/register.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { PatientComponent } from './patient/patient.component';
import { DoctorRoutingModule } from './doctor-routing.module';
import { DoctorComponent } from './doctor.component';
import { FilterPatientComponent } from './patient/filter-patient/filter-patient.component';
import { LabComponent } from './lab/lab.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { LayoutModule } from '../layout/layout.module';
import { SendResultComponent } from './send-result/send-result.component';
import { CKEditorModule } from '@ckeditor/ckeditor5-angular';
import { PipeModule } from 'src/app/pipes/pipe.module';
import { RequestMoreInfoComponent } from './patient/request-more-info/request-more-info.component';
import { AddClinicalQuestionsComponent } from './add-clinical-questions/add-clinical-questions.component';
import { ClinicalAnswerComponent } from './clinical-answer/clinical-answer.component';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

const components = [
  RegisterComponent,
  PatientComponent,
  DoctorComponent,
  FilterPatientComponent,
  LabComponent,
  SendResultComponent,
  DashboardComponent,
  RequestMoreInfoComponent,
  ClinicalAnswerComponent,
  AddClinicalQuestionsComponent,
];

@NgModule({
  declarations: [
    components,
    AddClinicalQuestionsComponent,
  ],
  imports: [
    SharedModule,
    CommonModule,
    HttpClientModule,
    DoctorRoutingModule,
    CKEditorModule,
    LayoutModule,
    PipeModule,
    NgbDropdownModule,
  ],
  exports: [],
  providers: [],
})
export class DoctorModule { }
