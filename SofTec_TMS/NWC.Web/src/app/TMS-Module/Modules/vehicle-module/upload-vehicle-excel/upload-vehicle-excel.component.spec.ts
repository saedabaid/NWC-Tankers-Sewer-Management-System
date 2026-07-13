import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadVehicleExcelComponent } from './upload-vehicle-excel.component';

describe('UploadVehicleExcelComponent', () => {
  let component: UploadVehicleExcelComponent;
  let fixture: ComponentFixture<UploadVehicleExcelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadVehicleExcelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadVehicleExcelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
