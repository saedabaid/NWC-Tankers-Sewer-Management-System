import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleOutsideDataComponent } from './vehicle-outside-data.component';

describe('VehicleOutsideDataComponent', () => {
  let component: VehicleOutsideDataComponent;
  let fixture: ComponentFixture<VehicleOutsideDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VehicleOutsideDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VehicleOutsideDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
