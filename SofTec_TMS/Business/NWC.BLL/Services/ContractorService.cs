using Infrastructure;
using LinqKit;
using NWC.BLL.Interfaces;
using NWC.BLL.Validators;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace NWC.BLL.Services
{
    public class ContractorService : IContractorService
    {
        #region Properties
        private readonly ILoggedInUserService _loggedInUser;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<NWC_Contractor> _contractorRepository;
        private readonly IRepository<vw_NWC_ContractorList> _contractorListRepository;
        private readonly IRepository<NWC_ContactPerson> _contactPersonRepository;
        private readonly IRepository<NWC_ContractorAttachment> _ContractorAttachment;
        #endregion

        #region Constructors
        public ContractorService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._contractorRepository = new Repository<NWC_Contractor>(ctx);
            this._contractorListRepository = new Repository<vw_NWC_ContractorList>(ctx);
            this._contactPersonRepository = new Repository<NWC_ContactPerson>(ctx);
            this._ContractorAttachment = new Repository<NWC_ContractorAttachment>(ctx);
        }
        #endregion

        #region Query
        public IEnumerable<LookUpDTO<long>> GetLite(string searchText, out ReturnResult result)
        {
            var contractorsDTOs = new List<LookUpDTO<long>>();
            try
            {
                using (_unitofWork)
                {
                    var search = searchText.Trim().ToLower();
                    var contractors = this._contractorRepository.GetQuery()
                                .Where(s => s.ContractorFullName.Contains(search) || s.Code.Contains(search))
                                .Take(10).Select(x => new LookUpDTO<long>
                                {
                                    Id = x.ID,
                                    Name = x.ContractorFullName + " / " + x.Code
                                });

                    if (contractors != null)
                        contractorsDTOs = contractors.ToList();
                }
                result = new ReturnResult() { ResultCode = ReturnResultCode.Success };
            }
            catch (Exception ex)
            {
                result = new ReturnResult() { ResultCode = ReturnResultCode.Exception, ResultMessage = ex.Message };
            }
            return contractorsDTOs;
        }

        public DescriptiveResponse<SearchResult<ContractorDTO>> SearchContractorList(ContractorSearchCriteriaDTO searchCriteria)
        {

            //test------------------------------------------------------------------------------------------------
            var text = $"fire from (Contractor-SearchContractorList) service-Lang: {this._loggedInUser.LoggedInUser.Lang}, thread lang: {Thread.CurrentThread.CurrentCulture.Name}";
            LoggerManager.LogMsg(c => c.TrackingMsg(text));
            //ExceptionManager.GetExceptionLogger().LogInformation(text);
            //----------------------------------------------------------------------------------------------------


            #region Predicate
            var predicate = PredicateBuilder.New<vw_NWC_ContractorList>(true);
            predicate = predicate.And(s => s.IsDeleted == false);
            predicate = predicate.And(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);
            predicate = predicate.And(s => _loggedInUser.SubBranches.Contains(s.CompanyAddressCityID));


            if (!string.IsNullOrEmpty(searchCriteria.FilterModel?.SearchKeyword))
            {
                var word = searchCriteria.FilterModel.SearchKeyword.Trim();
                predicate = predicate.And(s => s.Code.Contains(word) || s.ContractorFullName.Contains(word));
            }

            if (searchCriteria.Id.HasValue)
            {
                predicate = predicate.And(s => s.ID == searchCriteria.Id.Value);
            }


            #region Advanced search
            if (!string.IsNullOrEmpty(searchCriteria.CommercialIDNumber))
            {
                var searchCommericalID = searchCriteria.CommercialIDNumber.Trim();
                predicate = predicate.And(s => s.CommercialIDNumber.Contains(searchCommericalID));
            }
            if (!string.IsNullOrEmpty(searchCriteria.TaxNumber))
            {
                var searchTax = searchCriteria.TaxNumber.Trim();
                predicate = predicate.And(s => s.TaxNumber.Contains(searchTax));
            }
            if (!string.IsNullOrEmpty(searchCriteria.MOI))
            {
                var searchMOI = searchCriteria.MOI.Trim();
                predicate = predicate.And(s => s.MOI.Contains(searchMOI));
            }
            //if (!string.IsNullOrEmpty(searchCriteria.ContactPersonName))
            //{
            //    var searchName = searchCriteria.ContactPersonName.Trim();
            //    predicate = predicate.And(s => s.FullName.Contains(searchName));
            //} 
            #endregion

            #endregion

            #region Sort
            //Expression<Func<NWC_Contractor, object>> sort;
            //switch (searchCriteria.OrderBy)
            //{
            //    case ContractorSearchCriteriaDTO.OrderByExepression.Code:
            //        sort = x => x.Code;
            //        break;
            //    case ContractorSearchCriteriaDTO.OrderByExepression.CommericalID:
            //        sort = x => x.CommercialIDNumber;
            //        break;
            //    case ContractorSearchCriteriaDTO.OrderByExepression.MOI:
            //        sort = x => x.MOI;
            //        break;
            //    case ContractorSearchCriteriaDTO.OrderByExepression.TaxNumber:
            //        sort = x => x.TaxNumber;
            //        break;
            //    //case ContractorSearchCriteriaDTO.OrderByExepression.ContactPerson:
            //    //    sort = x => x.FullName;
            //    //    break;
            //    case ContractorSearchCriteriaDTO.OrderByExepression.ContarctorName:
            //    default:
            //        sort = x => x.ContractorFullName;
            //        break;
            //}
            #endregion

            #region skip & take
            var skip = 0;
            var take = 20;
            if (searchCriteria.FilterModel != null && searchCriteria.FilterModel.PageFilter != null)
            {
                skip = (searchCriteria.FilterModel.PageFilter.PageIndex - 1) * searchCriteria.FilterModel.PageFilter.PageSize;
                take = searchCriteria.FilterModel.PageFilter.PageSize;
            }
            #endregion

            var contractorQuery = this._contractorListRepository.GetQuery()
               .Where(predicate)
               .OrderBy(s => s.ContractorFullName)
               .Skip(skip)
               .Take(take);

            #region response
            var result = new SearchResult<ContractorDTO>();
            if (contractorQuery != null && contractorQuery.Any())
            {
                var count = this._contractorListRepository.GetQuery().Count(predicate);
                result.Result = contractorQuery.AsEnumerable().Select(a => a.WrapToContractorDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<ContractorDTO>>.Success(result);
            #endregion

        }

        public DescriptiveResponse<IEnumerable<AttachmentDTO>> GetContractorAttachments(long contractorId)
        {
            var contractAttachments = _ContractorAttachment.GetQuery()
                .Where(a => a.ContractorID == contractorId && !a.IsDeleted && a.NWC_Contractor.IsDeleted != true
                            && a.NWC_Contractor.SubID == _loggedInUser.LoggedInUser.SubscriberId
                            && _loggedInUser.SubBranches.Contains(a.NWC_Contractor.CompanyAddressCityID))
                .AsEnumerable()
                .Select(a => a.WrapContractorAttachment());

            return DescriptiveResponse<IEnumerable<AttachmentDTO>>.Success(contractAttachments);
        }
        #endregion

        #region Commands
        public DescriptiveResponse<long?> AddContractor(ContractorDTO dto)
        {
            //test------------------------------------------------------------------------------------------------
            var text = $"fire from (Contractor-AddContractor) service-Lang: {this._loggedInUser.LoggedInUser.Lang}, thread lang: {Thread.CurrentThread.CurrentCulture.Name}";
            LoggerManager.LogMsg(c => c.TrackingMsg(text));
            //ExceptionManager.GetExceptionLogger().LogInformation(text);
            //----------------------------------------------------------------------------------------------------

            #region Validations
            var validator = new ContractorValidator(ValidationMode.Create, this._loggedInUser, _contractorRepository);
            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                var failures = results.Errors.Select(s => s.ErrorMessage);
                return DescriptiveResponse<long?>.Error(failures);
            }
            #endregion

            var contractor = dto.WrapToContractor();
            var person = dto.WrapToContactperson();

            #region prepare model
            contractor.IsActive = true;
            contractor.IsBlackListed = false;
            contractor.IsDeleted = false;
            contractor.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
            contractor.CreatedDate = DateTimeHelper.GetDateTimeNow();
            contractor.SubID = _loggedInUser.LoggedInUser.SubscriberId;

            if (person != null)
            {
                person.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
                person.CreatedDate = DateTimeHelper.GetDateTimeNow();
                person.SubID = _loggedInUser.LoggedInUser.SubscriberId;

                contractor.NWC_ContactPerson = person;
            }
            #endregion

            using (_unitofWork)
            {
                this._contractorRepository.Add(contractor);
            }

            #region Attachments
            if (dto.ContractorAttachments != null && dto.ContractorAttachments.Count() > 0)
            {
                var attachments = new List<NWC_ContractorAttachment>();

                foreach (var item in dto.ContractorAttachments)
                {
                    var newPath = Utilities.MoveFile(AttachmentType.Contractor, item.RelativePath, contractor.ID);

                    #region new Attachment
                    var newAttach = new NWC_ContractorAttachment
                    {
                        ContractorID = contractor.ID,
                        RelativePath = newPath,
                        DocumentName = item.DocumentName,
                        IsDeleted = false,
                        CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                        CreatedDate = DateTimeHelper.GetDateTimeNow()
                    };

                    #endregion

                    attachments.Add(newAttach);
                }

                using (_unitofWork)
                {
                    this._ContractorAttachment.AddRange(attachments);
                }
            }
            #endregion

            return DescriptiveResponse<long?>.Success(contractor.ID);
        }

        public DescriptiveResponse<bool> EditContractor(ContractorDTO dto)
        {
            //test------------------------------------------------------------------------------------------------
            var text = $"fire from (Contractor-EditContractor) service-Lang: {this._loggedInUser.LoggedInUser.Lang}, thread lang: {Thread.CurrentThread.CurrentCulture.Name}";
            LoggerManager.LogMsg(c => c.TrackingMsg(text));
            //ExceptionManager.GetExceptionLogger().LogInformation(text);
            //----------------------------------------------------------------------------------------------------

            #region Validations
            var validator = new ContractorValidator(ValidationMode.Update, this._loggedInUser, _contractorRepository );
            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                var failures = results.Errors.Select(s => s.ErrorMessage);
                return DescriptiveResponse<bool>.Error(failures);
            }
            #endregion


            var existContractor = this._contractorRepository.GetQuery()
               .FirstOrDefault(a => a.ID == dto.ID
                                    && a.IsDeleted != true
                                    && a.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                    && _loggedInUser.SubBranches.Contains(a.CompanyAddressCityID));

            #region Map Models
            existContractor.Code = dto.Code;
            existContractor.ContractorFullName = dto.ContractorFullName;
            existContractor.CommercialIDNumber = dto.CommercialIDNumber;
            existContractor.TaxNumber = dto.TaxNumber;
            existContractor.MOI = dto.MOI;
            existContractor.CompanyAddressCityID = dto.CompanyAddressCityID;
            existContractor.CompanyAddressPostalCode = dto.CompanyAddressPostalCode;
            existContractor.CompanyAddress = dto.CompanyAddress;
            existContractor.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;
            existContractor.UpdatedDate = DateTimeHelper.GetDateTimeNow();


            #endregion

            NWC_ContactPerson newPerson = null;
            using (_unitofWork)
            {
                if (!existContractor.ContactPersonId.HasValue && !string.IsNullOrEmpty(dto.FirstName))
                {
                    newPerson = dto.WrapToContactperson();

                    newPerson.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
                    newPerson.CreatedDate = DateTimeHelper.GetDateTimeNow();
                    newPerson.SubID = _loggedInUser.LoggedInUser.SubscriberId;

                    this._contactPersonRepository.Add(newPerson);
                }
            }

            using (_unitofWork)
            {
                long? deleteId = null;
                if (newPerson != null)
                {
                    existContractor.ContactPersonId = newPerson.ID;
                }
                else if (string.IsNullOrEmpty(dto.FirstName)) //deattach for be able to delete
                {
                    deleteId = existContractor.ContactPersonId;
                    existContractor.ContactPersonId = null; 
                }

                this._contractorRepository.Update(existContractor);


                if (newPerson == null)
                {
                    long searchId = existContractor.ContactPersonId.HasValue
                                        ? existContractor.ContactPersonId.Value
                                        : deleteId.HasValue ? deleteId.Value : 0;
                    var existPerson =  this._contactPersonRepository.FindById(searchId);

                    if (existPerson != null)
                    {
                        if (!string.IsNullOrEmpty(dto.FirstName))
                        {
                            existPerson.FirstName = dto.FirstName;
                            existPerson.SecondName = dto.SecondName;
                            existPerson.LastName = dto.LastName;

                            existPerson.FullName = string.IsNullOrEmpty(existPerson.SecondName)
                                                   ? $"{existPerson.FirstName} {existPerson.LastName}"
                                                   : $"{existPerson.FirstName} {existPerson.SecondName} {existPerson.LastName}";

                            existPerson.Position = dto.Position;
                            existPerson.MobileCode = dto.MobileCode;
                            existPerson.Mobile = dto.Mobile;
                            existPerson.LandlineNumbeCode = dto.LandlineNumbeCode;
                            existPerson.LandlineNumber = dto.LandlineNumber;
                            existPerson.Email = dto.Email;
                            existPerson.PersonalIDType = dto.PersonalIDType;
                            existPerson.PersonalIDNumber = dto.PersonalIDNumber;
                            existPerson.PersonAddressPostalCode = dto.PersonAddressPostalCode;
                            existPerson.PersonAddress = dto.PersonAddress;
                            existPerson.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;
                            existPerson.UpdatedDate = DateTimeHelper.GetDateTimeNow();

                            this._contactPersonRepository.Update(existPerson);
                        }
                        else
                        {
                            this._contactPersonRepository.Delete(existPerson);
                        } 
                    }
                }
                
            }


            #region Attachments
            if (dto.ContractorAttachments != null && dto.ContractorAttachments.Count() > 0)
            {
                using (_unitofWork)
                {
                    foreach (var item in dto.ContractorAttachments)
                    {
                        if ((item.ID == null || item.ID == 0) && !item.IsDeleted)
                        {
                            #region new Attachment
                            var newPath = Utilities.MoveFile(AttachmentType.Contractor, item.RelativePath, existContractor.ID);
                            var newAttach = new NWC_ContractorAttachment
                            {
                                ContractorID = existContractor.ID,
                                RelativePath = newPath,
                                DocumentName = item.DocumentName,
                                IsDeleted = false,
                                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                                CreatedDate = DateTimeHelper.GetDateTimeNow()
                            };
                            this._ContractorAttachment.Add(newAttach);
                            #endregion
                        }
                        else if (item.IsDeleted)
                        {
                            #region delete attachment
                            var oldAttach = this._ContractorAttachment.GetQuery().FirstOrDefault(a => a.ID == item.ID && !a.IsDeleted);
                            if (oldAttach != null)
                            {
                                oldAttach.IsDeleted = true;
                                oldAttach.DeletedBy = this._loggedInUser.LoggedInUser.StaffId;
                                oldAttach.DeletedDate = DateTimeHelper.GetDateTimeNow();

                                this._ContractorAttachment.Update(oldAttach);
                            }
                            #endregion
                        }
                    }
                }
            }
            #endregion


            return DescriptiveResponse<bool>.Success(true);
        }


        public DescriptiveResponse<bool> ActivateContractor(long id)
        {
            var existContractor = this._contractorRepository.GetQuery()
              .FirstOrDefault(a => a.ID == id
                                   && a.IsDeleted != true
                                   && !a.IsActive 
                                   && a.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                   && _loggedInUser.SubBranches.Contains(a.CompanyAddressCityID));

            using (_unitofWork)
            {
                existContractor.IsActive = true;
                this._contractorRepository.Update(existContractor);
            }

            return DescriptiveResponse<bool>.Success(true);
        }

        public DescriptiveResponse<bool> DeActivateContractor(long id)
        {
            var existContractor = this._contractorRepository.GetQuery()
              .FirstOrDefault(a => a.ID == id
                                   && a.IsDeleted != true
                                   && a.IsActive
                                   && a.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                   && _loggedInUser.SubBranches.Contains(a.CompanyAddressCityID)
                                   && !a.NWC_Contract.Any(s => s.NWC_ContractStatus.EnumId == (int)ContractStatusEnum.Active 
                                                              || s.NWC_ContractStatus.EnumId == (int)ContractStatusEnum.New)
                                   );

            if (existContractor == null)
            {
                return DescriptiveResponse<bool>.Success(false);
            }

            using (_unitofWork)
            {
                existContractor.IsActive = false;
                this._contractorRepository.Update(existContractor);
            }

            return DescriptiveResponse<bool>.Success(true);
        }

        public DescriptiveResponse<bool> AddContractorToBlackListed(long id)
        {
            var existContractor = this._contractorRepository.GetQuery()
              .FirstOrDefault(a => a.ID == id
                                   && a.IsDeleted != true
                                   && !a.IsBlackListed
                                   && a.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                   && _loggedInUser.SubBranches.Contains(a.CompanyAddressCityID)
                                   && !a.NWC_Contract.Any(s => s.NWC_ContractStatus.EnumId == (int)ContractStatusEnum.Active
                                                              || s.NWC_ContractStatus.EnumId == (int)ContractStatusEnum.New));

            if (existContractor == null)
            {
                return DescriptiveResponse<bool>.Success(false);
            }

            using (_unitofWork)
            {
                existContractor.IsBlackListed = true;
                this._contractorRepository.Update(existContractor);
            }

            return DescriptiveResponse<bool>.Success(true);
        }

        public DescriptiveResponse<bool> RemoveContractorFromBlackListed(long id)
        {
            var existContractor = this._contractorRepository.GetQuery()
              .FirstOrDefault(a => a.ID == id
                                   && a.IsDeleted != true
                                   && a.IsBlackListed
                                   && a.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                   && _loggedInUser.SubBranches.Contains(a.CompanyAddressCityID)
                                   );

            using (_unitofWork)
            {
                existContractor.IsBlackListed = false;
                this._contractorRepository.Update(existContractor);
            }

            return DescriptiveResponse<bool>.Success(true);
        }

        #endregion


    }
}
