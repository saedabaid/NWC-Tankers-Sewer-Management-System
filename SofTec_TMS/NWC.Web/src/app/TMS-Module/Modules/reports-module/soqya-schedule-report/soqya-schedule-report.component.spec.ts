import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SoqyaScheduleReportComponent } from './soqya-schedule-report.component';

describe('SoqyaScheduleReportComponent', () => {
  let component: SoqyaScheduleReportComponent;
  let fixture: ComponentFixture<SoqyaScheduleReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SoqyaScheduleReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SoqyaScheduleReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
