import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddUserSlot } from './add-user-slot';

describe('AddUserSlot', () => {
  let component: AddUserSlot;
  let fixture: ComponentFixture<AddUserSlot>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddUserSlot],
    }).compileComponents();

    fixture = TestBed.createComponent(AddUserSlot);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
