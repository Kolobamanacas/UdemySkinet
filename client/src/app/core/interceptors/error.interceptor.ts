import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { catchError, delay } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private toastr: ToastrService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((errorResponse) => {
        if (errorResponse) {
          if (errorResponse.status === 400) {
            if (errorResponse.error.errors) {
              throw errorResponse.error;
            } else {
              this.toastr.error(errorResponse.error.message, errorResponse.error.statusCode);
            }
          }

          if (errorResponse.status === 401) {
            this.toastr.error(errorResponse.error.message, errorResponse.error.statusCode);
          }

          if (errorResponse.status === 404) {
            this.router.navigateByUrl('/not-found');
          }

          if (errorResponse.status === 500) {
            const navigationExtras: NavigationExtras = { state: { error: errorResponse.error } };

            this.router.navigateByUrl('/server-error', navigationExtras);
          }
        }

        return throwError(errorResponse);
      })
    );
  }
}
