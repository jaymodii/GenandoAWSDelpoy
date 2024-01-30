import { Component } from '@angular/core';
import { RoutingPathConstant } from 'src/app/constants/routing/routing-path';

@Component({
  selector: 'app-session-expired',
  templateUrl: './session-expired.component.html',
  styleUrls: ['./session-expired.component.scss']
})
export class SessionExpiredComponent {
  loginPath:string = RoutingPathConstant.loginUrl;
}
