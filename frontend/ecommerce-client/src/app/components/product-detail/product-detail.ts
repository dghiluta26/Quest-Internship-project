import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from '../../models/product.model';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './product-detail.html',
  styleUrls: ['./product-detail.css']
})
export class ProductDetailComponent implements OnInit {
  product: Product | null = null;
  quantity: number = 1;
  cartQuantity: number = 0;
  errorMessage = '';
  addedToCart = false;

  get remainingStock(): number {
    return Math.max(0, (this.product?.stock ?? 0) - this.cartQuantity);
  }

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
      if (id) this.loadProduct(id);
    });

    this.cartService.cartItems$.subscribe(items => {
      if (!this.product) return;
      const item = items.find(i => i.product.id === this.product!.id);
      this.cartQuantity = item?.quantity ?? 0;
      if (this.quantity > this.remainingStock) {
        this.quantity = Math.max(1, this.remainingStock);
      }
      this.changeDetectorRef.detectChanges();
    });
  }

  loadProduct(id: number): void {
    this.productService.getProductById(id).subscribe({
      next: product => {
        this.product = product;
        this.cartQuantity = this.cartService.getCartQuantity(product.id);
        this.quantity = 1;
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
    if (this.product && this.remainingStock > 0) {
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
    if (this.quantity < this.remainingStock) {
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
