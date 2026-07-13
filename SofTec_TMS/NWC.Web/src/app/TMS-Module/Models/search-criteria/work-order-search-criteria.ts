import { FilterModel } from "../common/filter-model";

export enum DateperiodEnum { RequestDate = 1, ScheduleDate = 2, LastStatusModificationDate = 3, };


export class WorkOrderSearchCriteria {
    FilterModel = new FilterModel<string>();
    CustomerIDs: number[]
    ClassIDs: number[]
    PriorityIDs: number[]
    ServiceTypeIDs: number[]
    AreaIDs: string[]
    CityIDs: string[]
    ZoneIDs: number[]
    StationIDs: string[]
    StatusIDs: number[]
    VehicleIDs: string[]
    DriverIDs: string[]
    CategoryIDs: number[];
  SourceApps: string[];
    DatePeriod: DateperiodEnum;
    DateTimeFrom: Date;
    DateTimeTo: Date;
    VehicleID: String;
    excelFlage: boolean;

    CustomerIdNumber: string;
    CustomerMobile: string;
    TanckerCapacityAddIds: number[];
    Price: number;
    _operator: Operator

}

export enum Operator {

    LessThan = 1,
    Equal = 2,
    MoreThan = 3,
}
