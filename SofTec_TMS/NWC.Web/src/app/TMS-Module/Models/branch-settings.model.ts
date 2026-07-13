export class BranchSettingsDTO {
  Id: string;
  name: string = "";
  code: string;
  ShowPrice: boolean;
  ShowInvoice: boolean;
  TankerQuotaNo: number = 0;
  ShowCustomerClassEntryGate: boolean;
  AutoCancelationNewOrdersHours: number = 0;
  AutoCancelationOnHoldOrdersHours: number = 0;
  CheckToSave: boolean;
  ValidateConfermationCode: boolean;
  RequiresPayment: boolean;
  ValidationApprovalRequired: boolean;
  ApprovalLevelsCount: number = 0;
}
