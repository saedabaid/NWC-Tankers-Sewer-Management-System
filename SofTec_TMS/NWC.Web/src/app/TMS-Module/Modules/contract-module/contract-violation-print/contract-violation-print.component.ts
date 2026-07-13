import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { AuthenticationService } from "src/app/shared/Services/authentication/authentication.service";
import { ContractTermsViolationsDTo } from "src/app/TMS-Module/Models/contract-terms-violations.model";
import { ContractViolationSC } from "src/app/TMS-Module/Models/search-criteria/contract-violation-SC.model";
import { ContractService } from "src/app/TMS-Module/Services/contract.service";
import * as printJS from "print-js";
import { ContractTermsViolationsInVoiceDTO } from "src/app/TMS-Module/Models/contract-terms-violations-invoice.model";

@Component({
  selector: "app-contract-violation-print",
  templateUrl: "./contract-violation-print.component.html",
  styleUrls: ["./contract-violation-print.component.scss"],
})
export class ContractViolationPrintComponent implements OnInit {

  violationInvoice: ContractTermsViolationsInVoiceDTO = new ContractTermsViolationsInVoiceDTO();
  IsArabic = false;

  detailsItems: any;
  //userName = '';

  constructor(
    private route: ActivatedRoute,
    //public _auth: AuthenticationService,
    private _tran: TranslateService,
    private contractService: ContractService
  ) { }

  ngOnInit() {
    let violationId = this.route.snapshot.paramMap.get("Id");
    if (violationId) {
      // let filters = new ContractViolationSC();
      // filters.Id = +violationId;
      // filters.PageFilter.PageSize = 1;
      // filters.PageFilter.PageIndex = 1;

      // this.contractService.GetContractViolations(filters).subscribe((res) => {
      //   if (res.Value && res.Value.Result && res.Value.Result.length > 0) {
      //     this.violation = res.Value.Result[0];
      //     this.binddetailsItems(this.violation);
      //   }
      // });

      this.contractService.GetTermViolationInvoice(+violationId).subscribe(res => {
        if (!res.IsErrorState && res.Value) {
          this.violationInvoice = res.Value;
          this.binddetailsItems(this.violationInvoice);
        }


      })



    }

    this.IsArabic = this._tran.currentLang == "ar";
    this._tran.onLangChange.subscribe((res) => {
      this.IsArabic = res.lang == "ar";
    });

    //this.userName = this._auth.getCurrentuserName();
  }

  floor(value) {
    return value ? Math.floor(value) : 0;
  }

  floorReminder(value) {
    let reminder = value ? value - Math.floor(value) : 0;
    return Math.floor(reminder * 100) / 100;
  }

  printJS() {
    printJS({
      printable: "printSection",
      type: "html",
      scanStyles: false,
      css: ["/assets/print/print.css", "/assets/print/bootstrap.css"],
      header: "Violation Print",
    });
  }


  // binddetailsItems(item: ContractTermsViolationsDTo) {
  //   this.detailsItems = [
  //     {
  //       title: "الإسم",
  //       content: item.DriverName,
  //       titleEn: "Name",
  //     },
  //     {
  //       title: "رقم التصريح",
  //       content: item.VehicleCode ? (item.VehicleCode + ' | ' + item.VehiclePlateNo) : '',
  //       titleEn: "Declaration No.",
  //     },
  //     {
  //       title: "محطة التعبئة",
  //       content: item.StationName,
  //       titleEn: "Station",
  //     },
  //     {
  //       title: "المبلغ",
  //       content: item.TotalPenalty,
  //       titleEn: "The Amount of",
  //     },
  //     // {
  //     //   title: "طريقة الدفع",
  //     //   content: "نقدا",
  //     //   titleEn: "Payment Mode",
  //     // },
  //     {
  //       title: "البيان",
  //       content: item.ContractTermName,
  //       titleEn: "For",
  //     },
  //     {
  //       title: "الضريبة",
  //       content: "0",
  //       titleEn: "Vat",
  //     },
  //   ];
  // }

  binddetailsItems(item: ContractTermsViolationsInVoiceDTO) {
    this.detailsItems = [
      {
        title: "الإسم",
        content: item.DriverName,
        titleEn: "Name",
      },
      {
        title: "رقم التصريح",
        content: item.VehicleCode ? (item.VehicleCode + ' | ' + item.VehiclePlateNo) : '',
        titleEn: "Declaration No.",
      },
      {
        title: "محطة التعبئة",
        content: item.StationName,
        titleEn: "Station",
      },
      {
        title: "المبلغ",
        content: item.ValueWithVAT,
        titleEn: "The Amount of",
      },
      // {
      //   title: "طريقة الدفع",
      //   content: "نقدا",
      //   titleEn: "Payment Mode",
      // },
      {
        title: "البيان",
        content: item.ContractTermName,
        titleEn: "For",
      },
      {
        title: "الضريبة",
        content: item.VAT,
        titleEn: "Vat",
      },
    ];
  }

}
