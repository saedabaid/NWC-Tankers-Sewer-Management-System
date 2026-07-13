import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ControlPanelService } from '@tms-services/control-panel.service';
import {Location} from '@angular/common';
import { LookupService } from '@tms-services/lookup.service';
import { LandmarkType } from '@tms-models/landmark-type';


@Component({
  selector: 'app-create-landmark-types',
  templateUrl: './create-landmark-types.component.html',
  styleUrls: ['./create-landmark-types.component.scss']
})
export class CreateLandmarkTypesComponent implements OnInit {

  id = '';
  landmarkType = new LandmarkType();

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
        this.landmarkType = res.Value
      })
    }
  }


  ngOnInit() {
    this.titleService.setTitle(this._translate.instant('CeateNewLandmarkType'));

    this._translate.onLangChange.subscribe((res) => {
      this.titleService.setTitle(this._translate.instant('CeateNewLandmarkType'));
    });
  }

  save() {
    this.cpService.saveLandmarkType(this.landmarkType).subscribe(res => {
      if(!res.IsErrorState) {
        this._location.back();
      }
    })
  }


  cancel() {
    this._location.back();
  }

}
