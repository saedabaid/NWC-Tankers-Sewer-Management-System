import { FilterModel } from '../common/filter-model';

export enum DateperiodEnum { RequestDate = 1, ScheduleDate = 2, LastStatusModificationDate = 3, }


export class StaffSearchCriteria {
  FilterModel = new FilterModel<string>();
  Id: string[];
  Name: string[];
  branchId: string[];
  RoleId: string[];
  stationId: string[];
}

export enum Operator {

    LessThan = 1,
    Equal = 2,
    MoreThan = 3,
}



