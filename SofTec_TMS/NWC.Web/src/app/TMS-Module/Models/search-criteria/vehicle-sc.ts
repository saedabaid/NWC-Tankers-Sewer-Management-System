import { FilterModel } from "../common/filter-model";
import { PageFilter } from "../common/page-fillter-model";

export class VehicleSC {
  //FilterModel = new FilterModel<string>();
  PageFilter = new PageFilter;
  StatusIDList: number[];
  VechilePlateNumberOrCode: string;
  Driver: string;
  DriverCode: string;
  VehicleIDs: string[];
  DriverIDs: string[];
  ServiceTypeID:number;
}
