import { Component, output } from '@angular/core';

@Component({
  selector: 'app-users-empty-state',
  imports: [],
  templateUrl: './users-empty-state.html',
  styleUrl: './users-empty-state.scss',
})
export class UsersEmptyState {
  addUserClicked = output<void>();
}
