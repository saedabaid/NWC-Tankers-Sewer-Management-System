using Infrastructure;
using LinqKit;
using NWC.BLL.Interfaces;
using NWC.BLL.Validators;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Constants;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;

namespace NWC.BLL.Services
{
    public class TransporterService : ITransporterService
    {
        #region Properties
        private readonly ILoggedInUserService _loggedInUser;
        private readonly DbContext ctx;
        private readonly IUnitofWork _unitOfWork;
        private readonly IRepository<Transporter> _repository;
        private readonly IRepository<TransporterType> _repositoryTransporterType;
        private readonly IRepository<TransporterStatus> _StatusRepository;
        private readonly IRepository<Transporter_Staff> _repositoryTransporterStaff;
        private readonly IRepository<Staff> _staffRepository;
        #endregion

        public TransporterService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            _loggedInUser = loggedInUser;

            ctx = (context == null ? new NWCContext() : context);
            _unitOfWork = new UnitofWork(ctx);
            _repository = new Repository<Transporter>(ctx);
            _StatusRepository = new Repository<TransporterStatus>(ctx);
            _repositoryTransporterStaff = new Repository<Transporter_Staff>(ctx);
            _staffRepository = new Repository<Staff>(ctx);
            _repositoryTransporterType = new Repository<TransporterType>(ctx);
        }

        public DescriptiveResponse<SearchResult<TransporterDTO>> Search(TransporterSC searchCriteria)
        {
            var predicate = PredicateBuilder.New<Transporter>
                (s => s.SubID == _loggedInUser.LoggedInUser.SubscriberId && s.isDeleted != true);

            if (!string.IsNullOrEmpty(searchCriteria.PlateNoOrCode))
                predicate = predicate.And(s => s.plateNo.Contains(searchCriteria.PlateNoOrCode) || s.code.Contains(searchCriteria.PlateNoOrCode));

            if (searchCriteria.NextExaminationDate.HasValue)
                predicate = predicate.And(s => s.NextExaminationDateHijri == searchCriteria.NextExaminationDate.ToString());

            if (searchCriteria.LicenceExpirationDate.HasValue)
                predicate = predicate.And(s => s.LicenseExpiryDateHijri == searchCriteria.LicenceExpirationDate.ToString());

            if (searchCriteria.InsuranceEndDate.HasValue)
                predicate = predicate.And(s => s.InsuranceEndDateHijri == searchCriteria.InsuranceEndDate.ToString());

            if (searchCriteria.EntranceDate.HasValue)
                predicate = predicate.And(s => s.EntranceDateHijri == searchCriteria.EntranceDate.ToString());

            if (!string.IsNullOrEmpty(searchCriteria.ChassisNo))
                predicate = predicate.And(s => s.chassisNo == searchCriteria.ChassisNo);

            if (!string.IsNullOrEmpty(searchCriteria.SIMCardNo))
                predicate = predicate.And(s => s.SIMCardNo == searchCriteria.SIMCardNo);

            if (!string.IsNullOrEmpty(searchCriteria.DeviceCode))
                predicate = predicate.And(s => s.deviceCode == searchCriteria.DeviceCode);

            if (searchCriteria.BrandIDs != null && searchCriteria.BrandIDs.Any())
                predicate = predicate.And(s => searchCriteria.BrandIDs.Any(a => a == s.transporterBrand));

            if (searchCriteria.ModelIDs != null && searchCriteria.ModelIDs.Any())
                predicate = predicate.And(s => searchCriteria.ModelIDs.Any(a => a == s.TrackerModelNameEn));

            if (searchCriteria.YearIDs != null && searchCriteria.YearIDs.Any())
                predicate = predicate.And(s => searchCriteria.YearIDs.Any(a => a == s.transporterProductionYear));

            if (searchCriteria.VehicleTypeIDs != null && searchCriteria.VehicleTypeIDs.Any())
                predicate = predicate.And(s => searchCriteria.VehicleTypeIDs.Any(a => a == s.transporterType));

            //if (searchCriteria.GroupIDs != null && searchCriteria.GroupIDs.Any())
            //    predicate = predicate.And(s => searchCriteria.GroupIDs.Any(a => a == s.group));

            if (searchCriteria.StatusIDs != null && searchCriteria.StatusIDs.Any())
                predicate = predicate.And(s => searchCriteria.StatusIDs.Any(a => a == s.status));

            if (searchCriteria.BranchIDs != null && searchCriteria.BranchIDs.Any())
                predicate = predicate.And(s => searchCriteria.BranchIDs.Any(a => a == s.branch));

            //if (searchCriteria.SubBranchIDs != null && searchCriteria.SubBranchIDs.Any())
            //    predicate = predicate.And(s => searchCriteria.SubBranchIDs.Any(a => a == s.SubID));

            if (searchCriteria.LandmarkIDs != null && searchCriteria.LandmarkIDs.Any())
                predicate = predicate.And(s => searchCriteria.LandmarkIDs.Any(a => a == s.landmark));

            IQueryable<Transporter> workOrderList =
                _repository.GetQuery()
                    .Where(predicate)
                    .Include("Transporter_Staff")
                    .Include("Transporter_Staff.Staff1")
                    .OrderByDescending(s => s.ID);

            if (!searchCriteria.ExcelFlage)
            {
                var skip = 0;
                var take = 10;
                if (searchCriteria.PageFilter != null)
                {
                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                    take = searchCriteria.PageFilter.PageSize;
                }

                workOrderList = workOrderList
                    .Skip(skip)
                    .Take(take);
            }

            var result = new SearchResult<TransporterDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = _repository.GetQuery().Count(predicate);
                result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToTransporterDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<TransporterDTO>>.Success(result);
        }

