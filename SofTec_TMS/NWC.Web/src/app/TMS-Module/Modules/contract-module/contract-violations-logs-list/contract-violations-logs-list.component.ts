import { Component, Input, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ContractTermsViolationsLogsDTO } from 'src/app/TMS-Module/Models/contract-terms-violations-logs.model';
import { ContractService } from 'src/app/TMS-Module/Services/contract.service';

@Component({
  selector: 'app-contract-violations-logs-list',
  templateUrl: './contract-violations-logs-list.component.html',
  styleUrls: ['./contract-violations-logs-list.component.scss']
})
export class ContractViolationsLogsListComponent implements OnInit {

  IsArabic = false;
  advancedDiv: false;

  logs: ContractTermsViolationsLogsDTO[] = [];

  constructor(
    private contractService: ContractService,
    private translateService: TranslateService
  ) { }

  ngOnInit() {

    this.load();
    this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
  }

  load() {
    this.IsArabic = (this.translateService.currentLang == 'ar');
  }


  @Input()
  set ViolationId(violationId: number) {
    if (violationId) {
      console.log(violationId);

      this.contractService.GetContractViolationLogs(violationId).subscribe(res => {
        if (res.Value && !res.IsErrorState)
          this.logs = res.Value

      })


    }

  }




}
