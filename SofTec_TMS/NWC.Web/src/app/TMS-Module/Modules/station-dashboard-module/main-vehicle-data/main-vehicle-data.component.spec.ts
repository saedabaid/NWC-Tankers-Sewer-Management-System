import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MainVehicleDataComponent } from './main-vehicle-data.component';

describe('MainVehicleDataComponent', () => {
  let component: MainVehicleDataComponent;
  let fixture: ComponentFixture<MainVehicleDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MainVehicleDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MainVehicleDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
