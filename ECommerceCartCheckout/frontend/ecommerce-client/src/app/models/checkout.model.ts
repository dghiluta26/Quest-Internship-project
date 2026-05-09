export interface CheckoutItemRequest {
  productId: number;
  quantity: number;
}

export interface CheckoutRequest {
  userId: number;
  shippingAddress: string;
  items: CheckoutItemRequest[];
}

export interface CheckoutResponse {
  orderId: number;
  totalPrice: number;
  message: string;
}