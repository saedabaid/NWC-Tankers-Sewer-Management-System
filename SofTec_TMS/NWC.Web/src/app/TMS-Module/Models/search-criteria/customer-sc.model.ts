import { PageFilter } from "../common/page-fillter-model";

export class CustomerSC {
    PageFilter = new PageFilter;
    Id: number;
    Customer: number;

    AreaIDs: string[];
    CityIDs: string[];
    ZoneId: number;
    ServiceTypeId: number;

    ExcelFlage: boolean;

}