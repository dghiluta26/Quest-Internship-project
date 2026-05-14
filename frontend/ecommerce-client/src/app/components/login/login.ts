import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, NgIf],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class LoginComponent {
  request: LoginRequest = {
    email: '',
    password: ''
  };

  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login(): void {
    this.errorMessage = '';

    this.authService.login(this.request).subscribe({
      next: response => {
        this.authService.saveUser(response);
        this.router.navigate(['/']);
      },
      error: error => {
        this.errorMessage = error.error?.message || 'Login failed.';
      }
    });
  }
}