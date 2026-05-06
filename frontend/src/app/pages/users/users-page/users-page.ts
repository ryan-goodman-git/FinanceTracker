import { Component } from '@angular/core';
import { CreateUserModal } from '../../../features/users/create-user-modal/create-user-modal';
import { CreateUserData } from '../../../shared/models/create-user-data';
import { UserCardData } from '../../../shared/models/user-card-data';
import { UserCard } from '../../../shared/components/user-card/user-card';
import { AddUserSlot } from '../../../shared/components/add-user-slot/add-user-slot';
import {UsersEmptyState} from '../../../shared/components/users-empty-state/users-empty-state';

@Component({
  selector: 'app-users-page',
  imports: [CreateUserModal, UserCard, AddUserSlot, UsersEmptyState],
  templateUrl: './users-page.html',
  styleUrl: './users-page.scss',
})
export class UsersPage {
  isCreateUserModalOpen = false;
  users: UserCardData[] = [];

  handleCreateUserSubmitted(data: CreateUserData) {
    if (this.users.length >= 2) {
      this.isCreateUserModalOpen = false;
      return;
    }

    this.users.push({
      fullName: data.fullName,
      startingBalance: data.startingBalance,
      monthlySalary: data.monthlySalary,
      salaryDay: data.salaryDay,
    });

    this.isCreateUserModalOpen = false;
  }
}
