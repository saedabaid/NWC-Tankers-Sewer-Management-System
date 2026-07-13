import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AmrDataComponent } from './amr-data.component';

describe('AmrDataComponent', () => {
  let component: AmrDataComponent;
  let fixture: ComponentFixture<AmrDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AmrDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AmrDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
