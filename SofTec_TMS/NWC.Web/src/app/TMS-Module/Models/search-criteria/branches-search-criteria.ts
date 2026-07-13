import { OrderByExepression } from "../enums/order-by-exepression";
import { SortDirection } from "../enums/sort-direction";
import { PageFilter } from "../common/page-fillter-model";

export class BranchesSearchCriteriaDTO {
  PageFilter: PageFilter = new PageFilter();
  OrderBy: OrderByExepression;
  SortDirection: SortDirection;
  IsDeleted = false;
  SearchKeyword: string;
}
