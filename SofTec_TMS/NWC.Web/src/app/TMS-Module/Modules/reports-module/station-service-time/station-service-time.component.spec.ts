import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StationServiceTimeComponent } from './station-service-time.component';

describe('StationServiceTimeComponent', () => {
  let component: StationServiceTimeComponent;
  let fixture: ComponentFixture<StationServiceTimeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StationServiceTimeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StationServiceTimeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
