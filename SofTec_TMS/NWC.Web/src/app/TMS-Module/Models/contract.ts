import { AttachmentDTO } from "src/app/shared/datamodels/attachment-dto";

export class Contract {
    ID: number;
    Code: string;
    ContractorID: number;
    ContractorFullName: string;
    ContractTypeID: number;
    ContractTypeName: string;
    ConfirmationNo: string;
    AwardLetterNo: string;
    ContractStartDate: Date;
    ContractEndDate: Date;
    ContractStatusID: number;
    ContractStatusName: string;
    ContractStatusEnumId: number;
    TerminatedDate: Date;
    IsDeleted: boolean;
    StationsCount: number;

    Description: string;
    TerminationReasonID: number;
    IsTerminated: boolean;   //for edit only

    ContractAttachments: AttachmentDTO[] = [];

}