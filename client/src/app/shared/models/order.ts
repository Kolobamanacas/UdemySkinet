import { IAddress } from './address';

export interface IOrderToCreate {
  basketId: string;
  deliveryMethodId: string;
  shipToAddress: IAddress;
}

export interface IOrderItem {
  productId: string;
  productName: string;
  pictureUrl: string;
  price: number;
  quantity: number;
}

export interface IOrder {
  id: string;
  buyerEmail: string;
  orderDate: Date;
  shipToAddress: IAddress;
  deliveryMethod: string;
  shippingPrice: number;
  orderItems: IOrderItem[];
  subtotal: number;
  total: number;
  status: string;
}
