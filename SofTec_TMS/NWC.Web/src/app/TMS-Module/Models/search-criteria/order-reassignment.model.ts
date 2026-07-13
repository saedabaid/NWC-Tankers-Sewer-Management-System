import { PageFilter } from "../common/page-fillter-model";

export class OrderReassignmentReportSC {
    PageFilter: PageFilter = new PageFilter();

    AreaIDs: string[];
    CityIDs: string[];
    StationIDs: string[];
    OrderNumber: string;
    NewTankerCode: string;
    OldTankerCode: string;
    DateTimeFrom: Date;
    DateTimeTo: Date;

    ExcelFlage: boolean;
}
