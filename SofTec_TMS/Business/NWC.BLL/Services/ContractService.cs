using Infrastructure;using LinqKit;using NWC.BLL.Interfaces;using NWC.BLL.Validators;using NWC.DAL.NWCEntities;using NWC.DTO.Common;using NWC.DTO.Constants;using NWC.DTO.Enums;using NWC.DTO.Helpers;using NWC.DTO.Models;using NWC.DTO.Resources;using NWC.DTO.SearchCriteria;using NWC.DTO.Wrapper;using NWC_CCB_Integration.DTO.Logger;using System;using System.Collections.Generic;using System.Data;using System.Data.Entity;using System.Data.SqlClient;using System.Linq;using System.Threading;namespace NWC.BLL.Services{    public class ContractService : IContractService    {

        #region private variables        private readonly ILoggedInUserService _loggedInUser;        private readonly IUnitofWork _unitofWork;        private readonly IRepository<vw_NWC_ContractList> _contractListRepository;        private readonly IRepository<NWC_Contract> _contractRepository;        private readonly IRepository<vw_NWC_ContractTariff> _tariffListRepository;        private readonly IRepository<Branch> _Branch;        private readonly IRepository<Landmark> _Landmark;        private readonly IRepository<NWC_ContractTariff> _contractTariffRepository;        private readonly IRepository<vw_NWC_ContractStations> _ContractStationsRepository;        private readonly IRepository<NWC_ContractStations> _ContractStationsTableRepository;        private readonly IRepository<NWC_ContactPerson> _ContactPersonRepository;        private readonly IRepository<NWC_ContractAccessory> _contractAccessoryRepository;        private readonly IRepository<NWC_Contract> _ContractRepository;        private readonly IRepository<vw_NWC_ContractAccessory> _vwContractAccessoryRepository;        private readonly IRepository<NWC_ContractStationViolation> _ContractStationViolationRepository;        private readonly IRepository<NWC_ContractPrice> _ContractPriceRepository;        private readonly IRepository<NWC_ServiceType> _ServiceTypeRepository;        private readonly IRepository<NWC_CustomerLocationClass> _CustomerLocationClassRepository;        private readonly IRepository<vw_NWC_ContractPriceList> _ContractPriceListRepository;        private readonly IRepository<vw_NWC_ContractTerms> _ContractTermsView;        private readonly IRepository<NWC_ContractTerms> _ContractTermsTableRepository;        private readonly IRepository<NWC_ContractAttachment> _ContractAttachment;        private readonly IRepository<NWC_ContractStatus> _ContractStatusRepository;        private readonly IRepository<NWC_Contractor> _contractorRepository;        private readonly IRepository<NWC_TanckerCapacity> _tankerCapacityRepository;        private readonly IRepository<NWC_Zone> _ZoneRepository;        private readonly IRepository<vw_NWC_ContractTermsViolations> _ViolationsView;        private readonly IRepository<NWC_ContractTermsViolations> _TermsViolations;        private readonly IRepository<NWC_ContractViolationAttachment> _ViolationAttachment;        private readonly IRepository<Transporter> _TransporterRepository;        private readonly IRepository<NWC_ContractTermsViolationsLogs> _ContractTermsViolationsLogs;        private readonly IRepository<vw_NWC_ContractTermsViolationsLogs> _ContractTermsViolationsLogsView;        private readonly IRepository<NWC_ContractTermsViolationsInvoices> _ContractTermsViolationsInvoices;        private readonly IRepository<vw_NWC_ContractTermsViolationsInvoices> _ContractTermsViolationsInvoicesView;        private readonly IRepository<NWC_ViolationsApprovals> _NWC_ViolationsApprovals;        private readonly IRepository<vw_NWC_ViolationsApprovalsLogs> _vw_NWC_ViolationsApprovalsLogs;
        private readonly IRepository<NWC_ViolationsApprovalsLogs> _NWC_ViolationsApprovalsLogs;
        private readonly IRepository<vw_NWC_ViolationsApprovals> _vw_NWC_ViolationsApprovals;


































        #endregion
        #region cto.        public ContractService(ILoggedInUserService loggedInUser, DbContext context = null)        {            this._loggedInUser = loggedInUser;            var ctx = (context == null ? new NWCContext() : context);            this._unitofWork = new UnitofWork(ctx);            this._contractListRepository = new Repository<vw_NWC_ContractList>(ctx);            this._contractRepository = new Repository<NWC_Contract>(ctx);            this._tariffListRepository = new Repository<vw_NWC_ContractTariff>(ctx);            this._contractTariffRepository = new Repository<NWC_ContractTariff>(ctx);            this._ContractStationsRepository = new Repository<vw_NWC_ContractStations>(ctx);            this._ContractStationsTableRepository = new Repository<NWC_ContractStations>(ctx);            this._ContactPersonRepository = new Repository<NWC_ContactPerson>(ctx);            this._Branch = new Repository<Branch>(ctx);            this._Landmark = new Repository<Landmark>(ctx);            this._contractAccessoryRepository = new Repository<NWC_ContractAccessory>(ctx);            this._vwContractAccessoryRepository = new Repository<vw_NWC_ContractAccessory>(ctx);            this._ContractRepository = new Repository<NWC_Contract>(ctx);            this._ContractStationViolationRepository = new Repository<NWC_ContractStationViolation>(ctx);            this._ContractPriceRepository = new Repository<NWC_ContractPrice>(ctx);            this._ServiceTypeRepository = new Repository<NWC_ServiceType>(ctx);            this._CustomerLocationClassRepository = new Repository<NWC_CustomerLocationClass>(ctx);            this._ContractPriceListRepository = new Repository<vw_NWC_ContractPriceList>(ctx);            this._ContractTermsView = new Repository<vw_NWC_ContractTerms>(ctx);            this._ContractTermsTableRepository = new Repository<NWC_ContractTerms>(ctx);            this._ContractAttachment = new Repository<NWC_ContractAttachment>(ctx);            this._ContractStatusRepository = new Repository<NWC_ContractStatus>(ctx);            this._contractorRepository = new Repository<NWC_Contractor>(ctx);            this._tankerCapacityRepository = new Repository<NWC_TanckerCapacity>(ctx);            this._ZoneRepository = new Repository<NWC_Zone>(ctx);            this._ViolationsView = new Repository<vw_NWC_ContractTermsViolations>(ctx);            this._TermsViolations = new Repository<NWC_ContractTermsViolations>(ctx);            this._ViolationAttachment = new Repository<NWC_ContractViolationAttachment>(ctx);            this._TransporterRepository = new Repository<Transporter>(ctx);            this._ContractTermsViolationsLogs = new Repository<NWC_ContractTermsViolationsLogs>(ctx);            this._ContractTermsViolationsLogsView = new Repository<vw_NWC_ContractTermsViolationsLogs>(ctx);            this._ContractTermsViolationsInvoices = new Repository<NWC_ContractTermsViolationsInvoices>(ctx);            this._ContractTermsViolationsInvoicesView = new Repository<vw_NWC_ContractTermsViolationsInvoices>(ctx);            this._NWC_ViolationsApprovals = new Repository<NWC_ViolationsApprovals>(ctx);            this._vw_NWC_ViolationsApprovalsLogs = new Repository<vw_NWC_ViolationsApprovalsLogs>(ctx);            this._NWC_ViolationsApprovalsLogs = new Repository<NWC_ViolationsApprovalsLogs>(ctx);            this._vw_NWC_ViolationsApprovals = new Repository<vw_NWC_ViolationsApprovals>(ctx);        }
        #endregion
        #region Contract
        #region Query        public DescriptiveResponse<SearchResult<ContractDTO>> SearchContractList(ContractSearchCriteriaDTO searchCriteria)        {
















            #region Predicate            var predicate = PredicateBuilder.New<vw_NWC_ContractList>(true);            predicate = predicate.And(s => s.IsDeleted != true);
            //predicate = predicate.And(s => s.StaffId == this._loggedInUser.LoggedInUser.StaffId);
            predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);            if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))            {                var word = searchCriteria.FilterModel.SearchKeyword.Trim();                predicate = predicate.And(s => s.Code.Contains(word));            }            if (searchCriteria.Id.HasValue)            {                predicate = predicate.And(s => s.ID == searchCriteria.Id.Value);            }

























































            #region Date Time Predicate            //if (searchCriteria.DateTimeFrom != null)
                                                    //{
                                                    //    switch (searchCriteria.DatePeriod)
                                                    //    {
                                                    //        case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                                                    //            predicate = predicate.And(s => s.RequestTime >= searchCriteria.DateTimeFrom);
                                                    //            break;
                                                    //        case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                                                    //            predicate = predicate.And(s => s.ScheduledDeliveryTime >= searchCriteria.DateTimeFrom);
                                                    //            break;
                                                    //        case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                                                    //            predicate = predicate.And(s => s.LastStatusTime >= searchCriteria.DateTimeFrom);
                                                    //            break;
                                                    //    }
                                                    //}
                                                    //if (searchCriteria.DateTimeTo != null)
                                                    //{
                                                    //    switch (searchCriteria.DatePeriod)
                                                    //    {
                                                    //        case WorkOrderSearchCriteriaDTO.DateToSearch.RequestDate:
                                                    //            predicate = predicate.And(s => s.RequestTime <= searchCriteria.DateTimeTo);
                                                    //            break;
                                                    //        case WorkOrderSearchCriteriaDTO.DateToSearch.ScheduleDate:
                                                    //            predicate = predicate.And(s => s.ScheduledDeliveryTime <= searchCriteria.DateTimeTo);
                                                    //            break;
                                                    //        case WorkOrderSearchCriteriaDTO.DateToSearch.LastStatusModificationDate:
                                                    //            predicate = predicate.And(s => s.LastStatusTime <= searchCriteria.DateTimeTo);
                                                    //            break;
                                                    //    }
                                                    //}
            #endregion
            #endregion
            #region skip & take            var skip = 0;            var take = 20;            if (searchCriteria.FilterModel != null && searchCriteria.FilterModel.PageFilter != null)            {                skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;                take = searchCriteria.FilterModel.PageFilter.PageSize;            }


            #endregion
            var contractQuery = this._contractListRepository.GetQuery()
.Where(predicate)
.OrderBy(s => s.Code)//.ThenBy(s => s.PriorityID).ThenBy(s => s.ScheduledDeliveryTime)
                                                         .Skip(skip)
