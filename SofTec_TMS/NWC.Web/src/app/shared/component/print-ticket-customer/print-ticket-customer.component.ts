import { Component, OnInit } from '@angular/core';
import { PrintCustomerInvoiceDTO } from 'src/app/TMS-Module/Models/print-customer-invoice';
import { ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../../Services/authentication/authentication.service';
import { TranslateService } from '@ngx-translate/core';
import { GateService } from 'src/app/TMS-Module/Services/gate.service';
import { PrintDTO } from 'src/app/TMS-Module/Models/common/PrintDTO';
import { NgxQrcodeElementTypes, NgxQrcodeErrorCorrectionLevels } from '@techiediaries/ngx-qrcode';

@Component({
  selector: 'app-print-ticket-customer',
  templateUrl: './print-ticket-customer.component.html',
  styleUrls: ['./print-ticket-customer.component.scss']
})
export class PrintTicketCustomerComponent implements OnInit {
  value = '';
  elementType = NgxQrcodeElementTypes.IMG;
  correctionLevel = NgxQrcodeErrorCorrectionLevels.HIGH;
  PrintCustomerInvoice: PrintCustomerInvoiceDTO = new PrintCustomerInvoiceDTO();
  VehicleID: string;
  OrderID: number;
  PrintDTO: PrintDTO;
  currentLang: string;

  constructor(private route: ActivatedRoute, public _auth: AuthenticationService, private _tran: TranslateService, private gateService: GateService) {

  }

  ngOnInit() {
    this.VehicleID = this.route.snapshot.paramMap.get("VehicleID");
    this.OrderID = parseInt(this.route.snapshot.paramMap.get("WorkOrderID"));
    this.PrintDTO = { VehicleID: this.VehicleID, WorkOrderID: this.OrderID };
    var numberPattern = /^\w*-\d*-/;

    if (this.VehicleID) {
      this.gateService.GetPrintCustomerInvoice(this.PrintDTO).subscribe(res => {
        //console.log(res);
        if (res.Value != null) {
        
          this.PrintCustomerInvoice = res.Value;
          this.value = this.PrintCustomerInvoice.NetCost + "-" + this.PrintCustomerInvoice.InvoiceNo + "-" +
            this.PrintCustomerInvoice.TransporterCode.replace(numberPattern, "").toString() + "-" + this.PrintCustomerInvoice.OrderNumber + "-" + this.PrintCustomerInvoice.ZoneName + "-";

        }

      })
    }

    this.currentLang = this._tran.currentLang;
    this._tran.onLangChange.subscribe(a => {
      this.currentLang = a.lang;
    })

  }

  customRound(value: number, power: number) {
    if (!isNaN(value)) {
      let p = Math.pow(10, power);
      return Math.round((value) * p) / p;
    }
  }

}
