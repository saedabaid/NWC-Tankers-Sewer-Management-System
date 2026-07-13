import { Component, OnInit } from '@angular/core';
import { PrintDTO } from 'src/app/TMS-Module/Models/common/PrintDTO';
import { PrintVehicleInvoice } from 'src/app/TMS-Module/Models/print-vehicle-invoice.model';
import { ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { TranslateService } from '@ngx-translate/core';
import { GateService } from 'src/app/TMS-Module/Services/gate.service';

@Component({
  selector: 'app-print-vehicle-ticket',
  templateUrl: './print-vehicle-ticket.component.html',
  styleUrls: ['./print-vehicle-ticket.component.scss']
})
export class PrintVehicleTicketComponent implements OnInit {

  PrintDTO = new PrintDTO;
  invoice: PrintVehicleInvoice = new PrintVehicleInvoice();
  currentLang: string;
  myDate = new Date();

  constructor(
    private route: ActivatedRoute,
    public _auth: AuthenticationService,
    private _tran: TranslateService,
    private gateService: GateService
  ) { }

  ngOnInit() {
    this.currentLang = this._tran.currentLang;
    let VehicleID = this.route.snapshot.paramMap.get("VehicleID");
    this.PrintDTO.VehicleID = VehicleID;

    if (VehicleID) {
      this.gateService.GetPrintVehicleInvoice(this.PrintDTO).subscribe(res => {
        if (res.Value != null) {
          this.invoice = res.Value;
        }
      })
    }

  }


  customRound(value: number, power: number) {
    if (!isNaN(value)) {
      let p = Math.pow(10, power);
      return Math.round((value) * p) / p;
    }
  }

}
