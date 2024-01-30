import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { PatientService } from 'src/app/services/patient.service';
import { DoctorDetails } from 'src/app/models/patient/doctor-details.interface';
import { IResponse } from 'src/app/models/shared/response';
import { UserRole } from 'src/app/constants/system-constant';

@Component({
  selector: 'app-contact-us',
  templateUrl: './contact-us.component.html',
  styleUrls: ['./contact-us.component.scss'],
})
export class ContactUsComponent implements OnInit {
  userRole: string | null = '';

  contactEmail: string = 'info@genando.com';

  contactNumber: string = '+39 055 674100';

  contactName: string = '';

  constructor(
    private authService: AuthService,
    private patientService: PatientService
  ) {}
  ngOnInit(): void {
    this.userRole = this.authService.getUserType();
    if (this.userRole === UserRole.patientRoleId) {
      this.patientService.getDoctorDetails().subscribe({
        next: (response: IResponse<DoctorDetails>) => {
          this.contactName = response.data.name;
          this.contactEmail = response.data.email;
          this.contactNumber = response.data.phoneNumber;
        },
      });
    }
  }
}
