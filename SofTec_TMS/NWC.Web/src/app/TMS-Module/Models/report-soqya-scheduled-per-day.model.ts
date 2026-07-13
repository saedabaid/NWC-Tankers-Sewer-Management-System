export class Report_SoqyaScheduledPerDay {
    StationName: string;
    DayOfMonth: Date;
    TotalCounts: number;
    SumQuantities: number;
    CapacityList: CapacitySum[] = [];

}

export class CapacitySum {
    Quantity: number;
    SchedulesCount: number;
}