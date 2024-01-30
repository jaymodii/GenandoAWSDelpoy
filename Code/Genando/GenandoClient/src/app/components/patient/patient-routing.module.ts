import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { PatientDashboardComponent } from './patient-dashboard/patient-dashboard.component';
import { CommonLayoutComponent } from '../layout/common-layout/common-layout.component';
import { ContactUsComponent } from '../layout/contact-us/contact-us.component';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { FaqComponent } from '../layout/faq/faq.component';
import { ApiCallConstant } from 'src/app/constants/api-call/apis';
import { RequestMoreInfoComponent } from './request-more-info/request-more-info.component';
import { ClinicalPathUpdateComponent } from './clinical-path-update/clinical-path-update.component';
import { ProfileComponent } from '../layout/profile/profile.component';
import { TestExplanationComponent } from '../layout/test-explanation/test-explanation.component';

const routes: Routes = [
  {
    path: '',
    component: CommonLayoutComponent,
    children: [
      {
        path: RoutingPathConstant.dashboard,
        component: PatientDashboardComponent,
      },
      {
        path: RoutingPathConstant.clinicalPathUpdateName,
        component: ClinicalPathUpdateComponent,
      },
      {
        path: RoutingPathConstant.patientFaqName,
        component: FaqComponent,
      },
      {
        path: RoutingPathConstant.contactDoctorName,
        component: ContactUsComponent,
      },
      {
        path: RoutingPathConstant.profileName,
        component: ProfileComponent,
      },
      {
        path: RoutingPathConstant.testExplainationName,
        component: TestExplanationComponent,
      },
      {
        path: ApiCallConstant.REQUEST_MORE_INFO,
        component: RequestMoreInfoComponent,
      },
    ],
  },
];

@NgModule({
  declarations: [],
  imports: [CommonModule, RouterModule.forChild(routes)],

  exports: [RouterModule],
})
export class PatientRoutingModule { }
