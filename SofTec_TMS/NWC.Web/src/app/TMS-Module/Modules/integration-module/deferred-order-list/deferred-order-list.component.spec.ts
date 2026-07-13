import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeferredOrderListComponent } from './deferred-order-list.component';

describe('DeferredOrderListComponent', () => {
  let component: DeferredOrderListComponent;
  let fixture: ComponentFixture<DeferredOrderListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeferredOrderListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeferredOrderListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
