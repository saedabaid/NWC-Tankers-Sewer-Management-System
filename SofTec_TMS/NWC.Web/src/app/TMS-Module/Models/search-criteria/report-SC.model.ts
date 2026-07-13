import { PageFilter } from "../common/page-fillter-model";

export class ReportSC {
    PageFilter = new PageFilter;

    AreaIDs: string[];
    CityIDs: string[];
    StationIDs: string[];
    ZoneIDs: number[];

    DateFrom: Date;
    DateTo: Date;

    ExcelFlage = false;
}