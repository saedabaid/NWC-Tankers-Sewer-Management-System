import { PageFilter } from "../common/page-fillter-model";

export class SoqyaScheduleReportSC {
    PageFilter: PageFilter = new PageFilter();

    AreaIDs: string[];
    CityIDs: string[];
    ZoneIDs: number[];

    //MonthYearList: number[];
    CustomerId: number;
    CustomerAccountIDs: number[];
    ScheduleStatus: number;

    DateFrom: Date;
    DateTo: Date;

    B_Operator: Operator;
    Balance: number;

    NotScheduled_Operator: Operator;
    NotScheduledQty: number;

    ExcelFlage: boolean;
}

export enum Operator {

    LessThan = 1,
    Equal = 2,
    MoreThan = 3,
}