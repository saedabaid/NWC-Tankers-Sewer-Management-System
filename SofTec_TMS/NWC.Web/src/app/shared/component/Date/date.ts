import { Component, Input, OnChanges, SimpleChanges, Output, EventEmitter, Inject, LOCALE_ID } from '@angular/core';
import { NgbCalendar, NgbDatepickerI18n, NgbDate } from '@ng-bootstrap/ng-bootstrap';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { TranslateService } from '@ngx-translate/core';
import { DatePipe } from '@angular/common';

@Component({
    selector: 'date',
    templateUrl: './date.html',
  
})
export class DateComponent  implements OnChanges {

    @Input() date: Date;
    @Input() ShowTime: boolean = false;
    @Input() TimeUnderDate: boolean = false;
    @Input() ShowDate: boolean = true;
    @Input() DateFormat: string = 'd MMMM yyyy';
    @Input() ShowDayOfWeek: boolean = false;

    display_Date : string ='';
    display_Time: string = '';
    display_DayOfWeek: string = '';

    constructor(private datePipe: DatePipe , private _translate: TranslateService ,
        @Inject(LOCALE_ID) public localeId:string ) {
        _translate.onLangChange.subscribe(res => {
            this.displayDate();
        });
    }
    ngOnChanges(changes: SimpleChanges): void {
        if (!isNullOrUndefined(changes)) {
            
            this.displayDate();
        }
    }
    displayDate() {
        if (!isNullOrUndefined(this.date)) {
            let lan = this._translate.currentLang;
            this.localeId = (lan == 'ar')? 'ar-EG' : 'en-US';
            this.display_Date = this.datePipe.transform( this.date, this.DateFormat, null , this.localeId);
            
            if( this.ShowTime ){
                this.display_Time = this.datePipe.transform( this.date, 'hh:mm a', null , this.localeId);
            }

            if (this.ShowDayOfWeek) {
                this.display_DayOfWeek = this.datePipe.transform( this.date, 'EE', null , this.localeId);
            }
           
        }
    }

   
    

}
