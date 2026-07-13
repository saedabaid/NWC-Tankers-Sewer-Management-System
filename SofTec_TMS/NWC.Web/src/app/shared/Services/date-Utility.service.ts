import { DatePipe } from "@angular/common";
import { Inject, Injectable, LOCALE_ID } from "@angular/core";
import { NgbCalendarIslamicUmalqura, NgbDate, NgbDatepickerI18n } from "@ng-bootstrap/ng-bootstrap";
import { TranslateService } from "@ngx-translate/core";
import { IslamicI18n } from "../component/datePicker-gregorian-hijri/datepicker-islamicumalqura/IslamicI18n.service";
import { ConvertDateToExcel } from "../utilities/utilities";



@Injectable({
    providedIn: 'root',
})
export class DateUtilityService {

    private _IslamicI18n: NgbDatepickerI18n;

    constructor(
        private datePipe: DatePipe,
        private _translate: TranslateService,
        @Inject(LOCALE_ID) public localeId: string,
        private _NgbCalendarIslamicUmalqura: NgbCalendarIslamicUmalqura,
        //private _IslamicI18n= new IslamicI18n(_translate),
    ) {

        this._IslamicI18n = new IslamicI18n(_translate);

        this.setLocalId(_translate.currentLang);
        _translate.onLangChange.subscribe(res => {
            this.setLocalId(res.lang);
        });
    }

    setLocalId(lang: string) {
        this.localeId = (lang == 'ar') ? 'ar-EG' : 'en-US';
    }


    getDate(input: Date): string {
        if (input) {
            return input.toString().substring(0, 10);
        }
        return '';
    }

    getDateWithMonthName(input: Date): string {
        return this.getWithFormat(input, 'd MMMM yyyy');
    }

    getTime(input: Date): string {
        return this.getWithFormat(input, 'hh:mm a');
    }

    getDayOfWeek(input: Date): string {
        return this.getWithFormat(input, 'EE');
    }


    getWithFormat(input: Date, format: string): string {
        if (input) {
            return this.datePipe.transform(input, format, null, this.localeId);
        }
        return '';
    }

    ConvertDateToExcel(input: Date | string, includeTime: boolean): string {
        return ConvertDateToExcel(input, includeTime);

        // if (input) {
        //     let date = input.toString().substring(0, 10);
        //     if (!includeTime) {
        //         return date;
        //     }
        //     let time = input.toString().substring(11, 16);
        //     return date + ' ' + time;
        // }
        // return '';
    }


    getIslamicUmalquraDate(inputDate: Date | number, InputIsHijri: boolean): string {

        if (inputDate) {
            let hijriModel: NgbDate;
            if (InputIsHijri) {
                let date = (inputDate as number);
                let y = Math.floor(date / 10000);
                let m = Math.floor((date - (y * 10000)) / 100);
                let d = Math.floor((date - (y * 10000) - (m * 100)));

                hijriModel = new NgbDate(y, m, d); //year, month, day
            }
            else {
                if ((typeof inputDate) == "string")
                    inputDate = new Date(inputDate);
                hijriModel = this._NgbCalendarIslamicUmalqura.fromGregorian(inputDate as Date);
            }

            return `${hijriModel.day} ${this._IslamicI18n.getMonthFullName(hijriModel.month)} ${hijriModel.year}`;;
        }


    }


}