import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceMeterListComponent } from './device-meter-list.component';

describe('DeviceMeterListComponent', () => {
  let component: DeviceMeterListComponent;
  let fixture: ComponentFixture<DeviceMeterListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeviceMeterListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeviceMeterListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
