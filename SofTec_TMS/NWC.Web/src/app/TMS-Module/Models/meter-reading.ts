export class MeterReading {

    ID: number;
    DeviceMeterID: number;
    MeterReading: number;
    ReadingTime: Date;
    ReadingComment: string;

    //Device meter
    ConnectorTubeNumber: string;
    MeterSerialNumber: string;
    ManholeNumber: string;
    IsScadaAutoReading: boolean;
    ServiceTypeID: number;

    //station
    StationID: string;
    StationName: string;
    StationCode: string;

}