import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractViolationAddeditComponent } from './contract-violation-addedit.component';

describe('ContractViolationAddeditComponent', () => {
  let component: ContractViolationAddeditComponent;
  let fixture: ComponentFixture<ContractViolationAddeditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContractViolationAddeditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractViolationAddeditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
