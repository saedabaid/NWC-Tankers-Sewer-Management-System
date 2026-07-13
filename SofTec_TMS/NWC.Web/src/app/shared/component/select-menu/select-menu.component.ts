import { Component, OnInit, ViewEncapsulation, Input, Output, EventEmitter, SimpleChanges, OnChanges, ChangeDetectorRef, AfterViewChecked } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { isNullOrUndefined } from 'src/app/shared/utilities/utilities';

@Component({
  selector: 'select-menu',
  templateUrl: './select-menu.component.html',
  styleUrls: ['./select-menu.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SelectMenuComponent implements OnInit, OnChanges, AfterViewChecked {

  /**
   *
   */
  constructor(private _translate: TranslateService, private changeDetector: ChangeDetectorRef) { }

  // @Input('disabled') disabled = false;

  @Input('disabled') public set data(value: boolean) {
    this.dropdownSettings = {
      singleSelection: this.options.singleSelect ? true : false,
      text: this._translate.instant(this.options.text != undefined ? this.options.text : 'Select'),
      enableCheckAll: this.options.enableCheckAll != undefined ? this.options.enableCheckAll : true,
      selectAllText: this._translate.instant(this.options.selectAllText != undefined ? this.options.selectAllText : 'Select All'),
      unSelectAllText: this._translate.instant(this.options.unSelectAllText != undefined ? this.options.unSelectAllText : 'Deselect All'),
      enableSearchFilter: this.options.enableSearchFilter != undefined ? this.options.enableSearchFilter : false,
      maxHeight: this.options.maxHeight != undefined ? this.options.maxHeight : 200,
      badgeShowLimit: this.options.badgeShowLimit != undefined ? this.options.badgeShowLimit : 'All',
      classes: this.inline ? 'inline-display' : '',
      limitSelection: this.options.limitSelection != undefined ? this.options.limitSelection : 0,
      disabled: value,
      searchPlaceholderText: this._translate.instant(this.options.searchPlaceholderText != undefined ? this.options.searchPlaceholderText : 'Search...'),
      groupBy: this.options.groupBy != undefined ? this.options.groupBy : '',
      searchAutofocus: this.options.searchAutofocus != undefined ? this.options.searchAutofocus : true,
      labelKey: this.label != undefined ? this.label : 'DisplayName',
      primaryKey: this.itemKey != undefined ? this.itemKey : 'Value',
      position: 'bottom'
    };
  }

  //
  @Input('singleSelect') public set data2(value: boolean) {
    this.dropdownSettings = {
      singleSelection: value,
      text: this._translate.instant(this.options.text != undefined ? this.options.text : 'Select'),
      enableCheckAll: this.options.enableCheckAll != undefined ? this.options.enableCheckAll : true,
      selectAllText: this._translate.instant(this.options.selectAllText != undefined ? this.options.selectAllText : 'Select All'),
      unSelectAllText: this._translate.instant(this.options.unSelectAllText != undefined ? this.options.unSelectAllText : 'Deselect All'),
      enableSearchFilter: this.options.enableSearchFilter != undefined ? this.options.enableSearchFilter : false,
      maxHeight: this.options.maxHeight != undefined ? this.options.maxHeight : 200,
      badgeShowLimit: this.options.badgeShowLimit != undefined ? this.options.badgeShowLimit : 'All',
      classes: this.inline ? 'inline-display' : '',
      limitSelection: this.options.limitSelection != undefined ? this.options.limitSelection : 0,
      disabled: this.options.disabled ? this.options.disabled : false,
      searchPlaceholderText: this._translate.instant(this.options.searchPlaceholderText != undefined ? this.options.searchPlaceholderText : 'Search...'),
      groupBy: this.options.groupBy != undefined ? this.options.groupBy : '',
      searchAutofocus: this.options.searchAutofocus != undefined ? this.options.searchAutofocus : true,
      labelKey: this.label != undefined ? this.label : 'DisplayName',
      primaryKey: this.itemKey != undefined ? this.itemKey : 'Value',
      position: 'bottom'
    };
  }
  @Input()
  public set loading(val: boolean) {
    this._loading = val;
  }
  dropdownList = []; // list used by the component to add the inputs
  selectedItems = []; // list used by the component to get the selected
  dropdownSettings: any = {}; // settings for the component

  @Input() dropDownListInputs: any[] = []; // used to get the inputs from the parent;

  @Input() selectedList: any[] = []; // used to get the selected data from the parent
  @Output() selectedListChange: EventEmitter<any[]> = new EventEmitter<any[]>(); // used to update the selected array in the parent

  @Output() focusedItem: EventEmitter<string> = new EventEmitter<string>();

  @Input() inline: boolean; // used if we need to display the select menu inline not dropdown

  // @Input() singleSelect: boolean; // used if we need to use a single select

  @Input() label: string; // The property name which should be rendered as label in the dropdown

  @Input() itemKey: string; // The property by which the object is identified.

  @Input() options: any = {}; // options used in the component

  @Output() SearchVAlue: EventEmitter<any[]> = new EventEmitter<any[]>();

  _loading = false;

  ngOnInit() {
    this.dropdownList = this.dropDownListInputs; // make the component data = the sent data from the parent

    this.selectedItems = this.selectedList; // set intialized data
    if (!this.options) {
      this.options = {};
    }

    this.loadAfterLang();
    this._translate.onLangChange.subscribe((res: any) => {
      this.loadAfterLang();
    });

    // if (this.dropdownSettings.singleSelection) {
    //   this.dropdownSettings.classes += ' single-search';
    // }

    if (this.dropdownSettings.singleSelection) {
      this.dropdownSettings.classes += ' single-search';
      this.dropdownSettings.classes += ' single-search no-selectall';
    }

    if (!this.dropdownSettings.enableSearchFilter) {
      this.dropdownSettings.classes += ' no-search';
    }

    if (!this.dropdownSettings.enableCheckAll) {
      this.dropdownSettings.classes += ' no-selectall';
    }
  }

  onItemSelect(item) {
    this.selectedListChange.emit(this.selectedList);
    this.focusedItem.emit(item);
  }

  OnItemDeSelect(item: any) {
    this.selectedListChange.emit(this.selectedList);
    this.focusedItem.emit(item);
  }

  onSelectAll(items: any) {
    this.selectedListChange.emit(this.selectedList);
    this.focusedItem.emit(items);
  }

  onDeSelectAll(items: any) {
    this.selectedListChange.emit(this.selectedList);
    this.focusedItem.emit(items);
  }

  onSearchEvent(evt: any) {

    this.SearchVAlue.emit(evt.target.value);
  }

  dropdownopen(evt: any) {

  }

  loadAfterLang() {
    this.dropdownSettings = {
      singleSelection: this.options.singleSelect ? true : false,
      text: this._translate.instant(this.options.text != undefined ? this.options.text : 'Select'),
      enableCheckAll: this.options.enableCheckAll != undefined ? this.options.enableCheckAll : true,
      selectAllText: this._translate.instant(this.options.selectAllText != undefined ? this.options.selectAllText : 'Select All'),
      unSelectAllText: this._translate.instant(this.options.unSelectAllText != undefined ? this.options.unSelectAllText : 'Deselect All'),
      enableSearchFilter: this.options.enableSearchFilter != undefined ? this.options.enableSearchFilter : false,
      maxHeight: this.options.maxHeight != undefined ? this.options.maxHeight : 200,
      badgeShowLimit:  this.options.badgeShowLimit != undefined ? this.options.badgeShowLimit : 1,
      classes: this.inline ? 'inline-display' : '',
      disabled: this.options.disabled ? this.options.disabled : false,
      searchPlaceholderText: this._translate.instant(this.options.searchPlaceholderText != undefined ? this.options.searchPlaceholderText : 'Search...'),
      groupBy: this.options.groupBy != undefined ? this.options.groupBy : '',
      searchAutofocus: this.options.searchAutofocus != undefined ? this.options.searchAutofocus : true,
      labelKey: this.label != undefined ? this.label : 'DisplayName',
      primaryKey: this.itemKey != undefined ? this.itemKey : 'Value',
      position: 'bottom',
      noDataLabel: this._translate.instant(this.options.noDataLabel != undefined ? this.options.noDataLabel : 'NoDataFound')
    };
    // Important to NOT DISABLE select all checkbox
    if(this.options.limitSelection) {
      this.dropdownSettings.limitSelection = this.options.limitSelection
    }


  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!isNullOrUndefined(changes)
      && !isNullOrUndefined(changes.dropDownListInputs)
      && !isNullOrUndefined(changes.dropDownListInputs.currentValue)
    ) {

      if (!isNullOrUndefined(this.selectedList) && this.selectedList.length > 0 ) {
        this.selectedList = this.dropDownListInputs.filter(a => this.selectedList.findIndex(b => b.Id === a.Id) !== -1);
      }

    }
  }

  ngAfterViewChecked() {
    this.changeDetector.detectChanges();
  }

}
