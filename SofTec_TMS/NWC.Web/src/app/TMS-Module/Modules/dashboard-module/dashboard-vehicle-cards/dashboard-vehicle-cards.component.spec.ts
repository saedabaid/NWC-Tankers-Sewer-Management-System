import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardVehicleCardsComponent } from './dashboard-vehicle-cards.component';

describe('DashboardTilesCardsComponent', () => {
  let component: DashboardVehicleCardsComponent;
  let fixture: ComponentFixture<DashboardVehicleCardsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardVehicleCardsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardVehicleCardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
