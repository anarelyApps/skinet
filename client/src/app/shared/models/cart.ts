import {nanoid} from 'nanoid';

export type CartType = {
    id:string;
    items:CartItem[];
    deliveryMethodId?:number;
    paymentIntentId?:string;
    clientSecret?: string;
}

export type CartItem = {
    productId:number;
    productName:string;
    price:number;
    quantity:number;
    pictureUrl:string;
    brand:string;
    type:string;
}

export type Coupon = {
    couponId:string;
    amountOff:number;
    percentageOff:number;
    name:string;
    promotionCode:string;    
}

export class Cart implements CartType {
    id=nanoid();
    items: CartItem[]=[];
    deliveryMethodId?:number;
    paymentIntentId?:string;
    clientSecret?: string;
    coupon?:Coupon;
}