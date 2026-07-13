import { FilterModel } from "../common/filter-model";

export class DeviceMeterSearchCriteria {
    FilterModel = new FilterModel<string>();
    Id: number;

    // Advanced
    ConnectorTubeNumber: string;
    MeterSerialNumber: string;
    ManholeNumber: string;
    ServiceTypeID: number;
    StationID: string;

}