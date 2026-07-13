import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdersDayHoursChartComponent } from './orders-day-hours-chart.component';

describe('OrdersDayHoursChartComponent', () => {
  let component: OrdersDayHoursChartComponent;
  let fixture: ComponentFixture<OrdersDayHoursChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdersDayHoursChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdersDayHoursChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
