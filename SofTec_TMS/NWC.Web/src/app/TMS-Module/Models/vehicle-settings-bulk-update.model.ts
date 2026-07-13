import { VehicleSettingsSC } from "./search-criteria/vehicle-settings-SC.model";

export class VehicleSettingsBulkUpdate {
    ApplyOption: number = 2;
    VehicleSettingsSCModel: VehicleSettingsSC;
    ApplyVehicleIds: string[];

    ServiceTypeID: number;
    Capacity: number;
    StationID: string;
    AccessoryIDs: number[];
    CustLocationClassIDs: string[];

}
