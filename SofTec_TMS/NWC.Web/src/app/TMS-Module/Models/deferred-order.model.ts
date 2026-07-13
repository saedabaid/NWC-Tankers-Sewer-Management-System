export class DeferredOrder {

    ID: number;
    ORDERNUMBER: string;
    CISDIVISION: string;
    COMMENT: string;
    CONFIRMATIONCODE: string;
    CONTACTMOBILE: string;
    CONTACTNAME: string;
    CREDTTM: string; //exclude
    SCHEDDTTM: string;
    SERVICETYPE: string;
    SOURCEAPPLICATION: string;
    TANKERSIZE: string;
    TRANSACTIONID: string;
    ACCOUNTID: string;
    CUSTOMERCLASS: string;
    MOBILENUMBER: string;
    PERSONID: string;
    PERSONIDTYPE: string;
    PERSONIDVALUE: string;
    PERSONPRIMARYNAME: string;
    PREMISEID: string; 
    XYCOORDINATESGF: string;
    
    StatusId: number;
    ErrorMSG: string;
    LastUpdatedBy: string;
    LastUpdatedTime: Date;
    CreateTime: Date;

    StatusName: string;    

    //helpers
    helper_CustomerClassId: number;
    helper_ServiceTypeId: number;
    helper_ZoneId: number;
    helper_PersonIdTypeID: number;
    helper_latitude: number;
    helper_longitude: number;
    helper_ZoneName: string;
    helper_scheduleTime: Date;

}