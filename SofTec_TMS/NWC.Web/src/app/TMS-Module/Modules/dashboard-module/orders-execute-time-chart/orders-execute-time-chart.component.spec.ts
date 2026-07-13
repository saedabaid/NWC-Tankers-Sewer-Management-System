import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdersExecuteTimeChartComponent } from './orders-execute-time-chart.component';

describe('OrdersExecuteTimeChartComponent', () => {
  let component: OrdersExecuteTimeChartComponent;
  let fixture: ComponentFixture<OrdersExecuteTimeChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdersExecuteTimeChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdersExecuteTimeChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
