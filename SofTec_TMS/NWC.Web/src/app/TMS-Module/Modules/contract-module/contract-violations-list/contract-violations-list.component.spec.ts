import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractViolationsListComponent } from './contract-violations-list.component';

describe('ContractViolationsListComponent', () => {
  let component: ContractViolationsListComponent;
  let fixture: ComponentFixture<ContractViolationsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContractViolationsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractViolationsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
