import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractsTariffsComponent } from './contracts-tariffs-component';

describe('OrdersPerZoneComponent', () => {
  let component: ContractsTariffsComponent;
  let fixture: ComponentFixture<ContractsTariffsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ContractsTariffsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractsTariffsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
