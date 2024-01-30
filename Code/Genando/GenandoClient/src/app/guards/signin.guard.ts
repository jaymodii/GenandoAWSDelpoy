import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../constants/system-constant';
import { RoutingPathConstant } from '../constants/routing/routing-path';

export function SiginGuard(): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);
    
    const isAuthenticated = authService.isAuthenticate();
        if (isAuthenticated) {
            if (authService.getUserType() == UserRole.doctorRoleId) {
                router.navigate([RoutingPathConstant.doctorDashboardUrl]);
              }
              else if (authService.getUserType() == UserRole.patientRoleId) {
                router.navigate([RoutingPathConstant.patientDashboardUrl]);
              }
              else {
                router.navigate([RoutingPathConstant.labDashboardUrl]);
              }
          return false;
        }
        return true;
  };
}
