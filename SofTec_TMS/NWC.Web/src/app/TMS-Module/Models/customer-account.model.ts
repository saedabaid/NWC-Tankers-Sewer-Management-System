export class CustomerAccount {
    ID: number;
    CustomerId: number;
    CustomerLocationId: number;

    CL_Code: string;
    CL_ZoneID: number;
    CL_ClassID: number;
    CL_PriorityID: number;
    CL_CategoryID: number;
    CL_StatusID: number;
    // Latitude: number;
    // Longitude: number;
    CL_Address: string;
    AccountId_Integration: string;
    ServiceTypeId: number;

    //helpers
    CL_ZoneName: string;
    //className: string;
    CL_ClassAr: string;
    CL_ClassEn: string;
    //ServiceTypeName: string;
    ServiceTypeAr: string;
    ServiceTypeEn: string;

    IsDeleted: boolean;

}