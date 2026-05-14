import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginRequest, LoginResponse, RegisterRequest } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5223/api/auth';

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  register(request: RegisterRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request);
  }

  saveUser(response: LoginResponse): void {
    if (!this.isBrowser()) {
      return;
    }

    localStorage.setItem('token', response.token);
    localStorage.setItem('userId', response.userId.toString());
    localStorage.setItem('fullName', response.fullName);
    localStorage.setItem('email', response.email);
  }

  getUserId(): number | null {
    if (!this.isBrowser()) {
      return null;
    }

    const userId = localStorage.getItem('userId');

    return userId ? Number(userId) : null;
  }

  getFullName(): string | null {
    if (!this.isBrowser()) {
      return null;
    }

    return localStorage.getItem('fullName');
  }

  isLoggedIn(): boolean {
    if (!this.isBrowser()) {
      return false;
    }

    return !!localStorage.getItem('token');
  }

  logout(): void {
    if (!this.isBrowser()) {
      return;
    }

    localStorage.clear();
  }

  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }
}