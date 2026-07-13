import { AttachmentDTO } from "src/app/shared/datamodels/attachment-dto";

export class Contractor {
    ID: number;
    Code: string;
    ContractorFullName: string;
    CommercialIDNumber: string;
    TaxNumber: string;
    MOI: string;
    CompanyAddressCityID: string;
    CompanyAddressPostalCode: string;
    CompanyAddress: string;
    IsActive: boolean;
    IsBlackListed: boolean

    // Contact Person
    FirstName: string;
    SecondName: string;
    LastName: string;
    FullName: string;
    Position: string;
    MobileCode: string;
    Mobile: string;
    LandlineNumbeCode: string;
    LandlineNumber: string;
    Email: string;
    PersonalIDType: number;
    PersonalIDNumber: string;
    PersonAddressPostalCode: string;
    PersonAddress: string;
    
    AreaId: string;
    AreaName: string;
    CityName: string;

    ContractorAttachments: AttachmentDTO[] = [];

}