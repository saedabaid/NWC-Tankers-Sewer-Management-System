import { System, List } from "@amcharts/amcharts4/core";
export class PagesDTO{
  ID: string;   
  Name: string  ;   
  ModuleID: string;
  Pages: PagesVM[] = [];
} 
export class PagesVM {
  ID: string = null;
  Name: string = null;
  ModuleID: string;
  Path: string;
  IsGPS: boolean;
  IsCarRental: boolean;
  IsMaintenance: boolean;
  status: string;
  exist: boolean;

}

export class RoleVM {
  ModuleID: string = null;
  ModuleName: string = null;
  Name: string;
  status: boolean;
}
export class PagesList {
  ID: string = null;
  Name: string;
}
