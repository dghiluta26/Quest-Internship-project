import { TestBed } from '@angular/core/testing';
import { CartService } from './cart.service';
import { Product } from '../models/product.model';

describe('CartService', () => {
  let service: CartService;

  const product1: Product = {
    id: 1,
    name: 'Blue Top',
    description: 'Test product',
    price: 50,
    imageUrl: 'image.jpg',
    stock: 10
  };

  const product2: Product = {
    id: 2,
    name: 'Sneakers',
    description: 'Test product',
    price: 120,
    imageUrl: 'image.jpg',
    stock: 5
  };

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CartService);
  });

  it('should add a product to the cart', () => {
    service.addToCart(product1);

    const items = service.getCurrentItems();

    expect(items.length).toBe(1);
    expect(items[0].product.id).toBe(1);
    expect(items[0].quantity).toBe(1);
  });

  it('should increase quantity when the same product is added twice', () => {
    service.addToCart(product1);
    service.addToCart(product1);

    const items = service.getCurrentItems();

    expect(items.length).toBe(1);
    expect(items[0].quantity).toBe(2);
  });

  it('should calculate the total price correctly', () => {
    service.addToCart(product1);
    service.addToCart(product1);
    service.addToCart(product2);

    const total = service.getTotal();

    expect(total).toBe(220);
  });

  it('should increase and decrease quantity', () => {
    service.addToCart(product1);

    service.increaseQuantity(product1.id);
    service.decreaseQuantity(product1.id);

    const items = service.getCurrentItems();

    expect(items[0].quantity).toBe(1);
  });

  it('should remove item when quantity decreases to zero', () => {
    service.addToCart(product1);

    service.decreaseQuantity(product1.id);

    const items = service.getCurrentItems();

    expect(items.length).toBe(0);
  });

  it('should remove a product from the cart', () => {
    service.addToCart(product1);
    service.addToCart(product2);

    service.removeFromCart(product1.id);

    const items = service.getCurrentItems();

    expect(items.length).toBe(1);
    expect(items[0].product.id).toBe(2);
  });

  it('should clear the cart', () => {
    service.addToCart(product1);
    service.addToCart(product2);

    service.clearCart();

    const items = service.getCurrentItems();

    expect(items.length).toBe(0);
    expect(service.getTotal()).toBe(0);
  });

  it('should update cart count', () => {
  let latestCount = 0;

  service.cartCount$.subscribe(count => {
    latestCount = count;
  });

  service.addToCart(product1);
  service.addToCart(product1);
  service.addToCart(product2);

  expect(latestCount).toBe(3);
});
});