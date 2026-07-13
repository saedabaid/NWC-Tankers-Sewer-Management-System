using Infrastructure;
using LinqKit;
using NWC.BL.Denormalizer.CoreBusiness;
using NWC.BLL.Interfaces;
using NWC.BLL.Validators;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Constants;
using NWC.DTO.Enums;
using NWC.DTO.Extentions;
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
    public class ViolationService : IViolationService
    {
        #region Properties
        private readonly ILoggedInUserService _loggedInUser;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<vw_NWC_VehicleViolation> _vehicleViolationRep;
        #endregion

        #region Constructors
        public ViolationService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._vehicleViolationRep = new Repository<vw_NWC_VehicleViolation>(ctx);
        }
        #endregion

        #region Command

        #endregion

        #region Query
        public DescriptiveResponse<SearchResult<VehicleViolationDTO>> GetVehicleViolations(Guid vehicleID)
        {
            try
            {
                #region Predicate
                var predicate = PredicateBuilder.New<vw_NWC_VehicleViolation>(true);

                predicate = predicate.And(s => s.IsDeleted != true);
                predicate = predicate.And(s => s.PaymentStatusId != 1);
                predicate = predicate.And(s => s.VehicleId == vehicleID);
                #endregion

                #region skip & take
                var skip = 0;
                var take = 20;
                #endregion

                var vehicleNWCSettings = this._vehicleViolationRep.GetQuery()
                    .Where(predicate)
                    .OrderBy(s => s.Id)
                    .Skip(skip)
                    .Take(take);

                var result = new SearchResult<VehicleViolationDTO>();

                if (vehicleNWCSettings != null && vehicleNWCSettings.Any())
                {
                       var count = this._vehicleViolationRep.GetQuery().Count(predicate);
                    result.Result = vehicleNWCSettings.AsEnumerable().Select(a => a.WrapToVehicleViolationDTO(this.LanguageIsEnglish)).ToList();
                    result.TotalCount = count;
                }

                return DescriptiveResponse<SearchResult<VehicleViolationDTO>>.Success(result);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ViolationService => GetVehicleViolations: "));
                return DescriptiveResponse<SearchResult<VehicleViolationDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion

        #region Helpers
        private bool LanguageIsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
            }
        }
        #endregion
    }
}
