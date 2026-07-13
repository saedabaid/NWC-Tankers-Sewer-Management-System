import { Lookup } from "./common/lookup";

export class ZoneDTO {
    ID: number;
    Code: string = '';
    Name: string = '';
    CityID: string;
    AreaID: string;
    ZoneWithoutTanker: boolean;
    BackupStations: Station[] = [];
    MainStation: Station = new Station();
    AllowedTankerTypes: Lookup<string>[] = [];
    RestrictedTankerTypes: Lookup<string>[] = [];

    ExcelSheetRowId: number;
    ExcelValidation: string;
    CityName: string;
    IntegrationID: string;
}
export class Station {
    ID: string = null;
    Distance: number = null;
    StationName: string;
}