.Take(take);

















            #region response            var result = new SearchResult<ContractDTO>();            if (contractQuery != null && contractQuery.Any())            {                var count = this._contractListRepository.GetQuery().Count(predicate);                result.Result = contractQuery.AsEnumerable().Select(a => a.WrapToContractDTO()).ToList();                result.TotalCount = count;            }            return DescriptiveResponse<SearchResult<ContractDTO>>.Success(result);
















            #endregion        }        public DescriptiveResponse<IEnumerable<AttachmentDTO>> GetContractAttachments(long contractId)        {            var contractAttachments = _ContractAttachment.GetQuery()                .Where(a => a.ContractID == contractId && !a.IsDeleted && a.NWC_Contract.IsDeleted != true                            && a.NWC_Contract.SubID == _loggedInUser.LoggedInUser.SubscriberId)                .AsEnumerable()                .Select(a => a.WrapContractAttachment());            return DescriptiveResponse<IEnumerable<AttachmentDTO>>.Success(contractAttachments);        }

































        #endregion
        #region Command        public DescriptiveResponse<long?> AddContract(ContractDTO dto)        {












            #region Validations            var validator = new ContractValidator(ValidationMode.Create, this._loggedInUser, _contractRepository, this._contractorRepository);            var results = validator.Validate(dto);            if (!results.IsValid)            {                var failures = results.Errors.Select(s => s.ErrorMessage);                return DescriptiveResponse<long?>.Error(failures);            }


            #endregion
            var contract = ContractWrapper.WrapToContract(dto);            contract.ContractStatusID = GetContractStatusId(contract.ContractStartDate, contract.ContractEndDate);













            #region prepare model            contract.IsDeleted = false;            contract.SubID = this._loggedInUser.LoggedInUser.SubscriberId;            contract.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;            contract.CreatedDate = DateTimeHelper.GetDateTimeNow();


            //contract.StatusChangedDate = DateTimeHelper.GetDateTimeNow();
            #endregion
            using (_unitofWork)            {                this._contractRepository.Add(contract);            }













            #region Attachments            if (dto.ContractAttachments != null && dto.ContractAttachments.Count() > 0)            {                var attachments = new List<NWC_ContractAttachment>();                foreach (var item in dto.ContractAttachments)                {                    var newPath = Utilities.MoveFile(AttachmentType.Contract, item.RelativePath, contract.ID);

















                    #region new Attachment                    var newAttach = new NWC_ContractAttachment
                                                               {
                                                                   ContractID = contract.ID,
                                                                   RelativePath = newPath,
                                                                   DocumentName = item.DocumentName,
                                                                   IsDeleted = false,
                                                                   CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                                                                   CreatedDate = DateTimeHelper.GetDateTimeNow()
                                                               };



                    #endregion
                    attachments.Add(newAttach);                }                using (_unitofWork)                {                    this._ContractAttachment.AddRange(attachments);                }            }


            #endregion

            return DescriptiveResponse<long?>.Success(contract.ID);        }
        public DescriptiveResponse<long?> AddViolationApproval(ViolationApprovalsDTO dto)
        {
            try
            {
                if (_vw_NWC_ViolationsApprovals.GetQuery().FirstOrDefault(x => x.LevelID == dto.LevelNo && x.StaffID == dto.StaffId) != null)
                {
                    return DescriptiveResponse<long?>.Error(ErrorStatus.LevelRepeated);
                }
                if (dto.LevelNo == 1 || _vw_NWC_ViolationsApprovals.GetQuery().FirstOrDefault(x => x.LevelID == (dto.LevelNo - 1) && x.LandmarkID == dto.Landmark && x.IsDeleted != true)  != null)
                {
                    var contract = ContractWrapper.WrapToViolationApprovalsDTO(dto);
                    using (_unitofWork)
                    {
                        this._NWC_ViolationsApprovals.Add(contract);
                    }
                    return DescriptiveResponse<long?>.Success(contract.ID);
                }
                else
                    return DescriptiveResponse<long?>.Error(ErrorStatus.PreviousLevel);
            }
            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => AddViolationApproval: "));                return DescriptiveResponse<long?>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);            }
        }

        public DescriptiveResponse<bool> EditContract(ContractDTO dto)        {








            #region Validations            var validator = new ContractValidator(ValidationMode.Update, this._loggedInUser, _contractRepository, this._contractorRepository);            var results = validator.Validate(dto);            if (!results.IsValid)            {                var failures = results.Errors.Select(s => s.ErrorMessage);                return DescriptiveResponse<bool>.Error(failures);            }


            #endregion
            //var contract = ContractWrapper.WrapToContract(dto);

            var existContract = this._contractRepository.GetQuery()
.FirstOrDefault(a => a.ID == dto.ID
&& a.IsDeleted != true
&& a.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Terminated
&& a.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Finished
&& a.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

















            #region Map Models            existContract.Code = dto.Code;            existContract.ContractTypeID = dto.ContractTypeID;            existContract.AwardLetterNo = dto.AwardLetterNo;            existContract.ConfirmationNo = dto.ConfirmationNo;            existContract.ContractorID = dto.ContractorID;            existContract.ContractStartDate = dto.ContractStartDate;            existContract.ContractEndDate = dto.ContractEndDate;            existContract.Description = dto.Description;


















            //existContract.ContractStatusID = contract.ContractStatusID;
            //existContract.IsTerminated = contract.IsTerminated;
            #endregion
            #region get Status Id            //if (dto.IsTerminated == true)
                                              //{
                                              //    existContract.TerminatedDate = dto.TerminatedDate;
                                              //    existContract.TerminationReasonID = dto.TerminationReasonID;
                                              //    existContract.ContractStatusID = GetContractStatusId(existContract.ContractStartDate, existContract.ContractEndDate, true);
                                              //}
                                              //else
                                              //{
                                              //}
            existContract.ContractStatusID = GetContractStatusId(existContract.ContractStartDate, existContract.ContractEndDate);
































            #endregion
            #region prepare model            existContract.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;            existContract.UpdatedDate = DateTimeHelper.GetDateTimeNow();


            //existContract.StatusChangedDate = DateTimeHelper.GetDateTimeNow();
            #endregion

            using (_unitofWork)            {
















                #region deactivate finished                if (//dto.IsTerminated ||
                                         ContractStatusEnum.Finished == GetContractStatusEnum(existContract.ContractStartDate, existContract.ContractEndDate))                {                    foreach (var item_Station in existContract.NWC_ContractStations)                    {                        item_Station.IsActive = false;                        this._ContractStationsTableRepository.Update(item_Station);                    }                    foreach (var item_Tariff in existContract.NWC_ContractTariff)                    {                        item_Tariff.IsActive = false;                        this._contractTariffRepository.Update(item_Tariff);                    }                    foreach (var item_Price in existContract.NWC_ContractPrice)                    {                        item_Price.IsActive = false;                        this._ContractPriceRepository.Update(item_Price);                    }                    foreach (var item_Accessory in existContract.NWC_ContractAccessory)                    {                        item_Accessory.IsActive = false;                        this._contractAccessoryRepository.Update(item_Accessory);                    }                    foreach (var item_Terms in existContract.NWC_ContractTerms)                    {                        item_Terms.IsActive = false;                        this._ContractTermsTableRepository.Update(item_Terms);                    }                }


                #endregion
                this._contractRepository.Update(existContract);            }

















            #region Attachments            if (dto.ContractAttachments != null && dto.ContractAttachments.Count() > 0)            {                using (_unitofWork)                {                    foreach (var item in dto.ContractAttachments)                    {                        if ((item.ID == null || item.ID == 0) && !item.IsDeleted)                        {
















                            #region new Attachment                            var newPath = Utilities.MoveFile(AttachmentType.Contract, item.RelativePath, existContract.ID);                            var newAttach = new NWC_ContractAttachment                            {                                ContractID = existContract.ID,                                RelativePath = newPath,                                DocumentName = item.DocumentName,                                IsDeleted = false,                                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,                                CreatedDate = DateTimeHelper.GetDateTimeNow()                            };                            this._ContractAttachment.Add(newAttach);
















                            #endregion                        }                        else if (item.IsDeleted)                        {
















                            #region delete attachment                            var oldAttach = this._ContractAttachment.GetQuery().FirstOrDefault(a => a.ID == item.ID && !a.IsDeleted);                            if (oldAttach != null)                            {                                oldAttach.IsDeleted = true;                                oldAttach.DeletedBy = this._loggedInUser.LoggedInUser.StaffId;                                oldAttach.DeletedDate = DateTimeHelper.GetDateTimeNow();                                this._ContractAttachment.Update(oldAttach);                            }
















                            #endregion                        }                    }                }            }


            #endregion
            return DescriptiveResponse<bool>.Success(true);        }        public DescriptiveResponse<bool> DeleteContract(long contractId)        {            var existContract = this._contractRepository.GetQuery()                .FirstOrDefault(a => a.ID == contractId                                     && a.IsDeleted != true                                     && a.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Terminated                                     && a.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Finished                                     && a.SubID == this._loggedInUser.LoggedInUser.SubscriberId);            using (_unitofWork)            {


                #region deactivate finished & terminated contracts
                foreach (var item_Station in existContract.NWC_ContractStations)                {                    item_Station.IsDeleted = true;                    item_Station.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;                    item_Station.UpdatedDate = DateTimeHelper.GetDateTimeNow();                    this._ContractStationsTableRepository.Update(item_Station);                }                foreach (var item_Tariff in existContract.NWC_ContractTariff)                {                    item_Tariff.IsDeleted = true;                    item_Tariff.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;                    item_Tariff.UpdatedDate = DateTimeHelper.GetDateTimeNow();                    this._contractTariffRepository.Update(item_Tariff);                }                foreach (var item_Price in existContract.NWC_ContractPrice)                {                    item_Price.IsDeleted = true;                    item_Price.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;                    item_Price.UpdatedDate = DateTimeHelper.GetDateTimeNow();                    this._ContractPriceRepository.Update(item_Price);                }                foreach (var item_Accessory in existContract.NWC_ContractAccessory)                {                    item_Accessory.IsDeleted = true;                    item_Accessory.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;                    item_Accessory.UpdatedDate = DateTimeHelper.GetDateTimeNow();                    this._contractAccessoryRepository.Update(item_Accessory);                }                foreach (var item_Terms in existContract.NWC_ContractTerms)                {                    item_Terms.IsDeleted = true;                    item_Terms.UpdateBy = this._loggedInUser.LoggedInUser.StaffId;                    item_Terms.UpdateDate = DateTimeHelper.GetDateTimeNow();                    this._ContractTermsTableRepository.Update(item_Terms);                }
































                #endregion
                #region prepare model                existContract.IsDeleted = true;                existContract.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;                existContract.UpdatedDate = DateTimeHelper.GetDateTimeNow();
















                #endregion                this._contractRepository.Update(existContract);            }            return DescriptiveResponse<bool>.Success(true);        }        public DescriptiveResponse<bool> TerminateContract(long contractId)        {            var existContract = this._contractRepository.GetQuery()                .FirstOrDefault(a => a.ID == contractId                                     && a.IsDeleted != true                                     && a.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Terminated                                     && a.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Finished                                     && a.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

















            #region prepare model            existContract.ContractStatusID = this.GetContractStatusId((int)ContractStatusEnum.Terminated);            existContract.TerminatedDate = DateTimeHelper.GetDateTimeNow();


            //existContract.TerminationReasonID = this._loggedInUser.LoggedInUser.StaffId;
            #endregion
            using (_unitofWork)            {
















                #region deactivate finished & terminated contracts                foreach (var item_Station in existContract.NWC_ContractStations)                {                    item_Station.IsActive = false;                    this._ContractStationsTableRepository.Update(item_Station);                }                foreach (var item_Tariff in existContract.NWC_ContractTariff)                {                    item_Tariff.IsActive = false;                    this._contractTariffRepository.Update(item_Tariff);                }                foreach (var item_Price in existContract.NWC_ContractPrice)                {                    item_Price.IsActive = false;                    this._ContractPriceRepository.Update(item_Price);                }                foreach (var item_Accessory in existContract.NWC_ContractAccessory)                {                    item_Accessory.IsActive = false;                    this._contractAccessoryRepository.Update(item_Accessory);                }                foreach (var item_Terms in existContract.NWC_ContractTerms)                {                    item_Terms.IsActive = false;                    this._ContractTermsTableRepository.Update(item_Terms);                }


                #endregion

                this._contractRepository.Update(existContract);            }            return DescriptiveResponse<bool>.Success(true);        }
































































        #endregion
        #endregion
        #region Tariff
        #region Query        public DescriptiveResponse<SearchResult<ContractTariffDTO>> SearchTariffList(Filters<long> filter)        {

            //test------------------------------------------------------------------------------------------------
            var text = $"SearchOrders@LoggedInUser.Lang: {this._loggedInUser.LoggedInUser.Lang}@thread lang (SearchWorkOrders): {Thread.CurrentThread.CurrentCulture.Name}";            text = text.Replace("@", System.Environment.NewLine);            LoggerManager.LogMsg(c => c.TrackingMsg(text));

















            //----------------------------------------------------------------------------------------------------


            #region Predicate            var predicate = PredicateBuilder.New<vw_NWC_ContractTariff>(true);            predicate = predicate.And(s => s.ContractID == filter.SearchKeyword);            predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);            predicate = predicate.And(s => s.CotractIsDeleted != true);            predicate = predicate.And(s => s.IsDeleted != true);

































            #endregion
            #region skip & take            var skip = 0;            var take = 20;            if (filter.PageFilter != null)            {                skip = (filter.PageFilter.PageIndex - 1) * filter.PageFilter.PageSize;                take = filter.PageFilter.PageSize;            }


            #endregion
            var contractQuery = this._tariffListRepository.GetQuery()
.Where(predicate)
.OrderBy(s => s.StationName).ThenBy(s => s.DateFrom)
.Skip(skip)
.Take(take);

















            #region response            var result = new SearchResult<ContractTariffDTO>();            if (contractQuery != null && contractQuery.Any())            {                var count = this._tariffListRepository.GetQuery().Count(predicate);                result.Result = contractQuery.AsEnumerable().Select(a => a.WrapToTariffDTO()).ToList();                result.TotalCount = count;            }            return DescriptiveResponse<SearchResult<ContractTariffDTO>>.Success(result);
















            #endregion        }        public DescriptiveResponse<SearchResult<ContractTariffDTO>> ContractTariffReport(ContractTariffSc searchCriteria)        {
















            #region skip & take            var skip = 0;            var take = 10;            if (searchCriteria.PageFilter != null)            {                skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;                take = searchCriteria.PageFilter.PageSize;            }




            #endregion
            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_ContractTariff>(true);            predicate = predicate.And(s => s.IsDeleted != true);            if (searchCriteria.ServiceTypeIDs != null && searchCriteria.ServiceTypeIDs.Any())            {                var services = _loggedInUser.UserServicesIds.Intersect(searchCriteria.ServiceTypeIDs);                predicate = predicate.And(s => services.Any(a => a == s.ServiceTypeID));            }            else            {                predicate = predicate.And(s => _loggedInUser.UserServicesIds.Contains(s.ServiceTypeID));            }            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())            {                var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);                predicate = predicate.And(s => searchList.Any(a => a == s.StationID));            }            else            {                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Contains(s.StationID));            }            if (searchCriteria.ZoneIDs != null && searchCriteria.ZoneIDs.Any())            {                var searchList = _loggedInUser.PermittedZonesIds.Intersect(searchCriteria.ZoneIDs);                predicate = predicate.And(s => searchList.Any(a => a == s.ZoneID));            }            else            {                var searchList = _loggedInUser.PermittedZonesIds.Intersect(searchCriteria.ZoneIDs);            }            if (searchCriteria.CustomerLocationClassIDs != null && searchCriteria.CustomerLocationClassIDs.Any())            {                predicate = predicate.And(s => searchCriteria.CustomerLocationClassIDs.Contains(s.CustomerLocationClassID));            }            if (searchCriteria.TankerCapacities != null && searchCriteria.TankerCapacities.Any())            {                predicate = predicate.And(s => searchCriteria.TankerCapacities.Contains((int)s.TanckerCapacityId));            }            if (searchCriteria.TarrifStatus != null && searchCriteria.TarrifStatus == (int)TarrifStatusEnum.all)            {                var dateNow = DateTimeHelper.ConvertDateToHijriAsLong((DateTime)DateTime.Now);                predicate = predicate.And(s => searchCriteria.TarrifStatus == (int)TarrifStatusEnum.valid && s.DateFromHijri <= dateNow && s.DateToHijri >= dateNow);                predicate = predicate.And(s => searchCriteria.TarrifStatus == (int)TarrifStatusEnum.notValid && s.DateFromHijri > dateNow || s.DateToHijri < dateNow);            }

            //if (searchCriteria.DateFrom != null && searchCriteria.DateTo != null)
            //{
            //    var dateFromHijri = DateTimeHelper.ConvertDateToHijriAsLong((DateTime)searchCriteria.DateFrom);
            //    var dateToHijri = DateTimeHelper.ConvertDateToHijriAsLong((DateTime)searchCriteria.DateTo);
            //    predicate = predicate.And(s=> dateFromHijri s.DateFromHijri >=);
            //}

            var contractTariffQuery = this._tariffListRepository.GetQuery()
