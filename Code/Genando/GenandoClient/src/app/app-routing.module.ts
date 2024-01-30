import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DoctorModule } from './components/doctor/doctor.module';
import { PatientModule } from './components/patient/patient.module';
import { AuthGuard } from './guards/auth.guard';
import { LabModule } from './components/lab/lab.module';
import { UserRole } from './constants/system-constant';
import { AuthenticationModule } from './components/authentication/authentication.module';
import { SiginGuard } from './guards/signin.guard';

const routes: Routes = [
  {
    path: '',
    loadChildren: () => AuthenticationModule,
    canActivate: [SiginGuard()],
  },
  {
    path: UserRole.patient,
    loadChildren: () => PatientModule,
    canActivate: [AuthGuard()],
    data: { expectedRole: UserRole.patient },
  },
  {
    path: UserRole.doctor,
    loadChildren: () => DoctorModule,
    canActivate: [AuthGuard()],
    data: { expectedRole: UserRole.doctor },
  },
  {
    path: UserRole.lab,
    loadChildren: () => LabModule,
    canActivate: [AuthGuard()],
    data: { expectedRole: UserRole.lab },
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
