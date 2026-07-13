import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadZoneExcelComponent } from './upload-zone-excel.component';

describe('UploadZoneExcelComponent', () => {
  let component: UploadZoneExcelComponent;
  let fixture: ComponentFixture<UploadZoneExcelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadZoneExcelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadZoneExcelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
