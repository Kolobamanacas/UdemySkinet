import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IOrder } from '../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {
  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getOrdersForUser(): Observable<IOrder[]> {
    return this.httpClient.get<IOrder[]>(this.baseUrl + 'orders');
  }

  getOrderDetailed(id: string): Observable<IOrder> {
    return this.httpClient.get<IOrder>(this.baseUrl + 'orders/' + id);
  }
}
