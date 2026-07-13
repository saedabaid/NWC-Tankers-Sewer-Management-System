export interface IConfiguration{
   production:boolean,
   queryEndpoint: string,
   commandEndpoint:string,
   authenticationEndpoint: string,
   signalREndpoint: string
 }

 export const environment: IConfiguration = window['dynamic_confg'];
