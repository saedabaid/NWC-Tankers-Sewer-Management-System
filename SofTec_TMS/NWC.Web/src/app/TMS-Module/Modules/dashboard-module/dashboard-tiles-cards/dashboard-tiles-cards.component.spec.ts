import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardTilesCardsComponent } from './dashboard-tiles-cards.component';

describe('DashboardTilesCardsComponent', () => {
  let component: DashboardTilesCardsComponent;
  let fixture: ComponentFixture<DashboardTilesCardsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardTilesCardsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardTilesCardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
