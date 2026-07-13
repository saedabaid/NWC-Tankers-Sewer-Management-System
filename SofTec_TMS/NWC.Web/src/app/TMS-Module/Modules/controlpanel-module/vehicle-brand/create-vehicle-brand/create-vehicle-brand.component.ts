import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ControlPanelService } from '@tms-services/control-panel.service';
import {Location} from '@angular/common';
import { LookupService } from '@tms-services/lookup.service';
import { VehicleBrand } from '@tms-models/vehicle-brand';


@Component({
  selector: 'app-create-vehicle-brand',
  templateUrl: './create-vehicle-brand.component.html',
  styleUrls: ['./create-vehicle-brand.component.scss']
})
export class CreateVehicleBrandComponent implements OnInit {

  id = '';
  vehicleBrand = new VehicleBrand();

  constructor(
    private activRoute: ActivatedRoute,
    private _translate: TranslateService,
    private titleService: Title,
    private _location: Location,
    private cpService: ControlPanelService,
    private lookup: LookupService
  ) {
    if (this.activRoute.snapshot.paramMap.has('id')) {
      this.id = this.activRoute.snapshot.paramMap.get('id');
      this.lookup.getVehicleBrandById(this.id).subscribe(res => {
        this.vehicleBrand = res.Value
      })
    }
  }


  ngOnInit() {
    this.titleService.setTitle(this._translate.instant('CeateNewVehicleBrand'));

    this._translate.onLangChange.subscribe((res) => {
      this.titleService.setTitle(this._translate.instant('CeateNewVehicleBrand'));
    });
  }

  save() {
    console.log(this.vehicleBrand);
    this.cpService.saveVehicleBrand(this.vehicleBrand).subscribe(res => {
      if(!res.IsErrorState) {
        this._location.back();
      }
    })
  }


  cancel() {
    this._location.back();
  }

  onStaffRoleCategoryDDLChanged()
  {}

}
