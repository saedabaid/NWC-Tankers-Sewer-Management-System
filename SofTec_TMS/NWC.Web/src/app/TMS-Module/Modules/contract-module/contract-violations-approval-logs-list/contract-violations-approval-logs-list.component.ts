import { Component, Input, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ContractTermsApprovalViolationsLogsDTO } from 'src/app/TMS-Module/Models/contract-terms-violations-logs.model';
import { ContractService } from 'src/app/TMS-Module/Services/contract.service';

@Component({
  selector: 'app-contract-violations-approval-logs-list',
  templateUrl: './contract-violations-approval-logs-list.component.html',
  styleUrls: ['./contract-violations-approval-logs-list.component.scss']
})
export class ContractViolationsApprovalLogsListComponent implements OnInit {

  IsArabic = false;
  advancedDiv: false;

  logs: ContractTermsApprovalViolationsLogsDTO[] = [];

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

      this.contractService.GetContractApprovalViolationLogs(violationId).subscribe(res => {
      
        if (res.Value && !res.IsErrorState)
          this.logs = res.Value

      })

    }

  }


}
