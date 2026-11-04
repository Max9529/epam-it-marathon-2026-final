import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_BASE_URL } from '../../shared/config/api-base-url.token';

@Injectable({ providedIn: 'root' })
export class AccountRecoveryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  requestRecovery(email: string) {
    return this.http.post<{ message: string }>(`${this.baseUrl}/password-recovery`, { email });
  }
  confirmRecovery(email: string, code: string, password: string) {
    return this.http.post<{ message: string }>(`${this.baseUrl}/password-recovery/confirm`, { email, code, password });
  }
  requestVerification(accessToken: string) {
    return this.http.post<{ message: string }>(`${this.baseUrl}/email-verification`, { accessToken });
  }
  confirmVerification(accessToken: string, code: string) {
    return this.http.post<{ message: string }>(`${this.baseUrl}/email-verification/confirm`, { accessToken, code });
  }
}
