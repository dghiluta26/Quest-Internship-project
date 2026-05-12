import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CurrencyPipe, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CartItem } from '../../models/cart-item.model';
import { CheckoutRequest } from '../../models/checkout.model';
import { CartService } from '../../services/cart.service';
import { CheckoutService } from '../../services/checkout.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule, CurrencyPipe, RouterLink],
  templateUrl: './checkout.html',
  styleUrls: ['./checkout.css']
})
export class CheckoutComponent implements OnInit {
  cartItems: CartItem[] = [];
  frontendTotal = 0;

  shippingAddress = '';

  errorMessage = '';
  successMessage = '';
  isSubmitting = false;

  orderId: number | null = null;
  backendTotal: number | null = null;

  constructor(
    private cartService: CartService,
    private checkoutService: CheckoutService,
    private authService: AuthService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.cartService.cartItems$.subscribe(items => {
      this.cartItems = items;
      this.frontendTotal = this.cartService.getTotal();
    });
  }

  placeOrder(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isSubmitting) {
      return;
    }

    const userId = this.authService.getUserId();

    if (!userId) {
      this.errorMessage = 'You must be logged in to place an order.';
      return;
    }

    if (this.cartItems.length === 0) {
      this.errorMessage = 'Your cart is empty.';
      return;
    }

    if (!this.shippingAddress.trim()) {
      this.errorMessage = 'Shipping address is required.';
      return;
    }

    const request: CheckoutRequest = {
      userId,
      shippingAddress: this.shippingAddress,
      items: this.cartItems.map(item => ({
        productId: item.product.id,
        quantity: item.quantity
      }))
    };

    this.isSubmitting = true;

    this.checkoutService.placeOrder(request).subscribe({
      next: response => {
        this.orderId = response.orderId;
        this.backendTotal = response.totalPrice;
        this.successMessage = response.message;

        this.cartService.clearCart();
        this.isSubmitting = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = error.error?.message || this.getCheckoutErrorMessage(error.status);
        this.isSubmitting = false;
        this.changeDetectorRef.detectChanges();
      }
    });
  }

  private getCheckoutErrorMessage(status?: number): string {
    if (status === 0) {
      return 'Could not reach the API. Make sure the backend is running.';
    }

    if (status === 401) {
      return 'Your session expired. Please log in again.';
    }

    return 'Checkout failed.';
  }
}
