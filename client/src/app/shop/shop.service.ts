import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { IPagination } from '../shared/models/pagination';
import { IProduct } from '../shared/models/product';
import { IProductBrand } from '../shared/models/productBrand';
import { IProductType } from '../shared/models/productType';
import { ShopParams } from '../shared/models/shopParams';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = 'https://localhost:5001/api/';

  constructor(private http: HttpClient) { }

  getProducts(shopParams: ShopParams): Observable<IPagination | null> {
    let queryParams = new HttpParams();

    if (shopParams.brandId !== '') {
      queryParams = queryParams.append('brandId', shopParams.brandId);
    }

    if (shopParams.typeId !== '') {
      queryParams = queryParams.append('typeId', shopParams.typeId);
    }

    if (shopParams.search !== '') {
      queryParams = queryParams.append('search', shopParams.search);
    }

    queryParams = queryParams.append('sort', shopParams.sort);
    queryParams = queryParams.append('pageIndex', shopParams.pageNumber.toString());
    queryParams = queryParams.append('pageIndex', shopParams.pageSize.toString());

    return this.http
      .get<IPagination>(this.baseUrl + 'products', { observe: 'response', params: queryParams })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }

  getProduct(id: string): Observable<IProduct> {
    return this.http.get<IProduct>(this.baseUrl + 'products/' + id);
  }

  getBrands(): Observable<IProductBrand[]> {
    return this.http.get<IProductBrand[]>(this.baseUrl + 'products/brands');
  }

  getTypes(): Observable<IProductType[]> {
    return this.http.get<IProductType[]>(this.baseUrl + 'products/types');
  }
}
