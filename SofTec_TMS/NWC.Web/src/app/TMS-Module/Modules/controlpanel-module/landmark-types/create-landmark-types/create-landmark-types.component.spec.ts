import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateLandmarkTypesComponent } from './create-landmark-types.component';

describe('CreateLandmarkTypesComponent', () => {
  let component: CreateLandmarkTypesComponent;
  let fixture: ComponentFixture<CreateLandmarkTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateLandmarkTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateLandmarkTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
