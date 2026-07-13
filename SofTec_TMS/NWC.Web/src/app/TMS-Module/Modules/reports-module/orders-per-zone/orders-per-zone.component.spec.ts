import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdersPerZoneComponent } from './orders-per-zone.component';

describe('OrdersPerZoneComponent', () => {
  let component: OrdersPerZoneComponent;
  let fixture: ComponentFixture<OrdersPerZoneComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdersPerZoneComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdersPerZoneComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
