import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadTariffExcelComponent } from './upload-tariff-excel.component';

describe('UploadTariffExcelComponent', () => {
  let component: UploadTariffExcelComponent;
  let fixture: ComponentFixture<UploadTariffExcelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadTariffExcelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadTariffExcelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
