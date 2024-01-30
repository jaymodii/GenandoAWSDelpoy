import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { MessageService } from '../shared/services/message.service';
import { RoutingPathConstant } from '../constants/routing/routing-path';
import { MessageConstant } from '../constants/message-constant';
import { SystemConstant, UserRole } from '../constants/system-constant';

export function AuthGuard(): CanActivateFn {
  return (next: ActivatedRouteSnapshot) => {
    const authService = inject(AuthService);
    const messageService = inject(MessageService);
    const router = inject(Router);

    const expectedRole = next.data[SystemConstant.expectedRole];
    const isAuthenticated = authService.isAuthenticate();
    const userRole = authService.getUserType();
    if (!isAuthenticated) {
      messageService.error(MessageConstant.loginFirst, MessageConstant.close);
      router.navigate([RoutingPathConstant.loginUrl]);
      return false;
    }

    if (
      userRole === UserRole.doctorRoleId &&
      expectedRole !== UserRole.doctor
    ) {
      messageService.error(MessageConstant.unauthorize, MessageConstant.close);
      router.navigate([RoutingPathConstant.unauthorizeUrl]);
      return false;
    }

    if (
      userRole === UserRole.patientRoleId &&
      expectedRole !== UserRole.patient
    ) {
      messageService.error(MessageConstant.unauthorize, MessageConstant.close);
      router.navigate([RoutingPathConstant.unauthorizeUrl]);
      return false;
    }

    if (userRole === UserRole.labRoleId && expectedRole !== UserRole.lab) {
      messageService.error(MessageConstant.unauthorize, MessageConstant.close);
      router.navigate([RoutingPathConstant.unauthorizeUrl]);
      return false;
    }
    return true;
  };
}
