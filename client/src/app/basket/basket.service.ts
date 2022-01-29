import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Basket, IBasket, IBasketItem, IBasketTotals } from '../shared/models/basket';
import { IProduct } from '../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  private basketSource = new BehaviorSubject<IBasket | undefined>(undefined);
  private basketTotalSource = new BehaviorSubject<IBasketTotals | undefined>(undefined);
  basket$ = this.basketSource.asObservable();
  basketTotal$ = this.basketTotalSource.asObservable();
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getBasket(id: string): Observable<void> {
    return this.http.get<IBasket>(this.baseUrl + 'basket?id=' + id)
      .pipe(map((basket: IBasket) => {
        this.basketSource.next(basket);
        this.calculateTotals();
      }));
  }

  setBasket(basket: IBasket): Subscription {
    return this.http.post<IBasket>(this.baseUrl + 'basket', basket)
      .subscribe((responseBasket: IBasket) => {
        this.basketSource.next(responseBasket);
        this.calculateTotals();
      }, (error) => {
        console.log(`Something went wrong during setting a basket. Here is an error:\n${error}`);
      });
  }

  getCurrentBasketValue(): IBasket | undefined {
    return this.basketSource.value;
  }

  addItemToBasket(product?: IProduct, quantity = 1): void {
    if (!product) {
      return;
    }

    const itemToAdd: IBasketItem = this.mapProductItemToBasketItem(product, quantity);
    const basket = this.getCurrentBasketValue() ?? this.createBasket();
    basket.items = this.addOrUpdateItem(basket.items, itemToAdd, quantity);
    this.setBasket(basket);
  }

  incrementItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();

    if (!basket) {
      return;
    }

    const foundItemIndex = basket.items.findIndex((basketItem: IBasketItem) => basketItem.id = item.id);
    basket.items[foundItemIndex].quantity++;
    this.setBasket(basket);
  }

  decrementItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();

    if (!basket) {
      return;
    }

    const foundItemIndex = basket.items.findIndex((basketItem: IBasketItem) => basketItem.id = item.id);

    if (basket.items[foundItemIndex].quantity > 1) {
      basket.items[foundItemIndex].quantity--;
      this.setBasket(basket);
    } else {
      this.removeItemFromBasket(item);
    }
  }

  removeItemFromBasket(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();

    if (!basket) {
      return;
    }

    basket.items = basket.items.filter((basketItem: IBasketItem) => basketItem.id !== item.id);

    if (basket.items.length > 0) {
      this.setBasket(basket);
    } else {
      this.deleteBasket(basket);
    }
  }

  deleteBasket(basket: IBasket) {
    return this.http.delete(this.baseUrl + 'basketId?id=' + basket.id).subscribe(() => {
      this.basketSource.next(undefined);
      this.basketTotalSource.next(undefined);
      localStorage.removeItem('basket_id');
    }, (error) => {
      console.log(`Something went wrong during basket deletion. Here is an error:\n${error}`);
    });
  }

  private mapProductItemToBasketItem(product: IProduct, quantity: number): IBasketItem {
    return {
      id: product.id,
      productName: product.name,
      price: product.price,
      pictureUrl: product.pictureUrl,
      brand: product.productBrandName,
      type: product.productTypeName,
      quantity,
    }
  }

  private createBasket(): IBasket {
    const basket = new Basket();
    localStorage.setItem('basket_id', basket.id);
    return basket;
  }

  private addOrUpdateItem(items: IBasketItem[], itemToAdd: IBasketItem, quantity: number): IBasketItem[] {
    const index = items.findIndex((item: IBasketItem) => item.id === itemToAdd.id);

    if (index === -1) {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    } else {
      items[index].quantity += quantity;
    }

    return items;
  }

  private calculateTotals() {
    const basket = this.getCurrentBasketValue();
    const shipping = 0;
    const subtotal: number = basket?.items.reduce((currentSum: number, currentItem: IBasketItem) => {
      return (currentItem.price * currentItem.quantity) + currentSum;
    }, 0) ?? 0;
    const total = subtotal + shipping;
    this.basketTotalSource.next({ shipping, subtotal, total });
  }
}
