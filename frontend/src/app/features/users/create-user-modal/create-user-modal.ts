import { Component, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateUserData } from '../../../shared/models/create-user-data';

@Component({
  selector: 'app-create-user-modal',
  imports: [FormsModule],
  templateUrl: './create-user-modal.html',
  styleUrl: './create-user-modal.scss',
})
export class CreateUserModal {
  closed = output<void>();
  submitted = output<CreateUserData>();

  fullName = '';
  startingBalance = 0;
  monthlySalary = 0;
  salaryDay = 1;

}
