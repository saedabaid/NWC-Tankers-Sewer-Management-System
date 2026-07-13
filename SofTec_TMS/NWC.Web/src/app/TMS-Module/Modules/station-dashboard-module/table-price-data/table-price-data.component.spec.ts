import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TablePriceDataComponent } from './table-price-data.component';

describe('TablePriceDataComponent', () => {
  let component: TablePriceDataComponent;
  let fixture: ComponentFixture<TablePriceDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TablePriceDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TablePriceDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
