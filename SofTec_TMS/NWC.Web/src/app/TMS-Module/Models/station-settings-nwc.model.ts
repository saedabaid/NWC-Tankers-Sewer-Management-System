import { Lookup } from "./common/lookup";

export class StationSettingsNWC {
  StationId: string;
  StationName: string = "";
  AreaId: string;
  AreaName: string;
  CityId: string;
  CityName: string;
  StationServiceIds: number[];
  StationServiceNames: string;
  CustomerClassIds: number[];
  CustomerClassNames: string;
  StatusId: number;
  StatusName: string;
  IsVirtual: boolean;
  AllCapacities: Lookup<number>[] = [];
  SelectedCapacities: Lookup<number>[] =[];
}
