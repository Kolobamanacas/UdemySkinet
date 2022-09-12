import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Observable } from 'rxjs';
import { BasketService } from 'src/app/basket/basket.service';
import { IBasket, IBasketItem } from '../../models/basket';

@Component({
  selector: 'app-basket-summary',
  templateUrl: './basket-summary.component.html',
  styleUrls: ['./basket-summary.component.scss']
})
export class BasketSummaryComponent implements OnInit {
  basket$?: Observable<IBasket | undefined>;
  @Input() isReadOnly = false;
  @Output() decrement: EventEmitter<IBasketItem> = new EventEmitter<IBasketItem>();
  @Output() increment: EventEmitter<IBasketItem> = new EventEmitter<IBasketItem>();
  @Output() remove: EventEmitter<IBasketItem> = new EventEmitter<IBasketItem>();

  constructor(private basketService: BasketService) { }

  ngOnInit(): void {
    this.basket$ = this.basketService.basket$;
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
