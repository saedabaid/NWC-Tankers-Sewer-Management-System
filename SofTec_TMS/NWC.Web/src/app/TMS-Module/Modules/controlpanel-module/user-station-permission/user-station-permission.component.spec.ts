import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserStationPermissionComponent } from './user-station-permission.component';

describe('UserStationPermissionComponent', () => {
  let component: UserStationPermissionComponent;
  let fixture: ComponentFixture<UserStationPermissionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserStationPermissionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserStationPermissionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
