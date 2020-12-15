import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TravelExpensesComponent } from './travel-expenses.component';

describe('TravelExpensesComponent', () => {
  let component: TravelExpensesComponent;
  let fixture: ComponentFixture<TravelExpensesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TravelExpensesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TravelExpensesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
