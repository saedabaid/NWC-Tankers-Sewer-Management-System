import { Lookup } from "./common/lookup";

export class VehicleSettingsNWC {
    
    VehicleID: string;
    Code: string;
    Status: number;
    IsDeleted: boolean;
    AccessoryIDs: number[];
    CustLocationClassIDs: number[];
    ServiceTypeID: number;
    StationID: string;
    StationName: string;
    CodePlateNumber: string;
    Capacity: number;
    transporterTypeId: number;
    transporterTypeNameEn: string;
    transporterTypeNameAr: string;
    
    Accessories: Lookup<number>[];
    CustLocationClasses: Lookup<number>[];
    ServiceTypes: Lookup<number>[];
    CheckToSave: boolean = false;
    
  }