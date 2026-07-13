import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { BsModalService } from "ngx-bootstrap/modal";
//import { BsModalService } from 'ngx-bootstrap';
import { LoaderService } from "src/app/shared/loader.service";
import { alertService } from "src/app/shared/Services/alert/alert.service";
import { ExcelService } from "src/app/shared/Services/excel/ExcelService";
import { ImportExcelService } from "src/app/shared/Services/excel/import-excel.service";
import { StaffService } from '@tms-services/staff-service';
import { StaffDTO } from "../../../Models/staff-dto";

@Component({
  selector: "app-upload-vehicle-excel",
  templateUrl: "./upload-staff-excel.component.html",
  styleUrls: ["./upload-staff-excel.component.scss"],
})
export class UploadStaffExcelComponent implements OnInit {
  staffs: StaffDTO[] = [];
  @Output() confirm: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    private importExcelService: ImportExcelService,
    private modalService: BsModalService,
    private mainloading: LoaderService,
    private _alert: alertService,
    private _ExcelService: ExcelService,
    private staffService: StaffService
  ) {}

  ngOnInit() {}

  changeExcelFile(file) {
    let res = this.importExcelService.importXLSX(file);
    let packages = {
      IDs: [],
      Branch: [],
      SubBranch: [],
      Landmark: [],
      Code: [],
      FirstName: [],
      LastName: [],
      MiddleName: [],
      StaffRole: [],
      Mobile: [],
      Email: [],
      PermittedBranch: []
    };

    res.subscribe((r) => {
      if (r.rows) {
        this.importExcelService.matchObjects(r, packages.IDs, "IDs");
        this.importExcelService.matchObjects(r, packages.Branch, "Branch");
        this.importExcelService.matchObjects(r, packages.SubBranch, "SubBranch");
        this.importExcelService.matchObjects(r, packages.Landmark, "Landmark");
        this.importExcelService.matchObjects(r, packages.Code, "Code");
        this.importExcelService.matchObjects(r, packages.FirstName, "FirstName");
        this.importExcelService.matchObjects(r, packages.LastName, "LastName");
        this.importExcelService.matchObjects(r, packages.MiddleName, "MiddleName");
        this.importExcelService.matchObjects(r, packages.StaffRole, "StaffRole");
        this.importExcelService.matchObjects(r, packages.Mobile, "Mobile");
        this.importExcelService.matchObjects(r, packages.Email, "Email");
        this.importExcelService.matchObjects(r, packages.PermittedBranch, "PermittedBranch");
        // this.importExcelService.matchObjects(r, packages.xxxxx1, "طراز المركبة");
        // this.importExcelService.matchObjects(r, packages.xxxxx2, "رمز المركبة");
        // this.importExcelService.matchObjects(r, packages.xxxxx3, "اسم السائق");
        // this.importExcelService.matchObjects(r, packages.xxxxx4, "رقم هوية السائق");
        // this.importExcelService.matchObjects(r, packages.xxxxx5, "رقم جوال السائق");
        // this.importExcelService.matchObjects(r, packages.xxxxx6, "لون المركبة");
        // this.importExcelService.matchObjects(r, packages.xxxxx7, "رقم تأهيل المقاول");
        // this.importExcelService.matchObjects(r, packages.xxxxx8, "سنة الصنع");

        // add packages ain Objects
        for (var i = 0; i < packages.Code.length; i++) {
          let item = new StaffDTO();
          debugger;
          item.IDs = packages.IDs[i];
          item.Branch = packages.Branch[i];
          item.SubBranch = packages.SubBranch[i];
          item.Landmark = packages.Landmark[i];
          item.Code = packages.Code[i];
          item.FirstName = packages.FirstName[i];
          item.LastName = packages.LastName[i];
          item.MiddleName = packages.MiddleName[i];
          item.StaffRole = packages.StaffRole[i];
          item.Mobile = packages.Mobile[i];
          item.Email = packages.Email[i];
          item.PermittedBranch = packages.PermittedBranch[i];
          item.ExcelSheetRowId = i + 2;
          this.staffs.push(item);
        }

        res.unsubscribe();
      }
    });
  }

  saveList() {
    this.mainloading.PreloaderIcreaseCount();
    this.staffService.AddRange(this.staffs).subscribe(
      (res) => {
        if (res.IsErrorState == true) {
          this._alert.error(res.ErrorDescription);
        } else {
          let excelJson = res.Value.map((value) => {
            let r = {
              RowNumber: value.ExcelSheetRowId,
              Error: value.ExcelValidation,
            };
            return r;
          });

          this._ExcelService.exportAsExcelFile(excelJson, "FailedStaffList");

          this.confirm.emit(true);

          this.cancel();
        }
      },
      (err) => {
        this.mainloading.PreloaderDecreaseCount();
      },
      () => {
        this.mainloading.PreloaderDecreaseCount();
      }
    );
  }

  cancel() {
    this.modalService.hide(1);
  }
}
