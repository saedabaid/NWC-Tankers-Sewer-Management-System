import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StationOrderCapacityComponent } from './station-order-capacity.component';

describe('StationOrderCapacityComponent', () => {
  let component: StationOrderCapacityComponent;
  let fixture: ComponentFixture<StationOrderCapacityComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StationOrderCapacityComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StationOrderCapacityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
