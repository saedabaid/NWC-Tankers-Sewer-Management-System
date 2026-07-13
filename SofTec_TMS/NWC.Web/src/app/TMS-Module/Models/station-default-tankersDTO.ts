import { Lookup } from "./common/lookup";

export class StationDefaultTankersDTO {
  StationID !: string;
  AllTankers: Lookup<number>[] = [];
  Capacities: Lookup<number>[] = [];
}
