import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditTariffComponent } from './add-edit-tariff.component';

describe('AddEditTariffComponent', () => {
  let component: AddEditTariffComponent;
  let fixture: ComponentFixture<AddEditTariffComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddEditTariffComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditTariffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
