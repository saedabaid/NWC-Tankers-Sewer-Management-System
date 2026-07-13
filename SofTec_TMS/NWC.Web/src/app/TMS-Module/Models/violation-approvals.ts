import { Lookup } from "./common/lookup";
import { StaffListDto } from "./staff-list-dto";

export class violationapprovalsDto {
  Id:string;
  Staff: Array<StaffListDto> = [];
  StaffId: string;

  Branch: string;
  BranchName: string;
  SubBranch: string;
  SubBranchName: string;
  Landmark: string;
  LandmarkName: string;
  LevelNo: number=1;
}
