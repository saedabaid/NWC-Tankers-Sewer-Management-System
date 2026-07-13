import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LandmarkTypesComponent } from './landmark-types.component';

describe('LandmarkTypesComponent', () => {
  let component: LandmarkTypesComponent;
  let fixture: ComponentFixture<LandmarkTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LandmarkTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LandmarkTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
