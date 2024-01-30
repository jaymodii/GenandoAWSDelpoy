import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterComponent } from './register/register.component';
import { PatientComponent } from './patient/patient.component';
import { LabComponent } from './lab/lab.component';
import { FaqComponent } from '../layout/faq/faq.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CommonLayoutComponent } from '../layout/common-layout/common-layout.component';
import { ContactUsComponent } from '../layout/contact-us/contact-us.component';
import { ProfileComponent } from '../layout/profile/profile.component';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';
import { ApiCallConstant } from 'src/app/constants/api-call/apis';
import { RequestMoreInfoComponent } from './patient/request-more-info/request-more-info.component';
import { SendResultComponent } from './send-result/send-result.component';
import { ClinicalAnswerComponent } from './clinical-answer/clinical-answer.component';
import { AddClinicalQuestionsComponent } from './add-clinical-questions/add-clinical-questions.component';
import { TestExplanationComponent } from '../layout/test-explanation/test-explanation.component';

const routes: Routes = [
  {
    path: '',
    component: CommonLayoutComponent,
    children: [
      {
        path: RoutingPathConstant.patientListName,
        component: PatientComponent,
      },
      {
        path: RoutingPathConstant.patientRegisterName,
        component: RegisterComponent,
        data: { isPatient: true },
      },
      {
        path: RoutingPathConstant.getPatientName,
        component: RegisterComponent,
        data: { isPatient: true },
      },
      {
        path: RoutingPathConstant.getLabName,
        component: RegisterComponent,
        data: { isPatient: false },
      },
      {
        path: RoutingPathConstant.labName,
        component: LabComponent,
      },
      {
        path: RoutingPathConstant.labRegisterName,
        component: RegisterComponent,
        data: { isPatient: false },
      },
      {
        path: RoutingPathConstant.dashboard,
        component: DashboardComponent,
      },
      {
        path: RoutingPathConstant.patientFaqName,
        component: FaqComponent,
      },
      {
        path: '',
        component: DashboardComponent,
      },
      {
        path: RoutingPathConstant.contactUsName,
        component: ContactUsComponent,
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
        path: `${ApiCallConstant.REQUEST_MORE_INFO}/:id`,
        component: RequestMoreInfoComponent,
      },
      {
        path: `${RoutingPathConstant.addClinicalQuestionsName}/:patientId`,
        component: AddClinicalQuestionsComponent,
      },
      {
        path: RoutingPathConstant.addClinicalQuestions,
        component: AddClinicalQuestionsComponent,
      },
      {
        path: `${RoutingPathConstant.sentResultName}/:clinicalProcessId`,
        component: SendResultComponent,
      },
      {
        path: `${RoutingPathConstant.seeClinicalPathName}/:patientId`,
        component: ClinicalAnswerComponent,
      },
      {
        path: `${RoutingPathConstant.seeRequestedInfoName}/:patientId`,
        component: ClinicalAnswerComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DoctorRoutingModule {}
