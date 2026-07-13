using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using NWC.DTO.Wrapper;
using LinqKit;
using NWC.DTO.SearchCriteria;
using System.Data.Entity;
using NWC_CCB_Integration.DTO.Logger;
using System.Configuration;

namespace NWC.BLL.Services
{
    public class CustomerSMSService : ICustomerSMSService
    {
        #region Properties
        private IUnitofWork _unitofWork;

        private IRepository<NWC_CustomerSMS> _customerSMSRepository;
        private IRepository<NWC_StateWorkOrder> _stateWorkOrderRepository;
        private ILoggedInUserService _loggedInUser;

        private string RangeDays
        {
            get
            {
                return ConfigurationManager.AppSettings["RangeDays"] != null ?
                    ConfigurationManager.AppSettings["RangeDays"] : "1";
            }
        }

        #endregion

        #region Constructors
        public CustomerSMSService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._customerSMSRepository = new Repository<NWC_CustomerSMS>(ctx);
            this._stateWorkOrderRepository = new Repository<NWC_StateWorkOrder>(ctx);

        }
        #endregion

        #region Command

        public DescriptiveResponse<bool> UpdateSuccessCustomerSMS(long smsID)
        {
            try
            {
                var sms = this._customerSMSRepository.FindById(smsID);

                using (_unitofWork)
                {
                    if (sms != null)
                    {
                        sms.StatusID = 2;
                        sms.SentTime = DateTimeHelper.GetDateTimeNow();
                    }

                    this._customerSMSRepository.Update(sms);
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "CustomerSMSService => UpdateSuccessCustomerSMS: "));

                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> UpdateFailCustomerSMS(long smsID)
        {
            try
            {
                var sms = this._customerSMSRepository.FindById(smsID);

                using (_unitofWork)
                {
                    if (sms != null)
                    {
                        sms.StatusID = 3;
                        sms.SentTime = DateTimeHelper.GetDateTimeNow();
                    }

                    this._customerSMSRepository.Update(sms);
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "CustomerSMSService => UpdateFailCustomerSMS: "));

                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        #endregion

        #region Query
        public DescriptiveResponse<SearchResult<CustomerSMSDTO>> GetCustomerSMSs(CustomerSMSSearchCriteria searchCriteria)
        {
            try
            {
                #region Predicate
                var orderStatusIDs = new List<int>();
                orderStatusIDs.Add(1);
                orderStatusIDs.Add(5);
                orderStatusIDs.Add(6);
                orderStatusIDs.Add(4);
                orderStatusIDs.Add(7);
                var predicate = PredicateBuilder.New<NWC_CustomerSMS>(true);

                if (searchCriteria.StatusIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.StatusIDs.Contains(s.StatusID));
                }

                var dtFrom = DateTimeHelper.GetDateTimeNow().AddMinutes(-searchCriteria.DelayInMin);
                var SendingRangeFrom = DateTimeHelper.GetDateTimeNow().AddDays(-double.Parse(RangeDays));
                predicate = predicate.And(s => s.CreatedTime <= dtFrom);
                predicate = predicate.And(s => s.CreatedTime > SendingRangeFrom);
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

                var userPermittedLandmarks = this._customerSMSRepository.GetQuery().Where(predicate).Join(
                      this._stateWorkOrderRepository.GetQuery(),
                      sms => sms.OrderNumber,
                      order => order.OrderNumber,
                      (sms, order) => new
                        { sms, order })
                      .Where(x => searchCriteria.StatusIDs.Contains(x.sms.StatusID) && orderStatusIDs.Contains(x.order.LastStatusID))
                      .OrderBy(s => s.sms.CreatedTime)
                      .Skip(skip)
                      .Take(take);

                var result = new SearchResult<CustomerSMSDTO>();

                if (userPermittedLandmarks != null && userPermittedLandmarks.Any())
                {
                    var count = this._customerSMSRepository.GetQuery().Where(predicate).Join(
                      this._stateWorkOrderRepository.GetQuery(),
                      sms => sms.OrderNumber,
                      order => order.OrderNumber,
                      (sms, order) => new
                      { sms, order })
                      .Where(x => searchCriteria.StatusIDs.Contains(x.sms.StatusID) && orderStatusIDs.Contains(x.order.LastStatusID))
                      .OrderBy(s => s.sms.CreatedTime)
                      .Count();
                    result.Result = userPermittedLandmarks.AsEnumerable().Select(a => a.sms.WrapCustomerSMS()).ToList();
                    result.TotalCount = count;
                }

                return DescriptiveResponse<SearchResult<CustomerSMSDTO>>.Success(result);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "CustomerSMSService => GetCustomerSMSs: "));
                return DescriptiveResponse<SearchResult<CustomerSMSDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion
    }
}
