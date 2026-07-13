import { Lookup } from "./common/lookup";

export class  AccessoryDTO {
    constructor(item: Lookup<number>) {
      this.ID = item.Id;
    }
    ID:number;
    Code :string;
    Name :string;
}