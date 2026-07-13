import { environment } from 'src/environments/environment';

export let Configuration = {
  temporarySession : false,  // true => clear the token after browser closed
  Ports: {
    AltairPort: 'http://localhost:4679',
    GpsPort: 'http://localhost:9000',
    RenCarsPort: 'http://localhost:32067',
    PostalPort: 'http://localhost:55760/',
    // SchoolPort: 'http://localhost:32597/',
    TMSPort: 'http://localhost:39645/'
  },
  defaultMapParams: {
    zoom: 10,
    mapType: 'google',
    centerX: 46.6753,
    centerY: 24.7136,
    minZoom: 2,
    maxZoom: 20
  },
  tokens:
  {
    GoogleMapDirectionsToken: 'AIzaSyB_HWxIIAw5vdXy4zgFh3KKEj6BAc9MWI4'
  },
  urls: {

    queryEndpoint:          environment.queryEndpoint, // Query
    commandEndpoint:        environment.commandEndpoint, // Command
    authenticationEndpoint: environment.authenticationEndpoint, // Auth
    signalREndpoint:        environment.signalREndpoint, // SignalR

    GoogleMapUrl: 'http://mt1.google.com/vt/lyrs=m&x={x}&y={y}&z={z}',
    ESRIMapUrl: 'https://server.arcgisonline.com/arcgis/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}',
    calculateRouteUrl: 'MapUtils/calculateRoute',
    signalRDriversHub: 'drivers',
    SaveOrderRouteUrl: 'MapUtils/SaveOrderRoute',
    // Staff
    SearchStaffLookups: 'Staff/Search',
    staff: {
      StaffSearch: 'Staff/Search',
      StaffAdd: 'Staff/Add',
      GetStaffById: 'Staff/GetStaffById',
      GetStaffByPersonalId: 'Staff/GetStaffByPersonalId',
      StaffUpdate: 'Staff/Update',
      StaffDelete: 'Staff',
      StaffRole: 'Staff/GetStaffRoles',
      StaffPages: 'Staff/GetPages',
      GetAllRoles: 'Staff/GetAllRoles',
      changePermssion: 'Staff/changePermssion',
      AddRange: 'Staff/bulk'
    },
    lookups: {
      GetVehicleBrandById: 'Lookup/GetVehicleBrandById',
      GetVehicleTypes: 'Lookup/GetTransporterTypes',
      GetVehicleBrand: 'Lookup/GetVehicleBrand',
      GetLandMarkTypes: 'Lookup/GetLandmarkTypes',
      SearchStaffRolesLookups: 'Lookup/GetStaffRoles',
      GetStaffRoleCategories: 'LookUp/GetStaffRoleCategories',
      GetStaffRolesByCategoryID: 'LookUp/GetStaffRolesByCategoryID',
      GetpagesList: 'LookUp/GetPageList',
      SearchStaffLandmarkLookups: 'Lookup/GetAllStationBasedOnCity',
      SearchStaffBranchLookups: 'Lookup/GetAllCities',
      GetTermsCategory: 'Lookup/GetTermsCategory',
      customerClassLookups: 'Lookup/GetCustomerLocationClasses',
      customerPrioritiesLookups: 'Lookup/GetCustomerLocationPriorities',
      servicesTypeLookups: 'Lookup/GetServiceTypes',
      getPermittedServicesTypes: 'Lookup/getPermittedServicesTypes',
      GetStaffSelectedCategory: 'Lookup/GetStaffSelectedCategory',
      GetStaffSelectedRoleName: 'Lookup/GetStaffSelectedRoleName',
      GetStaffDefaultPage: 'Lookup/GetStaffDefaultPage',
      GetVehicleLogTypes: 'Lookup/GetVehicleLogTypes',
      WorkOrderStatusesLookups: 'Lookup/GetWorkOrderStatuses',
      GetWorkOrderStatusesInDeassign: 'Lookup/GetWorkOrderStatusesInDeassign',
      SearchAreasNameLookups: 'Lookup/SearchAreasByName',
      SearchCitiesNameLookups: 'Lookup/SearchCitiesByName',
      SearchZonesNameLookups: 'Lookup/SearchZonesByName',
      SearchStationsLookups: 'Lookup/SearchStations',
      GetVehiclesLookups: 'Lookup/GetVehicles',
      SearchVehiclesLookups: 'Lookup/SearchVehicles',
      GetDriversLookups: 'Lookup/GetDrivers',
      SearchDriversByTanker: 'Lookup/SearchDriversByTanker',
      SearchDriversLookups: 'Lookup/SearchDrivers',
      SearchCustomersLookups: 'Lookup/SearchCustomers',
      SearchSoqyaCustomers: 'Lookup/SearchSoqyaCustomers',
      SearchCommercialCustomers: 'Lookup/SearchCommercialCustomers',
      SearchOrderNumberLookups: 'Lookup/SearchOrderNumbers',
      GetNextWorkOrderStatuses: 'Lookup/GetNextWorkOrderStatuses',
      GetSewerNextWorkOrderStatus: 'Lookup/GetSewerNextWorkOrderStatus',
      GeReasonsByStatusId: 'Lookup/GeReasonsByStatusId',
      GetAccessories: 'Lookup/GetAccessories',
      GetWorkOrderCategory: 'Lookup/GetWorkOrderCategory',
      GetZoneStations: 'Lookup/GetZoneStations',
      GetCustomerLocations: 'Lookup/GetCustomerLocations',
      GetDeassignReason: 'Lookup/GetDeassignReason',
      getZonesByNameOrCode: 'Lookup/getZonesByNameOrCode',
      searchContractsCodes: 'Lookup/SearchContractCode',
      GetContractTypes: 'Lookup/GetContractTypes',
      GetContractStatuses: 'Lookup/GetContractStatuses',
      GetContractTerminationReasons: 'Lookup/GetContractTerminationReasons',
      SearchContractorNameCode: 'Lookup/SearchContractorNameCode',
      searchUserByName: 'Lookup/searchUserByName',
      // getZonesByNameOrCode :'Lookup/getZonesByNameOrCode',
      SearchZonesByNameOrCode: 'Lookup/SearchZonesByNameOrCode',
      GetAllCities: 'Lookup/GetAllCities',
      GetStationBasedOnCity: 'Lookup/GetStationBasedOnCity',
      GetUserStations: 'Lookup/GetUserStations',
      GetAllStationBasedOnCity: 'Lookup/GetAllStationBasedOnCity',
      GetTransporterTypes: 'Lookup/GetTransporterTypes',
      SearchAllZones: 'Lookup/SearchAllZones',
      getContractStations: 'Lookup/getContractStations',
      GetPersonalIDTypes: 'Lookup/GetPersonalIDTypes',
      getCustomerLocationStations: 'Lookup/getCustomerLocationStations',
      SearchPermittedStations: 'Lookup/SearchPermittedStations',
      SearchPermittedSoqyaStations: 'Lookup/SearchPermittedSoqyaStations',
      SearchMeterSerial: 'Lookup/SearchMeterSerial',
      GetTanckerCapacities: 'Lookup/GetTanckerCapacities',
      GetTanckerCapacitiesByStation: 'Lookup/GetTanckerCapacitiesByStation',
      SearchStaff: 'Lookup/SearchStaff',
      SearchZonesBasedOnAssignedStations: 'Lookup/SearchZonesBasedOnAssignedStations',
      GetDeferredOrdersStatuses: 'Lookup/GetDeferredOrdersStatuses',
      GetTermsValueUnits: 'Lookup/GetTermsValueUnits',
      getCustomerAccountStations: 'Lookup/getCustomerAccountStations',
      GetCustomerAccounts: 'Lookup/GetCustomerAccounts',
      GetCustomerAccountsSameService: 'Lookup/GetCustomerAccountsSameService',
      GetCustomerAccountsSoqya: 'Lookup/GetCustomerAccountsSoqya',
      GetCustomerAccountsAddOrderPage: 'Lookup/GetCustomerAccountsAddOrderPage',
      GetCustomerCommercialAccountsAddOrderPage: 'Lookup/GetCustomerCommercialAccounts',
      GetCommingMonthYear: 'Lookup/GetCommingMonthYear',
      GetTransporterBrands: 'Lookup/GetTransporterBrands',
      GetTransporterGroups: 'Lookup/GetTransporterGroups',
      GetTransporterProductionYears: 'Lookup/GetTransporterProductionYears',
      GetTransporterManufacturer: 'Lookup/GetTransporterManufacturer',
      GetTransporterStatuses: 'Lookup/GetTransporterStatuses',
      GetTransporterStatusesInDeassign: 'Lookup/GetTransporterStatusesInDeassign',
      GetOrderInvoiceStatuses: 'Lookup/GetOrderInvoiceStatuses',
      SearchContractContractors: 'Lookup/SearchContractContractors',
      GetContractTerm: 'Lookup/GetContractTerm',
      GetContractTermsViolationStatuses: 'Lookup/GetContractTermsViolationStatuses',
      GetContractTermsViolationCancelReasons: 'Lookup/GetContractTermsViolationCancelReasons',
      GetStationStatuses: 'Lookup/GetStationStatuses',
      GetBranches: 'Lookup/GetBranches',
      GetSubBranches: 'Lookup/GetSubBranches',
      GetLandmarks: 'Lookup/GetLandmarks',
    },
    gate: {
      getStateVehicles: 'WorkOrderVehicle/GetStateVehicles',
      getWorkOrderVehicles: 'WorkOrderVehicle/GetWorkOrderVehicles',
      GetPrintDriverInvoice: 'WorkOrderVehicle/GetPrintDriverInvoice',
      GetPrintCustomerInvoice: 'WorkOrderVehicle/GetPrintCustomerInvoice',
      GetPrintVehicleInvoice: 'WorkOrderVehicle/GetPrintVehicleInvoice',
      ArriveVehicleToStation: 'Vehicle/ArriveVehicleToStation',
      OutForParking: 'Vehicle/OutForParking',
      GetVehicleLogReport: 'Vehicle/GetVehicleLogReport',
      GetVehicleDataReport: 'Vehicle/GetVehicleDataReport',
      GetVehiclePerformanceReport: 'Vehicle/GetVehiclePerformanceReport',
      GetOrderReassignmentReport: 'WorkOrderVehicle/GetOrderReassignmentReport'
    },
    sewer:{
      OutForWork: 'Sewer/OutForWork',
      SewerVehicleArrivedStation: 'Sewer/SewerVehicleArrivedStation',
      ArriveSewerVehicleWithOutOrderToStation: 'Sewer/ArriveSewerVehicleWithOutOrderToStation',
    }
    ,
    orders: {
      orderSearch: 'WorkOrder/Search',
      details: 'WorkOrder/GetOrderBasicDetails',
      GetWorkOrderComments: 'WorkOrder/GetWorkOrderComments',
      GetWorkOrderPayments: 'WorkOrder/GetWorkOrderPayments',
      AddOrderComments: 'WorkOrder/AddComment',
      GetWorkOrderComplaints: 'WorkOrder/GetWorkOrderComplaints',
      AssignWorkOrder: 'WorkOrder/AssignWorkOrder',
      DeassignWorkOrder: 'WorkOrder/DeassignWorkOrder',
      OutForDeliveryWorkOrder: 'WorkOrder/OutForDeliveryWorkOrder',
      ArrivedStation: 'WorkOrder/WOVehicleArrivedStation',
      ArrivedWorkOrder: 'WorkOrder/ArrivedWorkOrder',
      SewerConfirmWorkOrder: 'WorkOrder/SewerConfirmWorkOrder',
      SewerCompleteWorkOrder: 'WorkOrder/SewerCompleteWorkOrder',
      DeliveredWorkOrder: 'WorkOrder/DeliveredWorkOrder',
      CancelWorkOrder: 'WorkOrder/CancelWorkOrder',
      FailedToDeliver: 'WorkOrder/FailedToDeliver',
      OnHold: 'WorkOrder/OnHold',
      NotAssigned: 'WorkOrder/NotAssigned',
      UpdateWorkOrderShipment: 'WorkOrder/UpdateWorkOrderShipment',
      GetWorkOrderAccessory: 'WorkOrder/GetWorkOrderAccessory',
      GetWorkOrderStatusLogs: 'WorkOrder/GetWorkOrderStatusLogs',
      GetAssignableWorkOrders: 'WorkOrder/GetAssignableWorkOrders',
      GetWorkOrderChangeLogs: 'WorkOrder/GetWorkOrderChangeLogs',
      CreateWorkOrder: 'WorkOrder/CreateWorkOrder',
      BulkCreateWorkOrder: 'WorkOrder/BulkCreateWorkOrder',
      GetDailyOrderSummaryReport: 'WorkOrder/GetDailyOrderSummaryReport',
      GetDailyOrderDetailsReport: 'WorkOrder/GetDailyOrderDetailsReport',
      SearchDeferredWorkOrders: 'WorkOrder/SearchDeferredWorkOrders',
      IsCustomerBlacklisted: 'WorkOrder/IsCustomerBlacklisted',
      IsCustomerExceededQuota: 'WorkOrder/IsCustomerExceededQuota',
      IsZoneWithoutTankers: 'WorkOrder/IsZoneWithoutTankers',
      UpdateWorkOrdersStation: 'WorkOrder/UpdateWorkOrdersStation',
      GetNoOfOrdersForThisMonth: 'WorkOrder/GetNoOfOrdersForThisMonth'
    },
    zone: {
      zoneSearch: 'Zone/Search',
      Add: 'Zone/Add',
      GetZoneDetails: 'Zone/GetZoneDetails',
      Update: 'Zone/Update',
      Delete: 'Zone/Delete',
      AddRange: 'Zone/AddRange',
      CallGISService: 'Zone/CallGISService'
    },
    account: {
      ChangePassword: 'UsersManage/ChangePassword',
      ForgotPassword: 'UsersManage/ForgotPassword'
    },
    userlist: {
      userSearch: 'UserManage/Search',
      unlock: 'UsersManage/Unlock',
      lock: 'UsersManage/Lock',
      GetUserDetails: 'UserManage/GetUserDetails',
      Update: 'UserManage/Update',
      Delete: 'UsersManage/Delete'
    },
    Vehicles:
    {
      GetAssignableVehicles: 'WorkOrderVehicle/GetAssignableVehicles',
      GetVehicleDataReport: 'Vehicle/GetVehicleDataReport',
      Search: 'transporters/list',
      GetTransporterByNumber: 'transporters/GetTransporterByNumber',
      GetPermitList: 'Vehicle/GetPermitList',
      GetPermit: 'Vehicle/GetPermit',
      AddPermit: 'Vehicle/AddPermit',
      UpdateTanker: 'Vehicle/UpdateTanker',
      UpdateDriver:'Staff/UpdateDriver',
      searchVehicleType: 'vehicletype/searchVehicleType',
      Add: 'transporters',
      Update: 'transporters',
      Delete: 'transporters',
      GetOne: 'transporters',
      AddRange: 'transporters/bulk',
      UpdateVehicleStatus: 'transporters/UpdateVehicleStatus',
    },
    landmarks:
    {
      Search: 'landmarks/list',
      Add: 'landmarks',
      Update: 'landmarks',
      Delete: 'landmarks',
      GetById: 'landmarks',
    },
    apiAccountUrl: {
      apiGetPermissionsURL: 'User/staffPermissions'
    },
    contracts: {
      SearchContractList: 'Contract/SearchContractList',
      GetContractAttachments: 'Contract/GetContractAttachments',
      AddContract: 'Contract/AddContract',
      EditContract: 'Contract/EditContract',
      DeleteContract: 'Contract/DeleteContract',
      TerminateContract: 'Contract/TerminateContract',
      SearchTariffList: 'Contract/SearchTariffList',
      SearchStattionList: 'Contract/SearchStattionList',
      AddStation: 'Contract/AddStation',
      DeleteStation: 'Contract/DeleteStation',
      UpdateStation: 'Contract/UpdateStation',
      AddTariff: 'Contract/AddTariff',
      EditTariff: 'Contract/EditTariff',
      DeleteTariff: 'Contract/DeleteTariff',
      AddTariffRange: 'Contract/AddTariffRange',
      SearchContractAccessories: 'Contract/SearchContractAccessories',
      DeleteContractAccessory: 'Contract/DeleteContractAccessory',
      AddContractAccessories: 'Contract/AddContractAccessories',
      UpdateContractAccessory: 'Contract/UpdateContractAccessory',
      GetContractPriceList: 'Contract/GetContractPriceList',
      UpdatePriceList: 'Contract/UpdatePriceList',
      GetContractTermsList: 'Contract/GetContractTermsList',
      AddTerm: 'Contract/AddTerm',
      UpdateTerm: 'Contract/UpdateTerm',
      DeleteTerm: 'Contract/DeleteTerm',
      GetContractViolations: 'Contract/GetContractViolations',
      GetContractViolationsAttachments: 'Contract/GetContractViolationsAttachments',
      GetContractViolationLogs: 'Contract/GetContractViolationLogs',
      GetContractApprovalViolationLogs: 'Contract/GetContractApprovalViolationLogs',
      GetTermViolationInvoice: 'Contract/GetTermViolationInvoice',
      GetTermValueUnit: 'Contract/GetTermValueUnit',
      AddContractViolation: 'Contract/AddContractViolation',
      AddViolationApproval: 'Contract/AddViolationApproval',
      GetContractApprovalViolation:'Contract/GetContractApprovalViolation',
      AddViolationDecision:'Contract/AddViolationDecision',
      EditContractViolation: 'Contract/EditContractViolation',
      DeleteContractViolation: 'Contract/DeleteContractViolation',
      DeleteViolationApproval:'Contract/DeleteViolationApproval'
    },
    ControlPanel:
    {
      GetUserPermittedLandmarks: 'StaffUser/GetUserPermittedLandmarks',
      SaveUserPermittedLandmarks: 'StaffUser/SaveUserPermittedLandmarks',
      GetStationNWCSettings: 'Station/GetStationNWCSettings',
      GetStationNWCSetting: 'Station/GetStationNWCSetting',
      SaveStationNWCSettings: 'Station/SaveStationNWCSettings',
      SaveStationDefaultTankers: 'Station/SaveStationDefaultTankers',
      GetVehicleNWCSettings: 'Vehicle/GetVehicleNWCSettings',
      SaveVehicleNWCSettings: 'Vehicle/SaveVehicleNWCSettings',
      SaveVehicleNWCSettingsBulk: 'Vehicle/SaveVehicleNWCSettingsBulk',
      SearchCitySettings: 'ControlPanel/SearchCitySettings',
      GetCitySetting: 'ControlPanel/GetCitySetting',
      AddCitySettings: 'ControlPanel/AddCitySettings',
      UpdateCitySettings: 'ControlPanel/UpdateCitySettings',
      GetBranchSettings: 'ControlPanel/GetBranchSettings',
      SaveBranchSettings: 'ControlPanel/SaveBranchSettings',
      deleteBranch: 'ControlPanel/SaveBranchSettings',
      saveVehicleType: 'ControlPanel/saveVehicleType',
      saveVehicleBrand: 'ControlPanel/saveVehicleBrand',
      saveLandmarkType: 'ControlPanel/saveLandmarkType',
      updateVehicleType: 'ControlPanel/updateVehicleType',
      deleteVehicleType: 'vehicletype/deleteVehicleType',
      deleteLandMarkType: 'ControlPanel/deleteVehicleType',

      getAreaById: 'ControlPanel/getAreaById',
      addArea: 'ControlPanel/AddArea',
      editArea: 'ControlPanel/EditArea',
      deleteArea: 'ControlPanel/DeleteArea',

      getCityById: 'ControlPanel/getCityById',
      addCity: 'ControlPanel/AddArea',
      editCity: 'ControlPanel/editCity',
      deleteCity: 'ControlPanel/DeleteArea',

      getLandmarkById: 'ControlPanel/getLandmarkById',
      addLandmark: 'ControlPanel/AddLandmark',
      editLandmark: 'ControlPanel/EditLandmark',
      deleteLandmark: 'ControlPanel/DeleteLandmark',
    },
    Dashboard: {
      GetWorkOrdersCountPerStatus: 'Dashboard/GetWorkOrdersCountPerStatus',
      GetOrdersCountGroupByDayHours: 'Dashboard/GetOrdersCountGroupByDayHours',
      GetOrdersCountGroupByTop10Zones: 'Dashboard/GetOrdersCountGroupByTop10Zones',
      GetOrdersCountGroupByStatus: 'Dashboard/GetOrdersCountGroupByStatus',
      GetOrdersCountGroupByDate: 'Dashboard/GetOrdersCountGroupByDate',
      GetOrdersCountGroupByExecuteTime: 'Dashboard/GetOrdersCountGroupByExecuteTime',
            GetVehicleDataReportByStatus: 'Dashboard/GetVehicleDataReport',
            GetAreasWithNoPrices: 'Dashboard/GetAreasWithNoPrices',
    },
    File: {
      UploadFiles: 'File/UploadFiles',
      DeleteFile: 'File/DeleteFile',
      DownloadFile: 'File/DownloadFile'
    },
    contractor: {
      SearchContractorList: 'Contractor/SearchContractorList',
      AddContractor: 'Contractor/AddContractor',
      EditContractor: 'Contractor/EditContractor',
      GetContractorAttachments: 'Contractor/GetContractorAttachments',
      ActivateContractor: 'Contractor/ActivateContractor',
      DeActivateContractor: 'Contractor/DeActivateContractor',
      AddContractorToBlackListed: 'Contractor/AddContractorToBlackListed',
      RemoveContractorFromBlackListed: 'Contractor/RemoveContractorFromBlackListed'
    },
    deviceMeter: {
      SearchReadingList: 'DeviceMeter/SearchReadingList',
      AddReading: 'DeviceMeter/AddReading',
      DeleteReading: 'DeviceMeter/DeleteReading',
      SearchDeviceMeterList: 'DeviceMeter/SearchDeviceMeter',
    },
    customer: {
      SearchCustomerList: 'Customer/SearchCustomerList',
      SearchCustomerAccountList: 'Customer/SearchCustomerAccountList',
      CreateCustomerAndLocations: 'Customer/CreateCustomerAndLocations',
      EditCustomerAndLocations: 'Customer/EditCustomerAndLocations',
      DeleteCustomer: 'Customer/DeleteCustomer',
    },
    Integration: {
      EditDeferredOrder: 'Integration/EditDeferredOrder',
      CancelDeferredOrder: 'Integration/CancelDeferredOrder'
    },
    Soqya: {
      SearchSoqyaSchedules: 'Soqya/SearchSoqyaSchedules',
      AddSoqyeScheduleRecord: 'Soqya/AddSoqyeScheduleRecord',
      DeleteSoqyeScheduleRecord: 'Soqya/DeleteSoqyeScheduleRecord',
      EditSoqyeScheduleRecord: 'Soqya/EditSoqyeScheduleRecord',
      GetBalanceAndUsed: 'Soqya/GetBalanceAndUsed',
      AddRange: 'Soqya/AddRange',
      GetSoqyaSchedulesReport: 'Soqya/GetSoqyaSchedulesReport'
    },
    User: {
      GetUserPermissions: 'StaffUser/GetUserPermissions',
      GetUserProfile: 'StaffUser/GetUserProfile'
    },
    Violation: {
      GetVehicleViolations: 'Violation/GetVehicleViolations'
    },
    Report: {
      GetOrdersPerZone: 'Report/GetOrdersPerZone',
      GetStationOrderCapacity: 'Report/GetStationOrderCapacity',
      GetStationServiceTime: 'Report/GetStationServiceTime',
      GetTankerPermissionStatus: 'Report/GetTankerPermissionStatus',
      GetSoqyaSchedulePerDay: 'Report/GetSoqyaSchedulePerDay',
      ContractTariffReport: 'Report/ContractTariffReport'
    },
    ServerUtilities: {
      GetDateTimeNow: 'ServerUtilities/GetDateTimeNow'
    }
  },
  modulePrefixes: {
    NWCManagement: 'tms'

  },
  keys: {

    // planning service Keys
    LogisticsServiceToken: 'IBrkRtJb3z6Bp2sEGCAVIQBfZPCRGwhDxvMqMS1RtVnzz0O356IMaSekOQDjQ9WGHo-MUCYanlkLgieO8ePONcmytud7N553mfvSU-CjAdc0gIzEZm3uBKtVNjgE7wPdio0ptHLYrL9aIB-WKWmdIg..',
    LogisticsServiceDirectionsLanguage: 'en',
    UploadExtension: 'jpg,jpeg,tif,gif,png,bmp,pdf',
    maxFileSize: 5242880,
    LogisticsServiceTimeUnits: 'Minutes',
    LogisticsServiceDistanceUnits: 'Kilometers',
    LogisticsServiceUturnPolicy: 'NO_UTURNS',
    LogisticsServicePopulateDirections: 'true',
    // cookie Domain
    CookieDomain: '',
    hostName: 'vpn.nwc.com.sa'

  },

  messages: {
    defaultErrorMessage: 'unexpectedErrorMessage'
  },
  GridSetting: {
    Pagesize: 20
  },
  Logo: {
    src: '/assets/fmsBranding/styles/img/lts-logo.png',
    // src: '/assets/fmsBranding/styles/img/logo.png'
    // src: '/assets/TMSBranding/styles/img/NWC-landscape.png'
  },
  AutoRefresh: {
    milliseconds: 60000
  },
  DropDownListSearch: {
    Miliseconds: 1000
  },
  Dashboard: {
    DefaultServiceTypeId: 1
  },
  RegExp: {
    ValidMobileNumber: '^5\\d{8}$'
  },
  Gate: {
    EntryGateClasses: [1, 2, 4]
  }
};

