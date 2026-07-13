import { PageFilter } from "../common/page-fillter-model";

export class DailyOrderReportSC {
    PageFilter: PageFilter = new PageFilter();

    AreaIDs: string[];
    CityIDs: string[];
    StationIDs: string[];
    ServiceTypeIDs: number[];

    DateFrom: Date;
    DateTo: Date;

    ExcelFlage: boolean;

}
