import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { authInterceptorGuard } from './auth.interceptor.guard';

describe('authInterceptorGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => authInterceptorGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
