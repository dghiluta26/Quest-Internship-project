import { isPlatformBrowser } from '@angular/common';
import { HttpInterceptorFn } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const platformId = inject(PLATFORM_ID);

  if (!isPlatformBrowser(platformId)) {
    return next(request);
  }

  const token = localStorage.getItem('token');

  if (!token) {
    return next(request);
  }

  const authenticatedRequest = request.clone({
    headers: request.headers.set('Authorization', `Bearer ${token}`)
  });

  return next(authenticatedRequest);
};