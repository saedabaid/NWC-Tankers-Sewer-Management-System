using Infrastructure;
using Newtonsoft.Json;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Constants;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Security;

namespace NWC.BLL.Services
{
    public class LookupService : ILookupService
    {
        private static int DDlElementsCount
        {
            get
            {
                int _count;
                int.TryParse(ConfigurationManager.AppSettings["DDlElementsCount"] != null ?
                    ConfigurationManager.AppSettings["DDlElementsCount"] : string.Empty, out _count);

                return _count > 0 ? _count : 50;
            }
        }

        #region Properties
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<NWC_CustomerLocationClass> _CustomerLocationClassRepository;
        private readonly IRepository<NWC_CustomerLocationCategory> _CustomerLocationCategoryRepository;
        private readonly IRepository<NWC_CustomerLocationPriority> _CustomerLocationPriorityRepository;
        private readonly IRepository<NWC_CustomerLocationStatus> _customerLocationStatusRepository;
        private readonly IRepository<NWC_ServiceType> _ServiceTypeRepository;
        private readonly IRepository<NWC_UserServicePermission> _UserServicePermissionRepository;
        private readonly IRepository<NWC_WorkOrderStatus> _WorkOrderStatusRepository;
        private readonly IRepository<NWC_Accessory> _AccessoryRepository;
        private readonly IRepository<NWC_CustomerLocation> _CustomerLocationRepository;
        private readonly IRepository<vw_NWC_CustomerAccount> _VWCustomerAccountRepository;
        private readonly IRepository<vw_NWC_CustomerAccountZone> _customerAccountZoneRepository;
        private readonly IRepository<NWC_CustomerAccount> _CustomerAccountRepository;
        private readonly IRepository<Branch> _BranchRepository;
        private readonly IRepository<NWC_Zone> _ZoneRepository;
        private readonly IRepository<NWC_DeassignReason> _DeassignReasonRepository;
        private readonly IRepository<NWC_PersonalIDType> _PersonalIDTypeRepository;

        private readonly IRepository<StaffRoleCategory> _StaffRoleCategoryRepository;
        private readonly IRepository<StaffRoles> _StaffRolesRepository;

        //private readonly IRepository<NWC_Station> _StationRepository;
        private readonly IRepository<NWC_ZoneStations> _ZoneStationsRepository;
        private readonly IRepository<Landmark> _LandmarkRepository;
        private readonly IRepository<TransporterType> _TransporterTypeRepository;
        private readonly IRepository<Transporter> _TransporterRepository;
        private readonly IRepository<Staff> _StaffRepository;
        private readonly IRepository<NWC_Customer> _CustomerRepository;
        private readonly IRepository<vw_NWC_WorkOrderList> _StateWorkOrder;
        private readonly IRepository<NWC_StatusReason> _StatusReason;
        private readonly IRepository<NWC_UserLandmarkPermission> _userLandmarkPermission;
        private readonly ILoggedInUserService _LoggedInUserService;
        private readonly IRepository<UserBranchPermission> _userBranchPermission;
        private readonly IRepository<NWC_Contract> _ContractRepository;
        private readonly IRepository<NWC_ContractType> _contractTypesRepository;
        private readonly IRepository<NWC_ContractStatus> _contractStatusRepository;
        private readonly IRepository<NWC_Contractor> _contractorRepository;
        private readonly IRepository<NWC_ContractTerminationReason> _contractTerminationReasonRepository;
        private readonly IRepository<vw_NWC_ContractStations> _vwContractStaionsRepository;
        private readonly IRepository<NWC_TermsCategory> _TermsCategoryRepository;
        private readonly IRepository<NWC_DeviceMeter> _DeviceMeterRepository;
        private readonly IRepository<NWC_TanckerCapacity> _TanckerCapacity;
        private readonly IRepository<NWC_DeferredWorkOrderStatus> _DeferredWorkOrderStatus;
        private readonly IRepository<NWC_TermsValueUnits> _TermsValueUnits;
        //private readonly IRepository<NWC_SoqyaCustomerAccountBalance> _SoqyaBalanceRepository;
        private readonly IRepository<NWC_VehicleLogType> _VehicleLogType;
        private readonly IRepository<TransporterBrand> _TransporterBrand;
        private readonly IRepository<TransporterGroup> _TransporterGroup;
        private readonly IRepository<TransporterProductionYear> _TransporterProductionYear;
        private readonly IRepository<TransporterManufacturer> _TransporterManufacturer;
        private readonly IRepository<TransporterStatus> _TransporterStatus;
        private readonly IRepository<NWC_WorkOrderInvoiceStatus> _WorkOrderInvoiceStatus;
        private readonly IRepository<NWC_ContractTerms> _ContractTerms;
        private readonly IRepository<NWC_ContractTermsViolationStatus> _ContractTermsViolationStatus;
        private readonly IRepository<NWC_ContractTermsViolationCancelReason> _ContractTermsViolationCancelReason;
        private readonly IRepository<NWC_StationStatus> _StationStatus;
        private readonly IRepository<NWC_WorkOrderCategory> _WorkOrderCategory;
        private readonly IRepository<Transporter_Staff> _Transporter_Staff;
        private readonly IRepository<StaffRoles> _StaffRoles;
        private readonly IRepository<Landmark> _Landmark;
        private readonly IRepository<Branch> _Branch;
        private readonly IRepository<aspnet_Membership> _aspnet_Membership;
        private readonly IRepository<Pages> _PagesRepository;

        #endregion

        #region Constructors
        public LookupService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._LoggedInUserService = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);
            this._StaffRoles = new Repository<StaffRoles>(ctx);
            this._Landmark = new Repository<Landmark>(ctx);
            this._Branch = new Repository<Branch>(ctx);
            //===================
            this._CustomerLocationClassRepository = new Repository<NWC_CustomerLocationClass>(ctx);
            this._CustomerLocationCategoryRepository = new Repository<NWC_CustomerLocationCategory>(ctx);
            this._CustomerLocationPriorityRepository = new Repository<NWC_CustomerLocationPriority>(ctx);
            ;
            this._customerLocationStatusRepository = new Repository<NWC_CustomerLocationStatus>(ctx);
            ;
            this._ServiceTypeRepository = new Repository<NWC_ServiceType>(ctx);
            ;
            this._WorkOrderStatusRepository = new Repository<NWC_WorkOrderStatus>(ctx);
            ;
            this._BranchRepository = new Repository<Branch>(ctx);
            this._ZoneRepository = new Repository<NWC_Zone>(ctx);
            //this._StationRepository = stationRepository;
            this._ZoneStationsRepository = new Repository<NWC_ZoneStations>(ctx);
            this._LandmarkRepository = new Repository<Landmark>(ctx);
            this._TransporterRepository = new Repository<Transporter>(ctx);
            this._StaffRepository = new Repository<Staff>(ctx);
            this._CustomerRepository = new Repository<NWC_Customer>(ctx);
            this._StateWorkOrder = new Repository<vw_NWC_WorkOrderList>(ctx);
            this._StatusReason = new Repository<NWC_StatusReason>(ctx);
            this._AccessoryRepository = new Repository<NWC_Accessory>(ctx);
            this._CustomerLocationRepository = new Repository<NWC_CustomerLocation>(ctx);
            this._userLandmarkPermission = new Repository<NWC_UserLandmarkPermission>(ctx);
            this._userBranchPermission = new Repository<UserBranchPermission>(ctx);
            this._DeassignReasonRepository = new Repository<NWC_DeassignReason>(ctx);
            this._ContractRepository = new Repository<NWC_Contract>(ctx);
            this._contractTypesRepository = new Repository<NWC_ContractType>(ctx);
            this._contractStatusRepository = new Repository<NWC_ContractStatus>(ctx);
            this._contractorRepository = new Repository<NWC_Contractor>(ctx);
            this._contractTerminationReasonRepository = new Repository<NWC_ContractTerminationReason>(ctx);
            //this._TransporterRepository = new Repository<Transporter>(ctx);
            this._TransporterTypeRepository = new Repository<TransporterType>(ctx);
            this._contractorRepository = new Repository<NWC_Contractor>(ctx);
            this._PersonalIDTypeRepository = new Repository<NWC_PersonalIDType>(ctx);
            this._vwContractStaionsRepository = new Repository<vw_NWC_ContractStations>(ctx);
            this._TermsCategoryRepository = new Repository<NWC_TermsCategory>(ctx);
            this._DeviceMeterRepository = new Repository<NWC_DeviceMeter>(ctx);
            this._TanckerCapacity = new Repository<NWC_TanckerCapacity>(ctx);
            this._DeferredWorkOrderStatus = new Repository<NWC_DeferredWorkOrderStatus>(ctx);
            this._TermsValueUnits = new Repository<NWC_TermsValueUnits>(ctx);
            this._UserServicePermissionRepository = new Repository<NWC_UserServicePermission>(ctx);
            this._VWCustomerAccountRepository = new Repository<vw_NWC_CustomerAccount>(ctx);
            this._customerAccountZoneRepository = new Repository<vw_NWC_CustomerAccountZone>(ctx);
            this._CustomerAccountRepository = new Repository<NWC_CustomerAccount>(ctx);
            this._VehicleLogType = new Repository<NWC_VehicleLogType>(ctx);
            this._TransporterBrand = new Repository<TransporterBrand>(ctx);
            this._TransporterGroup = new Repository<TransporterGroup>(ctx);
            this._TransporterProductionYear = new Repository<TransporterProductionYear>(ctx);
            this._TransporterManufacturer = new Repository<TransporterManufacturer>(ctx);
            this._TransporterStatus = new Repository<TransporterStatus>(ctx);
            this._WorkOrderInvoiceStatus = new Repository<NWC_WorkOrderInvoiceStatus>(ctx);
            this._ContractTerms = new Repository<NWC_ContractTerms>(ctx);
            this._ContractTermsViolationStatus = new Repository<NWC_ContractTermsViolationStatus>(ctx);
            this._ContractTermsViolationCancelReason = new Repository<NWC_ContractTermsViolationCancelReason>(ctx);
            this._StationStatus = new Repository<NWC_StationStatus>(ctx);
            this._WorkOrderCategory = new Repository<NWC_WorkOrderCategory>(ctx);
            this._Transporter_Staff = new Repository<Transporter_Staff>(ctx);

