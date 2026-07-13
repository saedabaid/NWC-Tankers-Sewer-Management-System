import { PageFilter } from "../common/page-fillter-model";

export class contractTariffReport {
    PageFilter = new PageFilter;

    AreaIDs: string[];
    CityIDs: string[];
    StationIDs: string[];
    ZoneIDs: number[];
  TarrifStatus: number;
  TankerCapacities: number[];

    ExcelFlage = false;
}
