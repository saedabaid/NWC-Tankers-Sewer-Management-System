import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TestVPNComponent } from './test-vpn.component';

describe('TestVPNComponent', () => {
  let component: TestVPNComponent;
  let fixture: ComponentFixture<TestVPNComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TestVPNComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TestVPNComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
