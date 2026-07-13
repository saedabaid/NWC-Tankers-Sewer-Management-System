import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractViolationsLogsListComponent } from './contract-violations-logs-list.component';

describe('ContractViolationsLogsListComponent', () => {
  let component: ContractViolationsLogsListComponent;
  let fixture: ComponentFixture<ContractViolationsLogsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContractViolationsLogsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractViolationsLogsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
