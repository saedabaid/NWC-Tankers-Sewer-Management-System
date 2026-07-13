import { OrderByExepression } from "../enums/order-by-exepression";
import { SortDirection } from "../enums/sort-direction";
import { PageFilter } from "../common/page-fillter-model";


export class ZoneSearchCriteriaDTO {
   PageFilter :PageFilter = new PageFilter();
   OrderBy :OrderByExepression ;
   SortDirection  :SortDirection ;
   NameOrCode :string ;
   IsDeleted : boolean = false;
   AreaIDs: string[]
   CityIDs: string[]
   StationIDs: string[]
  
}