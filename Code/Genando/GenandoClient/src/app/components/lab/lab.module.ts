import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LabRoutingModule } from './lab-routing.module';
import { LabComponent } from './lab.component';
import { LabDashboardComponent } from './lab-dashboard/lab-dashboard.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { RouterModule } from '@angular/router';
import { UploadResultsComponent } from './upload-results/upload-results.component';
import { LayoutModule } from '../layout/layout.module';

const components = [
  LabDashboardComponent,
  UploadResultsComponent,
  LabComponent,
];

@NgModule({
  declarations: [...components],
  imports: [
    CommonModule,
    LabRoutingModule,
    SharedModule,
    RouterModule,
    LayoutModule,
  ],
  exports: [...components],
})

export class LabModule { }
