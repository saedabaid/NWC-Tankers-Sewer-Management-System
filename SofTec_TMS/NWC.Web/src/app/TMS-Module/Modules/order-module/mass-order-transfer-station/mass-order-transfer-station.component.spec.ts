import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MassOrderTransferStationComponent } from './mass-order-transfer-station.component';

describe('MassOrderTransferStationComponent', () => {
  let component: MassOrderTransferStationComponent;
  let fixture: ComponentFixture<MassOrderTransferStationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MassOrderTransferStationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MassOrderTransferStationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
