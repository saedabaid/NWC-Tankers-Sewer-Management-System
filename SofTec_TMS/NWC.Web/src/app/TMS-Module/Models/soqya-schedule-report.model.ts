export class SoqyaScheduleReportDTO {
    CustomerAccountId: number;
    AccountId_Integration: string;
    SoqyaBalance: number;
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
    CancelledSum: number;
    DeliveredSum: number;
    NotDeliveredSum: number;
    ScheduledSum: number;
    Year: number;
    Month: number;
    RemainingQty: number;

    helper_scheduleDate: Date;
    
}
