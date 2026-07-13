import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdersZonesChartComponent } from './orders-zones-chart.component';

describe('OrdersZonesChartComponent', () => {
  let component: OrdersZonesChartComponent;
  let fixture: ComponentFixture<OrdersZonesChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdersZonesChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdersZonesChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
