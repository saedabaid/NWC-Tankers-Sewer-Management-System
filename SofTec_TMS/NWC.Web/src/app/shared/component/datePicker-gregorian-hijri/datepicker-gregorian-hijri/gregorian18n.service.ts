import { NgbDatepickerI18n, NgbDateStruct } from "@ng-bootstrap/ng-bootstrap";
import { Injectable } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";

const WEEKDAYS = ['ن', 'ث', 'ر', 'خ', 'ج', 'س', 'ح'];
const MONTHS = ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو', 'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر',
  'نوفمبر', 'ديسمبر'];

const WEEKDAYSen = ['m', 't', 'w', 't', 'f', 's', 's'];
const MONTHSen = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October',
  'November', 'December'];

@Injectable()
export class GergorianI18n extends NgbDatepickerI18n {

  constructor(private _tran: TranslateService) {
    super();

  }

  getWeekdayShortName(weekday: number) {
    if (this._tran.currentLang == 'ar')
      return WEEKDAYS[weekday - 1];
    else
      return WEEKDAYSen[weekday - 1];
  }

  getMonthShortName(month: number) {
    if (this._tran.currentLang == 'ar')
      return MONTHS[month - 1];
    else
      return MONTHSen[month - 1];
  }

  getMonthFullName(month: number) {
    if (this._tran.currentLang == 'ar')
      return MONTHS[month - 1];
    else
      return MONTHSen[month - 1];
  }

  getDayAriaLabel(date: NgbDateStruct): string {
    return `${date.day}-${date.month}-${date.year}`;
  }
}