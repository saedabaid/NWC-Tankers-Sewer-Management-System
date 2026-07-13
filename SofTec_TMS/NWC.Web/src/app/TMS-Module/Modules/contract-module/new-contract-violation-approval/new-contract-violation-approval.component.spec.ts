import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewContractViolationApprovalComponent } from './new-contract-violation-approval/new-contract-violation-approval.component';

describe('NewContractViolationApprovalComponent', () => {
  let component: NewContractViolationApprovalComponent;
  let fixture: ComponentFixture<NewContractViolationApprovalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [NewContractViolationApprovalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewContractViolationApprovalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
