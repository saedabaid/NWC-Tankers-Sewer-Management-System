import { PageFilter } from "@tms-models/common/page-fillter-model";
import { OrderByExepression } from "@tms-models/enums/order-by-exepression";
import { SortDirection } from "@tms-models/enums/sort-direction";

export class ZonePriceSCDTO{
    PageFilter :PageFilter = new PageFilter();
    OrderBy :OrderByExepression ;
    SortDirection  :SortDirection ;
    StationIDs: string[]
}