import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Modules } from '../shared/Enums/modules.enum';
import { PermissionDataModel } from '../shared/datamodels/permissions.data.model';
import { TranslateService } from '@ngx-translate/core';
import { AuthenticationService } from '../shared/Services/authentication/authentication.service';
import { UserService } from '../TMS-Module/Services/user.service';
import { MenuLinkService } from '../shared/Services/menu/menu-link.service';
import { Router } from '@angular/router';

@Component({
  selector: 'side-menu',
  templateUrl: './side_menu.component.html'

})
export class Sidemenu implements OnInit {

  constructor(
    private Translate: TranslateService,
    private authService: AuthenticationService,
    private userService: UserService,
    private menuLink: MenuLinkService,
    private router: Router,

    ) {
    }

  // @Input() Permissions: PermissionDataModel[] = [];
  private Permissions: PermissionDataModel[] = [];
  @Output() subMenuState: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Input() isSubMenuOpend = false;

  @Output() menuState: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Input() isMenuState = false;
  subMenuHeader: any = {};
  ModuleNames = Modules;

  show_Resources = false;
  show_Maintenance = false;
  show_Operations = false;
  show_GPS = false;
  show_RentCars = false;
  show_Schools = false;
  show_TMS = false;
  show_SEWER = false;
  show_Reports = false;

  ResourcesLinks = [
    {
      key: 'FMSDashboard',
      route: 'dashboard',
      resKey: 'Res_Dashboard'
    },
    {
      key: 'Vehicle',
      route: 'vehicle/vehiclelist',
      resKey: 'Res_Vehicles'
    },
    {
      key: 'Staff',
      route: 'staff',
      resKey: 'Res_Staff'
    },
    {
      key: 'StructureAndDistribution',
      route: 'Altair',
      resKey: 'Res_Structure'
    }
  ];
  MaintenanceLinks = [
    {
      key: 'RequestForRepair',
      route: 'Altair',
      resKey: 'maint_RFR'
    },
    {
      key: 'AccidentRequestForRepair',
      route: 'Altair',
      resKey: 'maint_Accident'
    },
    {
      key: 'PMRequestForRepair',
      route: 'Altair',
      resKey: 'maint_PM_RFR'
    },
    {
      key: 'CMRequestForRepair',
      route: 'Altair',
      resKey: 'maint_CM_RFR'
    },
    {
      key: 'WorkOrder',
      route: 'Altair',
      resKey: 'maint_WorkOrder'
    },
    {

      key: 'VendorWorkOrder',
      route: 'Altair',
      resKey: 'maint_Vendor'
    },
    {

      key: 'PreventiveMaintenance',
      route: 'Altair',
      resKey: 'maint_Preventive'
    },
    {
      key: 'lnk_MaintenanceDashboard',
      route: 'Altair',
      resKey: 'maint_Dashboard'
    }
  ];
  OperationsLinks = [

    {
      key: 'Routes',
      route: 'Postal',
      resKey: 'oper_Routes'
    },
    {
      key: 'WorkOrderList',
      route: 'Postal',
      resKey: 'oper_WorkOrders'
    }

    // {
    //   key: 'WorkOrderCostList',
    //   route: 'Altair',
    //   resKey: 'oper_Monitor'
    // }
  ];
  GPSLinks = [
    {
      key: 'Map',
      route: 'GPS',
      resKey: 'Gps_LiveTracking'
    },
    {
      key: 'MapMonitor',
      route: 'GPS',
      resKey: 'Gps_FleetMonitoring'
      },
      {
          key: 'Landmarks',
          route: 'Altair',
          resKey: 'oper_Landmarks'
      },
    {
      key: 'Geofences',
      route: 'GPS',
      resKey: 'Gps_Geofences'
    },
    {
      key: 'GroupManagment',
      route: 'GPS',
      resKey: 'Gps_Group'
    },
    {
      key: 'GPSDashboard',
      route: 'School',
      resKey: 'GPS_DashboardKey'
    }
  ];
  RentCarsLinks = [
    {
      key: 'CarRentDashBoard',
      route: 'CarRent',
      resKey: 'Rent_Dashboard'
    },
    {
      key: 'PricePlanList',
      route: 'CarRent',
      resKey: 'Rent_PricePlans'
    },
    {
      key: 'AssignPricePlanToVehicles',
      route: 'CarRent',
      resKey: 'Rent_AssignPrice'
    },
    {
      key: 'PromotionsList',
      route: 'CarRent',
      resKey: 'Rent_Promotion'
    },
    {
      key: 'AdditionalServices',
      route: 'CarRent',
      resKey: 'Rent_AdditionalServices'
    },
    {
      key: 'Customers',
      route: 'CarRent',
      resKey: 'Rent_Customers'
    },
    {
      key: 'RentCar',
      route: 'CarRent',
      resKey: 'Rent_RentVehicle'
    },
    {
      key: 'ReturnPage',
      route: 'CarRent',
      resKey: 'Return Vehicle'
    },
    {
      key: 'ContractsList',
      route: 'CarRent',
      resKey: 'Rent_RentContracts'
    }
  ];
  Schools = [
    {
      key: 'PlanList',
      route: 'School',
      resKey: 'Sco_PlansManage'
    },
    {
      key: 'StudentList',
      route: 'School',
      resKey: 'Sco_StudentsManage'
    },
      {
          key: 'TripList',
          route: 'School',
          resKey: 'Sco_TripsManage'
      },
      {
          key: 'Dashboard',
          route: 'School',
          resKey: 'Sco_LiveDashboard'
      }
  ];
  ReportsLinks = [

    {
      key: 'GeneralReports',
      route: 'Altair',
      resKey: 'rep_General'
    },
    {
      key: 'GPSReports',
      route: 'GPS',
      resKey: 'rep_GPS'
    },
    {
      key: 'MaintenanceReports',
      route: 'Altair',
      resKey: 'rep_Maintenance'
    },
    {
      key: 'OperationReport',
      route: 'Altair',
      resKey: 'rep_Operations'
    },
    {
      key: 'RentalReports',
      route: 'CarRent',
      resKey: 'rep_RentCars'
    }

  ];
  TMS = [
    {
      key: 'tmsDashboard',
      route: 'dashboard',
      resKey: 'TMS_Dashboard'
    },
    {
      key: 'orderlist',
      route: 'order/orderlist',
      resKey: 'TMS_orderlist'
    },
    {
      key: 'zonelist',
      route: 'zone/zonelist',
      resKey: 'TMS_zonelist'
    },
    {
      key: 'entrygate',
      route: 'gate/entry',
      resKey: 'TMS_entrygate'
    },
    {
      key: 'exitgate',
      route: 'gate/exit',
      resKey: 'TMS_exitgate'
    },
    {
      key: 'tmsDeferredOrders',
      route: 'integration/deferredorderslist',
      resKey: 'DeferredOrders'
    },
    {
      key: 'tmsReports',
      route: 'reports',
      resKey: 'Reports'
    },
    {
      key: 'contractlist',
      route: 'contract/contractlist',
      resKey: 'TMS_contractlist'
    },
    {
      key: 'contractorlist',
      route: 'contractor/contractorlist',
      resKey: 'ContractorList'
    },
    {
      key: 'deviceMeterReading',
      route: 'devicemeter/readingslist',
      resKey: 'MeterReading'
    }
  ];
  Sewer = [
    {
      key: 'SewerEntryGate',
      route: 'sewer-gate/entry',
      resKey: 'SewerEntryGate'
    },
    {
      key: 'SewerExitGate',
      route: 'sewer-gate/exit',
      resKey: 'SewerExitGate'
    }
  ];
  subMenuLinks = this.ResourcesLinks;