.Where(predicate)
.OrderBy(s => s.StationName).ThenBy(s => s.DateFrom)
.Skip(skip)
.Take(take);































            #endregion
            #region response            var result = new SearchResult<ContractTariffDTO>();            if (contractTariffQuery != null && contractTariffQuery.Any())            {                var count = this._tariffListRepository.GetQuery().Count(predicate);                result.Result = contractTariffQuery.AsEnumerable().Select(a => a.WrapToTariffDTO()).ToList();                result.TotalCount = count;            }            return DescriptiveResponse<SearchResult<ContractTariffDTO>>.Success(result);
















            #endregion        }
































        #endregion
        #region Command        public DescriptiveResponse<AddItemsResponse> AddTariff(ContractTariffDTO dto)        {
















            #region Validations            var validator = new ContractTariffValidator(ValidationMode.Create, _contractTariffRepository, _contractRepository);            var results = validator.Validate(dto);            if (!results.IsValid)            {                var failures = results.Errors.Select(s => s.ErrorMessage);                return DescriptiveResponse<AddItemsResponse>.Error(failures);            }


            #endregion
            var tariffs = ContractWrapper.WrapToTariffsAdd(dto);            var removedCount = 0;            var totalCount = tariffs.Count();



            #region Validations for each Record ==> Warnings
            for (int i = totalCount - 1; i >= 0; i--)            {                var currentTariff = tariffs[i];                if (this._contractTariffRepository.GetQuery().Any(a =>                    !a.IsDeleted &&                    a.ContractID == currentTariff.ContractID &&                    a.StationID == currentTariff.StationID &&                    (a.ZoneID == null || currentTariff.ZoneID == null || a.ZoneID == currentTariff.ZoneID) &&                    (a.TanckerCapacityId == null || currentTariff.TanckerCapacityId == null || a.TanckerCapacityId == currentTariff.TanckerCapacityId) &&                    a.CustomerLocationClassID == currentTariff.CustomerLocationClassID &&                    a.ServiceTypeID == currentTariff.ServiceTypeID &&                    (a.DateFromHijri <= currentTariff.DateToHijri) && (currentTariff.DateFromHijri <= a.DateToHijri) // intercept                ))                {                    tariffs.Remove(tariffs[i]);                    removedCount++;                }            }

































            #endregion
            #region prepare model            foreach (var item in tariffs)            {                item.IsDeleted = false;
                //item.SubID = this._loggedInUser.LoggedInUser.SubscriberId;
                item.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;                item.CreatedDate = DateTimeHelper.GetDateTimeNow();            }


            #endregion
            using (_unitofWork)            {                this._contractTariffRepository.AddRange(tariffs);            }            return DescriptiveResponse<AddItemsResponse>.Success(new AddItemsResponse            {                failed = removedCount,                success = totalCount - removedCount            });        }        public DescriptiveResponse<bool> EditTariff(ContractTariffDTO dto)        {
















            #region Validations            var validator = new ContractTariffValidator(ValidationMode.Update, _contractTariffRepository, _contractRepository);            var results = validator.Validate(dto);            if (!results.IsValid)            {                var failures = results.Errors.Select(s => s.ErrorMessage);                return DescriptiveResponse<bool>.Error(failures);            }


            #endregion
            //var tariff = ContractWrapper.WrapToTariff(dto);

            var existTariff = this._contractTariffRepository.GetQuery()
.FirstOrDefault(a => a.ID == dto.ID && !a.IsDeleted && a.ZoneID != null);










            #region Map Models            //existTariff.ID = dto.ID;
                                           //existTariff.ContractID = dto.ContractID;
            existTariff.StationID = dto.StationID;            existTariff.ZoneID = dto.ZoneID;            existTariff.CustomerLocationClassID = dto.CustomerLocationClassID;            existTariff.ServiceTypeID = dto.ServiceTypeID;            existTariff.DateFrom = dto.DateFrom;            existTariff.DateTo = dto.DateTo;            existTariff.CubicMeterCharge = dto.CubicMeterCharge;            existTariff.DistanceCharge = dto.DistanceCharge;            existTariff.AfterFirstKM = dto.AfterFirstKM;            existTariff.TanckerCapacityId = dto.TanckerCapacityId;            existTariff.DateFromHijri = dto.DateFromHijri;            existTariff.DateToHijri = dto.DateToHijri;
































            #endregion
            #region prepare model            existTariff.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;            existTariff.UpdatedDate = DateTimeHelper.GetDateTimeNow();


            #endregion
            using (_unitofWork)            {                this._contractTariffRepository.Update(existTariff);            }            return DescriptiveResponse<bool>.Success(true);        }        public DescriptiveResponse<bool> DeleteTariff(long tariffId)        {            var existtariff = this._contractTariffRepository.GetQuery()              .FirstOrDefault(a => a.ID == tariffId && !a.IsDeleted);

















            #region prepare model            existtariff.IsDeleted = true;            existtariff.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;            existtariff.UpdatedDate = DateTimeHelper.GetDateTimeNow();


            #endregion
            using (_unitofWork)            {                this._contractTariffRepository.Update(existtariff);            }            return DescriptiveResponse<bool>.Success(true);        }        public DescriptiveResponse<List<ContractTariffDTO>> AddTariffRange(List<ContractTariffDTO> tariffsDTO)        {
















            #region fetch Ids            foreach (var dto in tariffsDTO)            {                dto.CustomerLocationClassID = this.getCustomerClasssId(dto.CustomerClassName);                dto.ServiceTypeID = this.getServiceTypeId(dto.ServiceTypeName);                dto.StationID = this.getStationId(dto.StationName, dto.ContractID);                dto.ZoneID = this.getZoneId(dto.ZoneName);                dto.TanckerCapacityId = this.validateTanckerCapacityId(dto.TanckerCapacityId);            }


            #endregion
            var failedList = new List<ContractTariffDTO>();            var addList = new List<NWC_ContractTariff>();            foreach (var dto in tariffsDTO)            {
















                #region Validations                var validator = new ContractTariffValidator(ValidationMode.CreateLogic2, _contractTariffRepository, _contractRepository, tariffsDTO);                var results = validator.Validate(dto);


                #endregion
                if (!results.IsValid)                {                    var failures = results.Errors.Select(s => s.ErrorMessage);                    dto.ExcelValidation = string.Join(", ", failures);                    failedList.Add(dto);                }                else                {                    var newTariff = dto.WrapToTariff();

















                    #region prepare model                    newTariff.IsDeleted = false;                    newTariff.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;                    newTariff.CreatedDate = DateTimeHelper.GetDateTimeNow();


                    #endregion
                    addList.Add(newTariff);                }            }            using (_unitofWork)            {                this._contractTariffRepository.AddRange(addList);            }            return DescriptiveResponse<List<ContractTariffDTO>>.Success(failedList);        }































































        #endregion
        #endregion
        #region Station
        #region Query        public DescriptiveResponse<SearchResult<ContractStationListDTO>> SearchStationList(searchCriteriaContractStationDTO searchCriteria)        {            var stationDTOs = new List<ContractStationListDTO>();            int Count = 0;            try            {
















                #region Predicate                var predicate = PredicateBuilder.New<vw_NWC_ContractStations>(true);                predicate = predicate.And(s => s.ContractStationIsDeleted != true);                predicate = predicate.And(s => s.stationIsDeleted != true);                List<Guid> stationListIDs = this._ContractStationsRepository.GetQuery().                     Where(z => z.SubId == _loggedInUser.LoggedInUser.SubscriberId                     && (_loggedInUser.SubBranches.Any(a => a == z.branchId) || _loggedInUser.Branches.Any(a => a == z.branchId)))                     .Select(s => s.StationID)                    .ToList();                predicate = predicate.And(s => stationListIDs.Contains(s.StationID));                predicate = predicate.And(s => s.ContractID == searchCriteria.ContractStationDTO.ContractID);                IEnumerable<Guid> searchListBranch = new List<Guid>();
                //permitted Areas
                if (searchCriteria.ContractStationDTO.AreaIDs != null && searchCriteria.ContractStationDTO.AreaIDs.Any())                {                    searchListBranch = _loggedInUser.Branches.Intersect(searchCriteria.ContractStationDTO.AreaIDs);
                    //predicate = predicate.And(s => searchListBranch.Any(a => a ==s.branchId ));
                }
                //permitted cities
                if (searchCriteria.ContractStationDTO.CityIDs != null && searchCriteria.ContractStationDTO.CityIDs.Any())                {                    searchListBranch = searchListBranch.Concat(_loggedInUser.SubBranches.Intersect(searchCriteria.ContractStationDTO.CityIDs));                    predicate = predicate.And(s => searchListBranch.Any(a => a == s.branchId));                }

                //permitted stations
                if (searchCriteria.ContractStationDTO.StationIDs != null && searchCriteria.ContractStationDTO.StationIDs.Any())                {                    var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.ContractStationDTO.StationIDs);                    predicate = predicate.And(s => searchList.Any(a => (a == s.StationID)));                }

































                #endregion
                #region skip & take                var skip = 0;                var take = 10;                if (searchCriteria.PageFilter != null)                {                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;                    take = searchCriteria.PageFilter.PageSize;                }



                #endregion
                var spec = new Specification<vw_NWC_ContractStations>(predicate);                Count = this._ContractStationsRepository.Get(spec).Count();                var Stations = this._ContractStationsRepository.GetQuery()                    .Where(predicate)                    .OrderBy(s => s.stationName)                    .Skip(skip)                    .Take(take);                if (Stations != null)                    stationDTOs = Stations.ToList().Select(s => s.WrapToContractStationListDTO()).ToList();                return DescriptiveResponse<SearchResult<ContractStationListDTO>>.Success(                    new SearchResult<ContractStationListDTO>() { Result = stationDTOs, TotalCount = Count });            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => SearchStationList: "));                return DescriptiveResponse<SearchResult<ContractStationListDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);            }        }




        #endregion
        #region command
        public DescriptiveResponse<AddItemsResponse> AddStation(ContractStationDTO dto)        {            try            {
















                #region Validations                var validator = new ContractStationValidator(ValidationMode.Create);                var results = validator.Validate(dto);                if (!results.IsValid)                {                    var failures = results.Errors.Select(s => s.ErrorMessage);                    return DescriptiveResponse<AddItemsResponse>.Error(failures);                }
































                #endregion
                #region check contactPerson                 NWC_ContactPerson ContactPerson = null;                if (!string.IsNullOrEmpty(dto.ContactPerson.FirstName) ||                   !string.IsNullOrEmpty(dto.ContactPerson.LastName) ||                   !string.IsNullOrEmpty(dto.ContactPerson.Mobile) ||                   !string.IsNullOrEmpty(dto.ContactPerson.SecondName) ||                   !string.IsNullOrEmpty(dto.ContactPerson.Position) ||                   !string.IsNullOrEmpty(dto.ContactPerson.PersonAddressPostalCode) ||                   !string.IsNullOrEmpty(dto.ContactPerson.Email) ||                   !string.IsNullOrEmpty(dto.ContactPerson.LandlineNumber))                {                    ContactPerson = dto.ContactPerson.WrapToNWC_ContactPerson();                    ContactPerson.IsDeleted = false;                    ContactPerson.CreatedBy = _loggedInUser.LoggedInUser.StaffId;                    ContactPerson.CreatedDate = DateTimeHelper.GetDateTimeNow();                    ContactPerson.SubID = _loggedInUser.LoggedInUser.SubscriberId;                }


                #endregion
                var invalidStationCount = 0;                List<string> Message = new List<string>();                using (_unitofWork)                {                    List<NWC_ContractStations> ContractStationlist = new List<NWC_ContractStations>();                    List<NWC_ContractPrice> ContractPriceList = new List<NWC_ContractPrice>();                    List<NWC_ServiceType> serviceTypes = this._ServiceTypeRepository.GetQuery().                        Where(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId).ToList();                    List<NWC_CustomerLocationClass> CustomerLocationClassList = this._CustomerLocationClassRepository.GetQuery()                        .Where(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId).ToList();                    var validator2 = new ContractStationValidator(ValidationMode.CheckIfExist, this._ContractStationsTableRepository, this._ContractRepository);                    for (int i = 0; i < dto.StationIDs.Count; i++)                    {
                        //dto.index = i;
                        //var results2 = validator2.Validate(dto);

                        //if (!results2.IsValid)
                        //{
                        //    invalidStationCount++;

                        //    foreach (var er in results2.Errors)
                        //    {
                        //        if (Message.Where(m => m == er.ErrorMessage).FirstOrDefault() == null)
                        //            Message.Add(er.ErrorMessage);
                        //    };
                        //}
                        //else
                        {                            Guid stationID = dto.StationIDs[i];                            var ContractStation = new NWC_ContractStations                            {                                StationID = stationID,                                ContractID = dto.ContractID,                                IsDeleted = false,                                CreatedBy = _loggedInUser.LoggedInUser.StaffId,                                CreatedDate = DateTimeHelper.GetDateTimeNow()                            };                            ContractStation.NWC_ContactPerson = ContactPerson;                            ContractStationlist.Add(ContractStation);                            foreach (var serviceType in serviceTypes)                            {                                foreach (var CustomerLocationClass in CustomerLocationClassList)                                {                                    NWC_ContractPrice ContractPrice = new NWC_ContractPrice()                                    {                                        ContractID = dto.ContractID,                                        StationID = stationID,                                        CustomerLocationClassID = CustomerLocationClass.ID,                                        ServiceTypeID = serviceType.ID,                                        PriceCharge = 0,                                        DateFrom = DateTimeHelper.GetDateTimeNow(),                                        DateTo = DateTimeHelper.GetDateTimeNow(),                                        IsDeleted = false,                                        CreatedBy = _loggedInUser.LoggedInUser.StaffId,                                        CreatedDate = DateTimeHelper.GetDateTimeNow()                                    };                                    ContractPriceList.Add(ContractPrice);                                }                            }                        }                    }                    this._ContractStationsTableRepository.AddRange(ContractStationlist);                    this._ContractPriceRepository.AddRange(ContractPriceList);                }                AddItemsResponse ItemsResponse = new AddItemsResponse() { success = dto.StationIDs.Count - invalidStationCount, failed = invalidStationCount, message = Message };                return DescriptiveResponse<AddItemsResponse>.Success(ItemsResponse);            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => AddStation: "));                return DescriptiveResponse<AddItemsResponse>.Error(ErrorStatus.UNEXPECTED_ERROR);            }        }        public DescriptiveResponse<bool> UpdateStation(ContractStationDTO dto)        {            try            {
















                #region Validations                var validator = new ContractStationValidator(ValidationMode.Update, this._ContractStationsTableRepository);                var results = validator.Validate(dto);                if (!results.IsValid)                {                    var failures = results.Errors.Select(s => s.ErrorMessage);                    return DescriptiveResponse<bool>.Error(failures);                }


                #endregion
                using (_unitofWork)                {                    var station = this._ContractStationsTableRepository.FindById(dto.ContractStationID);



                    #region check contactPerson 
                    if (!string.IsNullOrEmpty(dto.ContactPerson.FirstName) ||
!string.IsNullOrEmpty(dto.ContactPerson.LastName) ||
!string.IsNullOrEmpty(dto.ContactPerson.Mobile) ||
!string.IsNullOrEmpty(dto.ContactPerson.SecondName) ||
!string.IsNullOrEmpty(dto.ContactPerson.Position) ||
!string.IsNullOrEmpty(dto.ContactPerson.PersonAddressPostalCode) ||
!string.IsNullOrEmpty(dto.ContactPerson.Email) ||
!string.IsNullOrEmpty(dto.ContactPerson.LandlineNumber))                    {                        if (dto.ContactPerson.ID != 0)                        {                            station.NWC_ContactPerson.FirstName = dto.ContactPerson.FirstName;                            station.NWC_ContactPerson.LastName = dto.ContactPerson.LastName;                            station.NWC_ContactPerson.SecondName = dto.ContactPerson.SecondName;                            station.NWC_ContactPerson.FullName = dto.ContactPerson.FirstName + " " + dto.ContactPerson.SecondName + " " + dto.ContactPerson.LastName;                            station.NWC_ContactPerson.Mobile = dto.ContactPerson.Mobile;                            station.NWC_ContactPerson.Position = dto.ContactPerson.Position;                            station.NWC_ContactPerson.PersonAddressPostalCode = dto.ContactPerson.PersonAddressPostalCode;                            station.NWC_ContactPerson.Email = dto.ContactPerson.Email;                            station.NWC_ContactPerson.LandlineNumber = dto.ContactPerson.LandlineNumber;                            station.NWC_ContactPerson.PersonalIDNumber = dto.ContactPerson.PersonalIDNumber;                            station.NWC_ContactPerson.PersonalIDType = dto.ContactPerson.PersonalIDType;                            station.NWC_ContactPerson.PersonAddress = dto.ContactPerson.PersonAddress;                            station.NWC_ContactPerson.UpdatedBy = _loggedInUser.LoggedInUser.StaffId;                            station.NWC_ContactPerson.UpdatedDate = DateTimeHelper.GetDateTimeNow();                        }                        else                        {                            station.NWC_ContactPerson = dto.ContactPerson.WrapToNWC_ContactPerson();                            station.NWC_ContactPerson.IsDeleted = false;                            station.NWC_ContactPerson.CreatedBy = _loggedInUser.LoggedInUser.StaffId;                            station.NWC_ContactPerson.CreatedDate = DateTimeHelper.GetDateTimeNow();                            station.NWC_ContactPerson.SubID = _loggedInUser.LoggedInUser.SubscriberId;                        }                    }                    else                    {                        station.ContactPersonID = null;                        station.NWC_ContactPerson = null;                    }


                    #endregion
                    station.StationID = dto.StationIDs[0];                    station.UpdatedBy = _loggedInUser.LoggedInUser.StaffId;                    station.UpdatedDate = DateTimeHelper.GetDateTimeNow();                    this._ContractStationsTableRepository.Update(station);                }                return DescriptiveResponse<Boolean>.Success(true);            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => UpdateStation: "));                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);            }        }        public DescriptiveResponse<bool> DeleteStation(ContractStationListDTO dto)        {            try            {                using (_unitofWork)                {                    var ContractStation = this._ContractStationsTableRepository.FindById(dto.contractStationID);                    var ContractAccessoryList = this._contractAccessoryRepository.GetQuery().Where(s => s.StationID == dto.StationID).ToList();                    var ContractTariffList = this._contractTariffRepository.GetQuery().Where(s => s.StationID == dto.StationID).ToList();                    var ContractStationViolationList = this._ContractStationViolationRepository.GetQuery().Where(s => s.StationID == dto.StationID).ToList();                    var ContractPriceList = this._ContractPriceRepository.GetQuery().Where(s => s.StationID == dto.StationID).ToList();                    if (ContractStation != null)                    {                        ContractStation.IsDeleted = true;                        this._ContractStationsTableRepository.Update(ContractStation);                        if (ContractAccessoryList != null && ContractAccessoryList.Any())                        {                            foreach (var ContractAccessory in ContractAccessoryList)                            {                                ContractAccessory.IsDeleted = true;                                this._contractAccessoryRepository.Update(ContractAccessory);                            }                        }                        if (ContractTariffList != null && ContractTariffList.Any())                        {                            foreach (var ContractTariff in ContractTariffList)                            {                                ContractTariff.IsDeleted = true;                                this._contractTariffRepository.Update(ContractTariff);                            }                        }                        if (ContractStationViolationList != null && ContractStationViolationList.Any())                        {                            foreach (var ContractStationViolation in ContractStationViolationList)                            {                                ContractStationViolation.IsDeleted = true;                                this._ContractStationViolationRepository.Update(ContractStationViolation);                            }                        }                        if (ContractPriceList != null && ContractPriceList.Any())                        {                            foreach (var ContractPrice in ContractPriceList)                            {                                ContractPrice.IsDeleted = true;                                this._ContractPriceRepository.Delete(ContractPrice);                            }                        }                    }                }                return DescriptiveResponse<Boolean>.Success(true);            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => DeleteStation: "));                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);            }        }






























































        #endregion
        #endregion
        #region Accessory
        #region Query        public DescriptiveResponse<SearchResult<ContractAccessoryDTO>> SearchContractAccessories(ContractAccessorySC searchCriteria)        {
















            #region Predicate            var predicate = PredicateBuilder.New<vw_NWC_ContractAccessory>(true);            predicate = predicate.And(s => s.ContractID == searchCriteria.ContractID);            predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);            predicate = predicate.And(s => s.IsContractDeleted != true);
            //predicate = predicate.And(s => s.IsContractTerminated != true);
            predicate = predicate.And(s => s.ContractStatusEnumId != (int)ContractStatusEnum.Terminated
&& s.ContractStatusEnumId != (int)ContractStatusEnum.Finished);            predicate = predicate.And(s => s.IsDeleted != true);
































            #endregion
            #region skip & take            var skip = 0;            var take = 20;            if (searchCriteria.PageFilter != null)            {                skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;                take = searchCriteria.PageFilter.PageSize;            }


            #endregion
            var contractAccQuery = (LanguageIsEnglish)
