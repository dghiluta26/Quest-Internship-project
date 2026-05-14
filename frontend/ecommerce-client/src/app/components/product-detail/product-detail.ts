import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { CurrencyPipe, NgIf } from '@angular/common';
import { Product } from '../../models/product.model';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, NgIf],
  templateUrl: './product-detail.html',
  styleUrls: ['./product-detail.css']
})
export class ProductDetailComponent implements OnInit {
  product: Product | null = null;
  quantity: number = 1;
  errorMessage = '';
  addedToCart = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private cartService: CartService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.loadProduct(id);
      }
    });
  }

  loadProduct(id: number): void {
    this.productService.getProductById(id).subscribe({
      next: product => {
        this.product = product;
        this.errorMessage = '';
        this.changeDetectorRef.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Product not found.';
        this.changeDetectorRef.detectChanges();
      }
    });
  }

  addToCart(): void {
    if (this.product) {
      for (let i = 0; i < this.quantity; i++) {
        this.cartService.addToCart(this.product);
      }
      this.addedToCart = true;
      setTimeout(() => {
        this.addedToCart = false;
      }, 2000);
    }
  }

  increaseQuantity(): void {
    if (this.product && this.quantity < this.product.stock) {
      this.quantity++;
    }
  }

  decreaseQuantity(): void {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  goBack(): void {
    this.router.navigate(['']);
  }
}
