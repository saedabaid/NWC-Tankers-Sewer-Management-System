import { Configuration } from "src/app/shared/configurations/shared.config";

export class DashboardSC {
  //PageFilter: PageFilter = new PageFilter();

  StatusIDs: number[];
  AreaIDs: string[];
  CityIDs: string[];
  StationIDs: string[];
  ClassName: string;
  ServiceTypeID: number = +Configuration.Dashboard.DefaultServiceTypeId;
  DateFrom: Date;
  DateTo: Date;

}
