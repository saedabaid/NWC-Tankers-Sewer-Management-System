import { Component, OnInit, Output, EventEmitter, Input, ViewEncapsulation } from '@angular/core';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';

import { Observable, of } from 'rxjs';
import { mergeMap, map } from 'rxjs/operators';

@Component({
  selector: 'search-component',
  templateUrl: './search-component.component.html',
  styleUrls: ['./search-component.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SearchComponentComponent implements OnInit {

  searchString: string = '';
  //_value: string = '' ;
  @Input() name: string = 'mainSearch';
  @Input() get _value() {
    return this._value;
  }
  set _value(v) {
    this.asyncSelected=v;
  }

  set selected( v ){

  }
  @Input() placeholder: string = 'Search...';
  @Input() isTypeahead: boolean = false;
  @Input() typeaheadFunction: Function;
  @Input() field: string;
  @Output() searchKeyword: EventEmitter<any> = new EventEmitter<null>();
  @Output() searchClicked: EventEmitter<any> = new EventEmitter<null>();

  // TypeAhead
  asyncSelected: string ="";
  typeaheadLoading: boolean;
  noResult: boolean = false;
  dataSource: Observable<any>;

  _loading = false;

  constructor() {
    this.dataSource = Observable.create((observer: any) => {
      // Runs on every search
      observer.next(this.asyncSelected);
    }).pipe(
        mergeMap((token: string) =>  {
          this._loading = this.isTypeahead ? true : false; //when use search without autoComplete
          return this.getStatesAsObservable(token);
        })
      );
  }

  ngOnInit() {
  }

  getStatesAsObservable(token: string): Observable<any> {
    if(!this.isTypeahead){
      return of([]);
    }
    return this.typeaheadFunction(token).pipe(map( (r:any) => {
      this._loading = false;
       return r.Value ? r.Value : []}));

  }
  searchByKeyword() {
    this.searchKeyword.emit(this.asyncSelected)
  }
  searchByClicked() {
    this.searchClicked.emit(this.asyncSelected)
  }

  changeTypeaheadLoading(e: boolean): void {
    this.typeaheadLoading = e;
  }

  typeaheadNoResults(event: boolean): void {
    this.noResult = event;
  }

  typeaheadOnSelect(e: TypeaheadMatch): void {
    this.searchKeyword.emit(e.item);
    //this.searchClicked.emit(e.item);
  }
}
