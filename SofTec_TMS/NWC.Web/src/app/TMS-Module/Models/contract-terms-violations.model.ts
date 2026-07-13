import { AttachmentDTO } from "src/app/shared/datamodels/attachment-dto";

export class ContractTermsViolationsDTo {
    Id: number;
    ContractTermId: number;
    ViolationTicketNumber: string = '';
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

    ViolationAttachments: AttachmentDTO[] = [];
  Show: boolean;
  IsFinalDecision: boolean;
}
