import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IOrder } from 'src/app/shared/models/order';

@Component({
  selector: 'app-checkout-success',
  templateUrl: './checkout-success.component.html',
  styleUrls: ['./checkout-success.component.scss']
})
export class CheckoutSuccessComponent implements OnInit {
  order?: IOrder;

  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();

    if (!navigation || !navigation.extras || !navigation.extras.state) {
      return;
    }

    this.order = navigation.extras.state as IOrder;
  }

  ngOnInit(): void {
  }

}
