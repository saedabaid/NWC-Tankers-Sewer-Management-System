import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffRolesComponent } from './staff-roles.component';

describe('StaffRolesComponent', () => {
  let component: StaffRolesComponent;
  let fixture: ComponentFixture<StaffRolesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StaffRolesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffRolesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
