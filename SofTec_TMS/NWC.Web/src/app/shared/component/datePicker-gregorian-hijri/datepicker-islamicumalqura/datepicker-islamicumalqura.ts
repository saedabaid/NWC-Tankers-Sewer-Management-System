import { Component, Input, Output, EventEmitter, ViewEncapsulation } from '@angular/core';
import { NgbCalendarIslamicUmalqura, NgbDatepickerI18n, NgbDate, NgbCalendar } from '@ng-bootstrap/ng-bootstrap';
import { IslamicI18n } from './IslamicI18n.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'datepicker-islamicumalqura',
  templateUrl: './datepicker-islamicumalqura.html',
  providers: [
    { provide: NgbCalendar, useClass: NgbCalendarIslamicUmalqura },
    { provide: NgbDatepickerI18n, useClass: IslamicI18n },
    //{ provide: NgbCalendarIslamicUmalqura, useClass: NgbCalendarIslamicUmalqura }
  ],
  styleUrls: ['./datepicker-islamicumalqura.scss'],
  encapsulation: ViewEncapsulation.None
})
export class DatepickerIslamicumalqura {

  @Input() viewOnly: boolean = false;
  @Input() disabled: boolean = false;
  @Input() InputIsHijri: boolean = true;
  //@Input() selectedDate: Date;
  @Output() selectedDateChange: EventEmitter<Date | number> = new EventEmitter<Date | number>(); // to be able to use [()]

  hijriModel: NgbDate;
  displayHijri: string;

  constructor(
    private _IslamicI18n: NgbDatepickerI18n,
    private _NgbCalendarIslamicUmalqura: NgbCalendarIslamicUmalqura,
    private _translate: TranslateService
  ) {

    _translate.onLangChange.subscribe(res => {
      this.displayDate();
    });

  }


  @Input()
  set selectedDate(inputDate: Date | number) {
    if (!isNullOrUndefined(inputDate)) {
      if (this.InputIsHijri) {
        let date = (inputDate as number);
        let y = Math.floor(date / 10000);
        let m = Math.floor((date - (y * 10000)) / 100);
        let d = Math.floor((date - (y * 10000) - (m * 100)));

        this.hijriModel = new NgbDate(y, m, d); //year, month, day
      }
      else {
        if ((typeof inputDate) == "string")
          inputDate = new Date(inputDate);
        this.hijriModel = this._NgbCalendarIslamicUmalqura.fromGregorian(inputDate as Date);
      }

      this.displayDate();
    }
  }

  onDateSelected(evt: NgbDate) {
    this.displayDate();
    if (this.InputIsHijri) {
      let dateNum = (evt.year * 10000) + (evt.month * 100) + evt.day;
      this.selectedDateChange.emit(dateNum);
    }
    else {
      this.selectedDateChange.emit(this._NgbCalendarIslamicUmalqura.toGregorian(evt));
    }
  }

  displayDate() {
    if (!isNullOrUndefined(this.hijriModel)) {
      this.displayHijri = `${this.hijriModel.day} ${this._IslamicI18n.getMonthFullName(this.hijriModel.month)} ${this.hijriModel.year}`;
    }
  }

}
