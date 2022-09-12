import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IDeliveryMethod } from '../shared/models/deliveryMethod';
import { IOrder, IOrderToCreate } from '../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getDeliveryMethods(): Observable<IDeliveryMethod[]> {
    return this.httpClient.get<IDeliveryMethod[]>(
      this.baseUrl + 'orders/deliveryMethods')
      .pipe(
        map((deliveryMethods: IDeliveryMethod[]) => {
          return deliveryMethods.sort((a, b) => b.price - a.price);
        })
      );
  }

  createOrder(order: IOrderToCreate): Observable<IOrder> {
    return this.httpClient.post<IOrder>(this.baseUrl + 'orders', order);
  }
}
