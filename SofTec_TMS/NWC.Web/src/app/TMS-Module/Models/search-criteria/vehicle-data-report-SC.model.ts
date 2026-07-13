import { PageFilter } from "../common/page-fillter-model";

export class VehicleDataReportSC {
    PageFilter: PageFilter = new PageFilter();

    AreaIDs: string[];
    CityIDs: string[];
    StationIDs: string[];
    ServiceTypeIDs: number[];
    StatusIDs: number[];

    // DateTimeFrom: Date;
    // DateTimeTo: Date;

    Vehicle: string;
    Driver: string;

    ExcelFlage: boolean;

}
