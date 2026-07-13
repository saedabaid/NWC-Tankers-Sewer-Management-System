import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintVehicleTicketComponent } from './print-vehicle-ticket.component';

describe('PrintVehicleTicketComponent', () => {
  let component: PrintVehicleTicketComponent;
  let fixture: ComponentFixture<PrintVehicleTicketComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintVehicleTicketComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintVehicleTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
