import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeferredOrderDetailsComponent } from './deferred-order-details.component';

describe('DeferredOrderDetailsComponent', () => {
  let component: DeferredOrderDetailsComponent;
  let fixture: ComponentFixture<DeferredOrderDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeferredOrderDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeferredOrderDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
