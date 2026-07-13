import { PageFilter } from "../common/page-fillter-model";

export class ContractViolationSC {
    PageFilter = new PageFilter;
    Id: number;

    AreaIDs: string[];
    CityIDs: string[];
    StationIDs: string[];
    CategoryIds: number[];
    PaymentIds: number[];
    StatusIds: number[];
    CancelReasonIds: number[];

    ViolationTicketNumber: string;
    Contract: string;
    Term: string;
    Vehicle: string;
    Driver: string;

    IsDeleted: boolean;

    ViolationDateFrom: Date;
    ViolationDateTo: Date;

    ExcelFlage: boolean;

}
export class ContractApprovalViolation {
  PageFilter = new PageFilter;
  LandmarkID: string;
}
