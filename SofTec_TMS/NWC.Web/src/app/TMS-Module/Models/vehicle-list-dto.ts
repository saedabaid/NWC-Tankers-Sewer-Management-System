export class VehicleListDTO {
  ID: string;
  Code: string;
  PlateNo: string;
  ChassisNo: string;
  Group: string;
  Allocation: string;
  TransporterType: string;
  TransporterTypeName: string;
  Drivers: Array<string>;
  DriverNames: string;
  Status?: number;
  StatusName: string;
  DeviceCode: string;
  SIMCardNo: string;
  Image: string;
  ProductionYear: string;
  ProductionYearName: string;
  Branch: string;
  BranchName: string;
  SubBranch: string;
  SubBranchName: string;
  CurrentMileage?: number;
  InsuranceNo: string;
}
