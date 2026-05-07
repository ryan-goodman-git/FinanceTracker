import { Component, input } from '@angular/core';
import { UserCardData } from '../../models/user-card-data';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-user-card',
  imports: [CurrencyPipe],
  templateUrl: './user-card.html',
  styleUrl: './user-card.scss',
})

export class UserCard {
  user = input.required<UserCardData>();
}
