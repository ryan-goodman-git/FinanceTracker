import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { UserSummaryResponse } from '../../shared/models/user-summary-response';
import { RecurringTransactionResponse } from '../../shared/models/recurring-transaction-response';
import { CreateUserRequest } from '../../shared/models/create-user-request';
import { CreateUserResponse } from '../../shared/models/create-user-response';

@Injectable({
  providedIn: 'root',
})

export class UsersApiService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = 'http://localhost:5038';

  getUsers(): Observable<UserSummaryResponse[]> {
    return this.http.get<UserSummaryResponse[]>(`${this.baseUrl}/users`);
  }

  createUser(request: CreateUserRequest): Observable<CreateUserResponse> {
    return this.http.post<CreateUserResponse>(`${this.baseUrl}/users`, request);
  }

  getRecurringTransactionsForUser(userId: string): Observable<RecurringTransactionResponse[]> {
    return this.http.get<RecurringTransactionResponse[]>(`${this.baseUrl}/users/${userId}/recurring-transactions`);
  }
}
