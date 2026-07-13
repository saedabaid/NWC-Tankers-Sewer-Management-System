import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StationsDetailsComponent } from './stations-details.component';

describe('StationsDetailsComponent', () => {
  let component: StationsDetailsComponent;
  let fixture: ComponentFixture<StationsDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StationsDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StationsDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
