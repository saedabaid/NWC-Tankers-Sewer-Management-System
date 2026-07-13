export class VehiclePerformance {
  Id: string;
  Code: string;
  PlateNumber: string;
  Drivers: Array<string>;
  DriverNames: string;
  Status: number;
  StatusName: string;
  Branch: string;
  BranchName: string;
  SubBranch: string;
  SubBranchName: string;
  Station: string;
  StationName: string;
  Capacity: number;
  TotalExitTripsCount: number;
  TotalDeliveredOrdersCount: number;
  ResidentialDeliveredOrdersCount: number;
  CommercialDeliveredOrdersCount: number;
  ViolationsCount: number;
  PaidViolationsCount: number;
  UnPaidViolationsCount: number;
  PartiallyPaidViolationsCount: number;
  ClosedByCodeOrdersCount: number;
  ClosedByCodeOrdersPercentage: number;
}
