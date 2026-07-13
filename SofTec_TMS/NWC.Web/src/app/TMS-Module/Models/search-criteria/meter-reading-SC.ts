import { FilterModel } from "../common/filter-model";

export class MeterReadingSearchCriteria {
    FilterModel = new FilterModel<string>();
    Id: number;

    // Advanced
    DeviceMeterIDs: number[] = [];
    DateTimeFrom: Date
    DateTimeTo: Date ;

}