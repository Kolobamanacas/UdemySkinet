import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IAddress } from '../shared/models/address';
import { IUser } from '../shared/models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource?.asObservable();

  constructor(private httpClient: HttpClient, private router: Router) { }

  loadCurrentUser(token: string | null): Observable<void> {
    if (!token) {
      this.currentUserSource.next(undefined);
      return of();
    }

    let headers = new HttpHeaders();
    headers = headers.set('Authorization', `Bearer ${token}`);

    return this.httpClient.get<IUser>(this.baseUrl + 'account', { headers })
      .pipe<void>(map<IUser, void>((user: IUser) => {
        localStorage.setItem('token', user.token);
        this.currentUserSource.next(user);
      }));
  }

  login(values: { email: string, password: string }): Observable<void> {
    return this.httpClient.post<IUser>(this.baseUrl + 'account/login', values)
      .pipe<void>(map<IUser, void>((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }

        return user;
      }));
  }

  register(values: { email: string, password: string }): Observable<void> {
    return this.httpClient.post<IUser>(this.baseUrl + 'account/register', values)
      .pipe<void>(map<IUser, void>((user) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      }));
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUserSource.next(undefined);
    this.router.navigateByUrl('/');
  }

  doesEmailExist(email: string): Observable<boolean> {
    return this.httpClient.get<boolean>(this.baseUrl + 'account/emailexists?email=' + email);
  }

  getUserAddress(): Observable<IAddress> {
    return this.httpClient.get<IAddress>(this.baseUrl + 'account/address');
  }

  updateUserAddress(address: IAddress): Observable<IAddress> {
    return this.httpClient.put<IAddress>(this.baseUrl + 'account/address', address);
  }
}
