export class WOVArrivedStation {
  WorkOrderID: number;
  VehicleID: string;
  ConfirmationCode: string;
  IsPaid: boolean;
  //VehicleCustomerClassId: number;
  VehicleCustomerLocationClassesIds: number[] = []

}
