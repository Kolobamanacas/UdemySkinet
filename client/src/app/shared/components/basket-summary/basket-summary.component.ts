import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IBasketItem } from '../../models/basket';
import { IOrderItem } from '../../models/order';

@Component({
  selector: 'app-basket-summary',
  templateUrl: './basket-summary.component.html',
  styleUrls: ['./basket-summary.component.scss']
})
export class BasketSummaryComponent implements OnInit {
  @Input() isBasket = true;
  @Input() isReadOnly = false;
  @Input() basketItems: IBasketItem[] = [];
  @Input() orderItems: IOrderItem[] = [];
  @Output() decrement: EventEmitter<IBasketItem> = new EventEmitter<IBasketItem>();
  @Output() increment: EventEmitter<IBasketItem> = new EventEmitter<IBasketItem>();
  @Output() remove: EventEmitter<IBasketItem> = new EventEmitter<IBasketItem>();

  constructor() { }

  ngOnInit(): void {
  }

  decrementItemQuantity(basketItem: IBasketItem) {
    this.decrement.emit(basketItem);
  }

  incrementItemQuantity(basketItem: IBasketItem) {
    this.increment.emit(basketItem);
  }

  removeBasketItem(basketItem: IBasketItem) {
    this.remove.emit(basketItem);
  }
}
