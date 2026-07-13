import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangesLogComponent } from './changes-log.component';

describe('ChangesLogComponent', () => {
  let component: ChangesLogComponent;
  let fixture: ComponentFixture<ChangesLogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChangesLogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangesLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
