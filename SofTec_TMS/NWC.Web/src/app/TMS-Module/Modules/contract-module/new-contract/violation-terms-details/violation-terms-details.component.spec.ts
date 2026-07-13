import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViolationTermsDetailsComponent } from './violation-terms-details.component';

describe('ViolationTermsDetailsComponent', () => {
  let component: ViolationTermsDetailsComponent;
  let fixture: ComponentFixture<ViolationTermsDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViolationTermsDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViolationTermsDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
