import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateVehicleModelComponent } from './create-vehicle-model.component';

describe('CreateVehicleModelComponent', () => {
  let component: CreateVehicleModelComponent;
  let fixture: ComponentFixture<CreateVehicleModelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateVehicleModelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateVehicleModelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
