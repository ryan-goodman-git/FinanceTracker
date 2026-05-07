import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { UserSummaryResponse } from '../../shared/models/user-summary-response';

@Injectable({
  providedIn: 'root',
})

export class UsersApiService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = 'http://localhost:5038';

  getUsers(): Observable<UserSummaryResponse[]> {
    return this.http.get<UserSummaryResponse[]>(`${this.baseUrl}/users`);
  }
}
