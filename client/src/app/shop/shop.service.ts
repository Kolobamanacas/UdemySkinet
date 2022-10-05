import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IPagination, Pagination } from '../shared/models/pagination';
import { IProduct } from '../shared/models/product';
import { IProductBrand } from '../shared/models/productBrand';
import { IProductType } from '../shared/models/productType';
import { ShopParams } from '../shared/models/shopParams';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = environment.apiUrl;
  products: IProduct[] = [];
  productBrands: IProductBrand[] = [];
  productTypes: IProductType[] = [];
  pagination: Pagination = new Pagination();
  shopParams: ShopParams = new ShopParams();
  productsCache: Map<string, IProduct[]> = new Map();

  constructor(private httpClient: HttpClient) { }

  getProducts(useCache: boolean): Observable<IPagination | null> {
    if (!useCache) {
      this.productsCache = new Map();
    }

    const shopParamsKey: string = Object.values(this.shopParams).join('-');

    if (useCache
      && this.productsCache.size > 0
      && this.productsCache.has(shopParamsKey)) {
        this.pagination.data = this.productsCache.get(shopParamsKey) ?? [];
        return of(this.pagination);
    }

    let queryParams = new HttpParams();

    if (this.shopParams.brandId !== '') {
      queryParams = queryParams.append('brandId', this.shopParams.brandId);
    }

    if (this.shopParams.typeId !== '') {
      queryParams = queryParams.append('typeId', this.shopParams.typeId);
    }

    if (this.shopParams.search !== '') {
      queryParams = queryParams.append('search', this.shopParams.search);
    }

    queryParams = queryParams.append('sort', this.shopParams.sort);
    queryParams = queryParams.append('pageIndex', this.shopParams.pageNumber.toString());
    queryParams = queryParams.append('pageIndex', this.shopParams.pageSize.toString());

    return this.httpClient
      .get<IPagination>(this.baseUrl + 'products', { observe: 'response', params: queryParams })
      .pipe(
        map((response: HttpResponse<IPagination>) => {
          if (response.body) {
            const paramsKey: string = Object.values(this.shopParams).join('-');
            this.productsCache.set(paramsKey, response.body.data);
            this.pagination = response.body;
          }

          return this.pagination;
        })
      );
  }

  getProduct(id: string): Observable<IProduct> {
    let product: IProduct | undefined;

    this.productsCache.forEach((products: IProduct[]) => {
      product = products.find((productLocal: IProduct) => productLocal.id === id);
    });

    if (product) {
      return of(product);
    }

    return this.httpClient.get<IProduct>(this.baseUrl + 'products/' + id);
  }

  getBrands(): Observable<IProductBrand[]> {
    if (this.productBrands.length > 0) {
      return of(this.productBrands);
    }

    return this.httpClient.get<IProductBrand[]>(this.baseUrl + 'products/brands')
      .pipe(map((brands: IProductBrand[]) => {
        if (brands.length > 0) {
          this.productBrands = brands;
        }

        return brands;
      }));
  }

  getTypes(): Observable<IProductType[]> {
    if (this.productTypes.length > 0) {
      return of(this.productTypes);
    }

    return this.httpClient.get<IProductType[]>(this.baseUrl + 'products/types')
      .pipe(map((types: IProductType[]) => {
        if (types.length > 0) {
          this.productTypes = types;
        }

        return types;
      }));
  }

  getShopParams(): ShopParams {
    return this.shopParams;
  }

  setShopParams(params: ShopParams): void {
    this.shopParams = params;
  }
}
