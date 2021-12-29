import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { setTheme } from 'ngx-bootstrap/utils';
import { IPagination } from 'src/models/pagination';
import { IProduct } from 'src/models/product';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'SkiNet';
  products: IProduct[] = [];

  constructor(private http: HttpClient) {
    setTheme('bs4');
  }

  ngOnInit(): void {
    // TODO: Remove any!
    this.http.get<IPagination>('https://localhost:5001/api/products?pageSize=50').subscribe(
      (response: IPagination) => {
        this.products = response.data;
      }, (error) => {
        console.log(`Can't get data from https://localhost:5001/api/products. Error is: ${error}`);
      });
  }
}
