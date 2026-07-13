import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccessoriesChargesDetailsComponent } from './accessories-charges-details.component';

describe('AccessoriesChargesDetailsComponent', () => {
  let component: AccessoriesChargesDetailsComponent;
  let fixture: ComponentFixture<AccessoriesChargesDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccessoriesChargesDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccessoriesChargesDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
