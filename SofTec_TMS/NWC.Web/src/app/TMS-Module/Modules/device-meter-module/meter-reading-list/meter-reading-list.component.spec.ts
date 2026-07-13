import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MeterReadingListComponent } from './meter-reading-list.component';

describe('MeterReadingListComponent', () => {
  let component: MeterReadingListComponent;
  let fixture: ComponentFixture<MeterReadingListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MeterReadingListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MeterReadingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
