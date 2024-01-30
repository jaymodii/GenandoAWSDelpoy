import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { HeaderComponent } from './header/header.component';
import { ProfileComponent } from './profile/profile.component';
import { LayoutRoutingModule } from './layout-routing.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FaqComponent } from './faq/faq.component';
import { CommonLayoutComponent } from './common-layout/common-layout.component';
import { ContactUsComponent } from './contact-us/contact-us.component';
import { TestExplanationComponent } from './test-explanation/test-explanation.component';
import { HttpClientModule } from '@angular/common/http';

const components = [
  HeaderComponent,
  ProfileComponent,
  FaqComponent,
  ContactUsComponent,
  TestExplanationComponent,
  FaqComponent,
  CommonLayoutComponent,
];

@NgModule({
  declarations: [...components],
  imports: [
    LayoutRoutingModule,
    CommonModule,
    SharedModule,
    FormsModule,
    NgbModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
  exports: [...components, CommonModule],
})
export class LayoutModule {}
