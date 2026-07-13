import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ContractService } from 'src/app/TMS-Module/Services/contract.service';
import { PageFilter } from 'src/app/TMS-Module/Models/common/page-fillter-model';
import { isNullOrUndefined } from "src/app/shared/utilities/utilities";
import { TranslateService } from '@ngx-translate/core';
import { ContractPriceDTO } from 'src/app/TMS-Module/Models/contract-priceDTO';
import { alertService } from 'src/app/shared/Services/alert/alert.service';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { searchCriteriaContractDTO } from 'src/app/TMS-Module/Models/search-criteria/search-Criteria-Contract-DTO';
import { Configuration } from 'src/app/shared/configurations/shared.config';
import { LoaderService } from 'src/app/shared/loader.service';

@Component({
  selector: 'price-details',
  templateUrl: './price-details.component.html',
  styleUrls: ['./price-details.component.scss']
})
export class PriceDetailsComponent implements OnInit {

  advancedDiv = false;
  //ContractID : number ;
  contractPriceList :SearchResult<ContractPriceDTO> = new SearchResult<ContractPriceDTO>();
  ChangedContractPriceList : ContractPriceDTO[] = [];
  searchCriteriaContractDTO :searchCriteriaContractDTO = new searchCriteriaContractDTO();

  constructor(private _alert :alertService, private translateService: TranslateService ,
    private router: Router, private route: ActivatedRoute  , private ContractService :ContractService,
    private mainloading: LoaderService
    ) { }

  ngOnInit() {
   // this.ContractID = parseInt(this.route.snapshot.paramMap.get("Id")) ;
    this.searchCriteriaContractDTO.ContractID =parseInt(this.route.snapshot.paramMap.get("Id")) ;
    this.searchCriteriaContractDTO.PageFilter.PageIndex = 1;
    this.searchCriteriaContractDTO.PageFilter.PageSize =  Configuration.GridSetting.Pagesize;

    if( !isNullOrUndefined(  this.searchCriteriaContractDTO.ContractID) ){
     this.load();
     this.translateService.onLangChange.subscribe(res => {
      this.load();
    });
    }

  }

  load(){
    this.mainloading.PreloaderIcreaseCount();
    this.ContractService.GetContractPriceList(this.searchCriteriaContractDTO  ).subscribe(res=>{
      if(res.IsErrorState == false){
        this.contractPriceList= res.Value;
      }
    }
    ,err => {
      this.mainloading.PreloaderDecreaseCount();
    }
    ,() => {
      this.mainloading.PreloaderDecreaseCount();
    });
  }

  clear(){
    this.load();
  }
  onPriceChange(station : ContractPriceDTO , $event){

    var tempStation = Object.assign({}, station);
    //tempStation.PriceCharge = ( parseInt($event.target.value ) )? parseInt($event.target.value ) : -1 ;
    tempStation.PriceCharge = isNaN($event.target.value ) ? -1 : +$event.target.value;
    if( this.ChangedContractPriceList.find( s =>s.ContractPriceID == station.ContractPriceID) ){
      this.ChangedContractPriceList.find( s =>s.ContractPriceID == station.ContractPriceID).PriceCharge = tempStation.PriceCharge;
    }
    else{
      this.ChangedContractPriceList.push(tempStation);
    }


  }
  close() {
    this.ContractService.changeTab$.next("contractlist");
  }

  backBtn() {
    this.ContractService.changeTab$.next('stations');
  }

  nextBtn(){

    this.ContractService.changeTab$.next('traiff');
  }
  onPageIndexChanged(evt) {
    this.searchCriteriaContractDTO.PageFilter.PageIndex = evt;
    this.load();
  }

  onPageSizeChanged(evt) {
    this.searchCriteriaContractDTO.PageFilter.PageSize = evt;
    this.load();
  }

  save(){

    this.ChangedContractPriceList.map(s=>{
      if (isNullOrUndefined(s.PriceCharge ) || s.PriceCharge == -1){
        s.PriceCharge = 0;
      }});
    if(this.isValid())
      {
        this.mainloading.PreloaderIcreaseCount();
        this.ContractService.UpdatePriceList(this.ChangedContractPriceList).subscribe(res=>{
          if(! res.IsErrorState){
            this._alert.showSuccess();
            this.load();

            }
            else{
              this._alert.showError();
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

  isValid():boolean{
   console.log(  this.ChangedContractPriceList );
    //validate
    let message =[];
    if( this.ChangedContractPriceList.find(s=>s.PriceCharge < 0))
      message.push("priceMustBePositiveNum")
    if ( this.ChangedContractPriceList.find(s=>s.PriceCharge >922337203685477.5807) )
      message.push("priceMustBeLessThan922337203685477")

    if(message.length >0 ) {
      this._alert.errorList(message);
      return false
    }
    else{
      return true;
    }

}

}


