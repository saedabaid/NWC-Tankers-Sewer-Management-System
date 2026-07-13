import { OrderByExepression } from '../enums/order-by-exepression';
import { SortDirection } from '../enums/sort-direction';
import { PageFilter } from '../common/page-fillter-model';

export class VehicleSearchCriteriaDTO {
   PageFilter: PageFilter = new PageFilter();
   OrderBy: OrderByExepression ;
   SortDirection: SortDirection ;
   PlateNoOrCode: string;
   IsDeleted = false;

   BrandIDs: string[];
   ModelIDs: string[];
   YearIDs: string[];
   VehicleTypeIDs: string[];
   GroupIDs: string[];
   StatusIDs: string[];
   BranchIDs: string[];
   SubBranchIDs: string[];
   LandmarkIDs: string[];

   NextExaminationDate: string;
   LicenceExpirationDate: string;
   InsuranceEndDate: string;
   EntranceDate: string;
   ChassisNo: number;
   SIMCardNo: number;
   DeviceCode: number;
   Name: string;

}
