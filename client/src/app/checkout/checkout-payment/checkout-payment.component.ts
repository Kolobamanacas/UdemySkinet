import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BasketService } from 'src/app/basket/basket.service';
import { IBasket } from 'src/app/shared/models/basket';
import { IOrder, IOrderToCreate } from 'src/app/shared/models/order';
import { CheckoutService } from '../checkout.service';

@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss']
})
export class CheckoutPaymentComponent implements OnInit {
  @Input() checkoutForm: FormGroup = new FormGroup({});

  constructor(
    private basketService: BasketService,
    private checkoutService: CheckoutService,
    private toastrService: ToastrService,
    private router: Router) { }

  ngOnInit(): void {
  }

  submitOrder(): void {
    const basket = this.basketService.getCurrentBasketValue();

    if (!basket) {
      return;
    }

    const orderToCreate = this.getOrderToCreate(basket);
    this.checkoutService.createOrder(orderToCreate).subscribe((order: IOrder) => {
      this.toastrService.success('Order created successfully');
      this.basketService.deleteLocalBasket(basket.id);
      const navigationExtras: NavigationExtras = { state: order };
      this.router.navigate(['checkout/success'], navigationExtras);
    }, (error) => {
      this.toastrService.error(error.message);
      console.log(`Something went wrong during submitting of the order. Here is the error:\n${error}`);
    });
  }

  private getOrderToCreate(basket: IBasket): IOrderToCreate {
    return {
      basketId: basket.id,
      deliveryMethodId: this.checkoutForm.get('deliveryForm')?.get('deliveryMethod')?.value,
      shipToAddress: this.checkoutForm.get('addressForm')?.value
    }
  }
}
