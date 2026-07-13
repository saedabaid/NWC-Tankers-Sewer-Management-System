import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateVehicleTypesComponent } from './create-vehicle-types.component';

describe('CreateVehicleTypesComponent', () => {
  let component: CreateVehicleTypesComponent;
  let fixture: ComponentFixture<CreateVehicleTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateVehicleTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateVehicleTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
