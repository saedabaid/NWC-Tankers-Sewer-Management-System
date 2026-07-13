import { Lookup } from "./common/lookup";
import { StaffListDto } from "./staff-list-dto";

export class StaffDTO {
  IDs: string;
  Code: string = "";
  FirstName: string;
  LastName: string;
  Branch: string;
  BranchName: string;
  Landmark: string;
  SubBranch: string;
  MiddleName: string;
  StaffRole: string;
  Mobile: string;
  Email: string;
  PermittedBranch: string;

  ExcelSheetRowId: number;
  ExcelValidation: string;
}
