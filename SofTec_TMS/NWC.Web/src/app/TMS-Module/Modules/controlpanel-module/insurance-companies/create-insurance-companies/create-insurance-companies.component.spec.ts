import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateInsuranceCompaniesComponent } from './create-insurance-companies.component';

describe('CreateInsuranceCompaniesComponent', () => {
  let component: CreateInsuranceCompaniesComponent;
  let fixture: ComponentFixture<CreateInsuranceCompaniesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateInsuranceCompaniesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateInsuranceCompaniesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
