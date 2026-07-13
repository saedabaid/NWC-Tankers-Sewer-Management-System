import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DailyOrdersSummaryComponent } from './daily-orders-summary.component';

describe('DailyOrdersSummaryComponent', () => {
  let component: DailyOrdersSummaryComponent;
  let fixture: ComponentFixture<DailyOrdersSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DailyOrdersSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DailyOrdersSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
