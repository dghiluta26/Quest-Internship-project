import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Product } from '../models/product.model';
import { CartItem } from '../models/cart-item.model';

const CART_KEY = 'cart';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly cartItemsSubject: BehaviorSubject<CartItem[]>;
  cartItems$ ;

  private readonly cartCountSubject: BehaviorSubject<number>;
  cartCount$;

  constructor() {
    const saved = this.loadFromStorage();
    this.cartItemsSubject = new BehaviorSubject<CartItem[]>(saved);
    this.cartItems$ = this.cartItemsSubject.asObservable();
    this.cartCountSubject = new BehaviorSubject<number>(
      saved.reduce((t, i) => t + i.quantity, 0)
    );
    this.cartCount$ = this.cartCountSubject.asObservable();
  }

  addToCart(product: Product): void {
    const currentItems = this.cartItemsSubject.value;
    const existingItem = currentItems.find(item => item.product.id === product.id);

    if (existingItem && existingItem.quantity >= product.stock) return;

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
      item.product.id === productId && item.quantity < item.product.stock
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

  getCartQuantity(productId: number): number {
    return this.cartItemsSubject.value.find(i => i.product.id === productId)?.quantity ?? 0;
  }

  getTotal(): number {
    return this.cartItemsSubject.value.reduce(
      (total, item) => total + item.product.price * item.quantity,
      0
    );
  }

  private updateCart(items: CartItem[]): void {
    this.cartItemsSubject.next(items);
    this.saveToStorage(items);
    this.cartCountSubject.next(items.reduce((total, item) => total + item.quantity, 0));
  }

  private loadFromStorage(): CartItem[] {
    try {
      const raw = localStorage.getItem(CART_KEY);
      return raw ? JSON.parse(raw) : [];
    } catch {
      return [];
    }
  }

  private saveToStorage(items: CartItem[]): void {
    try {
      localStorage.setItem(CART_KEY, JSON.stringify(items));
    } catch {}
  }
}
