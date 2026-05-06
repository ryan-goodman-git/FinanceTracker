import { Component, output } from '@angular/core';

@Component({
  selector: 'app-add-user-slot',
  imports: [],
  templateUrl: './add-user-slot.html',
  styleUrl: './add-user-slot.scss',
})
export class AddUserSlot {
  addClicked = output<void>();
}
