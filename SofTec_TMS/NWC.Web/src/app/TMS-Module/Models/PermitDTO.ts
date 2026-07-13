import { AttachmentDTO } from "src/app/shared/datamodels/attachment-dto";
import { StaffModel } from "./staff-model";
import { VehiclePermitDTO } from "./vehicle-dto";

export class PermitDTO {
    ID: string;
    DriverID: string;
    TransporterID: string;
    PermitNumber: string;
    Discerption: string;
    OrganizationName: string;
    CRnumber: string;
    DriverIDSTR: string;
    StartDate: Date;
    Expirationdate: Date;
    Availabletimefrom: string;
    Availabletimeto: string;
    TankerCategory: string = "-1";
    LastValidationDate: Date;
    LastMaintenanceDate: Date;
    Ismaramy: boolean = false;
    IsHold: boolean = false;
    Maramu: string;
    TripsNumber: string;

    Area: string;
    City: string;
    Station: string;
    PermitStatus: string;

    DetectionformFileAttachments: AttachmentDTO[] = [];
    DeclarationFileAttachments: AttachmentDTO[] = [];
    OtherFileAttachments: AttachmentDTO[] = [];

    Driver:StaffModel;
    Tanker: VehiclePermitDTO;
}
