import { Component, OnInit, Input, Output, EventEmitter, ViewEncapsulation } from '@angular/core';
import { AmazingTimePickerService } from 'amazing-time-picker';
import { AuthenticationService } from '../../Services/authentication/authentication.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-amazing-timepicker',
  templateUrl: './amazing-timepicker.component.html',
  styleUrls: ['./amazing-timepicker.scss'],
  encapsulation: ViewEncapsulation.None
})

export class AmazingTimepickerComponent implements OnInit {
  @Input() selectedTime: string;
  @Output() selectedTimeChange: EventEmitter<string> = new EventEmitter<string>();
  @Input() viewOnly: boolean = false;
  @Input() id: string;
  constructor(
    private atp: AmazingTimePickerService, 
    private auth: AuthenticationService,
    private _translate: TranslateService
    ) { }
  isMeridian = false;
  showSpinners = false;
  ngOnInit() {
  }

  OpenTime() {
    const amazingTimePicker = this.atp.open({ locale: this._translate.currentLang });
    amazingTimePicker.afterClose().subscribe(time => {
      //console.log(time);
      this.selectedTimeChange.emit(time);
    });
  }
  onChange(val: any) {

  }

  onBlurMethod(val: any) {
    this.selectedTimeChange.emit(this.selectedTime);
  }
}
