import { Injectable, inject } from '@angular/core';
import {
  HttpInterceptor,
  HttpEvent,
  HttpHandler,
  HttpRequest,
  HTTP_INTERCEPTORS,
  HttpErrorResponse,
} from '@angular/common/http';
import {
  BehaviorSubject,
  Observable,
  catchError,
  filter,
  of,
  switchMap,
  take,
  throwError,
} from 'rxjs';
import { MessageService } from '../shared/services/message.service';
import { AuthService } from '../services/auth.service';
import { MessageConstant } from '../constants/message-constant';
import { SystemConstant } from '../constants/system-constant';
import { StorageHelperConstant } from '../constants/storage-helper/storage-helper';
import { StorageHelperService } from '../services/storage-helper.service';
import { CookieService } from 'ngx-cookie-service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private messageService = inject(MessageService);
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(
    null
  );

  constructor(
    private authService: AuthService,
    private storageHelper: StorageHelperService,
    private cookieService: CookieService
  ) { }

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((httpError: HttpErrorResponse) => {
        const { error } = httpError;
        if (httpError.error instanceof ErrorEvent) {
          this.messageService.error(MessageConstant.somethingWentWrong);
        } else {
          if (error.statusCode === 401) {
            if (
              this.cookieService.get('rememberMe') == 'True' &&
              this.authService.isJwtTokenExpire()
            ) {
              return this.handle401Error(req, next);
            }
            this.authService.logOut();
            this.messageService.error(error.message);
          } else if (error.statusCode === 400) {
            this.messageService.error(error.errors ? error.errors!.join('\n') : error.message);
          } else if (error.statusCode === 403) {
            this.messageService.error(error.errors ? error.errors!.join('\n') : error.message);
          } else if (error.statusCode === 500) {
            this.messageService.error(MessageConstant.internalServerError);
          } else {
            this.messageService.error(MessageConstant.somethingWentWrong);
          }
        }
        return of(error);
      })
    );
  }
  private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);
      const token = this.authService.getRefreshToken();
      if (token) {
        return this.authService.refreshToken().pipe(
          switchMap((res: any) => {
            this.isRefreshing = false;
            this.storageHelper.setAsLocal(
              StorageHelperConstant.authToken,
              res.data.accessToken
            );
            this.storageHelper.setAsLocal(
              StorageHelperConstant.refreshToken,
              res.data.refreshToken
            );
            this.refreshTokenSubject.next(res.data.accessToken);
            return next.handle(
              this.addTokenHeader(request, res.data.accessToken)
            );
          }),
          catchError((err) => {
            this.isRefreshing = false;
            this.authService.logOut();
            return throwError(err);
          })
        );
      }
    }

    return this.refreshTokenSubject.pipe(
      filter((token) => token !== null),
      take(1),
      switchMap((token) => next.handle(this.addTokenHeader(request, token)))
    );
  }

  private addTokenHeader(request: HttpRequest<any>, token: string) {
    return request.clone({
      headers: request.headers.set(SystemConstant.authorization, token),
    });
  }
}

export const ERROR_INTERCEPTOR = {
  multi: true,
  useClass: ErrorInterceptor,
  provide: HTTP_INTERCEPTORS,
};
