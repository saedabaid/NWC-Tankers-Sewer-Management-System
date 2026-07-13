import { ActivatedRoute } from '@angular/router';
import { statusLogs } from './../../../../Models/status-log';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { orderStatusLog } from '../../../../Services/status-log.service';
@Component({
  selector: 'status-log',
  templateUrl: './status-log.component.html',
  styleUrls: ['./status-log.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class StatusLogComponent implements OnInit {
  statusLogs: statusLogs
  constructor(private orderStatusLog: orderStatusLog, private router: ActivatedRoute) { }
  listStatus: statusLogs[] = []
  ngOnInit() {
  //  console.log(this.router.snapshot.params["ID"]);
    this.orderStatusLog.getLogStatus(+this.router.snapshot.params["ID"]).subscribe(res => {
     // console.log(res);

      this.listStatus = <statusLogs[]>res["Value"]
    })
  }

}
