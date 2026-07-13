import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddOrderCommentComponent } from './add-order-comment.component';

describe('AddOrderCommentComponent', () => {
  let component: AddOrderCommentComponent;
  let fixture: ComponentFixture<AddOrderCommentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddOrderCommentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddOrderCommentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
