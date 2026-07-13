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
using System.Text;
using System.Threading.Tasks;

namespace NWC.BLL.Services
{
    public class CustomerService : ICustomerService
    {
        #region Properties
        private readonly ILoggedInUserService _loggedInUser;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<NWC_Customer> _customerRepository;
        private readonly IRepository<NWC_CustomerLocation> _customerLocationRepository;
        private readonly IRepository<NWC_PersonalIDType> _personalIDTypeRepository;
        private readonly IRepository<NWC_Zone> _zoneRepository;
        private readonly IRepository<NWC_CustomerLocationClass> _customerLocationClassRepository;
        private readonly IRepository<NWC_CustomerLocationPriority> _customerLocationPriorityRepository;
        private readonly IRepository<NWC_CustomerLocationCategory> _customerLocationCategoryRepository;
        private readonly IRepository<NWC_CustomerLocationStatus> _customerLocationStatusRepository;
        private readonly IRepository<NWC_CustomerAccount> _customerAccountRepository;
        private readonly IRepository<NWC_StateWorkOrder> _stateWorkOrderRepository;
        private readonly IRepository<vw_NWC_CustomerAccountZone> _customerAccountZoneRepository;
        #endregion

        #region Constructors
        public CustomerService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._customerRepository = new Repository<NWC_Customer>(ctx);
            this._customerLocationRepository = new Repository<NWC_CustomerLocation>(ctx);
            this._personalIDTypeRepository = new Repository<NWC_PersonalIDType>(ctx);
            this._zoneRepository = new Repository<NWC_Zone>(ctx);
            this._customerLocationClassRepository = new Repository<NWC_CustomerLocationClass>(ctx);
            this._customerLocationPriorityRepository = new Repository<NWC_CustomerLocationPriority>(ctx);
            this._customerLocationCategoryRepository = new Repository<NWC_CustomerLocationCategory>(ctx);
            this._customerLocationStatusRepository = new Repository<NWC_CustomerLocationStatus>(ctx);
            this._customerAccountRepository = new Repository<NWC_CustomerAccount>(ctx);
            this._stateWorkOrderRepository = new Repository<NWC_StateWorkOrder>(ctx);
            this._customerAccountZoneRepository = new Repository<vw_NWC_CustomerAccountZone>(ctx);
        }
        #endregion

        #region NWC Portal
        public DescriptiveResponse<SearchResult<CustomerDTO>> SearchCustomerList(CustomerSC searchCriteria)
        {
            #region Predicate
            var predicate = PredicateBuilder.New<NWC_Customer>
                (s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                    && s.IsDeleted != true
                    //&& s.IntegrationId == searchIntegartionId
                    );

            if (searchCriteria.Id.HasValue)
            {
                var searchId = searchCriteria.Id.Value;
                predicate = predicate.And(s => s.ID == searchId);
            }

            //string searchIntegartionId = string.IsNullOrEmpty(searchCriteria.IntegartionId)
            //                                ? "TMS"
            //                                : searchCriteria.IntegartionId;
            if (!string.IsNullOrEmpty(searchCriteria.IntegartionId))
            {
                var searchText = searchCriteria.Customer.Trim();
                predicate = predicate.And(s => s.IntegrationId == searchText);
            }

            if (!string.IsNullOrEmpty(searchCriteria.Customer))
            {
                var searchText = searchCriteria.Customer.Trim();
                predicate = predicate.And(s => s.FullName.Contains(searchText)
                                                || s.Mobile.Contains(searchText)
                                                || s.IDNumber.Contains(searchText)
                                                );
            }

            //search all even non permitted
            if (searchCriteria.ServiceTypeId.HasValue)
            {
                var serviceId = searchCriteria.ServiceTypeId.Value;
                predicate = predicate.And(s => s.NWC_CustomerAccount.Any(a => a.ServiceTypeId == serviceId));
            }

            //search all even non permitted
            if (searchCriteria.ZoneId.HasValue)
            {
                var zoneId = searchCriteria.ZoneId.Value;
                predicate = predicate.And(s => s.NWC_CustomerAccount.Any(a => a.NWC_CustomerLocation.ZoneID == zoneId));
            }

            #endregion

            IQueryable<NWC_Customer> workOrderList =
                this._customerRepository.GetQuery()
                    .Where(predicate)
                    .OrderByDescending(s => s.CreatedDate);

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
            var result = new SearchResult<CustomerDTO>();
            if (workOrderList != null && workOrderList.Any())
            {
                var count = this._customerRepository.GetQuery().Count(predicate);
                result.Result = workOrderList.AsEnumerable().Select(a => a.WrapToCustomerDTO()).ToList();
                result.TotalCount = count;
            }

            return DescriptiveResponse<SearchResult<CustomerDTO>>.Success(result);
            #endregion
        }

