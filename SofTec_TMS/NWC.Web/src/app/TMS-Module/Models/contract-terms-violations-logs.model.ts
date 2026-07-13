
export class ContractTermsViolationsLogsDTO {
    Id: number;
    TermViolationId: number;
    ContractTermId: number;
    ViolationTicketNumber: string;
    ViolationLocation: string;
    IncidentTime: Date;
    TotalPenalty: number;
    IssueDate: Date;
    PaymentDueDate: Date;
    PaymentStatusDate: Date;
    ViolationDescription: string;
    IsDeleted: boolean;
    ContractTermCode: string;
    ContractTermDescription: string;
    ContractTermName: string;
    ContractID: number;
    ContractCode: string;
    CategoryId: number;
    CategoryAr: string;
    CategoryEn: string;
    StationID: string;
    StationName: string;
    PaymentStatusId: number;
    PaymentStatusAr: string;
    PaymentStatusEn: string;
    VehicleId: string;
    //VehicleCodePlateNo: string;
    VehicleCode: string;
    VehiclePlateNo: string;

    DriverId: string;
    DriverName: string;
    DriverMobileNumber: string;
    DriverCode: string;

    AddVehicleToBlacklist: boolean;
    TermUnitNoOfDays: number;
 
    StatusId: number;
    StatusNameAr: string;
    StatusNameEn: string;
    CancelReasonId: number;
    CancelReasonAr: string;
    CancelReasonEn: string;

    AttachementsDocumentsIds: string;
    AttachementsDocumentsNames: string;

    CreatedByName: string;
    CreatedDate: string;
    
}

export class ContractTermsApprovalViolationsLogsDTO {

  Id: number;
  IsDeleted: boolean;


  ContractTermId: number;
  ViolationTicketNumber: string;
  ViolationLocation: string;
  TotalPenalty: number;
  IncidentTime: Date;
  IssueDate: Date;

  VehicleId: string;
  VehicleCode: string;
  VehiclePlateNo: string;

  ViolationDescription: string;

  ContractTermCode: string;
  ContractTermDescription: string;
  ContractTermName: string;
  ContractID: number;

 
  PaymentStatusId: number;
  PaymentStatusAr: string;
  PaymentStatusEn: string;
  //VehicleCodePlateNo: string;


  //DriverId: string;
  //DriverName: string;
  //DriverMobileNumber: string;
  //DriverCode: string;

  CreatedByName: string;
  CreatedDate: string;
  LevelID: number;
  DecisionType: string;
}
