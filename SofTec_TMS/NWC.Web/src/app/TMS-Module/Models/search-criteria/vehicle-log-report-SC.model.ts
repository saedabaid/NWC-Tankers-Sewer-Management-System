import { PageFilter } from "../common/page-fillter-model";

export class VehicleLogReportSC {
    PageFilter: PageFilter = new PageFilter();

    AreaIDs: string[];
    CityIDs: string[];
    StationIDs: string[];
    ServiceTypeIDs: number[];

    DateTimeFrom: Date;
    DateTimeTo: Date;

    LogType: number;
    OrderNumber: string;
    Vehicle: string;
    Driver: string;

    ExcelFlage: boolean;

}
