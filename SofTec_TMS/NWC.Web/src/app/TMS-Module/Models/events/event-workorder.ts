import { AccessoryDTO } from "../Accessory";

export class EventWorkOrder {
    WorkOrderID: number;
    OrderNumber: string;
    CreatedBy: string;
    ScheduledDeliveryTime: Date;
    StatusID: number;
    StatusReasonID: number;
    StatusComment: string;
    StatusTime: Date;
    OrderQuantity: number;
    CustomerLocationID: number;
    ServiceTypeID: number;
    CustomerAccountId: number;
    StationID: string;
    Accessories: AccessoryDTO[];
    ConfirmationCode: string;
    RecieverName: string;
    RecieverMobile: string;
    Comments: string;

    VehicleID: string;
    DriverID: string;
    SourceApplication: string;


    BC_NoOfOrders: number;
    BC_HoldIntervalMin: number;
    BC_StartingTime: Date;

    CategoryID: number|null;

}