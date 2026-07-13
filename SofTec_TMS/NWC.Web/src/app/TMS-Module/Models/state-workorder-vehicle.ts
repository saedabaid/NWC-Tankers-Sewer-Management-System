import { Lookup } from "./common/lookup";

export class StateWorkOrderVeicle {
  ID: number;
  VehicleID: string;
  DriverName: string;
  DriverMobileNumber: string;
  DriverCode: string;
  VehicleStatusId: number;
  VehicleStatusName: string;
  VehicleStatusNameAr: string;
  IsAvailable: boolean;
  AvailableByID: string;
  AvailableTime: Date;
  IsOutOfService: boolean;
  OutOfServiceByID: string;
  OutOfServiceTime: Date;
  OutOfServiceReason: number;
  OutOfServiceComment: string;
  IsBlacklisted: boolean;
  BlacklistedByID: string;
  BlacklistedTime: Date;
  BlacklistedReason: number;
  BlacklistedComment: string;
  IsParking: boolean;
  ParkingByID: string;
  ParkingTime: Date;
  OrderCreateTime: Date;

  IsAssigned: boolean;
  IsDeassigned: boolean;
  IsInService: boolean;
  WorkOrderID: number;
  WorkOrderNumber: string;
  TotalCost: number;
  VehicleCode: string;
  VehicleCodePlateNo: string;
  LastStatusTime: Date;
  ConfirmarionCode: string;
  VehicleStatusColorCode: string;
  IsPaid: boolean;
  ConfirmationCode: string;
  Capacity: number;
  CityId: string;
  CitySettings_ShowInvoice: boolean;
  CitySettings_ShowCustomerClassEntryGate: boolean;
  //VehicleCustomerLocationClassId: number;
  VehicleCustomerLocationClassesIds: number[] = []

  CustLocationClasses: Lookup<number>[];
  PermitNumber: string;
  IsHold: boolean = false;
  PermitStatus: string;
  Expirationdate: Date;
}
