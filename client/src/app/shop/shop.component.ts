import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { IPagination } from '../shared/models/pagination';
import { IProduct } from '../shared/models/product';
import { IProductBrand } from '../shared/models/productBrand';
import { IProductType } from '../shared/models/productType';
import { ShopParams } from '../shared/models/shopParams';
import { ShopService } from './shop.service';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
  @ViewChild('search', { static: false }) searchTerm: ElementRef<HTMLInputElement> = {} as ElementRef;

  products: IProduct[] = [];
  brands: IProductBrand[] = [];
  types: IProductType[] = [];
  shopParams: ShopParams;
  totalCount = 0;
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low to High', value: 'priceAsc' },
    { name: 'Price: High to Low', value: 'priceDesc' }
  ];

  constructor(private shopServices: ShopService) {
    this.shopParams = shopServices.getShopParams();
  }

  ngOnInit(): void {
    this.getProducts(true);
    this.getBrands();
    this.getTypes();
  }

  getProducts(useCache: boolean = false): void {
    this.shopServices.getProducts(useCache)
      .subscribe((response: IPagination | null) => {
        this.products = response?.data ?? [];
        this.totalCount = response?.count ?? 0;
      }, (error) => {
        console.log(`An error occured during getting products. Here is an error message:\n${error.message}`);
      });
  }

  getBrands(): void {
    this.shopServices.getBrands()
      .subscribe((brands: IProductBrand[]) => {
        this.brands = [{ id: '', name: 'All' }, ...brands];
      }, (error) => {
        console.log(`An error occured during getting product brands. Here is an error message:\n${error.message}`);
      });
  }

  getTypes(): void {
    this.shopServices.getTypes()
      .subscribe((types: IProductType[]) => {
        this.types = [{ id: '', name: 'All' }, ...types];;
      }, (error) => {
        console.log(`An error occured during getting product types. Here is an error message:\n${error.message}`);
      });
  }

  onBrandSelected(brandId: string): void {
    this.shopServices.setShopParams({
      ...this.shopServices.getShopParams(),
      brandId,
      pageNumber: 1
    });
    this.updateLocalShopParams();
    this.getProducts();
  }

  onTypeSelected(typeId: string): void {
    this.shopServices.setShopParams({
      ...this.shopServices.getShopParams(),
      typeId,
      pageNumber: 1
    });
    this.updateLocalShopParams();
    this.getProducts();
  }

  onSortSelected(sort: string): void {
    this.shopServices.setShopParams({
      ...this.shopServices.getShopParams(),
      sort
    });
    this.updateLocalShopParams();
    this.getProducts();
  }

  onPageChanged(pageNumber: number): void {
    const params: ShopParams = this.shopServices.getShopParams();

    if (params.pageNumber === pageNumber) {
      return;
    }

    this.shopServices.setShopParams({ ...params, pageNumber });
    this.updateLocalShopParams();
    this.getProducts(true);
  }

  onSearch(): void {
    this.shopServices.setShopParams({
      ...this.shopServices.getShopParams(),
      search: this.searchTerm.nativeElement.value,
      pageNumber: 1
    });

    this.updateLocalShopParams();
    this.getProducts();
  }

  onReset() {
    this.searchTerm.nativeElement.value = '';
    this.shopParams = new ShopParams();
    this.shopServices.setShopParams(this.shopParams);
    this.updateLocalShopParams();
    this.getProducts();
  }

  private updateLocalShopParams() {
    this.shopParams = this.shopServices.getShopParams();
  }
}
