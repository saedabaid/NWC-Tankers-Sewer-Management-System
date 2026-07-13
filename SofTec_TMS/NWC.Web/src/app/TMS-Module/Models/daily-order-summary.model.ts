export class DailyOrderSummary {
    ServiceTypeID: number;
    ServiceTypeName: string;
    StationID: string;
    StationName: string;
    StationCode: string;
    
    TotalCount: number;
    TotalSum: number;
    FailedToDeliverCount: number;
    FailedToDeliverSum: number;
    DeliveredCount: number;
    DeliveredSum: number;
    CancelledCount: number;
    CancelledSum: number;
    CreateDate: Date;
}


export class DailyOrderSummaryExcel {
    Station: string;
    ServiceType: string;
    Date: string;

    TotalOrdersCount: number;
    TotalOrdersSum: number;
    DeliveredCount: number;
    DeliveredSum: number;
    FailedToDeliverCount: number;
    FailedToDeliverSum: number;
    CancelledCount: number;

}

