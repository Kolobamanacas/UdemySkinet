import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BasketService } from 'src/app/basket/basket.service';
import { IProduct } from 'src/app/shared/models/product';
import { BreadcrumbService } from 'xng-breadcrumb';
import { ShopService } from '../shop.service';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
  product?: IProduct;
  quantity: number = 1;

  constructor(
    private shopService: ShopService,
    private activatedRoute: ActivatedRoute,
    private breadCrumbService: BreadcrumbService,
    private basketService: BasketService) {
    this.breadCrumbService.set('@productDetails', ' ');
  }

  ngOnInit(): void {
    this.loadProduct();
  }

  addItemToBasket() {
    if (!this.product) {
      return;
    }

    this.basketService.addItemToBasket(this.product, this.quantity);
  }

  incrementQuantity() {
    this.quantity++;
  }

  decrementQuantity() {
    if (this.quantity === 1) {
      return;
    }

    this.quantity--;
  }

  loadProduct() {
    const id = this.activatedRoute.snapshot.paramMap.get('id') ?? '';

    this.shopService.getProduct(id).subscribe((product: IProduct) => {
      this.product = product;
      this.breadCrumbService.set('@productDetails', product.name);
    }, (error) => {
      console.log(`Error occured during getting a product. The message is:\n${error.message}`);
    });
  }
}
