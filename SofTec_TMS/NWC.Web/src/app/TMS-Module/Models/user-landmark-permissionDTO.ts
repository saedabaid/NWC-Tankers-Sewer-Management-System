import { PageFilter } from "./common/page-fillter-model";
import { OrderByExepression } from "./enums/order-by-exepression";
import { SortDirection } from "./enums/sort-direction";
import { Lookup } from "./common/lookup";

export class UserStationPermissionDTO{
    StaffID: string;
    FullName: string;
    PermittedLandmarkIDs: string[];
    PermittedServiceIDs :number[];
    DBPermittedLandmarks: string[];

    PermittedLandmarks: Lookup<string>[];
    PermittedServices: Lookup<number>[];
    
    PageFilter: PageFilter = new PageFilter();
    OrderBy: OrderByExepression ;
    SortDirection: SortDirection ;
    CheckToSave: boolean = false;

    bindingModel_Landmarks: Lookup<string>[];
    bindingModel_SelectedLandmarks: Lookup<string>[];
    


}