import { PageFilter } from "../common/page-fillter-model";

export class DeferredOrderSC {

    PageFilter = new PageFilter;

    DateTimeFrom: Date;
    DateTimeTo: Date;
    StatusIds: number[];
    OrderNo: string;
    MobileNo: string;
    PersonIdValue: string;

    Id: number;

    ExcelFlage: boolean;

}