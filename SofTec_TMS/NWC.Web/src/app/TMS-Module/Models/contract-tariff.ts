export class ContractTariff {
    ID: number;
    ContractID: number;
    StationID: string;
    StationName: string;
    ZoneID: number;
    ZoneName: string;
    CustomerLocationClassID: number;
    CustomerClassName: string;
    ServiceTypeID: number;
    ServiceTypeName: string;
    DateFrom: Date;
    DateTo: Date;
    CubicMeterCharge: number;
    DistanceCharge: number;
    AfterFirstKM: number;
    TanckerCapacityId: number;
    DateFromHijri: number;
    DateToHijri: number;


    // properties for add only
    StationsAddIds: string[];
    ZoneAddIds: number[];
    CustomerLocationClassAddIds: number[];
    ServiceTypeAddIds: number[];
    TanckerCapacityAddIds: number[];
    //-----------------------------------------------------------

    ExcelSheetRowId: number;
    ExcelValidation: string;
    
}