import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractViolationPrintComponent } from './contract-violation-print.component';

describe('ContractViolationPrintComponent', () => {
  let component: ContractViolationPrintComponent;
  let fixture: ComponentFixture<ContractViolationPrintComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContractViolationPrintComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractViolationPrintComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
