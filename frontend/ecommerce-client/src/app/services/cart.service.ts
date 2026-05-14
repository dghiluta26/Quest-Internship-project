import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Product } from '../models/product.model';
import { CartItem } from '../models/cart-item.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly cartItemsSubject = new BehaviorSubject<CartItem[]>([]);
  cartItems$ = this.cartItemsSubject.asObservable();

  private readonly cartCountSubject = new BehaviorSubject<number>(0);
  cartCount$ = this.cartCountSubject.asObservable();

  addToCart(product: Product): void {
    const currentItems = this.cartItemsSubject.value;

    const existingItem = currentItems.find(item => item.product.id === product.id);

    let updatedItems: CartItem[];

    if (existingItem) {
      updatedItems = currentItems.map(item =>
        item.product.id === product.id
          ? { ...item, quantity: item.quantity + 1 }
          : item
      );
    } else {
      updatedItems = [...currentItems, { product, quantity: 1 }];
    }

    this.updateCart(updatedItems);
  }

  increaseQuantity(productId: number): void {
    const updatedItems = this.cartItemsSubject.value.map(item =>
      item.product.id === productId
        ? { ...item, quantity: item.quantity + 1 }
        : item
    );

    this.updateCart(updatedItems);
  }

  decreaseQuantity(productId: number): void {
    const updatedItems = this.cartItemsSubject.value
      .map(item =>
        item.product.id === productId
          ? { ...item, quantity: item.quantity - 1 }
          : item
      )
      .filter(item => item.quantity > 0);

    this.updateCart(updatedItems);
  }

  removeFromCart(productId: number): void {
    const updatedItems = this.cartItemsSubject.value.filter(
      item => item.product.id !== productId
    );

    this.updateCart(updatedItems);
  }

  clearCart(): void {
    this.updateCart([]);
  }

  getCurrentItems(): CartItem[] {
    return this.cartItemsSubject.value;
  }

  getTotal(): number {
    return this.cartItemsSubject.value.reduce(
      (total, item) => total + item.product.price * item.quantity,
      0
    );
  }

  private updateCart(items: CartItem[]): void {
    this.cartItemsSubject.next(items);

    const count = items.reduce((total, item) => total + item.quantity, 0);

    this.cartCountSubject.next(count);
  }
}