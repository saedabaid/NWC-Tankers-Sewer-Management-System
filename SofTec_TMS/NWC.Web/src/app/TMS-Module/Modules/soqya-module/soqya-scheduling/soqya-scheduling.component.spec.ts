import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SoqyaSchedulingComponent } from './soqya-scheduling.component';

describe('SoqyaSchedulingComponent', () => {
  let component: SoqyaSchedulingComponent;
  let fixture: ComponentFixture<SoqyaSchedulingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SoqyaSchedulingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SoqyaSchedulingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
