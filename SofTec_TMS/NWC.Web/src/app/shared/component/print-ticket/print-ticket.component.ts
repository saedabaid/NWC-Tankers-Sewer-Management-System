import { AuthenticationService } from 'src/app/shared/Services/authentication/authentication.service';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { GateService } from 'src/app/TMS-Module/Services/gate.service';
import { PrintCustomerInvoiceDTO } from 'src/app/TMS-Module/Models/print-customer-invoice';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-print-ticket',
  templateUrl: './print-ticket.component.html',
  styleUrls: ['./print-ticket.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class PrintTicketComponent implements OnInit {

  constructor( ) {
  
   }

  ngOnInit() { 
 
  }

}
