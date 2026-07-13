import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddPermitComponent } from './add-permit.component';

describe('AddPermitComponent', () => {
  let component: AddPermitComponent;
  let fixture: ComponentFixture<AddPermitComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddPermitComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddPermitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
