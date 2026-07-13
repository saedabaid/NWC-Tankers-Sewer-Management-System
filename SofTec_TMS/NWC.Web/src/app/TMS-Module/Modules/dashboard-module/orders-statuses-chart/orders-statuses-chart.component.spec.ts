import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdersStatusesChartComponent } from './orders-statuses-chart.component';

describe('OrdersStatusesChartComponent', () => {
  let component: OrdersStatusesChartComponent;
  let fixture: ComponentFixture<OrdersStatusesChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdersStatusesChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdersStatusesChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
