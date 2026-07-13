import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { VehicleType } from '@tms-models/vehicle-type';
import { ControlPanelService } from '@tms-services/control-panel.service';
import {Location} from '@angular/common';
import { LookupService } from '@tms-services/lookup.service';



@Component({
  selector: 'app-create-vehicle-types',
  templateUrl: './create-vehicle-types.component.html',
  styleUrls: ['./create-vehicle-types.component.scss']
})
export class CreateVehicleTypesComponent implements OnInit {
  id = '';
  vehicleType = new VehicleType();

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
      this.lookup.getVehicleTypeById(this.id).subscribe(res => {
        this.vehicleType = res.Value
      })
    }
  }


  ngOnInit() {
    this.titleService.setTitle(this._translate.instant('CeateNewVehicleTypes'));

    this._translate.onLangChange.subscribe((res) => {
      this.titleService.setTitle(this._translate.instant('CeateNewVehicleTypes'));
    });
  }

  save() {
    console.log(this.vehicleType);
    this.cpService.saveVehicleType(this.vehicleType).subscribe(res => {
      if(!res.IsErrorState) {
        this._location.back();
      }
    })
  }


  cancel() {
    this._location.back();
  }
}