? this._vwContractAccessoryRepository.GetQuery()
.Where(predicate)
.OrderBy(s => s.StationName).ThenBy(s => s.AccessoryNameEn)
.Skip(skip).Take(take)
: this._vwContractAccessoryRepository.GetQuery()
.Where(predicate)
.OrderBy(s => s.StationName).ThenBy(s => s.AccessoryNameAr)
.Skip(skip).Take(take);

















            #region response            var result = new SearchResult<ContractAccessoryDTO>();            if (contractAccQuery != null && contractAccQuery.Any())            {                var count = this._vwContractAccessoryRepository.GetQuery().Count(predicate);                result.Result = contractAccQuery.AsEnumerable().Select(a => a.WrapToContractAccessory()).ToList();                result.TotalCount = count;            }            return DescriptiveResponse<SearchResult<ContractAccessoryDTO>>.Success(result);
















            #endregion        }
































        #endregion
        #region Command        public DescriptiveResponse<AddItemsResponse> AddContractAccessories(ContractAccessoryDTO contractAccessoryDTO)        {            try            {
















                #region Validations                var validator = new ContractAccessoryValidator(ValidationMode.Create, this._contractAccessoryRepository);                var results = validator.Validate(contractAccessoryDTO);                if (!results.IsValid)                {                    var failures = results.Errors.Select(s => s.ErrorMessage);                    return DescriptiveResponse<AddItemsResponse>.Error(failures);                }


                #endregion
                var contractAccessories = contractAccessoryDTO.WrapToContractAccessories(this._loggedInUser.LoggedInUser.StaffId);                var removedCount = 0;                var totalCount = contractAccessories.Count();



                #region Validations for each Record ==> Warnings
                for (int i = totalCount - 1; i >= 0; i--)                {                    var currentAccessory = contractAccessories[i];                    if (this._contractAccessoryRepository.GetQuery().Any(a =>                        !a.IsDeleted &&                        a.ContractID == currentAccessory.ContractID &&                        a.StationID == currentAccessory.StationID &&                        a.AccessoryID == currentAccessory.AccessoryID                    ))                    {                        contractAccessories.Remove(contractAccessories[i]);                        removedCount++;                    }                }



                #endregion

                using (_unitofWork)                {                    this._contractAccessoryRepository.AddRange(contractAccessories);                }                return DescriptiveResponse<AddItemsResponse>.Success(new AddItemsResponse                {                    failed = removedCount,                    success = totalCount - removedCount                });            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => AddContractAccessories: "));                return DescriptiveResponse<AddItemsResponse>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);            }        }        public DescriptiveResponse<long> UpdateContractAccessory(ContractAccessoryDTO contractAccessoryDTO)        {            try            {
                //var acc = this._contractAccessoryRepository.FindById(contractAccessoryDTO.ID);
                var acc = this._contractAccessoryRepository.GetQuery().FirstOrDefault(s =>
s.ID == contractAccessoryDTO.ID
&& s.IsDeleted != true
&& s.NWC_Contract.IsDeleted != true
&& s.NWC_Contract.SubID == this._loggedInUser.LoggedInUser.SubscriberId
&& s.NWC_Contract.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Terminated
&& s.NWC_Contract.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Finished);                var contractAccessory = contractAccessoryDTO.WrapToContractAccessory(acc, this._loggedInUser.LoggedInUser.StaffId);

















                #region Validations                var validator = new ContractAccessoryValidator(ValidationMode.Update, this._contractAccessoryRepository);                var results = validator.Validate(contractAccessoryDTO);                if (!results.IsValid)                {                    var failures = results.Errors.Select(s => s.ErrorMessage);                    return DescriptiveResponse<long>.Error(failures);                }


                #endregion
                using (_unitofWork)                {                    this._contractAccessoryRepository.Update(contractAccessory);                }                return DescriptiveResponse<long>.Success(contractAccessory.ID);            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => UpdateContractAccessory: "));                return DescriptiveResponse<long>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);            }        }        public DescriptiveResponse<bool> DeleteContractAccessory(long contractAccessoryID)        {            try            {                using (_unitofWork)                {
                    //var acc = this._contractAccessoryRepository.FindById(contractAccessoryID);
                    var acc = this._contractAccessoryRepository.GetQuery().FirstOrDefault(s =>
s.ID == contractAccessoryID
&& s.IsDeleted != true
&& s.NWC_Contract.IsDeleted != true
&& s.NWC_Contract.SubID == this._loggedInUser.LoggedInUser.SubscriberId
&& s.NWC_Contract.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Terminated
&& s.NWC_Contract.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Finished);                    acc.IsDeleted = true;                    acc.UpdatedDate = DateTimeHelper.GetDateTimeNow();                    acc.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;                    this._contractAccessoryRepository.Update(acc);                }                return DescriptiveResponse<bool>.Success(true);            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => DeleteContractAccessory: "));                return DescriptiveResponse<bool>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);            }        }

































































        #endregion
        #endregion
        #region Price
        #region query        public DescriptiveResponse<SearchResult<ContractPriceDTO>> GetContractPriceList(searchCriteriaContractDTO searchCriteria)        {            var result = new SearchResult<ContractPriceDTO>();            int Count = 0;            try            {
















                #region skip & take                var skip = 0;                var take = 10;                if (searchCriteria.PageFilter != null)                {                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;                    take = searchCriteria.PageFilter.PageSize;                }



                #endregion
                var permittedStation = _loggedInUser.UserLandmarksIds;                var contractPriceList = this._ContractPriceListRepository.GetQuery()                     .Where(cp => permittedStation.Contains(cp.StationID)                     && cp.ContractID == searchCriteria.ContractID && cp.ContractPriceIsDeleted != true);                if (contractPriceList != null && contractPriceList.Any())                {                    result.Result = contractPriceList.OrderBy(s => s.StationID).Skip(skip)                    .Take(take).ToList().Select(s => s.WrapToContractPriceDTO()).ToList();                    Count = contractPriceList.Count();                }                return DescriptiveResponse<SearchResult<ContractPriceDTO>>.Success(                    new SearchResult<ContractPriceDTO>() { Result = result.Result, TotalCount = Count });            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => GetContractPriceList: "));                return DescriptiveResponse<SearchResult<ContractPriceDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);            }        }

































        #endregion
        #region command        public DescriptiveResponse<bool> UpdatePriceList(List<ContractPriceDTO> ContractPriceList)        {            try            {
















                #region Validations                var validator = new ContractPriceValidator(ValidationMode.Update);                var results = validator.Validate(ContractPriceList);                if (!results.IsValid)                {                    var failures = results.Errors.Select(s => s.ErrorMessage);                    return DescriptiveResponse<bool>.Error(failures);                }


                #endregion
                using (_unitofWork)                {                    foreach (var contractPrice in ContractPriceList)                    {                        var _contractPrice = _ContractPriceRepository.FindById(contractPrice.ContractPriceID);                        _contractPrice.PriceCharge = contractPrice.PriceCharge;                        _contractPrice.UpdatedDate = DateTimeHelper.GetDateTimeNow();                        _contractPrice.UpdatedBy = _loggedInUser.LoggedInUser.StaffId;                        this._ContractPriceRepository.Update(_contractPrice);                    }                }                return DescriptiveResponse<Boolean>.Success(true);            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => UpdatePriceList: "));                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);            }        }






























































        #endregion
        #endregion
        #region Terms
        #region query        public DescriptiveResponse<SearchResult<vw_NWC_ContractTermsDTO>> GetContractTermsList(searchCriteriaContractDTO searchCriteria)        {            var ContractTermsDTOs = new List<vw_NWC_ContractTermsDTO>();            int Count = 0;            try            {
















                #region Predicate                var predicate = PredicateBuilder.New<vw_NWC_ContractTerms>(true);                predicate = predicate.And(s => s.ContractTermIsDeleted != true);
                //predicate = predicate.And(s => s.stationIsDeleted != true);
                //predicate = predicate.And(z => z.stationSubId == _loggedInUser.LoggedInUser.SubscriberId);
                predicate = predicate.And(z => z.TermsCategorySubID == _loggedInUser.LoggedInUser.SubscriberId);                predicate = predicate.And(z => _loggedInUser.UserLandmarksIds.Any(a => a == z.StationID));                predicate = predicate.And(s => s.ContractID == searchCriteria.ContractID);
































                #endregion
                #region skip & take                var skip = 0;                var take = 10;                if (searchCriteria.PageFilter != null)                {                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;                    take = searchCriteria.PageFilter.PageSize;                }

















                #endregion                var spec = new Specification<vw_NWC_ContractTerms>(predicate);                Count = this._ContractTermsView.Get(spec).Count();                var terms = this._ContractTermsView.GetQuery()                    .Where(predicate)                    .OrderBy(s => s.StationID)                    .Skip(skip)                    .Take(take);                if (terms != null && terms.Any())                    ContractTermsDTOs = terms.ToList().Select(s => s.WrapToContractTermsDTO()).ToList();                return DescriptiveResponse<SearchResult<vw_NWC_ContractTermsDTO>>.Success(                    new SearchResult<vw_NWC_ContractTermsDTO>() { Result = ContractTermsDTOs, TotalCount = Count });            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => GetContractTermsList: "));                return DescriptiveResponse<SearchResult<vw_NWC_ContractTermsDTO>>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);            }        }        public DescriptiveResponse<vw_NWC_ContractTermsDTO> GetTermValueUnit(long termId)        {            try            {                var termValueUnit = this._ContractTermsTableRepository.GetQuery()                        .Where(s => s.ID == termId)                        .Select(s => new vw_NWC_ContractTermsDTO                        {                            TotalValueUnitId = s.TotalValueUnitId,                            TotalValue = s.TotalValue,                            TotalValueUnitName = (LanguageIsEnglish ? s.NWC_TermsValueUnits.NameEn : s.NWC_TermsValueUnits.NameAr)                        })                        .FirstOrDefault();                return DescriptiveResponse<vw_NWC_ContractTermsDTO>.Success(termValueUnit);            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => GetTermValueUnit: "));                return DescriptiveResponse<vw_NWC_ContractTermsDTO>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);            }        }

































        #endregion
        #region command        public DescriptiveResponse<AddItemsResponse> AddTerm(ContractTermDTO dto)        {            try            {
















                #region Validations                var validator = new ContractTermsValidator(ValidationMode.Create, this._ContractTermsTableRepository);                var results = validator.Validate(dto);                var invalidStationCount = 0;                if (!results.IsValid)                {                    var failures = results.Errors.Select(s => s.ErrorMessage);                    return DescriptiveResponse<AddItemsResponse>.Error(failures);                }


                #endregion
                IEnumerable<string> Message = null;                using (_unitofWork)                {                    var validator2 = new ContractTermsValidator(ValidationMode.CheckIfExist, this._ContractTermsTableRepository);                    List<NWC_ContractTerms> ContractTermslist = new List<NWC_ContractTerms>();                    for (int i = 0; i < dto.StationIDs.Count; i++)                    {                        dto.index = i;                        var results2 = validator2.Validate(dto);                        if (!results2.IsValid)                        {                            invalidStationCount++;                            Message = results2.Errors.Select(s => s.ErrorMessage);                        }                        else                        {                            Guid? stationID = dto.StationIDs[i];                            var ContractTerm = new NWC_ContractTerms                            {                                StationID = stationID,                                ContractID = dto.ContractID,                                Description = dto.Description,                                Code = dto.ContractTermCode,                                TermsCategoryID = dto.TermsCategoryID,                                Name = dto.ContractTermName,                                IsDeleted = false,                                CreatedBy = _loggedInUser.LoggedInUser.StaffId,                                CreatedDate = DateTimeHelper.GetDateTimeNow(),                                TotalValue = dto.TotalValue,                                TotalValueUnitId = dto.TotalValueUnitId                            };                            ContractTermslist.Add(ContractTerm);                        }                    }                    this._ContractTermsTableRepository.AddRange(ContractTermslist);                }                AddItemsResponse ItemsResponse = new AddItemsResponse() { success = dto.StationIDs.Count - invalidStationCount, failed = invalidStationCount, message = Message };                return DescriptiveResponse<AddItemsResponse>.Success(ItemsResponse);            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => AddTerm: "));                return DescriptiveResponse<AddItemsResponse>.Error(ErrorStatus.UNEXPECTED_ERROR);            }        }        public DescriptiveResponse<bool> UpdateTerm(ContractTermDTO dto)        {            try            {
















                #region Validations                var validator = new ContractTermsValidator(ValidationMode.Update, this._ContractTermsTableRepository);                var results = validator.Validate(dto);                if (!results.IsValid)                {                    var failures = results.Errors.Select(s => s.ErrorMessage);                    return DescriptiveResponse<bool>.Error(failures);                }


                #endregion
                using (_unitofWork)                {                    var term = this._ContractTermsTableRepository.GetQuery().Where(s => s.ID == dto.ID).FirstOrDefault();                    term.UpdateBy = _loggedInUser.LoggedInUser.StaffId;                    term.UpdateDate = DateTimeHelper.GetDateTimeNow();                    term.StationID = dto.StationIDs[0];                    term.ContractID = dto.ContractID;                    term.Description = dto.Description;                    term.Code = dto.ContractTermCode;                    term.TermsCategoryID = dto.TermsCategoryID;                    term.Name = dto.ContractTermName;                    term.TotalValue = dto.TotalValue;                    term.TotalValueUnitId = dto.TotalValueUnitId;                    this._ContractTermsTableRepository.Update(term);                }                return DescriptiveResponse<bool>.Success(true);            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => UpdateTerm: "));                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);            }        }        public DescriptiveResponse<bool> DeleteTerm(long TermID)        {            try            {                using (_unitofWork)                {                    var term = this._ContractTermsTableRepository.GetQuery().Where(s => s.ID == TermID).FirstOrDefault();                    term.IsDeleted = true;                    this._ContractTermsTableRepository.Update(term);                }                return DescriptiveResponse<bool>.Success(true);            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => DeleteTerm: "));                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);            }        }































































        #endregion
        #endregion
        #region Terms Violations
        #region Query        public DescriptiveResponse<SearchResult<ContractTermsViolationsDTO>> GetContractViolations(ContractViolationSC searchCriteria)        {

            #region Predicate            var predicate = PredicateBuilder.New<vw_NWC_ContractTermsViolations>
               (s => s.ContractSubId == this._loggedInUser.LoggedInUser.SubscriberId
                   && s.IsDeleted != true);            if (searchCriteria.Id.HasValue)            {                var searchId = searchCriteria.Id.Value;                predicate = predicate.And(s => s.Id == searchId);            }



            #region lists Predicate
            //permitted stations
            if (searchCriteria.StationIDs != null && searchCriteria.StationIDs.Any())            {                var searchList = _loggedInUser.UserLandmarksIds.Intersect(searchCriteria.StationIDs);                predicate = predicate.And(s => searchList.Any(a => a == s.StationID));            }            else            {                predicate = predicate.And(s => _loggedInUser.UserLandmarksIds.Any(a => a == s.StationID));            }            if (searchCriteria.CategoryIds != null && searchCriteria.CategoryIds.Any())            {                predicate = predicate.And(s => searchCriteria.CategoryIds.Any(a => a == s.CategoryId));            }            if (searchCriteria.PaymentIds != null && searchCriteria.PaymentIds.Any())            {                predicate = predicate.And(s => searchCriteria.PaymentIds.Contains(s.PaymentStatusId));            }            if (searchCriteria.StatusIds != null && searchCriteria.StatusIds.Any())            {                predicate = predicate.And(s => searchCriteria.StatusIds.Any(a => a == s.StatusId));            }            if (searchCriteria.CancelReasonIds != null && searchCriteria.CancelReasonIds.Any())            {                predicate = predicate.And(s => searchCriteria.CancelReasonIds.Any(a => a == s.CancelReasonId));            }

            #endregion
            #region Date Predicate            if (searchCriteria.ViolationDateFrom.HasValue)            {                var from = searchCriteria.ViolationDateFrom.Value.Date; //from the start of the day
                predicate = predicate.And(s => s.IncidentTime >= from);            }            if (searchCriteria.ViolationDateTo.HasValue)            {                var to = searchCriteria.ViolationDateTo.Value.Date.AddDays(1); //to the start of the next day
                predicate = predicate.And(s => s.IncidentTime < to);            }




            #endregion
            #region Other Predicate
            if (!string.IsNullOrEmpty(searchCriteria.ViolationTicketNumber))            {                var searchText = searchCriteria.ViolationTicketNumber.Trim();                predicate = predicate.And(s => s.ViolationTicketNumber.Contains(searchText));            }            if (!string.IsNullOrEmpty(searchCriteria.Contract))            {                var searchText = searchCriteria.Contract.Trim();                predicate = predicate.And(s => s.ContractCode.Contains(searchText));            }            if (!string.IsNullOrEmpty(searchCriteria.Term))            {                var searchText = searchCriteria.Term.Trim();                predicate = predicate.And(s => s.ContractTermCode.Contains(searchText) || s.ContractTermName.Contains(searchText));            }            if (!string.IsNullOrEmpty(searchCriteria.Vehicle))            {                var searchText = searchCriteria.Vehicle.Trim();                predicate = predicate.And(s => s.VehiclePlateNo.Contains(searchText) || s.VehicleCode.Contains(searchText));            }            if (!string.IsNullOrEmpty(searchCriteria.Driver))            {                var searchDriver = searchCriteria.Driver.Trim();                var driverArr = searchCriteria.Driver.Trim().Split(' ');                if (driverArr.Length == 1)                {                    predicate = predicate.And(s => s.DriverFirstName.Contains(searchDriver)                                                || s.DriverMiddleName.Contains(searchDriver)                                                || s.DriverLastName.Contains(searchDriver)
                                                                                                                                            //|| s.DriverMobile.Contains(searchDriver)
                                                                                                                                            );                }                else                {                    predicate = predicate.And(s =>                            (s.DriverFirstName + " " + s.DriverMiddleName + " " + s.DriverLastName).Contains(searchDriver)                            || (s.DriverFirstName + " " + s.DriverLastName).Contains(searchDriver));                }            }





            //if (searchCriteria.IsDeleted.HasValue)
            //{
            //    var searchValue = searchCriteria.IsDeleted.Value;
            //    predicate = predicate.And(s => s.IsDeleted == searchValue);
            //}

            #endregion
            #endregion
            IQueryable<vw_NWC_ContractTermsViolations> workOrderList =
