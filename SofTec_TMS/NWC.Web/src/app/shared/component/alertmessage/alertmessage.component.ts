import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'alertmessage',
  templateUrl: './alertmessage.component.html',
  styleUrls: ['./alertmessage.component.css']
})
export class AlertmessageComponent {
  @Input() message: string = 'ConfirmMessage';
  @Output() confirm: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor() { }

  setConfirm(): void {
    this.confirm.emit(true);
  }

  decline(): void {
    this.confirm.emit(false);
  }

}
