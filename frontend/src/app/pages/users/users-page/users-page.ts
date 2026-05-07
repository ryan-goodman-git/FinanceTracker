import { Component, inject, signal } from '@angular/core';
import { CreateUserModal } from '../../../features/users/create-user-modal/create-user-modal';
import { CreateUserData } from '../../../shared/models/create-user-data';
import { UserCardData } from '../../../shared/models/user-card-data';
import { UserCard } from '../../../shared/components/user-card/user-card';
import { AddUserSlot } from '../../../shared/components/add-user-slot/add-user-slot';
import { UsersEmptyState } from '../../../shared/components/users-empty-state/users-empty-state';
import { UsersPageDataService } from '../../../features/users/services/users-page-data.service';

@Component({
  selector: 'app-users-page',
  imports: [CreateUserModal, UserCard, AddUserSlot, UsersEmptyState],
  templateUrl: './users-page.html',
  styleUrl: './users-page.scss',
})
export class UsersPage {
  private readonly usersPageDataService = inject(UsersPageDataService);

  isCreateUserModalOpen = false;
  users = signal<UserCardData[]>([]);

  ngOnInit() {
  this.usersPageDataService.getUsersPageData().subscribe((users) => {
    this.users.set(users);
  });
}

  handleCreateUserSubmitted(data: CreateUserData) {
    this.isCreateUserModalOpen = false;
  }
}
