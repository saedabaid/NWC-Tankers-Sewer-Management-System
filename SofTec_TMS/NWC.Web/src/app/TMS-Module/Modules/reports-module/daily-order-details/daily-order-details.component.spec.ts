import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DailyOrderDetailsComponent } from './daily-order-details.component';

describe('DailyOrderDetailsComponent', () => {
  let component: DailyOrderDetailsComponent;
  let fixture: ComponentFixture<DailyOrderDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DailyOrderDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DailyOrderDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
