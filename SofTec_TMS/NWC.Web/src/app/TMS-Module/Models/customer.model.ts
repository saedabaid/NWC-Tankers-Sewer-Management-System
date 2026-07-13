import { CustomerAccount } from "./customer-account.model";

export class Customer {

    ID: number;
    Code: string;
    FullName: string;
    IDTypeID: number;
    IDNumber: string;
    Email: string;
    Mobile: string;
    LandlineNumber: string;
    IntegrationId: string;
    customerAccounts: CustomerAccount[] = [];
    
}