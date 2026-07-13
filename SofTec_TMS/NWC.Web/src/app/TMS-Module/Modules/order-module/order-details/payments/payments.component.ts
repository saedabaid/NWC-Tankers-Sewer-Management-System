import { Component, OnInit } from '@angular/core';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { OrderPayment } from 'src/app/TMS-Module/Models/order-payment';
import { OrderDetailsService } from 'src/app/TMS-Module/Services/order-details.service';
import { ActivatedRoute } from '@angular/router';
import { LoaderService } from 'src/app/shared/loader.service';

@Component({
  selector: 'payments',
  templateUrl: './payments.component.html',
  styleUrls: ['./payments.component.scss']
})

export class PaymentsComponent implements OnInit {
  workOrderID : string;
  orderPaymentSearchResult : OrderPayment[] = [];

  constructor(private route: ActivatedRoute, private orderDetailsService: OrderDetailsService,
    private mainloading: LoaderService) {  }

  ngOnInit() {

    this.workOrderID = this.route.snapshot.paramMap.get("ID");

    this.mainloading.PreloaderIcreaseCount();
    this.orderDetailsService.getWorkOrderPayments(parseInt(this.workOrderID)).subscribe(res=>{
      if (res.IsErrorState == false) {
               this.orderPaymentSearchResult = res.Value;
             //  this.orderPaymentSearchResult.TotalCount = res.Value.TotalCount;
             }
            else {
              this.orderPaymentSearchResult = [];
             // this.orderPaymentSearchResult.TotalCount = 0
            }
    }
    ,err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    ,() => {
      this.mainloading.PreloaderDecreaseCount();
    });
   }
}
