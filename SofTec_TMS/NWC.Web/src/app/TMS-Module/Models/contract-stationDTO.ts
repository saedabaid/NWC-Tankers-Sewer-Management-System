import { ContactPersonDTO } from "./contact-personDTO";

export class ContractStationDTO{
    AreaIDs: string[] = [];
    CityIDs: string[] = [];
    StationIDs: string[] =[];
    ContactPerson :ContactPersonDTO = new  ContactPersonDTO() ;
    ContractID :number;
    ContractStationID :number;
}