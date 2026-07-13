export class WorkOrderPlannedRoutDTO {
    ID :number;
    WorkOrderID :number;
    CreatedTime :Date;
    CreatedBy :string;
    IsDeleted :boolean;
    LastModifiedBy :string;
    LastModifiedDate :Date;
    RouteJSON:string;
    RouteLatLngString:string;
    DrivingTime :Date;
    Distance :number;

}