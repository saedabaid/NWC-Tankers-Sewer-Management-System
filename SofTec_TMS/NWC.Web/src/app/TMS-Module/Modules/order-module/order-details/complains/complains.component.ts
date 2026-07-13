import { OrderComplaint } from 'src/app/TMS-Module/Models/order-compaint';
import { Component, OnInit, ViewEncapsulation, Input } from '@angular/core';
 
@Component({
  selector: ' complains',
  templateUrl: './complains.component.html',
  styleUrls: ['./complains.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ComplainsComponent implements OnInit {
  Complaints = { count :0 ,listComplaints:[] };
  IsNoComplaints :boolean =  true;

  @Input('OrderComplaints') public set OrderComplaints(value: OrderComplaint[] ) {
    this.Complaints.listComplaints = value;
    this.Complaints.count = value.length;
    if ( value.length >0)
      this. IsNoComplaints = false;
  }
  constructor() { }

  ngOnInit() {
  }

}
