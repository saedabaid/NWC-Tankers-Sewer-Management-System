
export class SoqyaScheduleDTO {
    Id: number;
    IsDeleted: boolean;
    Quantity: number;
    ScheduleDate: Date;
    TimeSlotFrom: string;
    TimeSlotTo: string;
    // CustomerBalanceId :number;
    // Balance :number;
    // BalanceYearMonth :string;
    SoqyaBalance: number;
    CustomerAccountId: number;
    CustomerId: number;
    CustomerLocationId: number;
    CustomerName: string;
    CustomerIdTypeId: number;
    CustomerIdNumber: string;
    CustomerMobile: string;
    CustomerLocationAddress: string;
    ZoneID: number;
    ZoneName: string;
    CityID: string;
    CityName: string;
    StationId: string;
    StationName: string;
    WorkOrderId: number;
    OrderNumber: string;
    LastStatusID: number;
    LastStatusNameEn: string;
    LastStatusNameAr: string;
    StatusColor: string;
    MonthYearAddIds: number[];
    ScheculeDayListAdd: number[];

    ExcelSheetRowId: number;
    ExcelValidation: string;
    AccountId:string;

    }

export class SoqyaBalanceDTO {
    Balance: number;
    UsedQuantity: number;
}