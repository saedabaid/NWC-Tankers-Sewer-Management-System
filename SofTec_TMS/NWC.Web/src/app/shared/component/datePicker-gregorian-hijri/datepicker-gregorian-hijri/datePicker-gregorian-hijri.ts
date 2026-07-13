import { Component, Input, OnChanges, SimpleChanges, Output, EventEmitter, ViewEncapsulation } from '@angular/core';
import { NgbCalendar, NgbDatepickerI18n, NgbDate } from '@ng-bootstrap/ng-bootstrap';
import { GergorianI18n } from './gregorian18n.service';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'datePicker-gregorian-hijri',
    templateUrl: './datePicker-gregorian-hijri.html',
    providers: [
        { provide: NgbDatepickerI18n, useClass: GergorianI18n },
    ],
    styleUrls: ['./datePicker-gregorian-hijri.scss'


    ],
    encapsulation: ViewEncapsulation.None
})
export class DatepickerGregorianHijri { // implements OnChanges

    @Input() viewOnly: boolean = false;
    @Input() disabled: boolean = false;
    //@Input() selectedDate: Date;
    @Output() selectedDateChange: EventEmitter<Date> = new EventEmitter<Date>(); // to be able to use [()]

    gergorianModel: NgbDate;
    displayGerogrian: string;
    myDate: Date;

    constructor(
        private coc: NgbCalendar,
        private _GregorianI18n: NgbDatepickerI18n,
        private _translate: TranslateService
    ) {
        _translate.onLangChange.subscribe(res => {
            this.displayDate();
        });
    }

    // ngOnChanges(changes: SimpleChanges): void {
    //     if (!isNullOrUndefined(changes)
    //         && !isNullOrUndefined(changes.selectedDate)
    //         && !isNullOrUndefined(changes.selectedDate.currentValue)
    //     ) {
    //         let changedDate: Date = changes.selectedDate.currentValue;
    //         if ((typeof changedDate) == "string")
    //             changedDate = new Date(changedDate);
    //         this.gergorianModel = new NgbDate(changedDate.getFullYear(), changedDate.getMonth() + 1, changedDate.getDate());
    //         this.displayDate();
    //     }
    // }
    @Input()
    set selectedDate(inputDate: Date) {
        if (!isNullOrUndefined(inputDate)) {
            this.myDate = new Date(inputDate);
            this.gergorianModel = new NgbDate(this.myDate.getFullYear(), this.myDate.getMonth() + 1, this.myDate.getDate());
            this.displayDate();
        }
    }

    onDateSelected(evt: NgbDate) {
        this.displayGerogrian = `${evt.day} ${this._GregorianI18n.getMonthFullName(evt.month)} ${evt.year}`;
        this.selectedDateChange.emit(new Date(evt.year, evt.month - 1, evt.day));
    }

    hijriDateChanged(evt: Date) {
        this.gergorianModel = new NgbDate(evt.getFullYear(), evt.getMonth() + 1, evt.getDate());
        this.displayDate();
        this.selectedDateChange.emit(evt)
    }

    displayDate() {
        if (!isNullOrUndefined(this.gergorianModel)) {
            this.displayGerogrian = `${this.gergorianModel.day} ${this._GregorianI18n.getMonthFullName(this.gergorianModel.month)} ${this.gergorianModel.year}`;
        }
    }

}