this._ViolationsView.GetQuery()
.Where(predicate)
.OrderByDescending(s => s.CreatedDate);            if (!searchCriteria.ExcelFlage)            {

                #region skip & take                var skip = 0;                var take = 10;                if (searchCriteria.PageFilter != null)                {                    skip = (searchCriteria.PageFilter.PageIndex - 1) * searchCriteria.PageFilter.PageSize;                    take = searchCriteria.PageFilter.PageSize;                }


                #endregion
                workOrderList = workOrderList
.Skip(skip)
.Take(take);            }

            #region response            var result = new SearchResult<ContractTermsViolationsDTO>();            if (workOrderList != null && workOrderList.Any())            {                var count = this._ViolationsView.GetQuery().Count(predicate);                result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToContractViolationsDTO()).ToList();                foreach (var oneResult in result.Result)                {                    oneResult.Show = HavePermmissionToDecide(oneResult.Id).Value;                }                result.TotalCount = count;            }            return DescriptiveResponse<SearchResult<ContractTermsViolationsDTO>>.Success(result);
            #endregion        }
        public DescriptiveResponse<SearchResult<ViolationApprovalsDTO>> GetContractApprovalViolation(ContractApprovalViolation searchCriteria)        {

            #region Predicate            var predicate = PredicateBuilder.New<vw_NWC_ViolationsApprovals>
               (s => s.IsDeleted != true);            if (searchCriteria != null && searchCriteria.LandmarkID.HasValue)            {                var searchId = searchCriteria.LandmarkID.Value;                predicate = predicate.And(s => s.LandmarkID == searchId);            }

            #endregion
            IQueryable<vw_NWC_ViolationsApprovals> vw_NWC_ViolationsApprovalList =
            this._vw_NWC_ViolationsApprovals.GetQuery()
            .Where(predicate);
            #region response            var result = new SearchResult<ViolationApprovalsDTO>();            if (vw_NWC_ViolationsApprovalList != null && vw_NWC_ViolationsApprovalList.Any())            {                var count = this._vw_NWC_ViolationsApprovals.GetQuery().Count(predicate);                result.Result = vw_NWC_ViolationsApprovalList.AsEnumerable().Select(a => a.WrapDTOToViolationApprovalsDTO()).ToList();            }            return DescriptiveResponse<SearchResult<ViolationApprovalsDTO>>.Success(result);
            #endregion        }

        public DescriptiveResponse<IEnumerable<AttachmentDTO>> GetContractViolationsAttachments(long violationId)        {            var contractAttachments = _ViolationAttachment.GetQuery()                .Where(a => a.ViolationID == violationId                            && !a.IsDeleted
                                                                                                              //&& a.NWC_ContractTermsViolations.IsDeleted != true
                                                                                                              && a.NWC_ContractTermsViolations.NWC_ContractTerms.NWC_Contract.SubID == _loggedInUser.LoggedInUser.SubscriberId)                .AsEnumerable()                .Select(a => a.WrapContractViolationAttachment());            return DescriptiveResponse<IEnumerable<AttachmentDTO>>.Success(contractAttachments);        }        public DescriptiveResponse<IEnumerable<ContractTermsViolationsLogsDTO>> GetContractViolationLogs(long violationId)        {            var logs = this._ContractTermsViolationsLogsView.GetQuery()                .Where(s => s.TermViolationId == violationId)                .AsEnumerable()                .Select(a => a.WrapToContractViolationsLogsDTO());            return DescriptiveResponse<IEnumerable<ContractTermsViolationsLogsDTO>>.Success(logs);        }
        public DescriptiveResponse<IEnumerable<ContractTermsApprovalViolationsLogsDTO>> GetContractApprovalViolationLogs(long violationId)        {            var logs = this._vw_NWC_ViolationsApprovalsLogs.GetQuery()                .Where(s => s.ViolationID == violationId)                .AsEnumerable()                .Select(a => a.WrapToContractApprovalViolationsLogsDTO());            return DescriptiveResponse<IEnumerable<ContractTermsApprovalViolationsLogsDTO>>.Success(logs);        }        public DescriptiveResponse<ContractTermsViolationsInVoiceDTO> GetTermViolationInvoice(long violationId)        {            var dbObject = this._ContractTermsViolationsInvoicesView.GetQuery()                .Where(s => s.ViolationId == violationId)                .FirstOrDefault();            if (dbObject != null)            {                var dto = dbObject.WrapToContractViolationsInvoiceDTO();                return DescriptiveResponse<ContractTermsViolationsInVoiceDTO>.Success(dto);            }            else            {                return DescriptiveResponse<ContractTermsViolationsInVoiceDTO>.Error(ErrorStatus.NOT_FOUNT);            }        }


        #endregion
        #region Command        public DescriptiveResponse<long?> AddContractViolation(ContractTermsViolationsDTO dto)        {

            #region Validations            var validator = new ContractTermViolationValidator(ValidationMode.Create, this._loggedInUser, this._ContractTermsTableRepository);            var results = validator.Validate(dto);            if (!results.IsValid)            {                var failures = results.Errors.Select(s => s.ErrorMessage);                return DescriptiveResponse<long?>.Error(failures);            }


            #endregion
            var violation = ContractWrapper.WrapToViolation(dto, RequireApproval((Guid)dto.StationID));

            #region prepare model            violation.IsDeleted = false;            violation.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;            violation.CreatedDate = DateTimeHelper.GetDateTimeNow();


            #endregion
            using (_unitofWork)            {                this._TermsViolations.Add(violation);            }
            #region Attachments            if (dto.ViolationAttachments != null && dto.ViolationAttachments.Count() > 0)            {                var attachments = new List<NWC_ContractViolationAttachment>();                foreach (var item in dto.ViolationAttachments)                {                    var newPath = Utilities.MoveFile(AttachmentType.ContractViolation, item.RelativePath, violation.Id);

                    #region new Attachment                    var newAttach = new NWC_ContractViolationAttachment
                                                               {
                                                                   ViolationID = violation.Id,
                                                                   RelativePath = newPath,
                                                                   DocumentName = item.DocumentName,
                                                                   IsDeleted = false,
                                                                   CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                                                                   CreatedDate = DateTimeHelper.GetDateTimeNow()
                                                               };



                    #endregion
                    attachments.Add(newAttach);                }                using (_unitofWork)                {                    this._ViolationAttachment.AddRange(attachments);                }            }

            #endregion
            #region Vehicle Blacklisted            if (violation.AddVehicleToBlacklist && violation.VehicleId.HasValue && violation.IsFinalDecision)            {                this.ChangeVehicleStatus(violation.VehicleId.Value, true);            }


            #endregion
            this.logContractTermViolation(violation.Id);            this.checkInvoiceToAdd_ContractTermViolation(violation.Id);            return DescriptiveResponse<long?>.Success(violation.Id);        }        public DescriptiveResponse<bool> EditContractViolation(ContractTermsViolationsDTO dto)        {

















            #region Validations            var validator = new ContractTermViolationValidator(ValidationMode.Update, this._loggedInUser, this._ContractTermsTableRepository);            var results = validator.Validate(dto);            if (!results.IsValid)            {                var failures = results.Errors.Select(s => s.ErrorMessage);                return DescriptiveResponse<bool>.Error(failures);            }


            #endregion
            var existViolation = this._TermsViolations.GetQuery()
.FirstOrDefault(a => a.Id == dto.Id
            && a.IsDeleted != true
            && a.NWC_ContractTerms.IsDeleted != true
            && a.NWC_ContractTerms.IsActive != false);            var wasBlackListed = existViolation.AddVehicleToBlacklist; //old DB Vehicle Blacklisted Value

            #region Map Models            //existViolation.ContractTermId = dto.ContractTermId;














            

existViolation.ViolationTicketNumber = dto.ViolationTicketNumber;            existViolation.ViolationLocation = dto.ViolationLocation;            existViolation.TotalPenalty = dto.TotalPenalty;            existViolation.IncidentTime = dto.IncidentTime;            existViolation.IssueDate = dto.IssueDate;            existViolation.PaymentDueDate = dto.PaymentDueDate;            existViolation.PaymentStatusId = dto.PaymentStatusId;            existViolation.PaymentStatusDate = dto.PaymentStatusDate;            existViolation.DriverId = dto.DriverId;            existViolation.VehicleId = dto.VehicleId;            existViolation.Description = dto.ViolationDescription;            existViolation.TermUnitNoOfDays = dto.TermUnitNoOfDays;            existViolation.AddVehicleToBlacklist = dto.AddVehicleToBlacklist;
            //existViolation.AddVehicleToBlacklist = (dto.AddVehicleToBlacklist ? true : existViolation.AddVehicleToBlacklist);
            existViolation.StatusId = dto.StatusId;            existViolation.CancelReasonId = dto.CancelReasonId;

































            #endregion
            #region prepare model            existViolation.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;            existViolation.UpdatedDate = DateTimeHelper.GetDateTimeNow();


            #endregion
            using (_unitofWork)            {                this._TermsViolations.Update(existViolation);            }

















            #region Attachments            if (dto.ViolationAttachments != null && dto.ViolationAttachments.Count() > 0)            {                using (_unitofWork)                {                    foreach (var item in dto.ViolationAttachments)                    {                        if ((item.ID == null || item.ID == 0) && !item.IsDeleted)                        {
















                            #region new Attachment                            var newPath = Utilities.MoveFile(AttachmentType.ContractViolation, item.RelativePath, existViolation.Id);                            var newAttach = new NWC_ContractViolationAttachment                            {                                ViolationID = existViolation.Id,                                RelativePath = newPath,                                DocumentName = item.DocumentName,                                IsDeleted = false,                                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,                                CreatedDate = DateTimeHelper.GetDateTimeNow()                            };                            this._ViolationAttachment.Add(newAttach);
















                            #endregion                        }                        else if (item.IsDeleted)                        {
















                            #region delete attachment                            var oldAttach = this._ViolationAttachment.GetQuery().FirstOrDefault(a => a.ID == item.ID && !a.IsDeleted);                            if (oldAttach != null)                            {                                oldAttach.IsDeleted = true;                                oldAttach.DeletedBy = this._loggedInUser.LoggedInUser.StaffId;                                oldAttach.DeletedDate = DateTimeHelper.GetDateTimeNow();                                this._ViolationAttachment.Update(oldAttach);                            }
















                            #endregion                        }                    }                }            }
































            #endregion
            #region Vehicle Blacklisted            if (dto.VehicleId.HasValue)            {                if (!wasBlackListed && dto.AddVehicleToBlacklist)                {                    this.ChangeVehicleStatus(dto.VehicleId.Value, true);                }                else if (wasBlackListed && !dto.AddVehicleToBlacklist)                {                    var flag = this.IsVehicleHaveOtherBlacklistedViolations(dto.VehicleId.Value, dto.Id);                    if (!flag)                    {
                        //this.RemoveVehicleBlacklistedMarkFromViolations(dto.VehicleId.Value);
                        this.ChangeVehicleStatus(dto.VehicleId.Value, false);                    }                }            }



            #endregion
            this.logContractTermViolation(dto.Id);            this.checkInvoiceToAdd_ContractTermViolation(dto.Id);            return DescriptiveResponse<bool>.Success(true);        }        public DescriptiveResponse<bool> DeleteContractViolation(long violationId)        {            var existViolation = this._TermsViolations.GetQuery()                .FirstOrDefault(a => a.Id == violationId                                     && a.IsDeleted != true                                     && a.NWC_ContractTerms.IsDeleted != true                                     && a.NWC_ContractTerms.IsActive != false);            using (_unitofWork)            {
















                #region prepare model                existViolation.IsDeleted = true;                existViolation.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;                existViolation.UpdatedDate = DateTimeHelper.GetDateTimeNow();
















                #endregion                this._TermsViolations.Update(existViolation);            }            this.logContractTermViolation(violationId);            return DescriptiveResponse<bool>.Success(true);        }
        public DescriptiveResponse<bool> DeleteViolationApproval(long violationApprovalId)        {            var existViolation = this._NWC_ViolationsApprovals.GetQuery()                .FirstOrDefault(a => a.ID == violationApprovalId                                     && a.IsDeleted != true);            using (_unitofWork)            {








                #region prepare model                existViolation.IsDeleted = true;                existViolation.LastModifiedBy = this._loggedInUser.LoggedInUser.StaffId;                existViolation.LastModifiedTime = DateTimeHelper.GetDateTimeNow();








                #endregion                this._NWC_ViolationsApprovals.Update(existViolation);            }            this._NWC_ViolationsApprovals.Update(existViolation);            return DescriptiveResponse<bool>.Success(true);        }        private bool logContractTermViolation(long violationId)        {            try            {                using (this._unitofWork)                {                    var violation = this._TermsViolations.FindById(violationId);                    var attachementObj = this._ViolationAttachment.GetQuery()                        .Where(s => s.ViolationID == violationId && !s.IsDeleted)                        .Select(s => new                        {                            s.ID,                            s.DocumentName                        });                    var newLog = new NWC_ContractTermsViolationsLogs                    {                        TermViolationId = violation.Id,                        ContractTermId = violation.ContractTermId,                        ViolationTicketNumber = violation.ViolationTicketNumber,                        ViolationLocation = violation.ViolationLocation,                        IncidentTime = violation.IncidentTime,                        TotalPenalty = violation.TotalPenalty,                        IssueDate = violation.IssueDate,                        PaymentDueDate = violation.PaymentDueDate,                        PaymentStatusId = violation.PaymentStatusId,                        PaymentStatusDate = violation.PaymentStatusDate,                        VehicleId = violation.VehicleId,                        DriverId = violation.DriverId,                        Description = violation.Description,                        TermUnitNoOfDays = violation.TermUnitNoOfDays,                        AddVehicleToBlacklist = violation.AddVehicleToBlacklist,                        IsDeleted = violation.IsDeleted,                        StatusId = violation.StatusId,                        CancelReasonId = violation.CancelReasonId,                    };                    if (attachementObj.Any())                    {                        newLog.AttachementsDocumentsIds = string.Join(", ", attachementObj.Select(s => s.ID));                        newLog.AttachementsDocumentsNames = string.Join(", ", attachementObj.Select(s => s.DocumentName));                    }                    newLog.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;                    newLog.CreatedDate = DateTimeHelper.GetDateTimeNow();                    this._ContractTermsViolationsLogs.Add(newLog);                }                return true;            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => logContractTermViolation: "));                return false;            }        }        private void checkInvoiceToAdd_ContractTermViolation(long violationId)        {            try            {                var invoiceExist = this._ContractTermsViolationsInvoices.GetQuery()                    .Where(s => s.ViolationId == violationId)                    .Any();                if (invoiceExist)                {                    return;                }                else                {                    var violation = this._TermsViolations.GetQuery()                            .Where(s => s.Id == violationId)                            .Select(a => new { a.TotalPenalty, a.ViolationTicketNumber, a.PaymentStatusId })                            .FirstOrDefault();                    if (violation.PaymentStatusId == 1) //paid 
                    {                        using (this._unitofWork)                        {                            var newInvoice = new NWC_ContractTermsViolationsInvoices                            {                                ViolationId = violationId,                                ViolationNo = violationId.ToString(),                                InvoiceNo = this.getNextValue_ContractTermViolationInvoice(),                                Value = violation.TotalPenalty,                                VAT = 0,                                ValueWithVAT = violation.TotalPenalty,                                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,                                CreatedDate = DateTimeHelper.GetDateTimeNow()                            };                            this._ContractTermsViolationsInvoices.Add(newInvoice);                        }                    }                }            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => checkInvoiceToAdd_ContractTermViolation: "));            }        }        public DescriptiveResponse<bool> HavePermmissionToDecide(long violationId)        {            try            {                var Violation = this._ViolationsView.GetQuery()               .Where(s => s.Id == violationId && !s.IsFinalDecision && s.StatusId == 1)               .SingleOrDefault();                if (Violation == null)                {                    return DescriptiveResponse<bool>.Success(false);                }                var CurrentPermmision = _NWC_ViolationsApprovals.GetQuery().Where(x => x.StaffID == this._loggedInUser.LoggedInUser.StaffId                && x.LandmarkID == Violation.StationID                && x.IsDeleted != true                && x.LevelID == Violation.CurrentApprovalStatus).FirstOrDefault();                return DescriptiveResponse<bool>.Success(CurrentPermmision != null);            }            catch (Exception ex)            {                return DescriptiveResponse<bool>.Error(ex.ToString());            }        }        public DescriptiveResponse<bool> AddViolationDecision(long violationId, bool Approval)        {            try            {                var Violation = this._ViolationsView.GetQuery()                    .Where(s => s.Id == violationId)                    .SingleOrDefault();

















                #region "validation"                if (Violation == null || Violation.StationID == null)                {                    LoggerManager.LogMsg(c => c.Log(string.Format("ContractService => AddViolationDecision:{0}", "Violation number is not right")));                    return DescriptiveResponse<bool>.Error(ErrorStatus.NOT_FOUNT);                }                else                {                    var Station = this._Landmark.GetQuery()                     .Where(s => s.Id == Violation.StationID)                     .SingleOrDefault();                    if (Station.Branch.NWC_BranchSetting.ValidationApprovalRequired != null && Station.Branch.NWC_BranchSetting.ValidationApprovalRequired != true)                    {                        LoggerManager.LogMsg(c => c.Log(string.Format("ContractService => AddViolationDecision:{0}", "Approval is not required")));                        return DescriptiveResponse<bool>.Error(ErrorStatus.NOT_FOUNT);                    }                    var CurrentPermmision = _NWC_ViolationsApprovals.GetQuery().Where(x => x.StaffID == this._loggedInUser.LoggedInUser.StaffId                    && x.LandmarkID == Station.Id                    && x.IsDeleted != true                    && x.LevelID == Violation.CurrentApprovalStatus).FirstOrDefault();                    if (CurrentPermmision == null)                    {                        LoggerManager.LogMsg(c => c.Log(string.Format("ContractService => AddViolationDecision:{0}", "User does not have a permission")));                        return DescriptiveResponse<bool>.Error(ErrorStatus.NOT_FOUNT);                    }
































                    #endregion
                    #region "Excecute"                    using (this._unitofWork)                    {                        var UpdatedViolation = this._TermsViolations.GetQuery()                        .Where(s => s.Id == violationId)                        .SingleOrDefault();

                        UpdatedViolation.LastDecisionDate = DateTime.Now;                        UpdatedViolation.LastDecisionBy = this._loggedInUser.LoggedInUser.StaffId;                        UpdatedViolation.StatusId = Approval ? 1 : 3;                        if (CurrentPermmision.LevelID == Station.Branch.NWC_BranchSetting.ApprovalLevelsCount || Approval == false)                        {                            UpdatedViolation.IsFinalDecision = true;                            if (Approval && UpdatedViolation.AddVehicleToBlacklist && UpdatedViolation.VehicleId != null)                            {                                this.ChangeVehicleStatus(UpdatedViolation.VehicleId.Value, true);                            }
                        }                        else                        {                            UpdatedViolation.IsFinalDecision = false;                            UpdatedViolation.CurrentApprovalStatus = Violation.CurrentApprovalStatus + 1;                        }

                        this._TermsViolations.Update(UpdatedViolation);                    }

































                    #endregion
                    #region "Log"                    using (this._unitofWork)                    {                        var NewLog = new NWC_ViolationsApprovalsLogs                        {
                            ViolationID = violationId,
                            DecisionType = Approval ? 1 : 0,
                            AprovalID = CurrentPermmision.ID,
                            CreateTime = DateTime.Now,                            CreatedBy = this._loggedInUser.LoggedInUser.StaffId                        };                        this._NWC_ViolationsApprovalsLogs.Add(NewLog);                    }
















                    #endregion                    return DescriptiveResponse<bool>.Success(true);                }            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(string.Format("ContractService => AddViolationDecision:{0}", ex)));                return DescriptiveResponse<bool>.Error(ErrorStatus.NOT_FOUNT);            }        }        private string getNextValue_ContractTermViolationInvoice()        {            using (var cc = new NWCContext())            {                var seq = cc.sp_NWC_SeqNextValue_ContractTermViolationInvoice();                var next = seq.Single().Value;                return next.ToString("D10");            }        }

































































        #endregion
        #endregion
        #region helpers
        #region Gets        private int GetContractStatusId(DateTime startDate, DateTime endDate, bool IsTerminated = false)        {            int enumId = IsTerminated                         ? (int)ContractStatusEnum.Terminated                         : (int)GetContractStatusEnum(startDate, endDate);            var status = this._ContractStatusRepository.GetQuery()                             .FirstOrDefault(s => s.EnumId == enumId                                                  && s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);            return status.ID;        }        private int GetContractStatusId(int enumId)        {            var status = this._ContractStatusRepository.GetQuery()                             .FirstOrDefault(s => s.EnumId == enumId                                                  && s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);            return status.ID;        }        private ContractStatusEnum GetContractStatusEnum(DateTime startDate, DateTime endDate)        {            var today = DateTimeHelper.GetDateTimeNow();            if (startDate.Date > today.Date)                return ContractStatusEnum.New;            else if (endDate.Date < today.Date)                return ContractStatusEnum.Finished;            else                return ContractStatusEnum.Active;        }        private List<LookUpDTO<int>> _CustomerClasses;        private int getCustomerClasssId(string name)        {            if (string.IsNullOrEmpty(name))                return 0;            if (_CustomerClasses == null || !_CustomerClasses.Any())            {                _CustomerClasses = this._CustomerLocationClassRepository.GetQuery()                                    .Where(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId)                                    .Select(s => new LookUpDTO<int>                                    {                                        Id = s.ID,                                        Name = s.NameAr,                                        IntegrationId = s.IntegrationId                                    }).ToList();                ;            }

            //var searchText = name.Trim();
            //var customerClass = _CustomerClasses.FirstOrDefault(s => s.Name.Contains(searchText));
            var customerClass = _CustomerClasses.FirstOrDefault(s => s.Name == name);            return customerClass == null ? 0 : customerClass.Id;        }        private List<LookUpDTO<int>> _ServiceTypes;        private int getServiceTypeId(string name)        {            if (string.IsNullOrEmpty(name))                return 0;            if (_ServiceTypes == null || !_ServiceTypes.Any())            {                _ServiceTypes = this._ServiceTypeRepository.GetQuery()                                    .Where(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId)                                    .Select(s => new LookUpDTO<int>                                    {                                        Id = s.ID,                                        Name = s.TypeAr,                                        IntegrationId = s.IntegrationId                                    }).ToList();                ;            }

            //var searchText = name.Trim();
            //var serviceType = _ServiceTypes.FirstOrDefault(s => s.Name.Contains(searchText));
            var serviceType = _ServiceTypes.FirstOrDefault(s => s.Name == name);            return serviceType == null ? 0 : serviceType.Id;        }        private List<int> _TanckerCapacities;        private int? validateTanckerCapacityId(int? capacity)        {            if (capacity == null)                return null;            if (this._TanckerCapacities == null || !this._TanckerCapacities.Any())            {                this._TanckerCapacities = this._tankerCapacityRepository.GetQuery()                                    .Select(s => s.Capacity).ToList();            }            var index = this._TanckerCapacities.FindIndex(s => s == capacity);            return (index == -1) ? 0 : capacity;        }        private Guid getStationId(string name, long contractId)        {            if (string.IsNullOrEmpty(name))                return Guid.Empty;

            //var searchText = name.Trim();

            var stations = this._ContractStationsRepository.GetQuery()
.Where(a =>
a.ContractID == contractId
&& a.stationIsDeleted != true
&& a.ContractStationIsDeleted != true
//&& a.stationName.Contains(searchText)
                                                  && a.stationName == name
)
.Select(s => s.StationID)
.ToList();            return (stations == null) || (stations.Count() < 1) ? Guid.Empty : stations[0];        }        private long? getZoneId(string name)        {            if (string.IsNullOrEmpty(name))                return null;

            //var searchText = name.Trim();

            var zones = this._ZoneRepository.GetQuery()
.Where(s => (s.SubID == this._loggedInUser.LoggedInUser.SubscriberId)
&& s.IsDeleted != true
&& s.Name == name
//&& s.Name.Contains(searchText)
                                                           )
.Select(s => s.ID)
.ToList();            return (zones == null) || (zones.Count() < 1) ? 0 : zones[0];        }





        #endregion

        #region Vehicle
        private bool ChangeVehicleStatus(Guid VehicleId, bool Blacklisted)        {            try            {                using (_unitofWork)                {                    var vehicle = this._TransporterRepository.GetQuery()                        .Where(s => s.ID == VehicleId                                    && s.isDeleted != true                                    && s.SubID == this._loggedInUser.LoggedInUser.SubscriberId)                        .FirstOrDefault();                    vehicle.status = (Blacklisted ? 12 : 13); // Blacklisted, Parking

                    this._TransporterRepository.Update(vehicle);                }                return true;            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => ChangeVehicleStatus: "));                return false;            }        }        private bool IsVehicleHaveOtherBlacklistedViolations(Guid VehicleId, long violationId)        {            return this._TermsViolations.GetQuery()                .Where(s => s.VehicleId == VehicleId                            && s.Id != violationId                            && s.AddVehicleToBlacklist == true                            && s.NWC_WorkOrderInvoiceStatus.ID != 1  //paid                            && s.IsDeleted != true                            && s.NWC_ContractTerms.IsDeleted != true                            && s.NWC_ContractTerms.IsActive != false                            )                .Any();        }        private bool RemoveVehicleBlacklistedMarkFromViolations(Guid VehicleId)        {            try            {                using (_unitofWork)                {                    var violations = this._TermsViolations.GetQuery()                    .Where(s => s.VehicleId == VehicleId
                                                                                          //&& s.Id != violationId
                                                                                          && s.AddVehicleToBlacklist == true
                                                                                                                   //&& s.NWC_WorkOrderInvoiceStatus.ID != 1  //paid
                                                                                                                   && s.IsDeleted != true                                && s.NWC_ContractTerms.IsDeleted != true                                && s.NWC_ContractTerms.IsActive != false                                )                    .ToList();                    foreach (var item in violations)                    {

                        //var m = this._TermsViolations.GetQuery().Where(s => s.Id == 4).FirstOrDefault();
                        //m.AddVehicleToBlacklist = false;
                        //this._TermsViolations.Update(m);
                        item.AddVehicleToBlacklist = false;                        this._TermsViolations.Update(item);                    }                }                return true;            }            catch (Exception ex)            {                LoggerManager.LogMsg(c => c.Log(ex, "ContractService => RemoveVehicleBlacklistedMarkFromViolations: "));                return false;            }        }

































        #endregion
        #region Helper        private static bool LanguageIsEnglish        {            get            {                return Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;            }        }        enum TarrifStatusEnum        {            all = 1,            valid = 2,            notValid = 3        }        private bool RequireApproval(Guid stationID)        {            var Station = this._Landmark.GetQuery().Where(s => s.Id == stationID).SingleOrDefault();
            if (Station == null || Station.Branch == null || Station.Branch.NWC_BranchSetting == null || Station.Branch.NWC_BranchSetting.ValidationApprovalRequired == null)            {                return false;            }            else            {                return (bool)Station.Branch.NWC_BranchSetting.ValidationApprovalRequired;            }        }

































        #endregion
        #endregion    }}