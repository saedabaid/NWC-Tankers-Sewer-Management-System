using Infrastructure;
using LinqKit;
using NWC.BL.Denormalizer.CoreBusiness;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Extentions;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace NWC.BLL.Services
{
    public class VehicleService : IVehicleService
    {
        #region Properties
        private readonly ILoggedInUserService _loggedInUser;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<Transporter> _vehicleRepository;
        private readonly IRepository<NWC_VehicleAccessory> _NWC_VehicleAccessoryRep;
        private readonly IRepository<NWC_VehicleCustomerLocationClass> _NWC_VehicleCustClassRep;
        private readonly IRepository<vw_NWC_VehicleNWCSettings> _vw_NWC_VehicleNWCSettings;
        private readonly IRepository<Branch> _BranchRepository;
        private readonly IRepository<NWC_ZoneStations> _zoneStationsRepository;
        private readonly IRepository<vw_NWC_AvailableTankerSizes> _availableTankerSizesRep;
        private readonly IRepository<vw_NWC_Report_VehicleLog> _Report_VehicleLog;
        private readonly IRepository<vw_NWC_Report_VehicleData> _Report_VehicleData;
        private readonly IRepository<sp_NWC_Report_VehiclePerformance_Result> _sp_NWC_Report_VehiclePerformance;
        private readonly IRepository<Permit> _permitRepo;
        private readonly IRepository<vw_permitList> _vwPermitListRepo;
        private readonly IRepository<PermitDetectionformAttachment> _PermitDetectionformAttachmentRepo;
        private readonly IRepository<PermitDeclarationAttachment> _PermitDeclarationAttachmentRepo;
        private readonly IRepository<PermitOtherFIleAttachment> _PermitOtherFIleAttachmentRepo;
        #endregion

        #region Constructors
        public VehicleService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._vehicleRepository = new Repository<Transporter>(ctx);
            this._NWC_VehicleAccessoryRep = new Repository<NWC_VehicleAccessory>(ctx);
            this._NWC_VehicleCustClassRep = new Repository<NWC_VehicleCustomerLocationClass>(ctx);
            this._vw_NWC_VehicleNWCSettings = new Repository<vw_NWC_VehicleNWCSettings>(ctx);
            this._zoneStationsRepository = new Repository<NWC_ZoneStations>(ctx);
            this._availableTankerSizesRep = new Repository<vw_NWC_AvailableTankerSizes>(ctx);
            this._Report_VehicleLog = new Repository<vw_NWC_Report_VehicleLog>(ctx);
            this._Report_VehicleData = new Repository<vw_NWC_Report_VehicleData>(ctx);
            this._BranchRepository = new Repository<Branch>(ctx);
            this._sp_NWC_Report_VehiclePerformance = new Repository<sp_NWC_Report_VehiclePerformance_Result>(ctx);
            this._permitRepo = new Repository<Permit>(ctx);
            this._PermitDetectionformAttachmentRepo = new Repository<PermitDetectionformAttachment>(ctx);
            this._PermitDeclarationAttachmentRepo = new Repository<PermitDeclarationAttachment>(ctx);
            this._PermitOtherFIleAttachmentRepo = new Repository<PermitOtherFIleAttachment>(ctx);
            this._vwPermitListRepo = new Repository<vw_permitList>(ctx);
        }
        #endregion

        #region Command
        public DescriptiveResponse<Boolean> SaveVehicleNWCSettings(List<VehicleNWCSettingsDTO> vehicleNWCSettingsDTOs)
        {
            try
            {
                using (_unitofWork)
                {
                    foreach (var dto in vehicleNWCSettingsDTOs)
                    {
                        // Add or Delete vehicle accessory
                        var dbVehicleAccList = this._NWC_VehicleAccessoryRep.GetQuery()
                            .Where(s => s.VehicleID == dto.VehicleID);

                        foreach (var vAccessory in dbVehicleAccList)
                        {
                            if (!dto.AccessoryIDs.Contains(vAccessory.AccessoryID))
                            {
                                this._NWC_VehicleAccessoryRep.Delete(vAccessory);
                            }
                        }

                        foreach (var accID in dto.AccessoryIDs)
                        {
                            var vehicleAccessory = new NWC_VehicleAccessory()
                            {
                                VehicleID = dto.VehicleID,
                                AccessoryID = accID
                            };

                            if (!dbVehicleAccList.Select(x => x.AccessoryID).Contains(accID))
                            {
                                this._NWC_VehicleAccessoryRep.Add(vehicleAccessory);
                            }
                        }

                        // Add or Delete vehicle customer location class
                        var dbVehicleClassList = this._NWC_VehicleCustClassRep.GetQuery()
                            .Where(s => s.VehicleID == dto.VehicleID);

                        foreach (var vClass in dbVehicleClassList)
                        {
                            if (!dto.CustLocationClassIDs.Contains(vClass.CustomerLocationClassID))
                            {
                                this._NWC_VehicleCustClassRep.Delete(vClass);
                            }
                        }

                        foreach (var classID in dto.CustLocationClassIDs)
                        {
                            var vehicleCustClass = new NWC_VehicleCustomerLocationClass()
                            {
                                VehicleID = dto.VehicleID,
                                CustomerLocationClassID = classID
                            };

                            if (!dbVehicleClassList.Select(x => x.CustomerLocationClassID).Contains(classID))
                            {
                                this._NWC_VehicleCustClassRep.Add(vehicleCustClass);
                            }
                        }

                        var transporter = this._vehicleRepository.GetQuery()
                            .FirstOrDefault(s => s.ID == dto.VehicleID);

                        transporter.ServiceTypeID = dto.ServiceTypeID;

                        this._vehicleRepository.Update(transporter);
                    }
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "VehicleService => SaveVehicleNWCSettings: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<Boolean> SaveVehicleNWCSettingsBulk(VehicleNWCSettingsBulkUpdateDTO updateModel)
        {
            try
            {
                List<Guid> updateVehicleList;
                #region get update Vehicle List
                if (updateModel.ApplyOption == ApplyOption.SelectedOnly)
                {
                    updateVehicleList = updateModel.ApplyVehicleIds;
                }
                else
                {
                    var searchCriteria = updateModel.VehicleSettingsSCModel;
                    // this prediacte same as the one in GetVehicleNWCSettings
                    #region Predicate
                    var predicate = PredicateBuilder.New<Transporter>(true);

                    predicate = predicate.And(s => s.isDeleted != true);
                    predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

                    if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
                    {
                        var myStations = this._loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                        predicate = predicate.And(s => myStations.Any(a => a == s.landmark));
                    }
                    else
                    {
                        predicate = predicate.And(s => this._loggedInUser.UserLandmarksIds.Any(a => a == s.landmark));
                    }

                    if (searchCriteria.VehicleIDs != null && searchCriteria.VehicleIDs.Any())
                    {
                        predicate = predicate.And(s => searchCriteria.VehicleIDs.Contains(s.ID));
                    }

                    if (searchCriteria.VehicleTypeIDs != null && searchCriteria.VehicleTypeIDs.Any())
                    {
                        predicate = predicate.And(s => searchCriteria.VehicleTypeIDs.Any(a => a == s.transporterType));
                    }
                    #endregion

                    var vehicleQuerable = this._vehicleRepository.GetQuery()
                        .Where(predicate)
                        .OrderBy(s => s.ID)
                        .Select(s => s.ID);

                    if (updateModel.ApplyOption == ApplyOption.CurrentPage)
                    {
                        #region skip & take
                        var skip = 0;
                        var take = 20;
                        if (searchCriteria != null && searchCriteria.PageFilter != null)
                        {
                            skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                            take = searchCriteria.PageFilter.PageSize;
                        }
                        #endregion
                        vehicleQuerable = vehicleQuerable
                                            .Skip(skip)
                                            .Take(take);
                    }

                    updateVehicleList = vehicleQuerable.ToList();
                }
                #endregion

                if (updateVehicleList == null || !updateVehicleList.Any())
                {
                    return DescriptiveResponse<Boolean>.Success(false);
                }

                using (_unitofWork)
                {
                    #region ServiceType & Capacity & Class
                    if (updateModel.ServiceTypeID.GetValueOrDefault() > 0
                        || updateModel.Capacity.GetValueOrDefault() > 0
                        || updateModel.StationID != null)
                    {
                        foreach (var item in updateVehicleList)
                        {
                            var myVehicle = this._vehicleRepository.FindById(item);
                            myVehicle.Capacity = updateModel.Capacity.HasValue ? updateModel.Capacity.Value : myVehicle.Capacity;
                            myVehicle.landmark = updateModel.StationID.HasValue ? updateModel.StationID.Value : myVehicle.landmark;
                            myVehicle.ServiceTypeID = updateModel.ServiceTypeID.HasValue ? updateModel.ServiceTypeID.Value : myVehicle.ServiceTypeID;

                            this._vehicleRepository.Update(myVehicle);
                        }
                    }
                    #endregion

                    #region Accessories
                    if (updateModel.AccessoryIDs != null && updateModel.AccessoryIDs.Any())
                    {
                        var dbVehicleAccList = this._NWC_VehicleAccessoryRep.GetQuery()
                                    .Where(s => updateVehicleList.Contains(s.VehicleID));

                        foreach (var vAccessory in dbVehicleAccList)
                        {
                            if (!updateModel.AccessoryIDs.Contains(vAccessory.AccessoryID))
                            {
                                this._NWC_VehicleAccessoryRep.Delete(vAccessory);
                            }
                        }

                        foreach (var accID in updateModel.AccessoryIDs)
                        {
                            foreach (var vehicleId in updateVehicleList)
                            {
                                if (!dbVehicleAccList.Any(x => x.AccessoryID == accID && x.VehicleID == vehicleId))
                                {
                                    var vehicleAccessory = new NWC_VehicleAccessory()
                                    {
                                        VehicleID = vehicleId,
                                        AccessoryID = accID
                                    };

                                    this._NWC_VehicleAccessoryRep.Add(vehicleAccessory);
                                }
                            }
                        }
                    }
                    #endregion

                    #region Customer Classes
                    if (updateModel.CustLocationClassIDs != null && updateModel.CustLocationClassIDs.Any())
                    {
                        var dbVehicleClassList = this._NWC_VehicleCustClassRep.GetQuery()
                                                .Where(s => updateVehicleList.Contains(s.VehicleID));

                        foreach (var vClass in dbVehicleClassList)
                        {
                            if (!updateModel.CustLocationClassIDs.Contains(vClass.CustomerLocationClassID))
                            {
                                this._NWC_VehicleCustClassRep.Delete(vClass);
                            }
                        }

                        foreach (var classID in updateModel.CustLocationClassIDs)
                        {
                            foreach (var vehicleId in updateVehicleList)
                            {
                                if (!dbVehicleClassList.Any(x => x.CustomerLocationClassID == classID && x.VehicleID == vehicleId))
                                {
                                    var vehicleCustClass = new NWC_VehicleCustomerLocationClass()
                                    {
                                        VehicleID = vehicleId,
                                        CustomerLocationClassID = classID
                                    };
                                    this._NWC_VehicleCustClassRep.Add(vehicleCustClass);
                                }
                            }
                        }
                    }
                    #endregion
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "VehicleService => SaveVehicleNWCSettingsBulk: "));
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<string> AddPermit(PermitDTO permitDto)
        {
            try
            {
                var isEdit = false;
                var permit = new Permit();

                if (permitDto.ID != Guid.Empty)
                {
                    isEdit = true;
                    permit = _permitRepo.GetQuery().FirstOrDefault(x => x.ID == permitDto.ID);
                    if (_permitRepo.GetQuery().FirstOrDefault(x => x.PermitNumber != permit.PermitNumber && x.PermitNumber == permitDto.PermitNumber) != null)
                    {
                        return DescriptiveResponse<string>.Error(ErrorStatus.Existsbefore);
                    }
                    permit.LastModifiedBy = this._loggedInUser.LoggedInUser.StaffId;
                    permit.LastModificationDate = DateTimeHelper.GetDateTimeNow();
                }
                else
                {
                    if (_permitRepo.GetQuery().FirstOrDefault(x => x.PermitNumber == permitDto.PermitNumber) != null)
                    {
                        return DescriptiveResponse<string>.Error(ErrorStatus.Existsbefore);
                    }
                    permit.ID = Guid.NewGuid();
                    permit.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
                    permit.CreationTime = DateTimeHelper.GetDateTimeNow();
                }

                permit.Availabletimefrom = permitDto.Availabletimefrom;
                permit.Availabletimeto = permitDto.Availabletimeto;
                permit.CRnumber = permitDto.CRnumber;
                permit.IsHold = permitDto.IsHold;
                permit.Discerption = permitDto.Discerption;
                permit.DriverID = permitDto.DriverID;
                permit.Expirationdate = permitDto.Expirationdate;
                permit.Ismaramy = permitDto.Ismaramy;
                permit.LastMaintenanceDate = permitDto.LastMaintenanceDate;
                permit.LastValidationDate = permitDto.LastValidationDate;
                permit.Maramu = permitDto.Maramu;
                permit.OrganizationName = permitDto.OrganizationName;
                permit.PermitNumber = permitDto.PermitNumber;
                permit.StartDate = permitDto.StartDate;
                permit.TankerCategory = permitDto.TankerCategory;
                permit.TransporterID = permitDto.TransporterID;
                permit.TripsNumber = permitDto.TripsNumber;
                //permit.PermitStatus = (permit.IsHold == true) ? (int)PermitStatusEnum.Hold : ((permit.Expirationdate > DateTime.Now) ? (int)PermitStatusEnum.Active : (int)PermitStatusEnum.Expired);
                using (_unitofWork)
                {
                    if (isEdit)
                    {
                        this._permitRepo.Update(permit);
                    }
                    else
                    {
                        this._permitRepo.Add(permit);
                    }

                }

                #region Attachments
                if (permitDto.DetectionformFileAttachments != null && permitDto.DetectionformFileAttachments.Count() > 0)
                {
                    var attachments = new List<PermitDetectionformAttachment>();

                    foreach (var item in permitDto.DetectionformFileAttachments)
                    {
                        if ((item.ID == null || item.ID == 0) && !item.IsDeleted)
                        {
                            var newPath = Utilities.MoveFile(AttachmentType.PermitDetectionform, item.RelativePath, permit.ID.ToString());

                            #region new Attachment
                            var newAttach = new PermitDetectionformAttachment
                            {
                                PermitID = permit.ID,
                                RelativePath = newPath,
                                DocumentName = item.DocumentName,
                                IsDeleted = false,
                                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                                CreatedDate = DateTimeHelper.GetDateTimeNow()
                            };

                            #endregion

                            attachments.Add(newAttach);
                        }
                        else if (item.IsDeleted)
                        {
                            using (_unitofWork)
                            {
                                #region delete attachment
                                var oldAttach = this._PermitDetectionformAttachmentRepo.GetQuery().FirstOrDefault(a => a.ID == item.ID && !a.IsDeleted);
                                if (oldAttach != null)
                                {
                                    oldAttach.IsDeleted = true;
                                    oldAttach.DeletedBy = this._loggedInUser.LoggedInUser.StaffId;
                                    oldAttach.DeletedDate = DateTimeHelper.GetDateTimeNow();

                                    this._PermitDetectionformAttachmentRepo.Update(oldAttach);
                                }
                                #endregion
                            }
                        }
                    }

                    using (_unitofWork)
                    {
                        this._PermitDetectionformAttachmentRepo.AddRange(attachments);
                    }
                }

                if (permitDto.DeclarationFileAttachments != null && permitDto.DeclarationFileAttachments.Count() > 0)
                {
                    var attachments = new List<PermitDeclarationAttachment>();

                    foreach (var item in permitDto.DeclarationFileAttachments)
                    {
                        if ((item.ID == null || item.ID == 0) && !item.IsDeleted)
                        {
                            var newPath = Utilities.MoveFile(AttachmentType.PermitDeclaration, item.RelativePath, permit.ID.ToString());

                            #region new Attachment
                            var newAttach = new PermitDeclarationAttachment
                            {
                                PermitID = permit.ID,
                                RelativePath = newPath,
                                DocumentName = item.DocumentName,
                                IsDeleted = false,
                                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                                CreatedDate = DateTimeHelper.GetDateTimeNow()
                            };

                            #endregion

                            attachments.Add(newAttach);
                        }
                        else if (item.IsDeleted)
                        {
                            using (_unitofWork)
                            {
                                #region delete attachment
                                var oldAttach = this._PermitDeclarationAttachmentRepo.GetQuery().FirstOrDefault(a => a.ID == item.ID && !a.IsDeleted);
                                if (oldAttach != null)
                                {
                                    oldAttach.IsDeleted = true;
                                    oldAttach.DeletedBy = this._loggedInUser.LoggedInUser.StaffId;
                                    oldAttach.DeletedDate = DateTimeHelper.GetDateTimeNow();

                                    this._PermitDeclarationAttachmentRepo.Update(oldAttach);
                                }
                                #endregion
                            }
                        }
                    }

                    using (_unitofWork)
                    {
                        this._PermitDeclarationAttachmentRepo.AddRange(attachments);
                    }
                }
                if (permitDto.OtherFileAttachments != null && permitDto.OtherFileAttachments.Count() > 0)
                {
                    var attachments = new List<PermitOtherFIleAttachment>();

                    foreach (var item in permitDto.OtherFileAttachments)
                    {
                        if ((item.ID == null || item.ID == 0) && !item.IsDeleted)
                        {
                            var newPath = Utilities.MoveFile(AttachmentType.PermitOtherFIle, item.RelativePath, permit.ID.ToString());

                            #region new Attachment
                            var newAttach = new PermitOtherFIleAttachment
                            {
                                PermitID = permit.ID,
                                RelativePath = newPath,
                                DocumentName = item.DocumentName,
                                IsDeleted = false,
                                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                                CreatedDate = DateTimeHelper.GetDateTimeNow()
                            };

                            #endregion

                            attachments.Add(newAttach);
                        }
                        else if (item.IsDeleted)
                        {
                            using (_unitofWork)
                            {
                                #region delete attachment
                                var oldAttach = this._PermitOtherFIleAttachmentRepo.GetQuery().FirstOrDefault(a => a.ID == item.ID && !a.IsDeleted);
                                if (oldAttach != null)
                                {
                                    oldAttach.IsDeleted = true;
                                    oldAttach.DeletedBy = this._loggedInUser.LoggedInUser.StaffId;
                                    oldAttach.DeletedDate = DateTimeHelper.GetDateTimeNow();

                                    this._PermitOtherFIleAttachmentRepo.Update(oldAttach);
                                }
                                #endregion
                            }
                        }
                    }

                    using (_unitofWork)
                    {
                        this._PermitOtherFIleAttachmentRepo.AddRange(attachments);
                    }
                }
                #endregion


                return DescriptiveResponse<string>.Success(permit.ID.ToString());
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "VehicleService => AddPermit: "));
                return DescriptiveResponse<string>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        public DescriptiveResponse<string> UpdateTanker(VehicleDTO transporterDTO)
        {
            try
            {
                var entity = _vehicleRepository.GetQuery()
            .FirstOrDefault(a => a.ID == transporterDTO.Id
                                 && a.isDeleted != true
                                 && a.SubID == _loggedInUser.LoggedInUser.SubscriberId);


                entity.plateNo = transporterDTO.PlateNo;
                entity.licenseExpiryDate = transporterDTO.LicenseExpiryDate;
                entity.code = transporterDTO.Code;
                entity.insuranceStartDate = transporterDTO.InsuranceStartDate;
                entity.OwnerIDValue = transporterDTO.OwnerIDValue;
                entity.OwnerFullNameAR = transporterDTO.OwnerFullNameAR;
                entity.LastModificationDate = DateTime.Now;
                using (_unitofWork)
                {
                    this._vehicleRepository.Update(entity);
                }
                return DescriptiveResponse<string>.Success(entity.ID.ToString());
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "VehicleService => AddPermit: "));
                return DescriptiveResponse<string>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }



        #endregion

        #region Query
        public DescriptiveResponse<SearchResult<VehicleNWCSettingsDTO>> GetVehicleNWCSettings(VehicleSettingsSC searchCriteria)
        {
            try
            {
                // this prediacte same as the one in SaveVehicleNWCSettingsBulk
                #region Predicate
                var predicate = PredicateBuilder.New<vw_NWC_VehicleNWCSettings>(true);

                predicate = predicate.And(s => s.IsDeleted != true);
                predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

                if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
                {
                    var myStations = this._loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                    predicate = predicate.And(s => myStations.Any(a => a == s.StationID));
                }
                else
                {
                    predicate = predicate.And(s => this._loggedInUser.UserLandmarksIds.Any(a => a == s.StationID));
                }

                if (searchCriteria.VehicleIDs != null && searchCriteria.VehicleIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.VehicleIDs.Contains(s.Id));
                }

                if (searchCriteria.VehicleTypeIDs != null && searchCriteria.VehicleTypeIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.VehicleTypeIDs.Any(a => a == s.transporterType));
                }
                #endregion

                #region skip & take
                var skip = 0;
                var take = 20;
                if (searchCriteria != null && searchCriteria.PageFilter != null)
                {
                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                    take = searchCriteria.PageFilter.PageSize;
                }
                #endregion

                var vehicleNWCSettings = this._vw_NWC_VehicleNWCSettings.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.Id)
                    .Skip(skip)
                    .Take(take);

                var result = new SearchResult<VehicleNWCSettingsDTO>();

                if (vehicleNWCSettings != null && vehicleNWCSettings.Any())
                {
                    var count = this._vw_NWC_VehicleNWCSettings.GetQuery().Count(predicate);
                    result.Result = vehicleNWCSettings.AsEnumerable().Select(a => a.WrapToVehicleNWCSettingsDTO()).ToList();
                    result.TotalCount = count;
                }

                return DescriptiveResponse<SearchResult<VehicleNWCSettingsDTO>>.Success(result);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "VehicleService => GetVehicleNWCSettings: "));
                return DescriptiveResponse<SearchResult<VehicleNWCSettingsDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<SearchResult<AvailableTankerSizesDTO>> GetAvailableTankerSizesByZoneIntID(long zoneID, long defaultZoneID, int classID, int serviceTypeID)
        {
            try
            {
                LoggerManager.LogMsg(c => c.TrackingMsg($"-----------------------------GetAvailableTankerSizesByZoneIntID - Started -----------------------------"));
                LoggerManager.LogMsg(c => c.TrackingMsg($"zoneID: {zoneID} - defaultZoneID: {defaultZoneID} - classID: {classID} - serviceTypeID: {serviceTypeID}"));

                //Get main zone station
                var zoneStaition = this._zoneStationsRepository.GetQuery()
                                    .Where(s => s.Landmark.SubId == this._loggedInUser.LoggedInUser.SubscriberId &&
                                    s.ZoneID == zoneID &&
                                    s.IsMain &&
                                    s.Landmark.StatusID == (int)StationStatusEnum.InService).FirstOrDefault();

                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Get main zone StationID {0}", zoneStaition != null ? zoneStaition.StationID.ToString() : string.Empty)));

                //Get backup station
                if (zoneStaition == null)
                {
                    zoneStaition = this._zoneStationsRepository.GetQuery()
                                    .Where(s => s.Landmark.SubId == this._loggedInUser.LoggedInUser.SubscriberId &&
                                    s.ZoneID == zoneID &&
                                    s.IsMain != true &&
                                    s.Landmark.StatusID == (int)StationStatusEnum.InService).FirstOrDefault();

                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Get backup zone StationID {0}", zoneStaition != null ? zoneStaition.StationID.ToString() : string.Empty)));
                }

                #region Predicate
                var predicate = PredicateBuilder.New<vw_NWC_AvailableTankerSizes>(true);

                predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);
                predicate = predicate.And(s => s.Landmark.Value == zoneStaition.StationID);
                //predicate = predicate.And(s => s.CustomerLocationClassID == classID);
                predicate = predicate.And(s => s.ServiceTypeID == serviceTypeID);
                #endregion

                var vehicles = this._availableTankerSizesRep.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.ID)
                    .Distinct();

                var distinctVehicles = ExtentionsHelper.Distinct(vehicles, x => x.Capacity);

                var result = new SearchResult<AvailableTankerSizesDTO>();

                if (distinctVehicles != null && distinctVehicles.Any())
                {
                    var count = this._availableTankerSizesRep.GetQuery().Count(predicate);
                    result.Result = distinctVehicles.AsEnumerable().Select(a => a.WrapToAvailableTankerSizesDTO()).ToList();
                    result.TotalCount = count;

                    LoggerManager.LogMsg(c => c.TrackingMsg($"There are {count} available tanker sizes"));
                }
                else
                {
                    result.Result = new List<AvailableTankerSizesDTO>();
                    result.TotalCount = 0;

                    LoggerManager.LogMsg(c => c.TrackingMsg($"There are 0 available tanker sizes"));
                }

                foreach (var tankerSize in result.Result)
                {
                    if (!tankerSize.ShowPrice)
                        tankerSize.TankerPrice = 0;
                    else
                    {
                        var dt = DateTimeHelper.GetDateTimeNow();
                        var cost = WorkOrderCost.CalculateWorkOrderCost(0, dt, classID, serviceTypeID, zoneStaition, tankerSize.TankerSize);
                        tankerSize.TankerPrice = decimal.Round(cost, 2);

                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("scheduledDeliveryTime: {0}, classID: {1}, serviceTypeID: {2}, stationID: {3}, ZoneID: {4}, TankerSize: {5}, cost: {6}",
                                                                            dt, classID, serviceTypeID, zoneStaition.StationID, zoneStaition.ZoneID, tankerSize.TankerSize, tankerSize.TankerPrice)));
                    }
                }

                return DescriptiveResponse<SearchResult<AvailableTankerSizesDTO>>.Success(result);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "VehicleService => GetAvailableTankerSizesByZoneIntID: "));
                return DescriptiveResponse<SearchResult<AvailableTankerSizesDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }


        public DescriptiveResponse<List<int>> GetDefaultTankerSizesByCIS(string CIS)
        {
            try
            {
                LoggerManager.LogMsg(c => c.TrackingMsg($"-----------------------------GetDefefaultTankerSizesByCIS - Started -----------------------------"));
                LoggerManager.LogMsg(c => c.TrackingMsg($"CIS: {CIS}"));

                //Get main zone station
                var Branch = this._BranchRepository.GetQuery()
                                    .Where(s => s.code.ToLower() == CIS.ToLower() &&
                                    s.isDeleted != true &&
                                    s.IsSubBranch == false).FirstOrDefault();

                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Get branch {0}", Branch != null ? Branch.name : string.Empty)));
                List<int> ListSizes = new List<int>();
                if (Branch != null)
                {
                    var SizesSt = Branch.TankerSizes;
                    if (!string.IsNullOrEmpty(SizesSt))
                    {
                        foreach (var size in SizesSt.Split(','))
                        {
                            var a = int.Parse(size);
                            ListSizes.Add(a);
                        }
                    }
                }
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Get {0} sizes for this area", ListSizes != null ? ListSizes.ToList().Count.ToString() : string.Empty)));
                return DescriptiveResponse<List<int>>.Success(ListSizes);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "Default list is not exist here"));
                return DescriptiveResponse<List<int>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }

        }

        public DescriptiveResponse<SearchResult<VehicleLogsDTO>> GetVehicleLogReport(VehicleLogReportSC searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_Report_VehicleLog>(true);

            predicate = predicate.And(s => s.VehicleSubID == this._loggedInUser.LoggedInUser.SubscriberId);

            #region Lists Predicate

            if (!string.IsNullOrEmpty(searchCriteria.OrderNumber))
            {
                var searchOrder = searchCriteria.OrderNumber.Trim();
                predicate = predicate.And(s => s.OrderNumber.Contains(searchOrder));
            }

            if (!string.IsNullOrEmpty(searchCriteria.Vehicle))
            {
                var searchText = searchCriteria.Vehicle.Trim();
                predicate = predicate.And(s => s.VehiclePlateNo.Contains(searchText) || s.VehicleCode.Contains(searchText));
            }

            if (!string.IsNullOrEmpty(searchCriteria.Driver))
            {
                var searchDriver = searchCriteria.Driver.Trim();
                predicate = predicate.And(s => s.DriverName.Contains(searchDriver) || s.DriverMobile.Contains(searchDriver));
            }


            //permitted serviceTypes
            if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
            {
                var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);
                predicate = predicate.And(s => services.Any(a => a == s.VehicleServiceTypeID));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserServicesIds.Any(a => a == s.VehicleServiceTypeID));
            }

            //permitted stations
            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
            {
                var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                predicate = predicate.And(s => searchList.Any(a => a == s.VehicleStationId));
            }
            else
            {
                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Any(a => a == s.VehicleStationId));
            }

            if (searchCriteria.LogType != null)
            {
                predicate = predicate.And(s => s.ActionLogTypeID == searchCriteria.LogType);
            }

            #endregion

            #region Date Time Predicate

            if (searchCriteria.DateTimeFrom != null)
            {
                predicate = predicate.And(s => s.CreateTime >= searchCriteria.DateTimeFrom);
            }

            if (searchCriteria.DateTimeTo != null)
            {
                predicate = predicate.And(s => s.CreateTime < searchCriteria.DateTimeTo);
            }
            #endregion

            #endregion

            IQueryable<vw_NWC_Report_VehicleLog> workOrderList =
                this._Report_VehicleLog.GetQuery()
                    .Where(predicate)
                    .OrderByDescending(s => s.CreateTime);

            if (!searchCriteria.ExcelFlage)
            {
                #region skip & take
                var skip = 0;
                var take = 10;
                if (searchCriteria.PageFilter != null)
                {
                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                    take = searchCriteria.PageFilter.PageSize;
                }
                #endregion

                workOrderList = workOrderList
                    .Skip(skip)
                    .Take(take);
            }

            #region response
            var result = new SearchResult<VehicleLogsDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = this._Report_VehicleLog.GetQuery().Count(predicate);
                result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToVehicleLogsDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<VehicleLogsDTO>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<SearchResult<PermitDTO>> GetPermitList(PermitListSC searchCriteria)
        {
            try
            {
                #region Predicate
                var predicate = PredicateBuilder.New<vw_permitList>();

                if (searchCriteria.AreaIDs != null && searchCriteria.AreaIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.AreaIDs.Any(a => a == s.AreaId));
                }

                if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.CityIDs.Any(a => a == s.CityId));
                }


                if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
                {

                    predicate = predicate.And(s => searchCriteria.StationIDs.Any(a => a == s.stationId));
                }
                if (searchCriteria.PermitStatus != null && searchCriteria.PermitStatus != -1)
                {
                    if (searchCriteria.PermitStatus == (int)PermitStatusEnum.Hold)
                        predicate = predicate.And(s => s.IsHold.Value == true);
                    else
                    {
                        if (searchCriteria.PermitStatus == (int)PermitStatusEnum.Active)
                            predicate = predicate.And(s => s.Expirationdate.Value >= DateTime.Now && s.IsHold != true);
                        else
                            predicate = predicate.And(s => s.Expirationdate.Value < DateTime.Now&&s.IsHold!= true);
                    }
                }


                if (!string.IsNullOrEmpty(searchCriteria.DriverID))
                {
                    var searchText = searchCriteria.DriverID.Trim();
                    predicate = predicate.And(v => v.DriverID.Contains(searchText));
                }

                if (!string.IsNullOrEmpty(searchCriteria.DriverMobile))
                {
                    var searchText = searchCriteria.DriverMobile.Trim();
                    predicate = predicate.And(v => v.DriverMobile.Contains(searchText));
                }

                if (!string.IsNullOrEmpty(searchCriteria.TankerCode))
                {
                    var searchText = searchCriteria.TankerCode.Trim();
                    predicate = predicate.And(v => v.code.Contains(searchText));
                }

                if (searchCriteria.ExpirationdateFrom.HasValue)
                {
                    predicate = predicate.And(s => s.Expirationdate >= searchCriteria.ExpirationdateFrom.Value);
                }

                if (searchCriteria.ExpirationdateTo.HasValue)
                {
                    predicate = predicate.And(s => s.Expirationdate < searchCriteria.ExpirationdateTo.Value);
                }
                #endregion

                IQueryable<vw_permitList> permitList = _vwPermitListRepo.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.Expirationdate);


                #region response
                var result = new SearchResult<PermitDTO>();
                result.TotalCount = _vwPermitListRepo.GetQuery().Count(predicate);
                result.Result = permitList.AsEnumerable().Select(x =>
                {
                    var dto = new PermitDTO();
                    dto.ID = x.ID;
                    dto.Area = x.Area;
                    dto.Availabletimefrom = x.Availabletimefrom;
                    dto.Availabletimeto = x.Availabletimeto;
                    dto.City = x.City;
                    dto.Station = x.Station;
                    dto.DriverIDSTR = x.DriverID;
                    dto.DriverMobile = x.DriverMobile;
                    dto.Expirationdate = x.Expirationdate;
                    dto.IsHold = x.IsHold;
                    dto.TankerNumber = x.TankerNumber;
                    dto.PermitStatus = (x.IsHold == true) ? PermitStatusEnum.Hold.ToString() : ((x.Expirationdate >= DateTime.Now) ? PermitStatusEnum.Active.ToString() : PermitStatusEnum.Expired.ToString());
                    return dto;
                }).ToList();

                return DescriptiveResponse<SearchResult<PermitDTO>>.Success(result);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "VehicleService => GetPermitList: "));
                return DescriptiveResponse<SearchResult<PermitDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        public DescriptiveResponse<PermitDTO> GetPermit(Guid id)
        {
            try
            {

                Permit permit = _permitRepo.GetQuery()
                    .Where(x => x.ID == id).FirstOrDefault();


                #region response



                var result = new PermitDTO();
                result.ID = permit.ID;

                result.Availabletimefrom = permit.Availabletimefrom;
                result.Availabletimeto = permit.Availabletimeto;
                result.DriverID = permit.DriverID;
                result.CRnumber = permit.CRnumber;
                result.Expirationdate = permit.Expirationdate;
                result.Discerption = permit.Discerption;
                result.Ismaramy = permit.Ismaramy;
                result.LastMaintenanceDate = permit.LastMaintenanceDate;
                result.LastValidationDate = permit.LastValidationDate;
                result.Maramu = permit.Maramu;
                result.OrganizationName = permit.OrganizationName;
                result.PermitNumber = permit.PermitNumber;
                result.StartDate = permit.StartDate;
                result.TankerCategory = permit.TankerCategory;
                result.TripsNumber = permit.TripsNumber;
                result.IsHold = permit.IsHold;
                result.StaffDTO = permit.Staff.WarapToStaffDTO();
                result.TransporterDTO = permit.Transporter.WrapToTransporterDTO();
                result.DeclarationFileAttachments = _PermitDeclarationAttachmentRepo.GetQuery().Where(x => x.PermitID == permit.ID && x.IsDeleted != true)
                    .AsEnumerable().Select(x =>
                    {
                        var dto = new AttachmentDTO();
                        dto.ID = x.ID;
                        dto.DocumentName = x.DocumentName;
                        dto.RelativePath = x.RelativePath;
                        dto.IsDeleted = x.IsDeleted;
                        return dto;
                    }).ToList();
                result.DetectionformFileAttachments = _PermitDetectionformAttachmentRepo.GetQuery().Where(x => x.PermitID == permit.ID && x.IsDeleted != true)
                    .AsEnumerable().Select(x =>
                    {
                        var dto = new AttachmentDTO();
                        dto.ID = x.ID;
                        dto.DocumentName = x.DocumentName;
                        dto.RelativePath = x.RelativePath;
                        dto.IsDeleted = x.IsDeleted;
                        return dto;
                    }).ToList();
                result.OtherFileAttachments = _PermitOtherFIleAttachmentRepo.GetQuery().Where(x => x.PermitID == permit.ID && x.IsDeleted != true)
                    .AsEnumerable().Select(x =>
                    {
                        var dto = new AttachmentDTO();
                        dto.ID = x.ID;
                        dto.DocumentName = x.DocumentName;
                        dto.RelativePath = x.RelativePath;
                        dto.IsDeleted = x.IsDeleted;
                        return dto;
                    }).ToList();
                return DescriptiveResponse<PermitDTO>.Success(result);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "VehicleService => GetPermit: "));
                return DescriptiveResponse<PermitDTO>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<SearchResult<VehiclePerformanceDTO>> GetVehiclePerformanceReport(VehiclePerformanceReportSC searchCriteria)
        {
            int PageIndex = searchCriteria.ExcelFlage ? 0 : searchCriteria.PageFilter?.PageIndex - 1 ?? 0,
                PageSize = searchCriteria.ExcelFlage ? 999999999 : searchCriteria.PageFilter?.PageSize ?? 10;

            var list = _sp_NWC_Report_VehiclePerformance
                .ExecWithStoredProcedure("sp_NWC_Report_VehiclePerformance @PageIndex, @PageSize, @SubID, @AreaIDs, @CityIDs, @StationIDs, @ServiceTypeIDs, @VehicleCode, @DateFrom, @DateTo",
                                            new SqlParameter("PageIndex", SqlDbType.Int) { Value = SetDBValue(PageIndex) },
                                            new SqlParameter("PageSize", SqlDbType.Int) { Value = SetDBValue(PageSize) },
                                            new SqlParameter("SubID", SqlDbType.UniqueIdentifier) { Value = SetDBValue(_loggedInUser.LoggedInUser.SubscriberId) },
                                            new SqlParameter("AreaIDs", SqlDbType.NVarChar) { Value = SetDBValue(Join(searchCriteria.AreaIDs)) },
                                            new SqlParameter("CityIDs", SqlDbType.NVarChar) { Value = SetDBValue(Join(searchCriteria.CityIDs)) },
                                            new SqlParameter("StationIDs", SqlDbType.NVarChar) { Value = SetDBValue(Join(searchCriteria.StationIDs)) },
                                            new SqlParameter("ServiceTypeIDs", SqlDbType.NVarChar) { Value = SetDBValue(Join(searchCriteria.ServiceTypeIDs)) },
                                            new SqlParameter("VehicleCode", SqlDbType.NVarChar) { Value = SetDBValue(searchCriteria.VehicleCode) },
                                            new SqlParameter("DateFrom", SqlDbType.DateTime) { Value = SetDBValue(searchCriteria.DateTimeFrom) },
                                            new SqlParameter("DateTo", SqlDbType.DateTime) { Value = SetDBValue(searchCriteria.DateTimeTo) });

            #region response
            var result = new SearchResult<VehiclePerformanceDTO>()
            {
                TotalCount = list.FirstOrDefault().TotalCount ?? 0,
                Result = list.Select(a => a.WrapToVehiclePerformanceDTO()).ToList()
            };

            return DescriptiveResponse<SearchResult<VehiclePerformanceDTO>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<SearchResult<VehicleDataDTO>> GetVehicleDataReport(VehicleDataReportSC searchCriteria)
        {
            try
            {
                #region Predicate
                var predicate = PredicateBuilder.New<vw_NWC_Report_VehicleData>(s =>
               s.isDeleted != true
               && s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
            );

                #region Lists Predicate

                if (!string.IsNullOrEmpty(searchCriteria.Vehicle))
                {
                    var searchText = searchCriteria.Vehicle.Trim();
                    predicate = predicate.And(s => s.PlateNo.Contains(searchText) || s.Code.Contains(searchText));
                }

                if (!string.IsNullOrEmpty(searchCriteria.Driver))
                {
                    var searchDriver = searchCriteria.Driver.Trim();
                    var driverArr = searchCriteria.Driver.Trim().Split(' ');
                    if (driverArr.Length == 1)
                    {
                        predicate = predicate.And(s => s.DriverFirstName.Contains(searchDriver)
                                                    || s.DriverMiddleName.Contains(searchDriver)
                                                    || s.DriverLastName.Contains(searchDriver)
                                                    || s.DriverMobile.Contains(searchDriver));
                    }
                    else
                    {
                        predicate = predicate.And(s =>
                                (s.DriverFirstName + " " + s.DriverMiddleName + " " + s.DriverLastName).Contains(searchDriver)
                                || (s.DriverFirstName + " " + s.DriverLastName).Contains(searchDriver));
                    }
                }

                //permitted Areas
                if (searchCriteria.AreaIDs != null && searchCriteria.AreaIDs.Any())
                {
                    var areas = _loggedInUser.PermittedBranches.Intersect(searchCriteria.AreaIDs);
                    predicate = predicate.And(s => areas.Any(a => a == s.AreaId));
                }

                if (searchCriteria.ClassName != null)
                {

                    predicate = predicate.And(s => s.ClassesArEn != null && (s.ClassesArEn.Contains("سكن") == (searchCriteria.ClassName == "سكن")));
                    predicate = predicate.And(s => s.ClassesArEn != null && (s.ClassesArEn.Contains("تجار") == (searchCriteria.ClassName == "تجار")));
                    predicate = predicate.And(s => s.ClassesArEn != null && (s.ClassesArEn.Contains("حكوم") == (searchCriteria.ClassName == "حكوم")));
                }
                //else
                //{
                //    predicate = predicate.And(s => _loggedInUser.UserServicesIds.Any(a => a == s.ServiceTypeID));
                //}

                //permitted cities
                if (searchCriteria.CityIDs != null && searchCriteria.CityIDs.Any())
                {
                    var cities = _loggedInUser.PermittedBranches.Intersect(searchCriteria.CityIDs);
                    predicate = predicate.And(s => cities.Any(a => a == s.CityId));
                }
                else
                {
                    predicate = predicate.And(s => _loggedInUser.PermittedBranches.Any(a => a == s.CityId));
                }

                //permitted stations
                if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())
                {
                    var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);
                    predicate = predicate.And(s => searchList.Any(a => a == s.StationId));
                }
                else
                {
                    predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Any(a => a == s.StationId));
                }

                //permitted serviceTypes
                if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())
                {
                    var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);
                    predicate = predicate.And(s => services.Any(a => a == s.ServiceTypeID));
                }
                else
                {
                    predicate = predicate.And(s => _loggedInUser.UserServicesIds.Any(a => a == s.ServiceTypeID));
                }

                //statuses
                if (searchCriteria.StatusIDs != null && searchCriteria.StatusIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.StatusIDs.Any(a => a == s.StatusId));
                }

                #endregion

                #endregion

                IQueryable<vw_NWC_Report_VehicleData> workOrderList =
                    this._Report_VehicleData.GetQuery()
                        .Where(predicate)
                        .OrderBy(s => s.StationName);

                if (!searchCriteria.ExcelFlage)
                {
                    #region skip & take
                    var skip = 0;
                    var take = 10;
                    if (searchCriteria.PageFilter != null)
                    {
                        skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                        take = searchCriteria.PageFilter.PageSize;
                    }
                    #endregion

                    workOrderList = workOrderList
                        .Skip(skip)
                        .Take(take);
                }

                #region response
                var result = new SearchResult<VehicleDataDTO>();
                if (workOrderList != null && workOrderList.Any())
                {
                    var count = this._Report_VehicleData.GetQuery().Count(predicate);
                    result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToVehicleDataDTO()).ToList();
                    result.TotalCount = count;
                }

                return DescriptiveResponse<SearchResult<VehicleDataDTO>>.Success(result);
                #endregion
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "VehicleService => GetVehicleDataReport: "));
                return DescriptiveResponse<SearchResult<VehicleDataDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion
        #region "Helper"
        public object SetDBValue(object value) => value == null ? DBNull.Value : value;
        public string Join<T>(IEnumerable<T> list, string separator = ",") => list != null && list.Any() ? string.Join(separator, list) : null;
        public VehicleDataReportSC WrapVehicleToDashboardSC(DashboardSC dto)
        {
            return new VehicleDataReportSC { AreaIDs = dto.AreaIDs, CityIDs = dto.CityIDs, ServiceTypeIDs = new List<int>() { (int)ServiceTypeEnum.Ashyab }, StationIDs = dto.StationIDs, StatusIDs = dto.StatusIDs, ClassName = dto.ClassName, PageFilter = new PageFilter { PageSize = 20000, PageIndex = 1 } };
        }
        #endregion
    }
}
