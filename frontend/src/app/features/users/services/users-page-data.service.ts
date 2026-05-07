import { inject, Injectable } from '@angular/core';
import { forkJoin, map, Observable, of, switchMap } from 'rxjs';

import { UsersApiService } from '../../../core/services/users-api.service';
import { RecurringTransactionResponse } from '../../../shared/models/recurring-transaction-response';
import { UserCardData } from '../../../shared/models/user-card-data';
import { UserSummaryResponse } from '../../../shared/models/user-summary-response';

const recurringTransactionKind = {
  salary: 1,
  expense: 2,
} as const;

@Injectable({
  providedIn: 'root',
})
export class UsersPageDataService {
  private readonly usersApiService = inject(UsersApiService);

  getUsersPageData(): Observable<UserCardData[]> {
    return this.usersApiService.getUsers().pipe(
      switchMap((users) => {
        if (users.length === 0) {
          return of([]);
        }

        const recurringTransactionRequests = users.map((user) =>
          this.usersApiService.getRecurringTransactionsForUser(user.userId),
        );

        return forkJoin(recurringTransactionRequests).pipe(
          map((recurringTransactionsByUser) =>
            users.map((user, index) =>
              this.toUserCardData(user, recurringTransactionsByUser[index]),
            ),
          ),
        );
      }),
    );
  }

  private toUserCardData(
    user: UserSummaryResponse,
    recurringTransactions: RecurringTransactionResponse[],
  ): UserCardData {
    const salary = recurringTransactions.find(
      (transaction) => transaction.kind === recurringTransactionKind.salary,
    );

    const recurringBills = recurringTransactions
      .filter((transaction) => transaction.kind === recurringTransactionKind.expense)
      .map((transaction) => ({
        recurringTransactionId: transaction.recurringTransactionId,
        description: transaction.description,
        amount: transaction.amount,
        scheduledDayOfMonth: transaction.scheduledDayOfMonth,
      }));

    return {
      userId: user.userId,
      fullName: user.name,
      startingBalance: user.initialBalance,
      startDate: user.startDate,
      salary: salary
        ? {
            amount: salary.amount,
            scheduledDayOfMonth: salary.scheduledDayOfMonth,
          }
        : undefined,
      recurringBills,
    };
  }
}
