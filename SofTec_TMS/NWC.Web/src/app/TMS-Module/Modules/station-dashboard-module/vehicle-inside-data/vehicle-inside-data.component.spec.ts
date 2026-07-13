import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleInsideDataComponent } from './vehicle-inside-data.component';

describe('VehicleInsideDataComponent', () => {
  let component: VehicleInsideDataComponent;
  let fixture: ComponentFixture<VehicleInsideDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VehicleInsideDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VehicleInsideDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
