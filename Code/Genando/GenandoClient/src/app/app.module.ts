import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import {
  BrowserModule,
  HAMMER_GESTURE_CONFIG,
  HammerModule,
} from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { JwtModule } from '@auth0/angular-jwt';
import { NgbActiveModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrModule } from 'ngx-toastr';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ErrorModule } from './components/error-components/error.module';
import { LabModule } from './components/lab/lab.module';
import { API_INTERCEPTOR } from './interceptors/api-response.interceptor';
import { ERROR_INTERCEPTOR } from './interceptors/error.interceptor';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ApiCallConstant } from './constants/api-call/apis';
import { AuthService } from './services/auth.service';
import { SharedModule } from './shared/shared.module';
import { PatientModule } from './components/patient/patient.module';
import { LayoutModule } from './components/layout/layout.module';
import { StorageHelperConstant } from './constants/storage-helper/storage-helper';
import { DoctorModule } from './components/doctor/doctor.module';
import { CookieService } from 'ngx-cookie-service';
import { StoreModule } from '@ngrx/store';
import { FilterReducer } from './store-ngrx/filter.reducer';
import { MyHammerConfig } from './configuration/hammer-config';

@NgModule({
  declarations: [AppComponent],
  imports: [
    AppRoutingModule,
    LayoutModule,
    PatientModule,
    LabModule,
    DoctorModule,
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    SharedModule,
    HttpClientModule,
    NgbModule,
    HammerModule,
    ToastrModule.forRoot(),
    JwtModule.forRoot({
      config: {
        tokenGetter: () => {
          return localStorage.getItem(StorageHelperConstant.authToken);
        },
        allowedDomains: [ApiCallConstant.BASE_URL],
        disallowedRoutes: [ApiCallConstant.LOGIN_URL],
      },
    }),
    ErrorModule,
    StoreModule.forRoot({
      filter:FilterReducer
    })
  ],
  providers: [
    NgbActiveModal,
    AuthService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    ERROR_INTERCEPTOR,
    API_INTERCEPTOR,
    CookieService,
    {
      provide: HAMMER_GESTURE_CONFIG,
      useClass: MyHammerConfig,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
