import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TableDayOrderComponent } from './table-day-order.component';

describe('TableDayOrderComponent', () => {
  let component: TableDayOrderComponent;
  let fixture: ComponentFixture<TableDayOrderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TableDayOrderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TableDayOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
