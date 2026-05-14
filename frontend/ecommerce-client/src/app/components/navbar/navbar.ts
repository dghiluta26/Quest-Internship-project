import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgIf } from '@angular/common';
import { Observable } from 'rxjs';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, AsyncPipe, NgIf],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class NavbarComponent {
  cartCount$: Observable<number>;

  constructor(
    private cartService: CartService,
    public authService: AuthService
  ) {
    this.cartCount$ = this.cartService.cartCount$;
  }

  logout(): void {
    this.authService.logout();
    window.location.href = '/';
  }
}