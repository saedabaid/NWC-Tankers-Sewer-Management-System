import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateVehicleBrandComponent } from './create-vehicle-brand.component';

describe('CreateVehicleBrandComponent', () => {
  let component: CreateVehicleBrandComponent;
  let fixture: ComponentFixture<CreateVehicleBrandComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateVehicleBrandComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateVehicleBrandComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
