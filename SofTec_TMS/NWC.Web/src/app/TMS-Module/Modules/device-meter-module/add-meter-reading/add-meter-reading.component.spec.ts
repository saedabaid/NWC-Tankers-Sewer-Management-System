import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddMeterReadingComponent } from './add-meter-reading.component';

describe('AddMeterReadingComponent', () => {
  let component: AddMeterReadingComponent;
  let fixture: ComponentFixture<AddMeterReadingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddMeterReadingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddMeterReadingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