        public DescriptiveResponse<CustomerDTO> CreateCustomerAndLocations(CustomerDTO dto)
        {
            #region Validations
            var validator = new CustomerValidator(ValidationMode.CreateLogic2, this._loggedInUser, this._customerRepository);
            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                var failures = results.Errors.Select(s => s.ErrorMessage);
                return DescriptiveResponse<CustomerDTO>.Error(failures);
            }
            #endregion

            var customer = dto.WrapToCustomerAndAccounts();

            #region prepare model
            customer.IsDeleted = false;
            customer.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
            customer.CreatedDate = DateTimeHelper.GetDateTimeNow();
            customer.SubID = _loggedInUser.LoggedInUser.SubscriberId;

            foreach (var item in customer.NWC_CustomerAccount)
            {
                item.IsDeleted = false;
                item.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
                item.CreatedDate = DateTimeHelper.GetDateTimeNow();

                item.NWC_CustomerLocation.IsDeleted = false;
                item.NWC_CustomerLocation.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
                item.NWC_CustomerLocation.CreatedDate = DateTimeHelper.GetDateTimeNow();
                item.NWC_CustomerLocation.SubID = _loggedInUser.LoggedInUser.SubscriberId;
            }
            #endregion

            using (_unitofWork)
            {
                this._customerRepository.Add(customer);
            }

