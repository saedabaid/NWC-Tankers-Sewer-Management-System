export class DirectionsServiceRequestObject {
    source: string;
    destination: string;
    waypoints: string;
    mode: string = "driving";
    language: string = "en";

}