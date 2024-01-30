import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from './components/button/button.component';
import { InputComponent } from './components/input/input.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TextareaComponent } from './components/textarea/textarea.component';
import { DropDownComponent } from './components/drop-down/drop-down.component';
import { DialogBoxComponent } from './components/dialog-box/dialog-box.component';
import { AccordionComponent } from './components/accordion/accordion.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TableComponent } from './components/table/table.component';
import { PaginationComponent } from './components/pagination/pagination.component';
import { DatepickerComponent } from './components/datepicker/datepicker.component';
import { LinkboxComponent } from './components/linkbox/linkbox.component';
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { AvatarProfileComponent } from './components/avatar-profile/avatar-profile.component';
import { FooterComponent } from '../components/layout/footer/footer.component';
import { LoaderComponent } from './components/loader/loader.component';
import { PipeModule } from '../pipes/pipe.module';
import { FormInputWrapperComponent } from './components/form-input-wrapper/form-input-wrapper.component';
import { InfoGroupComponent } from './components/info-group/info-group.component';
import { RouterModule } from '@angular/router';
import { NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { CheckboxComponent } from './components/checkbox/checkbox.component';
import { RadioButtonComponent } from './components/radio-button/radio-button.component';
import { DirectiveModule } from '../directives/directive.module';

const components = [
  AccordionComponent,
  ButtonComponent,
  InputComponent,
  TextareaComponent,
  DropDownComponent,
  DialogBoxComponent,
  TableComponent,
  PaginationComponent,
  DatepickerComponent,
  LinkboxComponent,
  ConfirmationDialogComponent,
  AvatarProfileComponent,
  FooterComponent,
  LoaderComponent,
  FormInputWrapperComponent,
  InfoGroupComponent,
  CheckboxComponent,
  RadioButtonComponent,
];

@NgModule({
  declarations: [...components],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    PipeModule,
    RouterModule,
    NgbTooltipModule,
    DirectiveModule
  ],
  exports: [...components, FormsModule, ReactiveFormsModule],
})
export class SharedModule {}
