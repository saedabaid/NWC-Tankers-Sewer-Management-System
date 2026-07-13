import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdersDateChartComponent } from './orders-date-chart.component';

describe('OrdersDateChartComponent', () => {
  let component: OrdersDateChartComponent;
  let fixture: ComponentFixture<OrdersDateChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdersDateChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdersDateChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
