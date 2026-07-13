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

namespace NWC.BLL.Services
{
    public class DriverSMSService : IDriverSMSService
    {
        #region Properties
        private IUnitofWork _unitofWork;

        private IRepository<NWC_DriverSMS> _driverSMSRepository;
        private IRepository<NWC_StateWorkOrder> _stateWorkOrderRepository;
        private ILoggedInUserService _loggedInUser;

        #endregion

        #region Constructors
        public DriverSMSService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._driverSMSRepository = new Repository<NWC_DriverSMS>(ctx);
            this._stateWorkOrderRepository = new Repository<NWC_StateWorkOrder>(ctx);

        }
        #endregion

        #region Command

        public DescriptiveResponse<bool> UpdateSuccessDriverSMS(long smsID)
        {
            try
            {
                var sms = this._driverSMSRepository.FindById(smsID);

                using (_unitofWork)
                {
                    if (sms != null)
                    {
                        sms.StatusID = 2;
                        sms.SentTime = DateTimeHelper.GetDateTimeNow();
                    }

                    this._driverSMSRepository.Update(sms);
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "DriverSMSService => UpdateSuccessDriverSMS: "));

                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<bool> UpdateFailDriverSMS(long smsID)
        {
            try
            {
                var sms = this._driverSMSRepository.FindById(smsID);

                using (_unitofWork)
                {
                    if (sms != null)
                    {
                        sms.StatusID = 3;
                        sms.SentTime = DateTimeHelper.GetDateTimeNow();
                    }

                    this._driverSMSRepository.Update(sms);
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "DriverSMSService => UpdateFailDriverSMS: "));

                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        #endregion

        #region Query
        public DescriptiveResponse<SearchResult<DriverSMSDTO>> GetDriverSMSs(DriverSMSSearchCriteria searchCriteria)
        {
            try
            {
                #region Predicate
                var orderStatusIDs = new List<int>();
                orderStatusIDs.Add(5);
                orderStatusIDs.Add(6);

                var predicate = PredicateBuilder.New<NWC_DriverSMS>(true);

                if (searchCriteria.StatusIDs.Any())
                {
                    predicate = predicate.And(s => searchCriteria.StatusIDs.Contains(s.StatusID));
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

                var userPermittedLandmarks = this._driverSMSRepository.GetQuery().Join(
                      this._stateWorkOrderRepository.GetQuery(),
                      sms => sms.OrderNumber,
                      order => order.OrderNumber,
                      (sms, order) => new
                        { sms, order })
                      .Where(x => searchCriteria.StatusIDs.Contains(x.sms.StatusID) && orderStatusIDs.Contains(x.order.LastStatusID))
                      .OrderBy(s => s.sms.CreatedTime)
                      .Skip(skip)
                      .Take(take);

                //var userPermittedLandmarks = this._driverSMSRepository.GetQuery()
                //    .Where(predicate)
                //    .OrderBy(s => s.CreatedTime)
                //    .Skip(skip)
                //    .Take(take);

                var result = new SearchResult<DriverSMSDTO>();

                if (userPermittedLandmarks != null && userPermittedLandmarks.Any())
                {
                    //var count = this._driverSMSRepository.GetQuery().Count(predicate);
                    var count = this._driverSMSRepository.GetQuery().Join(
                      this._stateWorkOrderRepository.GetQuery(),
                      sms => sms.OrderNumber,
                      order => order.OrderNumber,
                      (sms, order) => new
                      { sms, order })
                      .Where(x => searchCriteria.StatusIDs.Contains(x.sms.StatusID) && orderStatusIDs.Contains(x.order.LastStatusID))
                      .OrderBy(s => s.sms.CreatedTime)
                      .Count();
                    result.Result = userPermittedLandmarks.AsEnumerable().Select(a => a.sms.WrapDriverSMS()).ToList();
                    result.TotalCount = count;
                }

                return DescriptiveResponse<SearchResult<DriverSMSDTO>>.Success(result);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "DriverSMSService => GetDriverSMSs: "));
                return DescriptiveResponse<SearchResult<DriverSMSDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion
    }
}
