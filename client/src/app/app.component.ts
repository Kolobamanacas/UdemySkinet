import { Component, OnInit } from '@angular/core';
import { setTheme } from 'ngx-bootstrap/utils';
import { AccountService } from './account/account.service';
import { BasketService } from './basket/basket.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'SkiNet';

  constructor(private basketService: BasketService, private accountService: AccountService) {
    setTheme('bs4');
  }

  ngOnInit(): void {
    this.loadBasket();
    this.loadCurrentUser();
  }

  loadCurrentUser() {
    const token = localStorage.getItem('token');

    this.accountService.loadCurrentUser(token)
      .subscribe(
        () => { },
        (error) => {
          console.log(`Something went wrong during loading current user. Here is an error:\n${error}`);
        });
  }

  loadBasket() {
    const basketId = localStorage.getItem('basket_id');

    if (basketId) {
      this.basketService.getBasket(basketId).subscribe(() => {
        console.log('Basket is initialized.');
      }, (error) => {
        console.log(`Something went wrong during basket initizalization. Here is an error:\n${error}`);
      });
    }
  }
}
