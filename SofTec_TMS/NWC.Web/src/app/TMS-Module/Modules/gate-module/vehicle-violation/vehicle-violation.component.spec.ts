import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleViolationComponent } from './vehicle-violation.component';

describe('VehicleViolationComponent', () => {
  let component: VehicleViolationComponent;
  let fixture: ComponentFixture<VehicleViolationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VehicleViolationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VehicleViolationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
