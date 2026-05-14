import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CurrencyPipe, NgFor, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Product } from '../../models/product.model';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [NgFor, NgIf, CurrencyPipe, RouterLink],
  templateUrl: './product-list.html',
  styleUrls: ['./product-list.css']
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  errorMessage = '';
  cartQuantities: Map<number, number> = new Map();

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.cartService.cartItems$.subscribe(items => {
      this.cartQuantities = new Map(items.map(i => [i.product.id, i.quantity]));
      this.changeDetectorRef.detectChanges();
    });
  }

  loadProducts(): void {
    this.productService.getProducts().subscribe({
      next: products => {
        this.products = products;
        this.changeDetectorRef.detectChanges();
      },
      error: err => {
        setTimeout(() => {
          this.errorMessage = 'Could not load products.';
          this.changeDetectorRef.detectChanges();
        });
      }
    });
  }

  getCartQuantity(productId: number): number {
    return this.cartQuantities.get(productId) ?? 0;
  }

  addToCart(product: Product): void {
    this.cartService.addToCart(product);
  }
}