  ngOnInit() {
    this.authService.getUserPermissions().subscribe(res => {
      this.Permissions = res;
      if (this.Permissions && this.Permissions.length > 0) {
        this.callShowHeader();
      }
    });
  }

  onModuleClick(moduleHeader) {
    this.subMenuHeader = {
      icon: moduleHeader.icon,
      name: this.Translate.instant(moduleHeader.resKey)
    };
    this.isSubMenuOpend = !this.isSubMenuOpend;
    this.subMenuState.emit(this.isSubMenuOpend);
    switch (moduleHeader.Key) {
      case Modules.Resources:
        this.subMenuLinks = this.ResourcesLinks.filter(x => this.Permissions.find(p => p.PageUniqueName.indexOf(x.key) !== -1));
        break;
      case Modules.Maintenance:
        this.subMenuLinks = this.MaintenanceLinks.filter(x => this.Permissions.find(p => p.PageUniqueName.indexOf(x.key) !== -1));
        break;
      case Modules.Operations:
        this.subMenuLinks = this.OperationsLinks.filter(x => this.Permissions.find(p => p.PageUniqueName.indexOf(x.key) !== -1));
        break;
      case Modules.GPS:
        this.subMenuLinks = this.GPSLinks.filter(x => this.Permissions.find(p => p.PageUniqueName.indexOf(x.key) !== -1));
        break;
      case Modules.RentCars:
        this.subMenuLinks = this.RentCarsLinks.filter(x => this.Permissions.find(p => p.PageUniqueName.indexOf(x.key) !== -1));
        break;
      case Modules.Reports:
        this.subMenuLinks = this.ReportsLinks.filter(x => this.Permissions.find(p => p.PageUniqueName.indexOf(x.key) !== -1));
        break;
      case Modules.Schools:
        this.subMenuLinks = this.Schools.filter(x => this.Permissions.find(p => p.PageUniqueName.indexOf(x.key) !== -1));
        break;
        case Modules.TMS:
        this.subMenuLinks = this.TMS.filter(x => this.Permissions.find(p => p.PageUniqueName.indexOf(x.key) !== -1));
        break;
        case Modules.Sewer:
        this.subMenuLinks = this.Sewer.filter(x => this.Permissions.find(p => p.PageUniqueName.indexOf(x.key) !== -1));
        break;
      default:
        this.subMenuLinks = [];
    }
  }

  callShowHeader() {
    this.show_Resources = this.showHeader('Resources');
    this.show_Maintenance = this.showHeader('Maintenance');
    this.show_Operations = this.showHeader('Operations');
    this.show_GPS = this.showHeader('GPS');
    this.show_RentCars = this.showHeader('RentCars');
    this.show_Schools = this.showHeader('Schools');
    this.show_TMS = this.showHeader('TMS');
    this.show_SEWER = this.showHeader('Sewer');
    this.show_Reports = this.showHeader('Reports');
  }

  showHeader(moduleName: string) {
    return this.Permissions.findIndex(p => p.ModuleUniqueName === moduleName) != -1;
  }

  hideSubmenu() {
    if (this.isSubMenuOpend) {
      this.isSubMenuOpend = false;
      this.subMenuState.emit(this.isSubMenuOpend);
    }
  }
  hideMenu() {
    this.hideSubmenu()
    this.isSubMenuOpend = false;
    this.menuState.emit(this.isSubMenuOpend);
  }

  menuHrefClicked(route: string) {
      this.router.navigate([`/tms/${route}`]).then(res => res && this.hideMenu());
  }
}