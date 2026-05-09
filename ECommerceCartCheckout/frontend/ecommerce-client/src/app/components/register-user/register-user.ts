import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { RegisterRequest } from '../../models/auth.model';

@Component({
  selector: 'app-register-user',
  standalone: true,
  imports: [FormsModule, NgIf],
  templateUrl: './register-user.html',
  styleUrls: ['./register-user.css']
})
export class RegisterUserComponent {
  request: RegisterRequest = {
    fullName: '',
    email: '',
    password: ''
  };

  confirmPassword = '';
  errorMessage = '';
  successMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  register(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.request.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    this.authService.register(this.request).subscribe({
      next: response => {
        this.successMessage = response.message;

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 800);
      },
      error: error => {
        this.errorMessage = error.error?.message || 'Registration failed.';
      }
    });
  }
}