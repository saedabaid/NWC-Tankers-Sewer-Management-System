import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { BsModalService } from "ngx-bootstrap/modal";
//import { BsModalService } from 'ngx-bootstrap';
import { LoaderService } from "src/app/shared/loader.service";
import { alertService } from "src/app/shared/Services/alert/alert.service";
import { ExcelService } from "src/app/shared/Services/excel/ExcelService";
import { ImportExcelService } from "src/app/shared/Services/excel/import-excel.service";
import { VehicleDTO } from "src/app/TMS-Module/Models/vehicle-dto";
import { VehicleListService } from "src/app/TMS-Module/Services/vehicle-list.service";

@Component({
  selector: "app-upload-vehicle-excel",
  templateUrl: "./upload-vehicle-excel.component.html",
  styleUrls: ["./upload-vehicle-excel.component.scss"],
})
export class UploadVehicleExcelComponent implements OnInit {
  vehicles: VehicleDTO[] = [];
  @Output() confirm: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    private importExcelService: ImportExcelService,
    private modalService: BsModalService,
    private mainloading: LoaderService,
    private _alert: alertService,
    private _ExcelService: ExcelService,
    private _vehicleListService: VehicleListService
  ) {}

  ngOnInit() {}

  changeExcelFile(file) {
    let res = this.importExcelService.importXLSX(file);
    let packages = {
      PlateNumber: [],
      Branch: [],
      SubBranch: [],
      Code: [],
      ChassisNumber: [],
      Landmark: [],
      Manufacturer: [],
      VehicleBrand: [],
      ProductionYear: [],
      Color: [],
      Type: [],
      FuelLitres: [],
      SpeedLimit: [],
      Capacity: [],
      CapacityUnit: [],
      LicenseType: [],
      DriverCode: [],
      CurrentMileage: [],
      HourRate: [],
      EngineNumber: [],
      LicenseNumber: [],
      InsuredBy: [],
      InsuranceNumber: [],
      InsuranceStartDate: [],
      InsuranceEndDate: [],
      Supplier: [],
      FuelCost: [],
      TemperatureSensorMaxValue: [],
      TemperatureSensorMinValue: [],
      DeviceCode: [],
      SimNo: [],
      ProviderName: [],
      TrackerProvider: [],
      LicenseExpiryDate: [],
      ServiceTypeID: [],
      Status: [],
      NextExaminationDate: [],
    };

    res.subscribe((r) => {
      if (r.rows) {
        this.importExcelService.matchObjects(r, packages.PlateNumber, "PlateNumber");
        this.importExcelService.matchObjects(r, packages.Branch, "Branch");
        this.importExcelService.matchObjects(r, packages.SubBranch, "Sub Branch");
        this.importExcelService.matchObjects(r, packages.Code, "Code");
        this.importExcelService.matchObjects(r, packages.ChassisNumber, "ChassisNumber");
        this.importExcelService.matchObjects(r, packages.Landmark, "Landmark");
        this.importExcelService.matchObjects(r, packages.Manufacturer, "Manufacturer");
        this.importExcelService.matchObjects(r, packages.VehicleBrand, "Vehicle Brand");
        this.importExcelService.matchObjects(r, packages.ProductionYear, "ProductionYear");
        this.importExcelService.matchObjects(r, packages.Color, "Color");
        this.importExcelService.matchObjects(r, packages.Type, "Type");
        this.importExcelService.matchObjects(r, packages.FuelLitres, "Fuel Litres");
        this.importExcelService.matchObjects(r, packages.SpeedLimit, "Speed Limit");
        this.importExcelService.matchObjects(r, packages.Capacity, "Capacity");
        this.importExcelService.matchObjects(r, packages.CapacityUnit, "Capacity Unit");
        this.importExcelService.matchObjects(r, packages.LicenseType, "License Type");
        this.importExcelService.matchObjects(r, packages.DriverCode, "DriverCode");
        this.importExcelService.matchObjects(r, packages.CurrentMileage, "CurrentMileage");
        this.importExcelService.matchObjects(r, packages.HourRate, "HourRate");
        this.importExcelService.matchObjects(r, packages.EngineNumber, "EngineNumber");
        this.importExcelService.matchObjects(r, packages.LicenseNumber, "License Number");
        this.importExcelService.matchObjects(r, packages.InsuredBy, "Insured By");
        this.importExcelService.matchObjects(r, packages.InsuranceNumber, "Insurance Number");
        this.importExcelService.matchObjects(r, packages.InsuranceStartDate, "Insurance Start Date");
        this.importExcelService.matchObjects(r, packages.InsuranceEndDate, "Insurance End Date");
        this.importExcelService.matchObjects(r, packages.Supplier, "Supplier");
        this.importExcelService.matchObjects(r, packages.FuelCost, "Fuel Cost");
        this.importExcelService.matchObjects(r, packages.TemperatureSensorMaxValue, "Temprature Sensor Max Value");
        this.importExcelService.matchObjects(r, packages.TemperatureSensorMinValue, "Temprature Sensor Min Value");
        this.importExcelService.matchObjects(r, packages.DeviceCode, "DeviceCode");
        this.importExcelService.matchObjects(r, packages.SimNo, "SimNo");
        this.importExcelService.matchObjects(r, packages.ProviderName, "ProviderName");
        this.importExcelService.matchObjects(r, packages.TrackerProvider, "TrackerProvider");
        this.importExcelService.matchObjects(r, packages.LicenseExpiryDate, "تاريخ أنتهاء الترخيص");
        this.importExcelService.matchObjects(r, packages.ServiceTypeID, "نوع الخدمة");
        this.importExcelService.matchObjects(r, packages.Status, "حالة المركبة");
        this.importExcelService.matchObjects(r, packages.NextExaminationDate, "تاريخ الفحص القادم");

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
          let item = new VehicleDTO();

          item.PlateNo = packages.PlateNumber[i];
          item.Branch = packages.Branch[i];
          item.SubBranch = packages.SubBranch[i];
          item.Code = packages.Code[i];
          item.ChassisNo = packages.ChassisNumber[i];
          item.Landmark = packages.Landmark[i];
          item.Model = packages.Manufacturer[i];
          item.Brand = packages.VehicleBrand[i];
          item.ProductionYear = packages.ProductionYear[i];
          item.Color = packages.Color[i];
          item.TransporterType = packages.Type[i];
          item.FuelLitres = packages.FuelLitres[i];
          item.SpeedLimit = packages.SpeedLimit[i];
          item.Capacity = packages.Capacity[i];
          item.CapacityUnit = packages.CapacityUnit[i];
          item.LicenseType = packages.LicenseType[i];
          item.DriverCode = packages.DriverCode[i];
          item.CurrentMileage = packages.CurrentMileage[i];
          item.HourRate = packages.HourRate[i];
          item.EngineNumber = packages.EngineNumber[i];
          item.LicenseNumber = packages.LicenseNumber[i];
          item.InsuredBy = packages.InsuredBy[i];
          item.InsuranceNumber = packages.InsuranceNumber[i];
          item.InsuranceStartDate = packages.InsuranceStartDate[i];
          item.InsuranceEndDate = packages.InsuranceEndDate[i];
          item.Supplier = packages.Supplier[i];
          item.FuelCost = packages.FuelCost[i];
          item.TemperatureSensorMaxValue = packages.TemperatureSensorMaxValue[i];
          item.TemperatureSensorMinValue = packages.TemperatureSensorMinValue[i];
          item.DeviceCode = packages.DeviceCode[i];
          item.SIMCardNo = packages.SimNo[i];
          item.Provider = packages.TrackerProvider[i];
          item.ProviderName = packages.ProviderName[i];
          item.LicenseExpiryDate = packages.LicenseExpiryDate[i];
          item.ServiceTypeID = packages.ServiceTypeID[i];
          item.Status = packages.Status[i];
          item.NextExaminationDate = packages.NextExaminationDate[i];

          item.ExcelSheetRowId = i + 2;
          this.vehicles.push(item);
        }

        res.unsubscribe();
      }
    });
  }

  saveList() {
    this.mainloading.PreloaderIcreaseCount();
    this._vehicleListService.AddRange(this.vehicles).subscribe(
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

          this._ExcelService.exportAsExcelFile(excelJson, "FailedVehicleList");

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
