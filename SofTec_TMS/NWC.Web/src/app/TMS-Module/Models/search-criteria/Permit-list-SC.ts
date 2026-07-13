import { PageFilter } from "../common/page-fillter-model";

export class PermitListSC {
    PageFilter: PageFilter = new PageFilter();

    AreaIDs: string[];
    CityIDs: string[];
    StationIDs: string[];
    DriverID: string;
    DriverMobile: string;
    TankerCode: string;
    PermitStatus: string="-1";
    ExpirationdateFrom: Date;
    ExpirationdateTo: Date;

    
}
