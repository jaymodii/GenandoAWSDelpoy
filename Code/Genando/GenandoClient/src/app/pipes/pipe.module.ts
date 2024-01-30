import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormatDatePipe } from './format-date.pipe';
import { PhoneNumberPipe } from './phone-number.pipe';
import { CalculateAgePipe } from './calculate-age.pipe';
import { PatientDashboardStatusPipe } from './patient-dashboard-status.pipe';
import { GenderTitlePipe } from './gender-title.pipe';
import { RemoveHtmlTagsPipe } from './remove-html-tags.pipe';
import { Base64ToImagePipe } from './base64-to-image.pipe';
import { QuestionNumberPipe } from './question-number.pipe';
import { ColumnNamePipe } from './column-name.pipe';
import { DecisionPipe } from './decision.pipe';
import { QuestionTypePipe } from './question-type.pipe';

const pipes = [
  FormatDatePipe,
  PhoneNumberPipe,
  CalculateAgePipe,
  PatientDashboardStatusPipe,
  GenderTitlePipe,
  Base64ToImagePipe,
  QuestionNumberPipe,
  RemoveHtmlTagsPipe,
  ColumnNamePipe,
  DecisionPipe,
  QuestionTypePipe,
];

@NgModule({
  declarations: [...pipes],
  imports: [CommonModule],
  exports: [...pipes],
})
export class PipeModule {}
