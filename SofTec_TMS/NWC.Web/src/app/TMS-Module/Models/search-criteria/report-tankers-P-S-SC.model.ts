import { PageFilter } from "../common/page-fillter-model";

export class ReportTankersPermissionsStatusSC {
    PageFilter = new PageFilter;

    AreaIDs: string[];
    CityIDs: string[];
    StationIDs: string[];

    Tanker: string;

    PermissionStatus: number;
    LicenseStatus: number;

    PermissionExpiryDateFrom: Date;
    PermissionExpiryDateTo: Date;
    LicenseExpiryDateFrom: Date;
    LicenseExpiryDateTo: Date;

    ExcelFlage = false;
}