import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
//import { PageChangedEvent } from 'ngx-bootstrap';
import { Configuration } from '../../configurations/shared.config';

@Component({
  selector: 'app-pagination-component',
  templateUrl: './pagination.component.html'
 
})
export class PaginationsComponent implements OnInit {
/*@Input() totalItems : number;
@Input() pageSize : number = Configuration.GridSetting.Pagesize;
@Input() currentPage : number;
  constructor() { }
  @Output() 
  onPageChanged = new EventEmitter<PageChangedEvent>();
  getPage(event: PageChangedEvent = <PageChangedEvent>{ page: 1, itemsPerPage: this.pageSize }) {
    this.onPageChanged.emit(event);
  }*/
  ngOnInit() {
  }
 
  
  _isListDisplayed: boolean;
  @Input()
  public set IsListDisplayed(val: boolean){  
    this._isListDisplayed = val;
  }

  _itemsCount: number = 0;
  @Input()
  public set ItemsCount(val: number){  
    this._itemsCount = val;
  }

  @Output() PageSizeChange:EventEmitter<number> = new EventEmitter<number>(); 

  @Input()  PageSize: number = Configuration.GridSetting.Pagesize;
  @Input()  pageIndex: any = 1;
  @Input()  pagescount: number = 0;
  @Output() pageNavigated = new EventEmitter();
  
  getPage(event: PageChangedEvent = <PageChangedEvent>{ page: 1, itemsPerPage: this.PageSize }) {
    this.pageIndex = event.page;
    this.pageNavigated.emit(this.pageIndex);
  }
}
