export class WorkOrderChangeLog {
  ID: number;
  WorkOrderId: number;
  ActionLogTypeID: number;
  //EventOrderId: number;
  //ParentWorkOrderId: number;
  //EventId: number;
  CreatedBy: string;
  CreatedByName: string;
  CreateTime: Date;
  //ventTypeID: number;
  StatusID: number;
  StatusName: string;
  StatusComment: string;
  DeassignReasonID: number;
  DeassignReasonName: string;
  OrderQuantity: number;
  CustomerLocationID: number;
  ServiceTypeID: number;
  NetCost: number;
  TotalCost: number;
  Accessories: string;
  Distance: number;
  VehicleID: string;
  DriverID: string;
  VehicleStatusId: number;
  ActionLogTypeName: string;
  VehicleStatusName: string;
  ServiceTypeName: string;
  VehicleCodePlateNo: string;
  DriverName: string;
  //OrderNumber: string;
  ScheduledDeliveryTime: Date;
  StatusTime: Date;
  //IsPaid: boolean;
  CommentId:number;
  Comment: string;
  CommentIsDeleted: boolean;
  StationName: string;

  CustomerAddress: string;
  ZoneName: string;
  CityName: string;


  PreviousStatusName: string;
  PreviousStatusTime: Date;
  PreviousStatusComment: string;
  PreviousVehicleCodePlateNo: string;
  PreviousVehicleStatusName: string;
  PreviousDriverName: string;
  PreviousDeassignReasonName: string;
  PreviousAccessories: string;
  PreviousDistance: number;
  PreviousOrderQuantity: number;
  PreviousScheduledDeliveryTime: Date;
  PreviousIsPaid: boolean;



  PreviousServiceTypeName: string;
  PreviousStationName: string;
  PreviousCustomerAddress: string;
  PreviousZoneName: string;
  PreviousCityName: string;
  PreviousTotalCost: number;
}
