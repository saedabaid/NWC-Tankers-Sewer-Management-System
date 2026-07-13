import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractorCreateComponent } from './contractor-create.component';

describe('ContractorCreateComponent', () => {
  let component: ContractorCreateComponent;
  let fixture: ComponentFixture<ContractorCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContractorCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractorCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
