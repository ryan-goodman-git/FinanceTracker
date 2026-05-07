export interface UserCardData {
  userId: string;
  fullName: string;
  startingBalance: number;
  startDate: string;
  salary?: UserSalaryCardData;
  recurringBills: RecurringBillCardData[];
}

export interface UserSalaryCardData {
  amount: number;
  scheduledDayOfMonth: number;
}

export interface RecurringBillCardData {
  recurringTransactionId: string;
  description: string;
  amount: number;
  scheduledDayOfMonth: number;
}
