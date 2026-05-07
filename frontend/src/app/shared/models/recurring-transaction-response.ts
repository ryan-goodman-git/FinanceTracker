export interface RecurringTransactionResponse {
  recurringTransactionId: string;
  userId: string;
  description: string;
  amount: number;
  type: TransactionTypeResponse;
  kind: RecurringTransactionKindResponse;
  startDate: string;
  endDate: string | null;
  scheduledDayOfMonth: number;
}

export type TransactionTypeResponse = 1 | 2;

export type RecurringTransactionKindResponse = 1 | 2;
