import { Component, OnInit, ViewEncapsulation, Input, Output, EventEmitter, AfterViewInit } from '@angular/core';
import { AuthenticationService } from '../../Services/authentication/authentication.service';
import { TranslateService } from '@ngx-translate/core';
import { esLocale } from 'ngx-bootstrap/locale';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-datepicker',
  templateUrl: './datepicker.component.html',
  styleUrls: ['./datepicker.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class DatepickerComponent implements OnInit {

  @Input() dateFormat: string = ' D MMM YYYY'
  @Input() selecteddate: Date;
  @Input() placeHolder = 'selectDate';
  @Input() DateID: string = "sd";

  @Output() selecteddateChange: EventEmitter<Date> = new EventEmitter<Date>();
  @Input() viewOnly: boolean = false;
  datePickerConfig: any;
  // = {
  //   containerClass: this._tran.currentLang == 'ar' ? ' rtl' : ' '
  // }
  @Input() PickerType: string = 'day'; //'month', 'year'

  @Input() 
  set MinDate(myDate: Date){
    if(myDate) {
      this.minDate = new Date(myDate);
    }
  }
  minDate: Date = null;

  constructor(
    private auth: AuthenticationService,
    private _tran: TranslateService,
    private localeService: BsLocaleService) {

      defineLocale('es', esLocale);
      this.localeService.use('es');
  }

  ngOnInit() {
    this.load();
    this._tran.onLangChange.subscribe((event) => {
      this.load();
    });
  }

  load() {
    this.localeService.use(this._tran.currentLang);
    this.datePickerConfig = {
      //containerClass: (this._tran.currentLang == 'ar' ? 'theme-red rtl' : 'theme-red'),
      containerClass: (this._tran.currentLang == 'ar' ? 'theme-green rtl' : 'theme-green'),
      
      dateInputFormat: this.dateFormat,
      adaptivePosition: true
    }
  }

  onChange(val: any) {
    if (val != null)
      this.selecteddateChange.emit(val);
  }

  onBlur() {
    if (this.selecteddate == null)
      this.selecteddateChange.emit(null);
  }

  onOpenCalendar(container) {
    container.monthSelectHandler = (event: any): void => {
      container._store.dispatch(container._actions.select(event.date));
    };     
    container.setViewMode(this.PickerType);
    //container.setViewMode('month');
  }

}
