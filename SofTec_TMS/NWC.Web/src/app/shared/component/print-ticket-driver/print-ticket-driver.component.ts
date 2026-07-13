import { Component, OnInit } from '@angular/core';
import { PrintCustomerInvoiceDTO } from 'src/app/TMS-Module/Models/print-customer-invoice';
import { PrintDriverInvoiceDTO } from 'src/app/TMS-Module/Models/print -driver-invoice';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthenticationService } from '../../Services/authentication/authentication.service';
import { GateService } from 'src/app/TMS-Module/Services/gate.service';
import { PrintDTO } from 'src/app/TMS-Module/Models/common/PrintDTO';
import { NgxQrcodeElementTypes, NgxQrcodeErrorCorrectionLevels } from '@techiediaries/ngx-qrcode';

@Component({
  selector: 'app-print-ticket-driver',
  templateUrl: './print-ticket-driver.component.html',
  styleUrls: ['./print-ticket-driver.component.scss']
})
export class PrintTicketDriverComponent implements OnInit {

  VehicleID: string;
  OrderID: number;
  PrintDTO: PrintDTO;
  PrintDriverInvoice: PrintDriverInvoiceDTO = new PrintDriverInvoiceDTO();
  currentLang: string;
  value = '';
  elementType = NgxQrcodeElementTypes.IMG;
  correctionLevel = NgxQrcodeErrorCorrectionLevels.HIGH;
  constructor(private route: ActivatedRoute, public _auth: AuthenticationService, private _tran: TranslateService, private gateService: GateService) {

  }

  ngOnInit() {
    this.VehicleID = this.route.snapshot.paramMap.get("VehicleID");
    this.OrderID = parseInt(this.route.snapshot.paramMap.get("WorkOrderID"));
    this.PrintDTO = { VehicleID: this.VehicleID, WorkOrderID: this.OrderID };
    var numberPattern = /^\w*-\d*-/;

    if (this.VehicleID) {
      this.gateService.GetPrintDriverInvoice(this.PrintDTO).subscribe(res => {
        //console.log(res);
        if (res.Value != null) {
          this.PrintDriverInvoice = res.Value;
       
          this.value = this.PrintDriverInvoice.NetCost + "-" + this.PrintDriverInvoice.InvoiceNo + "-" +
            this.PrintDriverInvoice.TransporterCode.replace(numberPattern, "").toString() + "-" + this.PrintDriverInvoice.OrderNumber + "-" + this.PrintDriverInvoice.ZoneName + "-";
          //رقم التصريح - رقم الفاتورة - رقم الطلب - اسم الزون - السعر
          console.log(this.value);
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


