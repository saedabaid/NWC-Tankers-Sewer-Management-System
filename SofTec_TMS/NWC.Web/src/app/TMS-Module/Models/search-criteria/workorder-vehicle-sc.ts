import { FilterModel } from "../common/filter-model";
import { PageFilter } from "../common/page-fillter-model";

export class WorkOrderVehicleSC {
  //FilterModel = new FilterModel<string>();
  PageFilter = new PageFilter;
  VechilePlateNumberOrCode: string;
  Driver: string;
  DriverCode: string;
  IsAssigned: boolean;
  IsDeassigned: boolean;
  WorkOrderNumber: string;
  WorkOrderStatusIDs: number[];
  VehicleIDs: string[];
  DriverIDs: string[];
  ServiceTypeID:number;
}
