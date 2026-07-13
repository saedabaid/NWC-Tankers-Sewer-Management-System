import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
//import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { SearchResult } from 'src/app/TMS-Module/Models/common/search-result';
import { VehicleViolation } from 'src/app/TMS-Module/Models/vehicle-violation.model';
import { ViolationService } from 'src/app/TMS-Module/Services/violation.service';

@Component({
  selector: 'app-vehicle-violation',
  templateUrl: './vehicle-violation.component.html',
  styleUrls: ['./vehicle-violation.component.scss'],
  encapsulation: ViewEncapsulation.None

})
export class VehicleViolationComponent {

  violations: SearchResult<VehicleViolation>;

  constructor(
    private modalService: BsModalService,
    private modalRef: BsModalRef,
    private violationService: ViolationService,

  ) {

    let vehicleID = modalService.config.initialState as string;
    this.violationService.GetVehicleViolations(vehicleID).subscribe(res => {
      if(!res.IsErrorState && res.Value){
        this.violations = res.Value;
      }

    })

  }


}
