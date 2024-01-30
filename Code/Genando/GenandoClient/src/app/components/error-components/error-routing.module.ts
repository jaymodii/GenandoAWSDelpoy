import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { SessionExpiredComponent } from './session-expired/session-expired.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';

const routes: Routes = [
  { path: RoutingPathConstant.sessionExpire, component: SessionExpiredComponent },
  { path: RoutingPathConstant.unauthorize, component: UnauthorizedComponent },
  { path: RoutingPathConstant.notFound, component: PageNotFoundComponent },
  { path: '**', component: PageNotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class ErrorRoutingModule {}
