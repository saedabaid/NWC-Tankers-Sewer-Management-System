import { OrderByExepression } from "../enums/order-by-exepression";
import { SortDirection } from "../enums/sort-direction";
import { PageFilter } from "../common/page-fillter-model";

export class StationSettingsSC {
  PageFilter: PageFilter = new PageFilter();
  OrderBy: OrderByExepression;
  SortDirection: SortDirection;
  IsDeleted = false;
  SearchKeyword: string;
  AreaIds: string[];
  CityIds: string[];
  Status?: number;
  CustomerClass?: number;
  ServiceType?: number;
}
