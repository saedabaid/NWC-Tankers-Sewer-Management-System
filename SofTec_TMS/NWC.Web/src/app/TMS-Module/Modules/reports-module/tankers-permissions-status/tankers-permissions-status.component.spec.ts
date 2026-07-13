import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TankersPermissionsStatusComponent } from './tankers-permissions-status.component';

describe('TankersPermissionsStatusComponent', () => {
  let component: TankersPermissionsStatusComponent;
  let fixture: ComponentFixture<TankersPermissionsStatusComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TankersPermissionsStatusComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TankersPermissionsStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
