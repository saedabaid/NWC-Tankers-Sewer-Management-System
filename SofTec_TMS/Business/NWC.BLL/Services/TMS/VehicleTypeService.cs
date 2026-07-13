using Infrastructure;
using LinqKit;
using NWC.BLL.Interfaces;
using NWC.BLL.Validators;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
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

namespace NWC.BLL.Services
{
    public class VehicleTypeService : IVehicleTypeService
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

        public VehicleTypeService(ILoggedInUserService loggedInUser, DbContext context = null)
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
                return DescriptiveResponse<SearchResult<VehicleTypeDTO>>.Error(ex.Message + "||" + ex.InnerException?.InnerException.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
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

    }
}
