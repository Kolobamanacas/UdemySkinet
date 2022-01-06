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
  @ViewChild('search', { static: true }) searchTerm: ElementRef<HTMLInputElement> = {} as ElementRef;

  products: IProduct[] = [];
  brands: IProductBrand[] = [];
  types: IProductType[] = [];
  shopParams = new ShopParams();
  totalCount = 0;
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low to High', value: 'priceAsc' },
    { name: 'Price: High to Low', value: 'priceDesc' }
  ];

  constructor(private shopServices: ShopService) { }

  ngOnInit(): void {
    this.getProducts();
    this.getBrands();
    this.getTypes();
  }

  getProducts(): void {
    this.shopServices.getProducts(this.shopParams)
      .subscribe((response: IPagination | null) => {
        this.products = response?.data ?? [];
        this.shopParams.pageNumber = response?.pageIndex ?? ShopParams.PageNumberDefault;
        this.shopParams.pageSize = response?.pageSize ?? ShopParams.PageSizeDefault;
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
    this.shopParams.brandId = brandId;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onTypeSelected(typeId: string): void {
    this.shopParams.typeId = typeId;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onSortSelected(sort: string): void {
    this.shopParams.sort = sort;
    this.getProducts();
  }

  onPageChanged(pageNumber: number): void {
    if (this.shopParams.pageNumber === pageNumber) {
      return;
    }

    this.shopParams.pageNumber = pageNumber;
    this.getProducts();
  }

  onSearch(): void {
    this.shopParams.search = this.searchTerm.nativeElement.value;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onReset() {
    this.searchTerm.nativeElement.value = '';
    this.shopParams = new ShopParams();
    this.getProducts();
  }
}