            this._StaffRoleCategoryRepository = new Repository<StaffRoleCategory>(ctx);
            this._StaffRolesRepository = new Repository<StaffRoles>(ctx);
            this._aspnet_Membership = new Repository<aspnet_Membership>(ctx);
            this._PagesRepository = new Repository<Pages>(ctx);
        }
        #endregion

        #region get
        public DescriptiveResponse<Guid> GetCityBasedOnStation(Guid StationID)
        {
            try
            {

                var lookups = this._LandmarkRepository.GetQuery()
                                    .Where(s => (s.Id == StationID)
                                    && _LoggedInUserService.UserLandmarksIds.Any(a => a == s.Id)
                                    && s.isDeleted != true).SingleOrDefault();
                if (lookups != null)
                {

                    return DescriptiveResponse<Guid>.Success((Guid)lookups.branchId);
                }
                else
                {
                    return DescriptiveResponse<Guid>.Error("the city is not exist");
                }

            }
            catch (Exception ex)
            {
                return DescriptiveResponse<Guid>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        public DescriptiveResponse<bool> IsBranchWithRestrictedSpecialOrders(Guid StationID)
        {
            try
            {
                var BranchID = GetCityBasedOnStation(StationID).Value;
                var path = @"c:\jsonFiles\branchData.json";
                var jsonFile = System.IO.File.ReadAllText(path);
                List<RestrictedBranches> BranchData = JsonConvert.DeserializeObject<List<RestrictedBranches>>(jsonFile);
                if (BranchData == null)
                    return DescriptiveResponse<bool>.Success(false);
                var isRestricted = BranchData.Where(x => x.BranchID.ToString().ToLower() == BranchID.ToString().ToLower()).Any();
                return DescriptiveResponse<bool>.Success(isRestricted);

            }
            catch (Exception ex)
            {
                //TODO
                //return DescriptiveResponse<bool>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
                return DescriptiveResponse<bool>.Success(false);
            }
        }
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationClasses()
        {
            try
            {
                var lookups = this._CustomerLocationClassRepository.GetQuery()
                                    .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr,
                                        IntegrationId = s.IntegrationId
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetCustomerLocationClasses: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationCategories()
        {
            try
            {
                var lookups = this._CustomerLocationCategoryRepository.GetQuery()
                                    .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr,
                                        IntegrationId = s.IntegrationId
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetCustomerLocationCategories: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationPriorities()
        {
            try
            {
                var lookups = this._CustomerLocationPriorityRepository.GetQuery()
                                    .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr,
                                        IntegrationId = s.IntegrationId
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetCustomerLocationPriorities: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationStatuses()
        {
            try
            {
                var lookups = this._customerLocationStatusRepository.GetQuery()
                                    .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr,
                                        IntegrationId = s.IntegrationId
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetCustomerLocationStatuses: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetServiceTypes()
        {
            try
            {
                var lookups = this._ServiceTypeRepository.GetQuery()
                                    .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.TypeEn : s.TypeAr,
                                        IntegrationId = s.IntegrationId
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetServiceTypes: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetVehicleLogTypes()
        {
            try
            {
                var lookups = this._VehicleLogType.GetQuery()
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.Id,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetVehicleLogTypes: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }



        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> getPermittedServicesTypes()
        {
            try
            {
                var lookups = this._UserServicePermissionRepository.GetQuery()
                            .Where(s => s.StaffId == this._LoggedInUserService.LoggedInUser.StaffId
                                        && !s.IsDeleted
                                        && s.NWC_ServiceType.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                        )
                            .Select(s => new LookUpDTO<int>
                            {
                                Id = s.NWC_ServiceType.ID,
                                Name = LanguageIsEnglish ? s.NWC_ServiceType.TypeEn : s.NWC_ServiceType.TypeAr
                            }).ToList();
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);


                //var permittedServiceIDs = _UserServicePermissionRepository.GetQuery().Where(x => x.StaffId == staffId)?.Select(s=>s.ServiceID).ToList();
                //if(permittedServiceIDs != null && permittedServiceIDs.Any())
                //{
                //    var lookups = this._ServiceTypeRepository.GetQuery()
                //                   .Where(s => permittedServiceIDs.Contains(s.ID) && s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                //                   .Select(s => new LookUpDTO<int>
                //                   {
                //                       Id = s.ID,
                //                       Name = LanguageIsEnglish ? s.TypeEn : s.TypeAr,
                //                       IntegrationId = s.IntegrationId
                //                   }).ToList();

                //    return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
                //}
                //return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(null);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => getPermittedServicesTypes: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetWorkOrderStatuses()
        {
            try
            {
                var lookups = this._WorkOrderStatusRepository.GetQuery()
                                    .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr,
                                        IntegrationId = s.IntegrationId
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetWorkOrderStatuses: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetWorkOrderStatusesInDeassign()
        {
            try
            {
                var lookups = this._WorkOrderStatusRepository.GetQuery()
                                    .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                                && (s.ID == 1 || s.ID == 2 || s.ID == 8))
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr,
                                        IntegrationId = s.IntegrationId
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetWorkOrderStatusesInDeassign: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetAccessories()
        {
            try
            {
                var lookups = this._AccessoryRepository.GetQuery()
                                    .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr,
                                        IntegrationId = s.IntegrationId
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetAccessories: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetZoneStations(long ZoneId)
        {
            try
            {
                var lookups = this._ZoneStationsRepository.GetQuery()
                                    .Where(s => (s.Landmark.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId) && (s.ZoneID == ZoneId))
                                    .Select(s => new LookUpDTO<Guid>
                                    {
                                        Id = s.StationID,
                                        Name = s.Landmark.name,
                                        IntegrationId = s.Landmark.IntegrationId
                                    }).Take(DDlElementsCount).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetZoneStations: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetUserStations(string searchKeyword)
        {
            try
            {
                var lookupsQuery = this._LandmarkRepository.GetQuery()
                                    .Where(s => (s.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId));


                #region search text -OR- return top 100
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookupsQuery = lookupsQuery.Where(s => s.name.Contains(text)).Take(DDlElementsCount);
                }
                else
                    lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                #endregion

                var lookups = lookupsQuery
                    .Select(s => new LookUpDTO<Guid>
                    {
                        Id = s.Id,
                        Name = s.name
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetUserStations: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<LookUpDTO<Guid>> GetMainZoneStation(long zoneId)
        {
            try
            {
                var lookups = this._ZoneStationsRepository.GetQuery()
                                    .Where(s => s.Landmark.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId && s.ZoneID == zoneId && s.IsMain)
                                    .Select(s => new LookUpDTO<Guid>
                                    {
                                        Id = s.StationID,
                                        Name = s.Landmark.name,
                                        IntegrationId = s.Landmark.IntegrationId
                                    }).ToList();

                return DescriptiveResponse<LookUpDTO<Guid>>.Success(lookups.FirstOrDefault());
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetMainZoneStation: "));
                return DescriptiveResponse<LookUpDTO<Guid>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetStaffSelectedCategory(Guid? key = null)
        {
            try
            {
                if (key != null)
                {
                    var lookups = this._StaffRolesRepository.GetQuery()
                                        .Where(s => s.subID == this._LoggedInUserService.LoggedInUser.SubscriberId && s.ID == key)
                                        .Select(s => new LookUpDTO<int>
                                        {
                                            Id = s.StaffRoleCategory.ID,
                                            Name = s.StaffRoleCategory.Name
                                        }).ToList();


                    return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
                }
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(null);

            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetStaffSelectedCategory: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<string> GetStaffSelectedRoleName(Guid? key = null)
        {
            try
            {
                if (key != null)
                {
                    string name = "";
                    var lookups = this._StaffRolesRepository.GetQuery()
                                        .FirstOrDefault(s => s.subID == this._LoggedInUserService.LoggedInUser.SubscriberId && s.ID == key);

                    if (lookups != null)
                    {
                        name = lookups.name;
                        return DescriptiveResponse<string>.Success(lookups.name);
                    }
                }
                return DescriptiveResponse<string>.Success(null);

            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetStaffSelectedRoleName: "));
                return DescriptiveResponse<string>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetStaffDefaultPage(Guid? key = null)
        {
            try
            {
                if (key != null)
                {
                    var lookups = this._StaffRolesRepository.GetQuery()
                                        .Where(s => s.subID == this._LoggedInUserService.LoggedInUser.SubscriberId && s.ID == key).ToList();

                    if (lookups != null && lookups.Count > 0)
                    {
                        if (lookups.FirstOrDefault().PageId != null && lookups.FirstOrDefault().isDefault == true)
                        {
                            var lookup = lookups.Select(s => new LookUpDTO<Guid>
                            {
                                Id = s.Pages.ID,
                                Name = s.Pages.Name
                            }).ToList();

                            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookup);
                        }
                    }
                }
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(null);

            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetStaffDefaultPage: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }


        public DescriptiveResponse<Guid> GetMainZoneStationByZoneIntID(string zoneIntId)
        {
            try
            {
                var StationID = (from ex in this._ZoneRepository.GetQuery()
                                 join ex1 in this._ZoneStationsRepository.GetQuery()
on ex.ID equals ex1.ZoneID
                                 where ex.IsDeleted != true && ex1.IsMain == true && ex.IntegrationId == zoneIntId
                                 select ex1.StationID).SingleOrDefault();

                return DescriptiveResponse<Guid>.Success(StationID);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetMainZoneStationByZoneInt: "));
                return DescriptiveResponse<Guid>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> GetStationStatusByZoneIntID(string zoneIntId)
        {
            try
            {
                var StationStatus = (from ex in this._ZoneRepository.GetQuery()
                                     join ex1 in this._ZoneStationsRepository.GetQuery()
                                     on ex.ID equals ex1.ZoneID
                                     where ex.IsDeleted != true && ex1.IsMain == true && ex.IntegrationId == zoneIntId
                                     select ex1.Landmark.IsOnline)
                                 .SingleOrDefault();

                if (StationStatus != null)
                    return DescriptiveResponse<bool>.Success(StationStatus.Value);

                throw new ArgumentNullException("GetStationStatusByZoneInt - StationStatus can not be null.");
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetStationStatusByZoneInt: "));
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerLocations(long CustomerId)
        {
            try
            {
                var lookups = this._CustomerLocationRepository.GetQuery()
                    .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId) &&
                                (s.CustomerID == CustomerId) &&
                                s.IsDeleted != true &&
                                s.StatusID != 1) //Blacklisted 
                                    .Select(s => new LookUpDTO<long>
                                    {
                                        Id = s.ID,
                                        Name = s.Address
                                    }).Take(DDlElementsCount).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetCustomerLocations: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerAccounts(long CustomerId, int[] serviceTypeId = null)
        {
            try
            {
                //var lookups = this._VWCustomerAccountRepository.GetQuery()
                var myQuery = this._customerAccountZoneRepository.GetQuery()
                    .Where(s => //(s.CustomerSubId == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                //&&
                                (s.CustomerId == CustomerId)
                                && s.IsDeleted != true
                                && s.CustomerLocationStatusId != 1 //Blacklisted 
                                );

                if (serviceTypeId != null && serviceTypeId.Any())
                {
                    myQuery = myQuery.Where(s => serviceTypeId.Contains(s.ServiceTypeId));
                }

                var lookups = myQuery
                        .Select(s => new LookUpDTO<long>
                        {
                            Id = s.Id,
                            Name = LanguageIsEnglish
                            ? s.ServiceTypeEn + " - " + s.CustomerLocationAddress + " - " + s.AccountId_Integration + " - " + s.ZoneName
                            : s.ServiceTypeAr + " - " + s.CustomerLocationAddress + " - " + s.AccountId_Integration + " - " + s.ZoneName
                        }).Take(DDlElementsCount).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetCustomerAccounts: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetCustomerCommercialAccounts(long CustomerId, int[] serviceTypeId = null)
        {
            try
            {
                //var lookups = this._VWCustomerAccountRepository.GetQuery()
                var myQuery = this._customerAccountZoneRepository.GetQuery()
                    .Where(s => //(s.CustomerSubId == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                //&&
                                (s.CustomerId == CustomerId)
                                && s.IsDeleted != true
                                && s.CustomerLocationStatusId != 1 //Blacklisted 
                                && s.CustomerLocationClassId == 2 // Commercial Order
                                );

                if (serviceTypeId != null && serviceTypeId.Any())
                {
                    myQuery = myQuery.Where(s => serviceTypeId.Contains(s.ServiceTypeId));
                }

                var lookups = myQuery
                        .Select(s => new LookUpDTO<long>
                        {
                            Id = s.Id,
                            Name = LanguageIsEnglish
                            ? s.ServiceTypeEn + " - " + s.CustomerLocationAddress + " - " + s.AccountId_Integration + " - " + s.ZoneName
                            : s.ServiceTypeAr + " - " + s.CustomerLocationAddress + " - " + s.AccountId_Integration + " - " + s.ZoneName
                        }).Take(DDlElementsCount).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetCustomerAccounts: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }


        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTypes()
        {
            try
            {
                var lookups = this._contractTypesRepository.GetQuery()
                    .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId))
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetContractTypes: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractStatuses()
        {
            try
            {
                var lookups = this._contractStatusRepository.GetQuery()
                    .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId))
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetContractStatuses: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTerminationReasons()
        {
            try
            {
                var lookups = this._contractTerminationReasonRepository.GetQuery()
                    .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId))
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.ReasonEn : s.ReasonAr
                                    })
                                    .ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetContractTerminationReasons: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> getZonesByNameOrCode()
        {
            try
            {
                var ZoneList = this._ZoneRepository.GetQuery().
                    Where(z => z.SubID == _LoggedInUserService.LoggedInUser.SubscriberId
                    && _LoggedInUserService.SubBranches.Contains(z.CityID))
                    .Take(DDlElementsCount)
                    .Select(s => new LookUpDTO<long>
                    {
                        Id = s.ID,
                        Name = s.Name + " | " + s.Code
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(ZoneList);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => getZonesByNameOrCode: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchUserByName(string searchKeyword)
        {
            try
            {
                var text = searchKeyword.Trim();
                var UserList = this._aspnet_Membership.GetQuery().
                    Where(z => z.Email.Contains(text))
                    .Take(DDlElementsCount)
                    .Select(s => new LookUpDTO<Guid>
                    {
                        Id = s.UserId,
                        Name = s.Email
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(UserList);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchUserByName: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> getContractStations(long contractId)
        {
            try
            {
                var stations = this._vwContractStaionsRepository.GetQuery()
                        .Where(a => a.ContractID == contractId && a.stationIsDeleted != true && a.ContractStationIsDeleted != true)
                        .Select(s => new LookUpDTO<Guid>
                        {
                            Id = s.StationID,
                            Name = s.stationName
                        })
                        .Distinct().ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(stations);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => getContractStations: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetPersonalIDTypes()
        {
            try
            {
                var PersonalIDTypes = this._PersonalIDTypeRepository.GetQuery().
                    Where(a => a.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                    .Select(s => new LookUpDTO<int>
                    {
                        Id = s.ID,
                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr,
                        IntegrationId = s.IntegrationId
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(PersonalIDTypes);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetPersonalIDTypes: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> getCustomerLocationStations(long customerLocationId)
        {
            try
            {
                var customerLocation = this._CustomerLocationRepository.GetQuery()
                    .FirstOrDefault(s => s.ID == customerLocationId && s.IsDeleted != true //&& s.NWC_Customer.IsDeleted != true
                    );

                if (customerLocation != null)
                {
                    var mainStation = customerLocation.NWC_Zone.NWC_ZoneStations
                                .Where(s => s.IsMain &&
                                            s.Landmark.StatusID == 1 &&
                                            s.Landmark.isDeleted != true &&
                                            this._LoggedInUserService.UserLandmarksIds.Contains(s.StationID))
                               .Select(s => new LookUpDTO<Guid>
                               {
                                   Id = s.Landmark.Id,
                                   Name = s.Landmark.name
                               }).ToList();

                    if (mainStation != null && mainStation.Count() > 0)
                    {
                        return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(mainStation);
                    }
                    else
                    {
                        var backupStations = customerLocation.NWC_Zone.NWC_ZoneStations
                                    .Where(s => !s.IsMain &&
                                                s.Landmark.StatusID == 1 &&
                                                s.Landmark.isDeleted != true &&
                                                s.Landmark.NWC_StationCustomerLocationClass.Any(a => a.CustomerLocationClassID == customerLocation.ClassID) &&
                                                this._LoggedInUserService.UserLandmarksIds.Contains(s.StationID))
                                   .Select(s => new LookUpDTO<Guid>
                                   {
                                       Id = s.Landmark.Id,
                                       Name = s.Landmark.name
                                   }).ToList();

                        return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(backupStations);
                    }
                }
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(null);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => getCustomerLocationStations: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> getCustomerAccountStations(long customerAccountId)
        {
            try
            {
                var customerAccount = this._CustomerAccountRepository.GetQuery()
                    .FirstOrDefault(s => s.ID == customerAccountId
                                            && s.IsDeleted != true
                                            && s.NWC_CustomerLocation.IsDeleted != true);

                if (customerAccount != null)
                {
                    var mainStation = customerAccount.NWC_CustomerLocation.NWC_Zone.NWC_ZoneStations
                                .Where(s => s.IsMain &&
                                            s.Landmark.StatusID == 1 &&
                                            s.Landmark.isDeleted != true &&
                                            this._LoggedInUserService.UserLandmarksIds.Contains(s.StationID) &&
                                            s.Landmark.StatusID == (int)StationStatusEnum.InService)
                               .Select(s => new LookUpDTO<Guid>
                               {
                                   Id = s.Landmark.Id,
                                   Name = s.Landmark.name
                               }).ToList();

                    if (mainStation != null && mainStation.Count() > 0)
                    {
                        return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(mainStation);
                    }
                    else
                    {
                        var backupStations = customerAccount.NWC_CustomerLocation.NWC_Zone.NWC_ZoneStations
                                    .Where(s => !s.IsMain &&
                                                s.Landmark.StatusID == 1 &&
                                                s.Landmark.isDeleted != true &&
                                                s.Landmark.NWC_StationCustomerLocationClass.Any(a => a.CustomerLocationClassID == customerAccount.NWC_CustomerLocation.ClassID) &&
                                                this._LoggedInUserService.UserLandmarksIds.Contains(s.StationID) &&
                                                s.Landmark.StatusID == (int)StationStatusEnum.InService)
                                   .Select(s => new LookUpDTO<Guid>
                                   {
                                       Id = s.Landmark.Id,
                                       Name = s.Landmark.name
                                   }).ToList();

                        return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(backupStations);
                    }
                }
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(null);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => getCustomerAccountStations: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }


        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetTermsCategory()
        {
            try
            {
                var TermsCategoryList = this._TermsCategoryRepository.GetQuery().
                                Where(a => a.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                .Select(s => new LookUpDTO<long>
                                {
                                    Id = s.ID,
                                    Name = LanguageIsEnglish ? s.CategoryEn : s.CategoryAr
                                }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(TermsCategoryList);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetTermsCategory: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTanckerCapacities()
        {
            try
            {
                var TermsCategoryList = this._TanckerCapacity.GetQuery()
                                .Select(s => new LookUpDTO<int>
                                {
                                    Id = s.Capacity,
                                    Name = s.Capacity.ToString()
                                }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(TermsCategoryList);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetTanckerCapacities: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTanckerCapacitiesByStation(Guid stationId, int customerAccountId)
        {
            try
            {
                var serviceTypeId = this._CustomerAccountRepository.GetQuery()
                    .Where(s => s.ID == customerAccountId)
                    .Select(s => s.ServiceTypeId)
                    .FirstOrDefault();

                var capacities = this._TransporterRepository.GetQuery()
                                .Where(s => s.isDeleted != true
                                            && s.landmark == stationId
                                            && s.ServiceTypeID == serviceTypeId
                                            && s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                            && s.Capacity != null)
                                .Select(s => s.Capacity)
                                .Distinct()
                                .ToList();

                var lookup = capacities
                    .Select(s => new LookUpDTO<int>
                    {
                        Id = s.Value,
                        Name = s.ToString()
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookup);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetTanckerCapacitiesByStation: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }

        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetDeferredOrdersStatuses()
        {
            try
            {
                var Statuses = this._DeferredWorkOrderStatus.GetQuery()
                    .Select(s => new LookUpDTO<int>
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(Statuses);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetDeferredOrdersStatuses: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTermsValueUnits()
        {
            try
            {
                var Statuses = this._TermsValueUnits.GetQuery()
                    .Select(s => new LookUpDTO<int>
                    {
                        Id = s.Id,
                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(Statuses);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetTermsValueUnits: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCommingMonthYear()
        {
            try
            {
                var result = new List<LookUpDTO<int>>();

                var startDate = DateTimeHelper.GetDateTimeNow();
                var endDate = startDate.AddMonths(13);

                while (startDate < endDate)
                {
                    var newItem = new LookUpDTO<int>
                    {
                        Id = startDate.Year * 100 + startDate.Month,
                        Name = startDate.ToString("MMM yyyy")
                    };

                    result.Add(newItem);

                    startDate = startDate.AddMonths(1);
                }

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(result);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetCommingMonthYear: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterBrands()
        {
            try
            {
                var lookUps = this._TransporterBrand.GetQuery()
                    .Where(s => s.IsDeleted != true)
                    .Select(s => new LookUpDTO<Guid>
                    {
                        Id = s.ID,
                        Name = LanguageIsEnglish ? s.name : s.NameAr
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookUps);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetTransporterBrands: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterGroups()
        {
            try
            {
                var lookUps = this._TransporterGroup.GetQuery()
                    .Select(s => new LookUpDTO<Guid>
                    {
                        Id = s.ID,
                        Name = LanguageIsEnglish ? s.name : s.name
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookUps);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetTransporterBrands: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterProductionYears()
        {
            try
            {
                var lookUps = this._TransporterProductionYear.GetQuery()
                    .Select(s => new LookUpDTO<Guid>
                    {
                        Id = s.ID,
                        Name = LanguageIsEnglish ? s.name : s.name
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookUps);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetTransporterBrands: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterManufacturer()
        {
            try
            {
                var lookUps = _TransporterManufacturer.GetQuery()
                    .Select(s => new LookUpDTO<Guid>
                    {
                        Id = s.ID,
                        Name = LanguageIsEnglish ? s.name : s.NameAr
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookUps);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => TransporterManufacturer: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTransporterStatuses()
        {
            try
            {
                var Statuses = this._TransporterStatus.GetQuery()
                    .Where(s => s.IsDeleted != true)
                    .Select(s => new LookUpDTO<int>
                    {
                        Id = s.ID,
                        Name = LanguageIsEnglish ? s.Name : s.NameAr
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(Statuses);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetTransporterStatuses: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetTransporterStatusesInDeassign()
        {
            try
            {
                var Statuses = this._TransporterStatus.GetQuery()
                    .Where(s => s.IsDeleted != true
                                && (s.ID == 0 || s.ID == 2))
                    .Select(s => new LookUpDTO<int>
                    {
                        Id = s.ID,
                        Name = LanguageIsEnglish ? s.Name : s.NameAr
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(Statuses);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetTransporterStatusesInDeassign: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetOrderInvoiceStatuses()
        {
            try
            {
                var Statuses = this._WorkOrderInvoiceStatus.GetQuery()
                    .Select(s => new LookUpDTO<int>
                    {
                        Id = s.ID,
                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(Statuses);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetOrderInvoiceStatuses: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> GetContractTerm(long contractId, Guid stationId, int categoryId)
        {
            try
            {

                var lookup = this._ContractTerms.GetQuery()
                    .Where(s => s.IsDeleted != true
                                && s.IsActive != false
                                //&& s.NWC_Contract.IsDeleted != true
                                && s.ContractID == contractId
                                && s.StationID == stationId
                                && s.TermsCategoryID == categoryId
                                )
                    .Select(s => new LookUpDTO<long>
                    {
                        Id = s.ID,
                        Name = s.Code + " - " + s.Name
                    });

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookup);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetContractTerm: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }

        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTermsViolationStatuses()
        {
            try
            {
                var Statuses = this._ContractTermsViolationStatus.GetQuery()
                    .Select(s => new LookUpDTO<int>
                    {
                        Id = s.Id,
                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(Statuses);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetContractTermsViolationStatuses: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetContractTermsViolationCancelReasons()
        {
            try
            {
                var Statuses = this._ContractTermsViolationCancelReason.GetQuery()
                    .Select(s => new LookUpDTO<int>
                    {
                        Id = s.Id,
                        Name = LanguageIsEnglish ? s.ReasonEn : s.ReasonAr
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(Statuses);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetContractTermsViolationCancelReasons: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetStationStatuses()
        {
            try
            {
                var Statuses = this._StationStatus.GetQuery()
                    .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                    .Select(s => new LookUpDTO<int>
                    {
                        Id = s.ID,
                        Name = LanguageIsEnglish ? s.StatusEn : s.StatusAr
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(Statuses);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetStationStatuses: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetWorkOrderCategory()
        {
            try
            {
                var Statuses = this._WorkOrderCategory.GetQuery()
                    .Select(s => new LookUpDTO<int>
                    {
                        Id = s.ID,
                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(Statuses);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetWorkOrderCategory: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }


        // --------By Omer

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetStaffRoleCategories()
        {
            try
            {
                var categories = this._StaffRoleCategoryRepository.GetQuery()
                    .Select(x => new LookUpDTO<int>
                    {
                        Id = x.ID,
                        Name = x.Name
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(categories);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetStaffRoleCategories: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetStaffRolesByCategoryID(int staffRoleCategoryID)
        {
            try
            {
                var catid = 10;
                var roles = this._StaffRolesRepository.GetQuery()
                    .Where(x => x.subID == this._LoggedInUserService.LoggedInUser.SubscriberId
                    && x.category == staffRoleCategoryID)
                    .Select(s => new LookUpDTO<Guid>
                    {
                        Id = s.ID,
                        Name = s.name,
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(roles);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetStaffRolesByCategoryID: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetPageList()
        {
            try
            {
                var roles = this._PagesRepository.GetQuery()
                    .Select(s => new LookUpDTO<Guid>
                    {
                        Id = s.ID,
                        Name = s.Name,
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(roles);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetPageList: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }


        #endregion

        #region AutoCompelete
        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchAreasByName(string searchKeyword)
        {
            try
            {
                IQueryable<Branch> lookupsQuery = this._BranchRepository.GetQuery();
                //var permitted = this._LoggedInUserService.PermittedBranches.Select(a => a).ToList();
                //permitted.AddRange(lookupsQuery.Where(s => permitted.Any(a => a == s.Id) && s.parentBranchId != null).Select(x => x.Branch2.Id));

                lookupsQuery = lookupsQuery.Where(s => //permitted.Any(a => (a == s.Id) || (a == s.parentBranchId))
                                             this._LoggedInUserService.Branches.Contains(s.Id)
                                             && s.isDeleted != true
                                             && s.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId
                                             && s.parentBranchId == null);

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookupsQuery = lookupsQuery.Where(s => s.name.Contains(text)).Take(DDlElementsCount);
                }
                else
                    lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                #endregion

                var lookups = lookupsQuery
                    .Select(s => new LookUpDTO<Guid>
                    {
                        Id = s.Id,
                        Name = s.name
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchAreasByName: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<CityDTO>> GetPermittedCities()
        {
            try
            {
                IQueryable<Branch> lookupsQuery = this._BranchRepository.GetQuery()
                 .Where(s => this._LoggedInUserService.SubBranches.Contains(s.Id));

                var lookups = lookupsQuery
                    .Select(s => new CityDTO
                    {
                        ID = s.Id,
                        Name = s.name,
                        TankerQuotaNo = (s.NWC_BranchSetting != null && s.NWC_BranchSetting.TankerQuotaNo.HasValue) ? s.NWC_BranchSetting.TankerQuotaNo.Value : -1,
                        AutoCancelationNewOrdersHours = (s.NWC_BranchSetting != null && s.NWC_BranchSetting.AutoCancelationNewOrdersHours.HasValue) ? s.NWC_BranchSetting.AutoCancelationNewOrdersHours.Value : -1,
                        AutoCancelationOnHoldOrdersHours = (s.NWC_BranchSetting != null && s.NWC_BranchSetting.AutoCancelationOnHoldOrdersHours.HasValue) ? s.NWC_BranchSetting.AutoCancelationOnHoldOrdersHours.Value : -1
                    }).ToList();

                return DescriptiveResponse<IEnumerable<CityDTO>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetPermittedCities: "));
                return DescriptiveResponse<IEnumerable<CityDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchCitiesByName(List<Guid> areasIds, string searchKeyword)
        {
            try
            {
                if (areasIds != null && areasIds.Any())
                {
                    IQueryable<Branch> lookupsQuery = this._BranchRepository.GetQuery()
                     .Where(s => areasIds.Any(b => b == s.parentBranchId)
                                            && s.isDeleted != true
                                            && s.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId
                                            && this._LoggedInUserService.SubBranches.Contains(s.Id));

                    #region search text -OR- return top 10
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        var text = searchKeyword.Trim();
                        lookupsQuery = lookupsQuery.Where(s => s.name.Contains(text)).Take(DDlElementsCount);
                    }
                    else
                        lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                    #endregion

                    var lookups = lookupsQuery
                        .Select(s => new LookUpDTO<Guid>
                        {
                            Id = s.Id,
                            Name = s.name
                        }).ToList();

                    return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
                }

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchCitiesByName: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchZonesByName(List<Guid> cityIds, string searchKeyword)
        {
            try
            {
                if (cityIds != null && cityIds.Any())
                {
                    IQueryable<NWC_Zone> lookupsQuery = this._ZoneRepository.GetQuery()
                        .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                     && cityIds.Contains(s.CityID)
                                     //&& s.NWC_ZoneStations.Any(x => cityIds.Contains(s.CityID))
                                     && s.IsDeleted == false);

                    #region return top 10 || search text
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        var text = searchKeyword.Trim();
                        lookupsQuery = lookupsQuery.Where(s => s.Name.Contains(text)).Take(DDlElementsCount);
                    }
                    else
                        lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                    #endregion

                    var lookups = lookupsQuery
                        .Select(s => new LookUpDTO<long>
                        {
                            Id = s.ID,
                            Name = s.Name
                        }).ToList();

                    return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookups);
                }

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchZonesByName: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchStations(List<long> zoneIds, string searchKeyword)
        {
            try
            {
                if (zoneIds != null && zoneIds.Any())
                {
                    IQueryable<Landmark> lookupsQuery = this._LandmarkRepository.GetQuery()
                        .Where(s => (s.isDeleted != true)
                                    && (s.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    && s.NWC_ZoneStations.Any(a => zoneIds.Contains(a.ZoneID))
                                    && this._LoggedInUserService.UserLandmarksIds.Contains(s.Id));

                    #region return top 10 || search text
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        var text = searchKeyword.Trim();
                        lookupsQuery = lookupsQuery.Where(s => s.name.Contains(text)).Take(DDlElementsCount);
                    }
                    else
                        lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                    #endregion

                    var lookups = lookupsQuery
                        .Select(s => new LookUpDTO<Guid>
                        {
                            Id = s.Id,
                            Name = s.name
                        }).ToList();

                    return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
                }

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchStations: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchVehicles(List<Guid> stationIds, string searchKeyword)
        {
            try
            {
                if (stationIds != null && stationIds.Any())
                {
                    IQueryable<Transporter> lookupsQuery = this._TransporterRepository.GetQuery()
                        .Where(s => s.isDeleted != true
                                    && (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    && stationIds.Any(x => x == s.landmark)
                                    && s.Transporter_Staff.Any());

                    #region search text -OR- return top 10
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        var text = searchKeyword.Trim();
                        lookupsQuery = lookupsQuery.Where(s => s.plateNo.Contains(text) || s.code.Contains(text)).Take(DDlElementsCount);
                    }
                    else
                        lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                    #endregion

                    var lookups = lookupsQuery
                        .Select(s => new LookUpDTO<Guid>
                        {
                            Id = s.ID,
                            Name = s.code + " | " + s.plateNo
                        }).ToList();

                    return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
                }

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchVehicles: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchVehicles(string searchKeyword)
        {
            try
            {
                IQueryable<Transporter> lookupsQuery = this._TransporterRepository.GetQuery()
                            .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                        && this._LoggedInUserService.PermittedBranches.Any(a => a == s.branch)
                                        && this._LoggedInUserService.UserLandmarksIds.Any(a => a == s.landmark)
                                        && s.isDeleted != true);

                #region search text -OR- return top 50
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookupsQuery = lookupsQuery.Where(s => s.plateNo.Contains(text) || s.code.Contains(text)).Take(DDlElementsCount);
                }
                else
                    lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                #endregion

                var lookups = lookupsQuery
                    .Select(s => new LookUpDTO<Guid>
                    {
                        Id = s.ID,
                        Name = s.code + " | " + s.plateNo
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchVehicles: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchDriversByTanker(List<Guid> transporterIds, string searchKeyword)
        {
            try
            {
                if (transporterIds != null && transporterIds.Any())
                {
                    var lookupsQuery = this._Transporter_Staff.GetQuery()
                        .Where(s => (s.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    && transporterIds.Any(a => a == s.Transporter)
                                    && (s.Staff1.StaffRoles.category == (short)6))//Driver = 6, Driver's Companion= 7 
                        .AsQueryable();

                    #region search text -OR- return top 10
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        var text = searchKeyword.Trim();
                        var staffArr = searchKeyword.Trim().Split(' ');
                        if (staffArr.Length == 1)
                        {
                            lookupsQuery = lookupsQuery
                                .Where(s => s.Staff1.FirstName.Contains(text) || s.Staff1.MiddleName.Contains(text) || s.Staff1.LastName.Contains(text));
                        }
                        else
                        {
                            lookupsQuery = lookupsQuery.Where(s =>
                                    (s.Staff1.FirstName + " " + s.Staff1.MiddleName + " " + s.Staff1.LastName).Contains(text)
                                    || (s.Staff1.FirstName + " " + s.Staff1.LastName).Contains(text));
                        }
                        lookupsQuery = lookupsQuery.Take(DDlElementsCount);

                        //lookupsQuery = lookupsQuery.Where(s => s.FirstName.Contains(text) || s.MiddleName.Contains(text) || s.LastName.Contains(text)).Take(DDlElementsCount);
                    }
                    else
                        lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                    #endregion

                    var lookups = lookupsQuery
                        .OrderByDescending(s => s.VehicleReceivingDate)
                        .Take(1)
                        .Select(s => new LookUpDTO<Guid>
                        {
                            Id = s.Staff1.ID,
                            Name = s.Staff1.FirstName + " " + s.Staff1.MiddleName + " " + s.Staff1.LastName
                        }).ToList();

                    return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
                }

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchDriversByTanker: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchDrivers(List<Guid> stationIds, string searchKeyword)
        {
            try
            {
                if (stationIds != null && stationIds.Any())
                {
                    IQueryable<Staff> lookupsQuery = this._StaffRepository.GetQuery()
                        .Where(s => (s.subID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    && (s.StaffRoles.subID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    && (s.StaffRoles.category == (short)6) //Driver = 6, Driver's Companion= 7 
                                    && stationIds.Any(x => x == s.AllocatedLandmark));

                    #region search text -OR- return top 10
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        var text = searchKeyword.Trim();
                        var staffArr = searchKeyword.Trim().Split(' ');
                        if (staffArr.Length == 1)
                        {
                            lookupsQuery = lookupsQuery
                                .Where(s => s.FirstName.Contains(text) || s.MiddleName.Contains(text) || s.LastName.Contains(text));
                        }
                        else
                        {
                            lookupsQuery = lookupsQuery.Where(s =>
                                    (s.FirstName + " " + s.MiddleName + " " + s.LastName).Contains(text)
                                    || (s.FirstName + " " + s.LastName).Contains(text));
                        }
                        lookupsQuery = lookupsQuery.Take(DDlElementsCount);

                        //lookupsQuery = lookupsQuery.Where(s => s.FirstName.Contains(text) || s.MiddleName.Contains(text) || s.LastName.Contains(text)).Take(DDlElementsCount);
                    }
                    else
                        lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                    #endregion

                    var lookups = lookupsQuery
                        .Select(s => new LookUpDTO<Guid>
                        {
                            Id = s.ID,
                            Name = s.FirstName + " " + s.MiddleName + " " + s.LastName
                        }).ToList();

                    return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
                }

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchDrivers: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchDrivers(string searchKeyword)
        {
            try
            {
                IQueryable<Staff> lookupsQuery = this._StaffRepository.GetQuery()
                    .Where(s => this._LoggedInUserService.UserLandmarksIds.Any(x => x == s.AllocatedLandmark)
                                && (s.StaffRoles.subID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                && (s.StaffRoles.category == (short)6) //Driver = 6, Driver's Companion= 7 
                                && s.isDeleted != true);// && s.staffRoleID == );

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    var staffArr = searchKeyword.Trim().Split(' ');
                    if (staffArr.Length == 1)
                    {
                        lookupsQuery = lookupsQuery
                            .Where(s => s.FirstName.Contains(text) || s.MiddleName.Contains(text) || s.LastName.Contains(text));
                    }
                    else
                    {
                        lookupsQuery = lookupsQuery.Where(s =>
                                (s.FirstName + " " + s.MiddleName + " " + s.LastName).Contains(text)
                                || (s.FirstName + " " + s.LastName).Contains(text));
                    }
                    lookupsQuery = lookupsQuery.Take(DDlElementsCount);

                    //lookupsQuery = lookupsQuery.Where(s => s.FirstName.Contains(text) || s.MiddleName.Contains(text) || s.LastName.Contains(text))
                    //    .Take(DDlElementsCount);
                }
                else
                    lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                #endregion

                var lookups = lookupsQuery.ToList()
                    .Select(s => new LookUpDTO<Guid>
                    {
                        Id = s.ID,
                        Name = string.Format("{0} {1} {2}", s.FirstName, s.MiddleName, s.LastName)
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchDrivers: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchStaff(string searchKeyword, short? category)
        {

            IQueryable<Staff> lookupsQuery = this._StaffRepository.GetQuery()
                .Where(s => (s.subID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                            && (s.StaffRoles.subID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                            && (s.StaffRoles.category == category || category == null));

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                var text = searchKeyword.Trim();
                var staffArr = searchKeyword.Trim().Split(' ');
                if (staffArr.Length == 1)
                {
                    lookupsQuery = lookupsQuery
                        .Where(s => s.FirstName.Contains(text) || s.MiddleName.Contains(text) || s.LastName.Contains(text) || s.personalID.Contains(text));
                }
                else
                {
                    lookupsQuery = lookupsQuery.Where(s =>
                            (s.FirstName + " " + s.MiddleName + " " + s.LastName).Contains(text)
                            || (s.FirstName + " " + s.LastName).Contains(text));
                }
            }
            lookupsQuery = lookupsQuery.Take(DDlElementsCount);

            //&& (s.FirstName.Contains(text) || s.MiddleName.Contains(text) || s.LastName.Contains(text))
            //).Take(DDlElementsCount);

            var lookups = lookupsQuery
                .Select(s => new LookUpDTO<Guid>
                {
                    Id = s.ID,
                    Name = s.FirstName + " " + s.MiddleName + " " + s.LastName
                }).ToList();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);

        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchCustomers(string searchKeyword)
        {
            try
            {
                IQueryable<NWC_Customer> lookupsQuery = this._CustomerRepository.GetQuery()
                            .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                        && s.NWC_CustomerAccount.Any(a => this._LoggedInUserService.PermittedZonesIds.Contains(a.NWC_CustomerLocation.ZoneID))
                                        && s.IsDeleted != true);

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookupsQuery = lookupsQuery
                        .Where(s => s.FullName.Contains(text) || s.IDNumber.Contains(text) || s.Mobile.Contains(text))
                        .Take(DDlElementsCount);
                }
                else
                    lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                #endregion

                var lookups = lookupsQuery.Select(s => new LookUpDTO<long>
                {
                    Id = s.ID,
                    Name = s.FullName + " | " + s.IDNumber + " | " + s.Mobile
                }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchCustomers: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchSoqyaCustomers(string searchKeyword)
        {
            try
            {
                IQueryable<NWC_Customer> lookupsQuery = this._CustomerRepository.GetQuery()
                            .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                        && s.NWC_CustomerAccount.Any(a => this._LoggedInUserService.PermittedZonesIds.Contains(a.NWC_CustomerLocation.ZoneID)
                                                                            && a.ServiceTypeId == (int)ServiceTypeEnum.Soqya)
                                        //&& s.NWC_CustomerAccount.Any(a => a.SoqyaBalance > 0)
                                        && s.IsDeleted != true);

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookupsQuery = lookupsQuery
                        .Where(s => s.FullName.Contains(text) || s.IDNumber.Contains(text) || s.Mobile.Contains(text))
                        .Take(DDlElementsCount);
                }
                else
                    lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                #endregion

                var lookups = lookupsQuery.Select(s => new LookUpDTO<long>
                {
                    Id = s.ID,
                    Name = s.FullName + " | " + s.IDNumber + " | " + s.Mobile
                }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchSoqyaCustomers: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchCommercialCustomers(string searchKeyword)
        {
            try
            {
                IQueryable<NWC_Customer> lookupsQuery = this._CustomerRepository.GetQuery()
                            .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                        && s.NWC_CustomerAccount.Any(a => this._LoggedInUserService.PermittedZonesIds.Contains(a.NWC_CustomerLocation.ZoneID) && a.NWC_CustomerLocation.NWC_CustomerLocationClass.ID == 2)
                                        && s.IsDeleted != true);

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookupsQuery = lookupsQuery
                        .Where(s => s.FullName.Contains(text) || s.IDNumber.Contains(text) || s.Mobile.Contains(text))
                        .Take(DDlElementsCount);
                }
                else
                    lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                #endregion

                var lookups = lookupsQuery.Select(s => new LookUpDTO<long>
                {
                    Id = s.ID,
                    Name = s.FullName + " | " + s.IDNumber + " | " + s.Mobile
                }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchCommercialCustomers: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchOrderNumbers(string searchKeyword)
        {
            var text = searchKeyword.Trim();

            var lookups = this._StateWorkOrder.GetQuery()
                                .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                        && s.IsDeleted != true
                                        && _LoggedInUserService.SubBranches.Contains(s.CityID.Value)
                                        && _LoggedInUserService.UserLandmarksIds.Contains(s.AssignedStationID)
                                        && s.OrderNumber.Contains(text))
                                        .Take(DDlElementsCount)
                                .Select(s => new LookUpDTO<long>
                                {
                                    Id = s.WorkOrderID,
                                    Name = s.OrderNumber
                                }).ToList();

            return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookups);
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetNextWorkOrderStatus(int currentStatusId)
        {
            try
            {
                var currentStatus = this._WorkOrderStatusRepository.GetQuery().FirstOrDefault(x => x.ID == currentStatusId);

                if (currentStatus != null)
                {
                    List<int> nextStatusesIds = currentStatus.NextStatusIDs.Split(',').Select(Int32.Parse).ToList();
                    var lookups = this._WorkOrderStatusRepository.GetQuery()
                                    .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId) && nextStatusesIds.Contains(s.ID))
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr
                                    }).ToList();

                    return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
                }
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetNextWorkOrderStatus: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }


        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetSewerNextWorkOrderStatus(int currentStatusId)
        {
            try
            {
                var currentStatus = this._WorkOrderStatusRepository.GetQuery().FirstOrDefault(x => x.ID == currentStatusId);

                if (currentStatus != null)
                {
                    List<int> nextStatusesIds = currentStatus.SewerNextStatusIDs.Split(',').Select(Int32.Parse).ToList();
                    var lookups = this._WorkOrderStatusRepository.GetQuery()
                                    .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId) && nextStatusesIds.Contains(s.ID))
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.NameEn : s.NameAr
                                    }).ToList();

                    return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
                }
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetNextWorkOrderStatus: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GeReasonsByStatusId(int statusId)
        {
            var lookups = this._StatusReason.GetQuery()
                                .Where(s => s.StatusID == statusId)
                                .Select(s => new LookUpDTO<int>
                                {
                                    Id = s.ID,
                                    Name = LanguageIsEnglish ? s.ReasonEn : s.ReasonAr,
                                    IntegrationId = s.IntegrationId
                                }).ToList();

            return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetDeassignReason()
        {
            try
            {
                var lookups = this._DeassignReasonRepository.GetQuery()
                                    .Where(s => s.SubID == _LoggedInUserService.LoggedInUser.SubscriberId)
                                    .Select(s => new LookUpDTO<int>
                                    {
                                        Id = s.ID,
                                        Name = LanguageIsEnglish ? s.ReasonEn : s.ReasonAr
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetDeassignReason: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchContractCode(string searchKeyword)
        {
            try
            {
                var text = searchKeyword.Trim();
                var lookups = this._ContractRepository.GetQuery()
                                    .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                                && s.IsDeleted != true
                                                && s.Code.Contains(text))
                                    .Select(s => new LookUpDTO<long>
                                    {
                                        Id = s.ID,
                                        Name = s.Code
                                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchContractCode: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchContractContractors(string searchKeyword)
        {
            try
            {
                var lookupsQuery = this._ContractRepository.GetQuery()
                                    .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                                && s.IsDeleted != true);

                #region search text -OR- return top 50
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookupsQuery = lookupsQuery.Where(s => s.Code.Contains(text) || s.NWC_Contractor.ContractorFullName.Contains(text));
                }
                else
                    lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                #endregion

                var lookups = lookupsQuery
                    .Select(s => new LookUpDTO<long>
                    {
                        Id = s.ID,
                        Name = s.Code + " | " + s.NWC_Contractor.ContractorFullName
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchContractContractors: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchContractorNameCode(string searchKeyword)
        {
            try
            {
                IQueryable<NWC_Contractor> lookupsQuery = this._contractorRepository.GetQuery()
                                .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                            && s.IsDeleted != true && s.IsActive && !s.IsBlackListed);

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookupsQuery = lookupsQuery.Where(s => s.ContractorFullName.Contains(text) || s.Code.Contains(text));
                }
                else
                    lookupsQuery = lookupsQuery.Take(DDlElementsCount);
                #endregion

                var lookups = lookupsQuery.Select(s => new LookUpDTO<long>
                {
                    Id = s.ID,
                    Name = s.ContractorFullName
                }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchContractorNameCode: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchZonesByNameOrCode(string searchKeyword)
        {
            try
            {
                var text = searchKeyword.Trim();

                var ZoneList = this._ZoneRepository.GetQuery()
                    .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                    && _LoggedInUserService.SubBranches.Contains(s.CityID)
                    && s.IsDeleted == false
                    && (s.Name.Contains(text) || s.Code.Contains(text)))
                    .Take(DDlElementsCount)
                    .Select(s => new LookUpDTO<long>
                    {
                        Id = s.ID,
                        Name = s.Name
                    }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(ZoneList);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchZonesByNameOrCode: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetAllCities(PageFilter PageFilter)
        {
            try
            {
                var lookups = this._BranchRepository.GetQuery().
                    Where(c => _LoggedInUserService.SubBranches.Contains(c.Id))
                        .Select(s => new LookUpDTO<Guid>
                        {
                            Id = s.Id,
                            Name = s.name
                        }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetAllCities: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetStationBasedOnCity(string searchKeyword, List<Guid> CityIDs)
        {
            try
            {
                var lookups = this._LandmarkRepository.GetQuery()
                                    .Where(s => (s.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    && _LoggedInUserService.UserLandmarksIds.Any(a => a == s.Id)
                                    && CityIDs.Any(c => c == s.branchId) && s.isDeleted != true);

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookups = lookups.Where(s => s.name.Contains(text)).Take(DDlElementsCount);
                }
                else
                    lookups = lookups.Take(DDlElementsCount);
                #endregion

                var lookupsResult = lookups
                     .Select(s => new LookUpDTO<Guid>
                     {
                         Id = s.Id,
                         Name = s.name
                     }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookupsResult);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetStationBasedOnCity: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetAllStationBasedOnCity(List<Guid> CityIDs)
        {
            try
            {
                var lookups = this._LandmarkRepository.GetQuery()
                                    .Where(s => (s.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    && _LoggedInUserService.UserLandmarksIds.Any(a => a == s.Id)
                                    && CityIDs.Any(c => c == s.branchId) && s.isDeleted != true);

                var lookupsResult = lookups
                     .Select(s => new LookUpDTO<Guid>
                     {
                         Id = s.Id,
                         Name = s.name
                     }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookupsResult);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => GetAllStationBasedOnCity: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetTransporterTypes()
        {
            var lookups = this._TransporterTypeRepository.GetQuery()
                                .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId && s.isDeleted != true))
                                .Select(s => new LookUpDTO<Guid>
                                {
                                    Id = s.ID,
                                    Name = LanguageIsEnglish ? s.name : s.NameAr
                                }).ToList();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchAllZones(string searchKeyword)
        {
            try
            {
                var lookups = this._ZoneRepository.GetQuery()
                                    .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    && s.IsDeleted != true);

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookups = lookups.Where(s => s.Name.Contains(text)).Take(DDlElementsCount);
                }
                else
                    lookups = lookups.Take(DDlElementsCount);
                #endregion

                var lookupsResult = lookups
                     .Select(s => new LookUpDTO<long>
                     {
                         Id = s.ID,
                         Name = s.Name
                     }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookupsResult);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchAllZones: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchPermittedStations(string searchKeyword)
        {
            try
            {
                var lookups = this._userLandmarkPermission.GetQuery()
                                    .Where(s => s.StaffId == this._LoggedInUserService.LoggedInUser.StaffId
                                                && !s.IsDeleted
                                                && s.Landmark.isDeleted != true
                                                && s.Landmark.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId);

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookups = lookups.Where(s => s.Landmark.name.Contains(text) || s.Landmark.code.Contains(text));
                }
                else
                    lookups = lookups.Take(DDlElementsCount);
                #endregion

                var lookupsResult = lookups
                     .Select(s => new LookUpDTO<Guid>
                     {
                         Id = s.Landmark.Id,
                         Name = s.Landmark.name
                     }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookupsResult);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchPermittedStations: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> SearchPermittedSoqyaStations(string searchKeyword)
        {
            try
            {
                var lookups = this._userLandmarkPermission.GetQuery()
                                    .Where(s => s.StaffId == this._LoggedInUserService.LoggedInUser.StaffId
                                                && !s.IsDeleted
                                                && s.Landmark.isDeleted != true
                                                && s.Landmark.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId
                                                && s.Landmark.NWC_StationServiceType.Any(a => a.ServiceTypeID == 2));

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookups = lookups.Where(s => s.Landmark.name.Contains(text) || s.Landmark.code.Contains(text));
                }
                else
                    lookups = lookups.Take(DDlElementsCount);
                #endregion

                var lookupsResult = lookups
                     .Select(s => new LookUpDTO<Guid>
                     {
                         Id = s.Landmark.Id,
                         Name = s.Landmark.name
                     }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookupsResult);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchPermittedSoqyaStations: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchMeterSerial(List<Guid> stationIds, string searchKeyword)
        {
            try
            {
                var lookups = this._DeviceMeterRepository.GetQuery()
                                .Where(s => s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId
                                            && s.Landmark.isDeleted != true
                                            && s.Landmark.SubId == this._LoggedInUserService.LoggedInUser.SubscriberId);

                lookups = (stationIds != null && stationIds.Any())
                            ? lookups.Where(s => stationIds.Any(x => x == s.StationID))
                            : lookups.Where(s => this._LoggedInUserService.UserLandmarksIds.Any(x => x == s.StationID));

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookups = lookups.Where(s => s.MeterSerialNumber.Contains(text)).Take(DDlElementsCount);
                }
                else
                    lookups = lookups.Take(DDlElementsCount);
                #endregion

                var lookupsResult = lookups
                     .Select(s => new LookUpDTO<long>
                     {
                         Id = s.ID,
                         Name = s.MeterSerialNumber
                     }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookupsResult);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchMeterSerial: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<long>>> SearchZonesBasedOnAssignedStations(string searchKeyword)
        {
            try
            {
                var lookups = this._ZoneRepository.GetQuery()
                                    .Where(s => (s.SubID == this._LoggedInUserService.LoggedInUser.SubscriberId)
                                    && s.NWC_ZoneStations.Any(m => this._LoggedInUserService.UserLandmarksIds.Contains(m.StationID))
                                    && s.IsDeleted != true);

                #region search text -OR- return top 10
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var text = searchKeyword.Trim();
                    lookups = lookups.Where(s => s.Name.Contains(text)).Take(DDlElementsCount);
                }
                else
                    lookups = lookups.Take(DDlElementsCount);
                #endregion

                var lookupsResult = lookups
                     .Select(s => new LookUpDTO<long>
                     {
                         Id = s.ID,
                         Name = s.Name
                     }).ToList();

                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Success(lookupsResult);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "LookupService => SearchZonesBasedOnAssignedStations: "));
                return DescriptiveResponse<IEnumerable<LookUpDTO<long>>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }


        //==================================


        #endregion

        #region Helpers
        private bool LanguageIsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
            }
        }

        //private string YearMonthLongToString(long input)
        //{
        //    try
        //    {
        //        long year = input / 100;
        //        int month = (int)(input - (year * 100));

        //        return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month)} {year}";
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerManager.LogMsg(c => c.Log(ex));
        //        return string.Empty;
        //    }
        //}
        #endregion


        #region New Lookup




        #region Not Used

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetStaffRoles()
        {
            var lookups = this._StaffRoles.GetQuery()
                            .Where(s => (s.subID == this._LoggedInUserService.LoggedInUser.SubscriberId && s.isDeleted != true))
                            .Select(s => new LookUpDTO<Guid>
                            {
                                Id = s.ID,
                                Name = LanguageIsEnglish ? s.name : s.NameAr
                            }).ToList();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetBranches(string searchKeyword)
        {
            var lookups = this._BranchRepository.GetQuery()
                             .Where(s => s.SubId == _LoggedInUserService.LoggedInUser.SubscriberId
                             && s.isDeleted != true && !s.IsSubBranch
                             && (string.IsNullOrEmpty(searchKeyword) || s.name.Contains(searchKeyword)))
                             .Select(s => new LookUpDTO<Guid>
                             {
                                 Id = s.Id,
                                 Name = s.name
                             }).ToList();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetSubBranches(string searchKeyword, List<Guid> parentBranchIds)
        {
            if (parentBranchIds == null)
                parentBranchIds = new List<Guid>();
            var lookups = this._BranchRepository.GetQuery()
                             .Where(s => s.SubId == _LoggedInUserService.LoggedInUser.SubscriberId
                             && s.isDeleted != true && s.IsSubBranch
                             && (string.IsNullOrEmpty(searchKeyword) || s.name.Contains(searchKeyword))
                             && (!parentBranchIds.Any() || parentBranchIds.Any(x => x == s.parentBranchId)))
                             .Select(s => new LookUpDTO<Guid>
                             {
                                 Id = s.Id,
                                 Name = s.name
                             }).ToList();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetLandmarks(string searchKeyword, List<Guid> branchIds)
        {
            if (branchIds == null)
                branchIds = new List<Guid>();
            var lookups = this._Landmark.GetQuery()
                           .Where(s => (s.isDeleted != true)
                            && (string.IsNullOrEmpty(searchKeyword) || s.name.Contains(searchKeyword))
                            && (!branchIds.Any() || branchIds.Any(x => x == s.branchId)))
                           .Select(s => new LookUpDTO<Guid>
                           {
                               Id = s.Id,
                               Name = s.name
                           }).ToList();

            return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Success(lookups);
        }
        #endregion
        #endregion
        class RestrictedBranches
        {
            public Guid BranchID { get; set; }
        }

    }
}
