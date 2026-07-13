import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeassignTankerComponent } from './deassign-tanker.component';

describe('DeassignTankerComponent', () => {
  let component: DeassignTankerComponent;
  let fixture: ComponentFixture<DeassignTankerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeassignTankerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeassignTankerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
