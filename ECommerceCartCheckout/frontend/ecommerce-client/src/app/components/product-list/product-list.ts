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

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadProducts();
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

  addToCart(product: Product): void {
    this.cartService.addToCart(product);
  }
}