import { IAddress } from './address';

export interface IOrderToCreate {
  basketId: string,
  deliveryMethodId: string,
  shipToAddress: IAddress
}

export interface IOrder {
  buyerEmail: string,
  orderDate: Date,
  shipToAddress: IAddress,
  deliveryMethod: string,
  shippingPrice: number,
  orderItems: IOrderItem[],
  subtotal: number,
  total: number,
  status: string
}

export interface IOrderItem {
  proudctId: string,
  productName: string,
  pictureUrl: string,
  price: number,
  quantity: number
}
