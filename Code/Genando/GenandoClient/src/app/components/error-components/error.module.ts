import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { SessionExpiredComponent } from './session-expired/session-expired.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { ErrorRoutingModule } from './error-routing.module';

const components = [
  PageNotFoundComponent,
  SessionExpiredComponent,
  UnauthorizedComponent,
];

@NgModule({
  declarations: [...components],
  imports: [CommonModule, ErrorRoutingModule],
})
export class ErrorModule {}
