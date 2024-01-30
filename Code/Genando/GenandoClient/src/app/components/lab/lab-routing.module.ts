import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LabDashboardComponent } from './lab-dashboard/lab-dashboard.component';
import { FaqComponent } from '../layout/faq/faq.component';
import { ContactUsComponent } from '../layout/contact-us/contact-us.component';
import { CommonLayoutComponent } from '../layout/common-layout/common-layout.component';
import { ProfileComponent } from '../layout/profile/profile.component';

import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { UploadResultsComponent } from './upload-results/upload-results.component';
import { TestExplanationComponent } from '../layout/test-explanation/test-explanation.component';

const routes: Routes = [
  {
    path: '',
    component: CommonLayoutComponent,
    children: [
      {
        path: RoutingPathConstant.dashboard,
        component: LabDashboardComponent,
      },
      {
        path: RoutingPathConstant.patientFaqName,
        component: FaqComponent,
      },
      {
        path: RoutingPathConstant.contactUsName,
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
        path: RoutingPathConstant.profileName,
        component: ProfileComponent,
      },
      {
        path: `${RoutingPathConstant.labUploadResultName}/:clinicalProcessTestId`,
        component: UploadResultsComponent,
      },
      {
        path: `${RoutingPathConstant.labUpdateResultName}/:clinicalProcessTestId`,
        component: UploadResultsComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LabRoutingModule { }