        public DescriptiveResponse<SearchResult<VehicleTypeDTO>> searchVehicleType(VehicleTypeDTO searchCriteria)
        {
            try
            {
                var predicate = PredicateBuilder.New<TransporterType>
                (s => s.SubID == _loggedInUser.LoggedInUser.SubscriberId && s.isDeleted != true);

            if (!string.IsNullOrEmpty(searchCriteria.Name))
                predicate = predicate.And(s => s.name.Contains(searchCriteria.Name));



            IQueryable<TransporterType> workOrderList =
                _repositoryTransporterType.GetQuery()
                  .Where(predicate)
                    .OrderByDescending(s => s.ID);

            var skip = 0;
                var take = 10;
                if (searchCriteria.PageFilter != null)
                {
                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;
                    take = searchCriteria.PageFilter.PageSize;
                }

                workOrderList = workOrderList
                    .Skip(skip)
                    .Take(take);

            var result = new SearchResult<VehicleTypeDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = _repositoryTransporterType.GetQuery().Count();
                result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToTransporterTypeDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<VehicleTypeDTO>>.Success(result);
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<SearchResult<VehicleTypeDTO>>.Error(ex.Message +"||"+ ex.InnerException?.InnerException.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<TransporterDTO> GetOne(Guid id)
        {
            var predicate = PredicateBuilder.New<Transporter>
                (s => s.SubID == _loggedInUser.LoggedInUser.SubscriberId
                && s.isDeleted != true
                && s.ID == id);
            Transporter entity = _repository.GetQuery().FirstOrDefault(predicate);
            return DescriptiveResponse<TransporterDTO>.Success(entity.WrapToTransporterDTO());
        }
        public DescriptiveResponse<TransporterDTO> GetTransporterByNumber(string transporterNo)
        {
            var predicate = PredicateBuilder.New<Transporter>
                (s =>  s.isDeleted != true
                && s.code == transporterNo);
            Transporter entity = _repository.GetQuery().FirstOrDefault(predicate);
            return DescriptiveResponse<TransporterDTO>.Success(entity.WrapToTransporterDTO());
        }
        public DescriptiveResponse<bool> DeleteVehicleType(Guid ID)
        {
            try
            {
                using (_unitOfWork)
                {
                    var acc = this._repositoryTransporterType.GetQuery().FirstOrDefault(s =>
                                    s.ID == ID);
                    if (acc != null)
                    {
                        acc.isDeleted = true;
                        this._repositoryTransporterType.Update(acc);
                    }
                }
                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<Guid?> Add(TransporterDTO dto)
        {
            var validator = new TransporterValidator(ValidationMode.Create, _loggedInUser, _repository);
            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                var failures = results.Errors.Select(s => s.ErrorMessage);
                return DescriptiveResponse<Guid?>.Error(failures);
            }

            else if (IsReachedTheMaximum())
            {
                var errors = new List<string>();
                errors.Add(LanguageIsEnglish ? "You have reached the maximum number of vehicles on your license" : "لقد وصلت للحد المسموح به من المركبات حسب رخصتك الحالية");
                return DescriptiveResponse<Guid?>.Error(errors);
            }

            var entity = TransporterWrapper.WrapDtoToTransporter(dto);

            entity.isDeleted = false;
            entity.SubID = _loggedInUser.LoggedInUser.SubscriberId;
            entity.CreatedBy = _loggedInUser.LoggedInUser.StaffId;
            entity.CreationTime = DateTimeHelper.GetDateTimeNow();
            foreach (var staff in dto.Staff)
                entity.Transporter_Staff.Add(new Transporter_Staff
                {
                    Id = Guid.NewGuid(),
                    Staff = staff.Id,
                    VehicleReceivingDate = DateTimeHelper.GetDateTimeNow(),
                    SubId = _loggedInUser.LoggedInUser.SubscriberId,
                    CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                    CreationTime = DateTimeHelper.GetDateTimeNow()
                });

            using (_unitOfWork)
            {
                _repository.Add(entity);
            }

            return DescriptiveResponse<Guid?>.Success(entity.ID);
        }

        public DescriptiveResponse<bool> Edit(TransporterDTO dto)
        {
            var validator = new TransporterValidator(ValidationMode.Update, _loggedInUser, _repository);
            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                var failures = results.Errors.Select(s => s.ErrorMessage);
                return DescriptiveResponse<bool>.Error(failures);
            }

            var entity = _repository.GetQuery()
                .FirstOrDefault(a => a.ID == dto.Id
                                     && a.isDeleted != true
                                     && a.SubID == _loggedInUser.LoggedInUser.SubscriberId);

            entity = TransporterWrapper.WrapDtoToTransporter(dto, entity);


            entity.LastModifiedBy = _loggedInUser.LoggedInUser.StaffId;
            entity.LastModificationDate = DateTimeHelper.GetDateTimeNow();

            using (_unitOfWork)
            {
                foreach (var staff in entity.Transporter_Staff.Where(staff => !dto.Staff.Any(x => x.Id == staff.Staff1.ID)).ToList())
                    _repositoryTransporterStaff.Delete(staff);
                foreach (var staff in dto.Staff.Where(staff => !entity.Transporter_Staff.Any(x => x.Staff1.ID == staff.Id)).ToList())
                {
                    _repositoryTransporterStaff.Add(new Transporter_Staff
                    {
                        Id = Guid.NewGuid(),
                        Staff = staff.Id,
                        Transporter = entity.ID,
                        VehicleReceivingDate = DateTimeHelper.GetDateTimeNow(),
                        SubId = _loggedInUser.LoggedInUser.SubscriberId,
                        CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                        CreationTime = DateTimeHelper.GetDateTimeNow()
                    });
                }

                _repository.Update(entity);
            }

            return DescriptiveResponse<bool>.Success(true);
        }

        public DescriptiveResponse<bool> Delete(Guid id)
        {
            var entity = _repository.GetQuery()
                .FirstOrDefault(a => a.ID == id && a.SubID == _loggedInUser.LoggedInUser.SubscriberId);

            entity.isDeleted = true;
            entity.LastModifiedBy = _loggedInUser.LoggedInUser.StaffId;
            entity.LastModificationDate = DateTime.Now;

            using (_unitOfWork)
            {
                _repository.Update(entity);
            }

            return DescriptiveResponse<bool>.Success(true);
        }

        public DescriptiveResponse<List<TransporterExcelDTO>> AddRange(List<TransporterExcelDTO> dtos)
        {

            var failedList = new List<TransporterExcelDTO>();

            var _transporterTypeRepository = new Repository<TransporterType>(ctx);
            var _transporterProductionYearRepository = new Repository<TransporterProductionYear>(ctx);
            var _branchRepository = new Repository<Branch>(ctx);
            var _landmarkRepository = new Repository<Landmark>(ctx);
            var _transporterBrandRepository = new Repository<TransporterBrand>(ctx);
            var _transporterManufacturerRepository = new Repository<TransporterManufacturer>(ctx);

            foreach (var dto in dtos)
            {
                var validator = new TransporterValidator(ValidationMode.Create, _loggedInUser, _repository, dtos);
                var results = validator.Validate(new TransporterDTO());

                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    dto.ExcelValidation = string.Join(", ", failures);

                    failedList.Add(dto);
                }
  
               else if (IsReachedTheMaximum())
                {
                    var errors = new List<string>();
                    errors.Add(LanguageIsEnglish ? "You have reached the maximum number of vehicles allowed" : "لقد وصلت إلى الحد الأقصى لعدد المركبات المسموح به");
                    return DescriptiveResponse<List<TransporterExcelDTO>>.Error(errors);
                }
                else
                {
                    using (_unitOfWork)
                    {
                        var entity = dto.WrapExcelDtoToTransporter(new Transporter());
                        entity.isDeleted = false;
                        entity.SubID = _loggedInUser.LoggedInUser.SubscriberId;
                        entity.CreatedBy = _loggedInUser.LoggedInUser.StaffId;
                        entity.CreationTime = DateTimeHelper.GetDateTimeNow();
                        entity.status = (int)VehicleStatusEnum.Parking;
                        try
                        {
                            var driverId = _staffRepository.GetQuery().FirstOrDefault(x => x.code == dto.DriverCode)?.ID;
                            if (driverId != null)
                                _repositoryTransporterStaff.Add(new Transporter_Staff
                                {
                                    Id = Guid.NewGuid(),
                                    Staff = driverId,
                                    Transporter = entity.ID,
                                    VehicleReceivingDate = DateTimeHelper.GetDateTimeNow(),
                                    SubId = _loggedInUser.LoggedInUser.SubscriberId,
                                    CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                                    CreationTime = DateTimeHelper.GetDateTimeNow()
                                });
                            entity.transporterType = _transporterTypeRepository.GetQuery().FirstOrDefault(x => x.name == dto.TransporterType || x.NameAr == dto.TransporterType)?.ID;
                            entity.transporterProductionYear = _transporterProductionYearRepository.GetQuery().FirstOrDefault(x => x.name == dto.ProductionYear)?.ID;
                            entity.branch = _branchRepository.GetQuery().FirstOrDefault(x => x.name.Trim() == dto.SubBranch.Trim())?.Id;
                            entity.landmark = _landmarkRepository.GetQuery().FirstOrDefault(x => x.name.Trim() == dto.Landmark.Trim())?.Id;
                            entity.transporterBrand = _transporterBrandRepository.GetQuery().FirstOrDefault(x => x.name.Trim() == dto.Brand.Trim() || x.NameAr.Trim() == dto.Brand.Trim())?.ID;
                            entity.transporterManufacturer = _transporterManufacturerRepository.GetQuery().FirstOrDefault(x => x.name == dto.Model.Trim() || x.NameAr == dto.Model.Trim())?.ID;
                            if (entity.branch != null && entity.landmark != null)
                                _repository.Add(entity);
                            else
                                failedList.Add(dto);
                        }
                        catch
                        {
                            failedList.Add(dto);
                        }
                    }
                }
            }

            return DescriptiveResponse<List<TransporterExcelDTO>>.Success(failedList);
        }

        public DescriptiveResponse<bool> updateVehicleStatus(EditTransporterStatusDTO dto)
        {
            if (dto.TransporterID == Guid.Empty || !Enum.IsDefined(typeof(VehicleStatusEnum), dto.VehicleStatusID))
            {
                return DescriptiveResponse<bool>.Error(ErrorStatus.INPUT_IS_NULL);
            }

            var entity = _repository.FindById(dto.TransporterID);

            if (!IsValidVehicleStatusWorkflow((int)entity.status, (int)dto.VehicleStatusID))
            {
                return DescriptiveResponse<bool>.Error(ErrorStatus.INPUT_INVALID);
            }
            if (entity.status == (int)VehicleStatusEnum.Assigned || entity.status == (int)VehicleStatusEnum.InService || dto.VehicleStatusID == (int)VehicleStatusEnum.Available)
            {
                return DescriptiveResponse<bool>.Error(ErrorStatus.INPUT_INVALID);
            }

            entity.LastModifiedBy = _loggedInUser.LoggedInUser.StaffId;
            entity.LastModificationDate = DateTimeHelper.GetDateTimeNow();
            entity.status = dto.VehicleStatusID;

            using (_unitOfWork)
            {
                _repository.Update(entity);
            }

            return DescriptiveResponse<bool>.Success(true);
        }


        private bool IsValidVehicleStatusWorkflow(int currentStatusID, int nextStatusID)
        {
            var transporterStatus = this._StatusRepository.FindById(currentStatusID);

            return transporterStatus.NextStatusIDs != null && transporterStatus.NextStatusIDs.Contains(nextStatusID.ToString());
        }

      private bool  IsReachedTheMaximum()
        {
      return _repository.GetQuery().Count() > 14000;
        }

        private bool LanguageIsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
            }
        }
    }
}
