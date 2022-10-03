import { AfterViewInit, Component, ElementRef, Input, OnDestroy, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NavigationExtras, Router } from '@angular/router';
import { loadStripe, Stripe, StripeCardCvcElement, StripeCardExpiryElement, StripeCardNumberElement, StripeElements } from '@stripe/stripe-js';
import { ToastrService } from 'ngx-toastr';
import { from } from 'rxjs';
import { BasketService } from 'src/app/basket/basket.service';
import { IBasket } from 'src/app/shared/models/basket';
import { IChangeEvent } from 'src/app/shared/models/event';
import { IOrderToCreate } from 'src/app/shared/models/order';
import { environment } from 'src/environments/environment';
import { CheckoutService } from '../checkout.service';

@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss']
})
export class CheckoutPaymentComponent implements AfterViewInit, OnDestroy {
  @Input() checkoutForm: FormGroup = new FormGroup({});
  @ViewChild('cardNumber', { static: true }) cardNumberElement?: ElementRef;
  @ViewChild('cardExpiry', { static: true }) cardExpiryElement?: ElementRef;
  @ViewChild('cardCvc', { static: true }) cardCvcElement?: ElementRef;

  stripe?: Stripe;
  cardNumber?: StripeCardNumberElement;
  cardExpiry?: StripeCardExpiryElement;
  cardCvc?: StripeCardCvcElement;
  cardError?: string;
  isLoading: boolean = false;
  isNumberValid: boolean = false;
  isExpiryValid: boolean = false;
  isCvcValid: boolean = false;

  cardHandler = this.onChange.bind(this);

  constructor(
    private basketService: BasketService,
    private checkoutService: CheckoutService,
    private toastrService: ToastrService,
    private router: Router) { }

  ngAfterViewInit(): void {
    from(loadStripe(environment.stripePublicKey))
      .subscribe((leadedStripe: Stripe | null) => {
        if (!leadedStripe) {
          return;
        }

        this.stripe = leadedStripe;
        const elements: StripeElements = this.stripe.elements();
        this.cardNumber = elements.create('cardNumber');
        this.cardNumber.mount(this.cardNumberElement?.nativeElement);
        this.cardNumber.on('change', this.cardHandler);

        this.cardExpiry = elements.create('cardExpiry');
        this.cardExpiry.mount(this.cardExpiryElement?.nativeElement);
        this.cardExpiry.on('change', this.cardHandler);

        this.cardCvc = elements.create('cardCvc');
        this.cardCvc.mount(this.cardCvcElement?.nativeElement);
        this.cardCvc.on('change', this.cardHandler);
      });
  }

  ngOnDestroy(): void {
    this.cardNumber?.destroy();
    this.cardExpiry?.destroy();
    this.cardCvc?.destroy();
  }

  onChange(event: IChangeEvent): void {
    if (!event?.error) {
      this.cardError = undefined;
    } else {
      this.cardError = event.error.message;
    }

    switch (event.elementType) {
      case 'cardNumber':
        this.isNumberValid = event.complete;
        break;
      case 'cardExpiry':
        this.isExpiryValid = event.complete;
        break;
      case 'cardCvc':
        this.isCvcValid = event.complete;
        break;
      default:
        break;
    }
  }

  async submitOrder(): Promise<void> {
    this.isLoading = true;
    const basket = this.basketService.getCurrentBasketValue();

    if (!basket) {
      this.isLoading = false;
      return;
    }

    let createdOrder;
    let paymentResult;

    try {
      createdOrder = await this.createOrder(basket);
    } catch (error) {
      this.isLoading = false;
      console.log('Something went wrong during creating the order. Here is the error:');
      console.log(error);
    }

    try {
      paymentResult = await this.confirmPaymentWithStripe(basket);
    } catch (error) {
      this.isLoading = false;
      console.log('Something went wrong during payment condirmation. Here is the error:');
      console.log(error);
    }


    if (!paymentResult) {
      this.isLoading = false;
      this.toastrService.error("Can't get payment result");
      return;
    }

    if (!paymentResult.paymentIntent) {
      this.isLoading = false;
      this.toastrService.error(paymentResult.error.message);
      return;
    }

    this.basketService.deleteBasket(basket);
    const navigationExtras: NavigationExtras = { state: createdOrder };
    this.router.navigate(['checkout/success'], navigationExtras);
    this.isLoading = false;
  }

  isFormInvalid(): boolean {
    return this.isLoading
      || (this.checkoutForm.get('paymentForm')?.invalid ?? true)
      || !this.isNumberValid
      || !this.isExpiryValid
      || !this.isCvcValid;
  }

  private getOrderToCreate(basket: IBasket): IOrderToCreate {
    return {
      basketId: basket.id,
      deliveryMethodId: this.checkoutForm.get('deliveryForm')?.get('deliveryMethod')?.value,
      shipToAddress: this.checkoutForm.get('addressForm')?.value
    }
  }

  private async createOrder(basket: IBasket) {
    const orderToCreate = this.getOrderToCreate(basket);
    return this.checkoutService.createOrder(orderToCreate).toPromise();
  }

  private async confirmPaymentWithStripe(basket: IBasket) {
    if (!basket.clientSecret || !this.cardNumber) {
      return;
    }

    return this.stripe?.confirmCardPayment(basket.clientSecret, {
      payment_method: {
        card: this.cardNumber,
        billing_details: {
          name: this.checkoutForm.get('paymentForm')?.get('cardHolderName')?.value
        }
      }
    });
  }
}
