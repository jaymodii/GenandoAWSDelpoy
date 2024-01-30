import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProfileComponent } from './profile/profile.component';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';

const routes: Routes = [
  {
    path: RoutingPathConstant.profileName,
    component: ProfileComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LayoutRoutingModule {}
