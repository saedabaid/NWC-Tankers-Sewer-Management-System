import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExitGateListComponent } from './exit-gate-list.component';

describe('ExitGateListComponent', () => {
  let component: ExitGateListComponent;
  let fixture: ComponentFixture<ExitGateListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExitGateListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExitGateListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
