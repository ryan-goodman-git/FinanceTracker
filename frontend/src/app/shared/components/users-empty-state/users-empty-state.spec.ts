import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersEmptyState } from './users-empty-state';

describe('UsersEmptyState', () => {
  let component: UsersEmptyState;
  let fixture: ComponentFixture<UsersEmptyState>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsersEmptyState],
    }).compileComponents();

    fixture = TestBed.createComponent(UsersEmptyState);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
