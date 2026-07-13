import { PageFilter } from "../common/page-fillter-model";

export class VehiclePerformanceReportSC {
    PageFilter: PageFilter = new PageFilter();

    AreaIDs: string[];
    CityIDs: string[];
    StationIDs: string[];
    ServiceTypeIDs: number[];

    DateTimeFrom: Date;
    DateTimeTo: Date;
    VehicleCode: string;

    ExcelFlage: boolean;
}
