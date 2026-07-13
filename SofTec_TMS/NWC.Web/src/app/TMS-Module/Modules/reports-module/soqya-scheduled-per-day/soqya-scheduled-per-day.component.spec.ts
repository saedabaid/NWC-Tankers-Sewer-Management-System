import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SoqyaScheduledPerDayComponent } from './soqya-scheduled-per-day.component';

describe('SoqyaScheduledPerDayComponent', () => {
  let component: SoqyaScheduledPerDayComponent;
  let fixture: ComponentFixture<SoqyaScheduledPerDayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SoqyaScheduledPerDayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SoqyaScheduledPerDayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
