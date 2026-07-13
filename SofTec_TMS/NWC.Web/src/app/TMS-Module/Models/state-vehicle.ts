import { Lookup } from "./common/lookup";

export class StateVeicle {
  ID: number;
  VehicleID: string;
  VehicleCodePlateNo: string;
  DriverID: string;
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
  VehicleStatusColorCode: string;
  dispatch: boolean = false;
  Capacity: number;
  PermitNumber: string;
  IsHold: boolean = false;
  PermitStatus: string;
  Expirationdate: Date;
  CitySettingsShowInvoice: boolean;
  CitySettings_ShowCustomerClassEntryGate: boolean;
  //VehicleCustomerLocationClassId: number;
  VehicleCustomerLocationClassesIds: number[] = []

  CustLocationClasses: Lookup<number>[];

}
