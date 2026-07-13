import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintTicketDriverComponent } from './print-ticket-driver.component';

describe('PrintTicketDriverComponent', () => {
  let component: PrintTicketDriverComponent;
  let fixture: ComponentFixture<PrintTicketDriverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintTicketDriverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintTicketDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
