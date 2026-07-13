import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ManualDispatchTankerComponent } from './manual-dispatch-tanker.component';

describe('ManualDispatchTankerComponent', () => {
  let component: ManualDispatchTankerComponent;
  let fixture: ComponentFixture<ManualDispatchTankerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ManualDispatchTankerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManualDispatchTankerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
