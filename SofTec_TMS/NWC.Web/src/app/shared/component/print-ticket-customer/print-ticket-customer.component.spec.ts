import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintTicketCustomerComponent } from './print-ticket-customer.component';

describe('PrintTicketCustomerComponent', () => {
  let component: PrintTicketCustomerComponent;
  let fixture: ComponentFixture<PrintTicketCustomerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintTicketCustomerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintTicketCustomerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
