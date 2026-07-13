import { OrderDetails } from "./order-details";

export class OrderExcel {
OrderNo :string;
//OrderConfirmationCode:string;
CustomerName :string;
CustomerCode :string;
CustomerLocation :string;
CustomerClass :string;
CustomerPriority:string;
CustomerCity :string;
CustomerZone :string;
stationName:string;
VehiclePlateNo :string;
AssignedDriverName :string;
DriverCode:string;
ServiceType :string;
RequestedAt :Date;
ScheduledTime :Date;
Quantity :number;
Accessories :string; 
Status :string;
LastStatusModificationDate :string;
Reason  :string; 
SourceApplication: string;
OrderCategory: string;

constructor ( order : OrderDetails) {
    this.OrderNo = order.OrderNumber;
    //this.OrderConfirmationCode = order.ConfirmationCode;
    this.CustomerName = order.CustomerName;
    this.CustomerCode = order.CustomerCode;
    this.CustomerLocation = order.CustomerAddress;
    this.CustomerClass = order.ClassName;
    this.CustomerPriority = order.PriorityName;
    this.CustomerCity = order.CityName;
    this.CustomerZone = order.ZoneName;
    this.stationName = order.StationName;
    this.VehiclePlateNo = order.VehicleCodePlateNo;
    this.AssignedDriverName = order.DriverName;
    this.DriverCode = order.DriverCode;
    this.ServiceType = order.ServiceType;
    this.RequestedAt = order.RequestTime;
    this.ScheduledTime = order.ScheduledDeliveryTime;
    this.Quantity = order.OrderQuantity;
    this.Accessories = order.AccessoryNames; 
    this.Status = order.LastStatusName;
    this.LastStatusModificationDate = order.LastStatusTime.toString();
    this.Reason  = order.LastStatusReason; 
    this.SourceApplication = order.SourceApplication
    this.OrderCategory = order.CategoryName
}
}