            return DescriptiveResponse<CustomerDTO>.Success(customer.WrapToCustomerDTO());
        }

        public DescriptiveResponse<bool> EditCustomerAndLocations(CustomerDTO dto)
        {
            #region Validations
            var validator = new CustomerValidator(ValidationMode.CreateLogic2, this._loggedInUser, this._customerRepository);
            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                var failures = results.Errors.Select(s => s.ErrorMessage);
                return DescriptiveResponse<bool>.Error(failures);
            }
            #endregion

            var customer = this._customerRepository.GetQuery()
                            .FirstOrDefault(s => s.ID == dto.ID
                                        && s.IsDeleted != true
                                        && s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

            #region Map Model
            customer.FullName = dto.FullName;
            customer.Mobile = dto.Mobile;
            customer.IDTypeID = dto.IDTypeID;
            customer.IDNumber = dto.IDNumber;
            #endregion

            #region prepare model
            customer.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;
            customer.UpdatedDate = DateTimeHelper.GetDateTimeNow();
            #endregion

            using (_unitofWork)
            {
                this._customerRepository.Update(customer);
            }


            #region Customer Accounts
            if (dto.customerAccounts != null && dto.customerAccounts.Count() > 0)
            {
                using (_unitofWork)
                {
                    foreach (var item in dto.customerAccounts)
                    {
                        if ((item.ID == null || item.ID == 0) && !item.IsDeleted)
                        {
                            #region new Account
                            var newAccount = new NWC_CustomerAccount
                            {
                                CustomerId = customer.ID,
                                ServiceTypeId = item.ServiceTypeId,
                                AccountId_Integration = item.AccountId_Integration,

                                IsDeleted = false,
                                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                                CreatedDate = DateTimeHelper.GetDateTimeNow()
                            };

                            newAccount.NWC_CustomerLocation = new NWC_CustomerLocation
                            {
                                CustomerID = item.CustomerId,
                                Code = !string.IsNullOrEmpty(item.CL_Code) ? item.CL_Code.Trim() : string.Empty,
                                ZoneID = item.CL_ZoneID,
                                ClassID = item.CL_ClassID,
                                PriorityID = item.CL_PriorityID,
                                CategoryID = item.CL_CategoryID,
                                StatusID = item.CL_StatusID,
                                Address = item.CL_Address,

                                IsDeleted = false,
                                CreatedBy = this._loggedInUser.LoggedInUser.StaffId,
                                CreatedDate = DateTimeHelper.GetDateTimeNow(),
                                SubID = _loggedInUser.LoggedInUser.SubscriberId
                            };
                            this._customerAccountRepository.Add(newAccount);

                            #endregion
                        }
                        else if (item.ID > 0 && !item.IsDeleted)
                        {
                            #region update Account
                            var existAccount = this._customerAccountRepository.GetQuery()
                                           .FirstOrDefault(s => s.ID == item.ID
                                                           && s.CustomerId == customer.ID
                                                           && !s.IsDeleted);
                            if (existAccount != null)
                            {
                                existAccount.ServiceTypeId = item.ServiceTypeId;
                                existAccount.AccountId_Integration = item.AccountId_Integration;
                                //existAccount.SoqyaBalance = item.SoqyaBalance;
                                existAccount.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;
                                existAccount.UpdatedDate = DateTimeHelper.GetDateTimeNow();
                                
                                this._customerAccountRepository.Update(existAccount);
                            }

                            var existLocation = this._customerLocationRepository.GetQuery()
                                .FirstOrDefault(s => s.ID == existAccount.CustomerLocationId
                                                     && s.IsDeleted != true);

                            if (existLocation != null)
                            {
                                existLocation.Address = item.CL_Address;
                                existLocation.ZoneID = item.CL_ZoneID;
                                existLocation.ClassID = item.CL_ClassID;
                                existLocation.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;
                                existLocation.UpdatedDate = DateTimeHelper.GetDateTimeNow();
                            }

                            this._customerLocationRepository.Update(existLocation);

                            #endregion
                        }
                        else if (item.ID > 0 && item.IsDeleted)
                        {
                            #region Delete Account
                            var oldAccount = this._customerAccountRepository.GetQuery()
                                            .FirstOrDefault(s => s.ID == item.ID
                                                            && s.CustomerId == customer.ID
                                                            && !s.IsDeleted);
                            if (oldAccount != null)
                            {
                                oldAccount.IsDeleted = true;
                                oldAccount.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;
                                oldAccount.UpdatedDate = DateTimeHelper.GetDateTimeNow();

                                this._customerAccountRepository.Update(oldAccount);
                            }
                            #endregion
                        }
                    }
                }

            }
            #endregion

            return DescriptiveResponse<bool>.Success(true);
        }

        public DescriptiveResponse<bool> DeleteCustomer(long customerId)
        {
            var existCustomer = this._customerRepository.GetQuery()
                .FirstOrDefault(a => a.ID == customerId
                                     && a.IsDeleted != true
                                     && a.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                     );

            using (_unitofWork)
            {
                #region prepare model
                existCustomer.IsDeleted = true;
                existCustomer.UpdatedBy = this._loggedInUser.LoggedInUser.StaffId;
                existCustomer.UpdatedDate = DateTimeHelper.GetDateTimeNow();
                #endregion
                this._customerRepository.Update(existCustomer);
            }

            return DescriptiveResponse<bool>.Success(true);
        }

        public DescriptiveResponse<SearchResult<CustomerAccountDTO>> SearchCustomerAccountList(CustomerAccountSC searchCriteria)
        {
            var customerVaild = this._customerRepository.GetQuery()
                        .Any(s => s.ID == searchCriteria.CustomerId
                                    && s.IsDeleted != true
                                    && s.SubID == this._loggedInUser.LoggedInUser.SubscriberId);

            var result = new SearchResult<CustomerAccountDTO>();
            if (customerVaild)
            {
                result.Result = this._customerAccountZoneRepository.GetQuery()
                        .Where(s => s.CustomerId == searchCriteria.CustomerId
                                    && !s.IsDeleted)
                        .AsEnumerable().Select(a => a.WrapToCustomerAccountDTO()).ToList();
            }

            return DescriptiveResponse<SearchResult<CustomerAccountDTO>>.Success(result);
        }
        #endregion

        #region Integration

        #region Customer
        public NWC_Customer GetCustomerIfExist(CustomerDTO dto)
        {
            var existCustomer = this._customerRepository.GetQuery()
                .FirstOrDefault(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                     && s.IsDeleted == false
                                     //&& s.Code == dto.Code
                                     && s.IDNumber == dto.IDNumber);
            return existCustomer;
        }

        public DescriptiveResponse<CustomerDTO> CreateCustomer(CustomerDTO dto)
        {
            try
            {
                var existCustomer = GetCustomerIfExist(dto);

                if (existCustomer == null || existCustomer.ID == 0)
                {
                    #region Validations
                    var validator = new CustomerValidator(ValidationMode.Create, this._loggedInUser, this._customerRepository);
                    var results = validator.Validate(dto);
                    if (!results.IsValid)
                    {
                        var failures = results.Errors.Select(s => s.ErrorMessage);
                        return DescriptiveResponse<CustomerDTO>.Error(failures);
                    }
                    #endregion

                    var customer = dto.WrapToCustomer();

                    #region prepare model
                    customer.IsDeleted = false;
                    customer.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
                    customer.CreatedDate = DateTimeHelper.GetDateTimeNow();
                    customer.SubID = _loggedInUser.LoggedInUser.SubscriberId;
                    #endregion

                    using (_unitofWork)
                    {
                        this._customerRepository.Add(customer);
                    }
                    return DescriptiveResponse<CustomerDTO>.Success(customer.WrapToCustomerDTO());
                }
                existCustomer.Mobile = dto.Mobile;
                try
                {
                    //Update mobile number
                    using (_unitofWork)
                    {
                        this._customerRepository.Update(existCustomer);
                    }
                }
                catch (Exception ex)
                {
                    LoggerManager.LogMsg(c => c.Log(ex, "CustomerService => Failed to update mobile number: "));
                }
                return DescriptiveResponse<CustomerDTO>.Success(existCustomer.WrapToCustomerDTO());
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "CustomerService => CreateCustomer: "));
                return DescriptiveResponse<CustomerDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion

        #region Customer Location
        public NWC_CustomerLocation GetCustomerLocationIfExist(CustomerLocationDTO dto)
        {
            var ZoneCount = (from ex in this._customerAccountRepository.GetQuery()
                             join ex1 in this._customerLocationRepository.GetQuery() on ex.CustomerLocationId equals ex1.ID
                             join ex2 in this._customerRepository.GetQuery() on ex.CustomerId equals ex2.ID
                             where ex1.IntegrationId == dto.IntegrationId
                             && ex1.ZoneID == dto.ZoneID
                             && ex.CustomerId == dto.CustomerID
                             && ex.IsDeleted == false
                             && ex1.IsDeleted == false
                             && ex2.IsDeleted == false
                             select ex1
                     );
            return ZoneCount.FirstOrDefault();
        }

        public DescriptiveResponse<CustomerLocationDTO> CreateCustomerLocation(CustomerLocationDTO dto)
        {
            try
            {
                var existCustomerLoc = GetCustomerLocationIfExist(dto);

                if (existCustomerLoc == null)
                {
                    #region Validations
                    var validator = new CustomerLocationValidator(ValidationMode.Create, this._loggedInUser, _customerLocationRepository);
                    var results = validator.Validate(dto);
                    if (!results.IsValid)
                    {
                        var failures = results.Errors.Select(s => s.ErrorMessage);
                        return DescriptiveResponse<CustomerLocationDTO>.Error(failures);
                    }
                    #endregion

                    var customerLoc = dto.WrapToCustomerLocation();

                    #region prepare model
                    customerLoc.IsDeleted = false;
                    customerLoc.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
                    customerLoc.CreatedDate = DateTimeHelper.GetDateTimeNow();
                    customerLoc.SubID = _loggedInUser.LoggedInUser.SubscriberId;
                    #endregion

                    using (_unitofWork)
                    {
                        this._customerLocationRepository.Add(customerLoc);
                    }

                    return DescriptiveResponse<CustomerLocationDTO>.Success(customerLoc.WrapToCustomerLocationDTO());
                }

                return DescriptiveResponse<CustomerLocationDTO>.Success(existCustomerLoc.WrapToCustomerLocationDTO());
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "CustomerService => CreateCustomerLocation: "));
                return DescriptiveResponse<CustomerLocationDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<CustomerLocationDTO> GetCustomerLocByIntegrationID(string integrationID)
        {
            try
            {
                var existCustomerlocation = this._customerLocationRepository.GetQuery()
                .FirstOrDefault(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                     && s.IsDeleted == false
                                     && s.IntegrationId == integrationID);

                return DescriptiveResponse<CustomerLocationDTO>.Success(existCustomerlocation.WrapToCustomerLocationDTO());
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "CustomerService => GetCustomerLocByIntegrationID: "));
                return DescriptiveResponse<CustomerLocationDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion

        #region Customer Account
        public NWC_CustomerAccount GetCustomerAccountIfExist(CustomerAccountDTO dto)
        {
            var existCustomerAccount = this._customerAccountRepository.GetQuery()
                .FirstOrDefault(s => s.CustomerId == dto.CustomerId
                                     && s.CustomerLocationId == dto.CustomerLocationId
                                     && s.ServiceTypeId == dto.ServiceTypeId
                                     && s.IsDeleted == false
                                     && s.AccountId_Integration == dto.AccountId_Integration);

            return existCustomerAccount;
        }

        public DescriptiveResponse<CustomerAccountDTO> CreateCustomerAccount(CustomerAccountDTO dto)
        {
            try
            {
                var existCustomerLoc = GetCustomerAccountIfExist(dto);

                if (existCustomerLoc == null)
                {
                    #region Validations
                    //var validator = new CustomerLocationValidator(ValidationMode.Create, this._loggedInUser, _customerLocationRepository);
                    //var results = validator.Validate(dto);
                    //if (!results.IsValid)
                    //{
                    //    var failures = results.Errors.Select(s => s.ErrorMessage);
                    //    return DescriptiveResponse<CustomerLocationDTO>.Error(failures);
                    //}
                    #endregion

                    var customerAccount = dto.WrapToCustomerAccount();

                    #region prepare model
                    customerAccount.IsDeleted = false;
                    customerAccount.CreatedBy = this._loggedInUser.LoggedInUser.StaffId;
                    customerAccount.CreatedDate = DateTimeHelper.GetDateTimeNow();
                    #endregion

                    using (_unitofWork)
                    {
                        this._customerAccountRepository.Add(customerAccount);
                    }

                    return DescriptiveResponse<CustomerAccountDTO>.Success(customerAccount.WrapToCustomerAccountDTO());
                }

                return DescriptiveResponse<CustomerAccountDTO>.Success(existCustomerLoc.WrapToCustomerAccountDTO());
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "CustomerService => CreateCustomerAccount: "));
                return DescriptiveResponse<CustomerAccountDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion

        public DescriptiveResponse<SoqyaCustomerBalanceDTO> CreateCustomerBalance(SoqyaCustomerBalanceDTO dto)
        {
            try
            {
                var response = new DescriptiveResponse<SoqyaCustomerBalanceDTO>();
                response.Value = new SoqyaCustomerBalanceDTO();

                dto.Customer.IDTypeID = GetId_PersonalIDType(dto.Customer.IntegrationId_IDType);

                var customer = CreateCustomer(dto.Customer);

                response.Value.Customer = customer.Value;
                response.IsErrorState = customer.IsErrorState;
                response.Errors = customer.Errors;

                if (!customer.IsErrorState && customer.Value != null)
                {
                    var custClass = this._customerLocationClassRepository.GetQuery().Where(x => x.IntegrationId == dto.CustomerLocation.IntegartionId_Class).FirstOrDefault();

                    dto.CustomerLocation.CustomerID = customer.Value.ID;
                    dto.CustomerLocation.ClassID = custClass != null ? custClass.ID : 0;

                    var customerLoc = CreateCustomerLocation(dto.CustomerLocation);

                    response.Value.CustomerLocation = customerLoc.Value;
                    response.IsErrorState = customerLoc.IsErrorState;
                    response.Errors = customerLoc.Errors;

                    if (!customerLoc.IsErrorState && customerLoc.Value != null)
                    {
                        dto.Account.CustomerId = customer.Value.ID;
                        dto.Account.CustomerLocationId = customerLoc.Value.ID;

                        var account = CreateCustomerAccount(dto.Account);

                        response.Value.Account = account.Value;
                        response.IsErrorState = account.IsErrorState;
                        response.Errors = account.Errors;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "CustomerService => CreateCustomerBalance: "));
                return DescriptiveResponse<SoqyaCustomerBalanceDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        #region Soqya
        public DescriptiveResponse<bool> AddSoqyaCustomerBalance(SoqyaCustomerBalanceDTO dto)
        {
            try
            {
                var response = new DescriptiveResponse<bool>();

                dto.Customer.IDTypeID = GetId_PersonalIDType(dto.Customer.IntegrationId_IDType);

                var customer = CreateCustomer(dto.Customer);

                response.Value = !customer.IsErrorState && customer.Value != null;
                response.IsErrorState = customer.IsErrorState;
                response.Errors = customer.Errors;

                if (!customer.IsErrorState && customer.Value != null)
                {
                    var custClass = this._customerLocationClassRepository.GetQuery().Where(x => x.IntegrationId == dto.CustomerLocation.IntegartionId_Class).FirstOrDefault();

                    dto.CustomerLocation.CustomerID = customer.Value.ID;
                    dto.CustomerLocation.ClassID = custClass != null ? custClass.ID : 0;

                    var customerLoc = CreateCustomerLocation(dto.CustomerLocation);

                    response.Value = !customerLoc.IsErrorState && customerLoc.Value != null;
                    response.IsErrorState = customerLoc.IsErrorState;
                    response.Errors = customerLoc.Errors;

                    if (!customerLoc.IsErrorState && customerLoc.Value != null)
                    {
                        dto.Account.CustomerId = customer.Value.ID;
                        dto.Account.CustomerLocationId = customerLoc.Value.ID;

                        var account = CreateCustomerAccount(dto.Account);

                        response.Value = !account.IsErrorState && account.Value != null;
                        response.IsErrorState = account.IsErrorState;
                        response.Errors = account.Errors;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "CustomerService => AddSoqyaCustomerBalance: "));
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion

        #region Helper IntegartionId
        private int GetId_PersonalIDType(string integartionId)
        {
            var personalType = this._personalIDTypeRepository.GetQuery()
                .FirstOrDefault(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                            && s.IntegrationId == integartionId);

            return (personalType == null) ? 0 : personalType.ID;
        }

        private long GetId_Zone(string integartionId)
        {
            var zone = this._zoneRepository.GetQuery()
                .FirstOrDefault(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                                     && s.IsDeleted == false
                                     && s.IntegrationId == integartionId);

            return (zone == null) ? 0 : zone.ID;
        }

        private int GetId_LocationClass(string integartionId)
        {
            var locationClass = this._customerLocationClassRepository.GetQuery()
                .FirstOrDefault(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                            && s.IntegrationId == integartionId);

            return (locationClass == null) ? 0 : locationClass.ID;
        }

        private int GetId_LocationPriority(string integartionId)
        {
            var locationPriority = this._customerLocationPriorityRepository.GetQuery()
                .FirstOrDefault(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                            && s.IntegrationId == integartionId);

            return (locationPriority == null) ? 0 : locationPriority.ID;
        }

        private int GetId_LocationCategory(string integartionId)
        {
            var locationCategory = this._customerLocationCategoryRepository.GetQuery()
                .FirstOrDefault(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                            && s.IntegrationId == integartionId);

            return (locationCategory == null) ? 0 : locationCategory.ID;
        }

        private int GetId_LocationStatus(string integartionId)
        {
            var locationStatus = this._customerLocationStatusRepository.GetQuery()
                .FirstOrDefault(s => s.SubID == this._loggedInUser.LoggedInUser.SubscriberId
                            && s.IntegrationId == integartionId);

            return (locationStatus == null) ? 0 : locationStatus.ID;
        }
        #endregion

        public string InsertCustomerAccounts()
        {
            var successCount = 0;
            var failCount = 0;

            var logs = new StringBuilder();

            try
            {
                int skip = this._customerAccountRepository.GetQuery().Count();
                int take = 1000;

                //var orders = this._stateWorkOrderRepository.GetQuery().OrderBy(x => x.CreateTime).Skip(skip).Take(take).ToList();
                var orders = this._stateWorkOrderRepository.GetQuery().Join(this._customerLocationRepository.GetQuery().Where(x => x.IntegrationId != null),
                    order => order.CustomerLocationID,
                    cl => cl.ID,
                    (order, cl) => new { order })
                    .OrderBy(x => x.order.CreateTime).Skip(skip).Take(take).ToList();

                foreach (var order in orders)
                {
                    var cl = this._customerLocationRepository.GetQuery().FirstOrDefault(x => x.ID == order.order.CustomerLocationID.Value);

                    if (cl != null && !string.IsNullOrEmpty(cl.IntegrationId))
                    {
                        var custAccount = CreateCustomerAccount(new CustomerAccountDTO()
                        {
                            CustomerLocationId = cl.ID,
                            CustomerId = cl.CustomerID,
                            AccountId_Integration = cl.IntegrationId,
                            ServiceTypeId = order.order.ServiceTypeID.Value
                        });

                        if (!custAccount.IsErrorState && custAccount.Value != null && custAccount.Value.ID > 0)
                            successCount++;
                        else
                            failCount++;
                    }
                }

                logs.AppendLine(string.Format("Create customer account - Success: {0} - Fails: {1}", successCount, failCount));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "CustomerService => InsertCustomerAccounts: "));
            }

            return logs.ToString();
        }

        public string UpdateWorkOrders()
        {
            var successCount = 0;
            var failCount = 0;

            var logs = new StringBuilder();

            try
            {
                //int skip = this._stateWorkOrderRepository.GetQuery().Where(x => x.CustomerAccountId != null).Count();
                int take = 1000;

                var orders = this._stateWorkOrderRepository.GetQuery().Where(x => x.CustomerAccountId == null || !x.CustomerAccountId.HasValue).OrderBy(x => x.CreateTime).Take(take).ToList();

                logs.AppendLine(string.Format("Update Work Orders - Orders: {0}", orders.Count));

                foreach (var order in orders)
                {
                    var custAccount = this._customerAccountRepository.GetQuery().FirstOrDefault(x => x.CustomerLocationId == order.CustomerLocationID.Value && x.ServiceTypeId == order.ServiceTypeID);

                    if (custAccount != null)
                    {
                        using (_unitofWork)
                        {
                            order.CustomerAccountId = custAccount.ID;
                            this._stateWorkOrderRepository.Update(order);
                        }

                        successCount++;
                    }
                }

                logs.AppendLine(string.Format("Update Work Orders - Success: {0} - Fails: {1}", successCount, failCount));
            }
            catch (Exception ex)
            {
                failCount++;
                LoggerManager.LogMsg(c => c.Log(ex, "CustomerService => UpdateWorkOrders: "));
            }

            return logs.ToString();
        }
        #endregion
    }
}
