using NWC.DAL.NWCEntities;
using FluentValidation;
using NWC.DTO.Resources;
using Infrastructure;
using System.Linq;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using System;
using NWC.BLL.Interfaces;
using NWC.DTO.Helpers;

namespace NWC.BLL.Validators
{
    public class WorkOrderValidator : AbstractValidator<EventWorkOrderDTO>
    {
        private long? contractId = null;

        #region services
        private readonly IRepository<NWC_EventWorkOrder> _eventWorkOrderRepository;
        private readonly IRepository<NWC_Contract> _contractRepository;
        private readonly IRepository<NWC_ContractStations> _contractStationsRepository;
        private readonly IRepository<NWC_ContractTariff> _contractTariffRepository;
        private readonly IRepository<NWC_ContractAccessory> _contractAccessoryRepository;
        private readonly IRepository<NWC_CustomerLocation> _customerLocationRepository;
        private readonly IRepository<NWC_CustomerAccount> _customerAccountRepository;
        private readonly IRepository<NWC_StateWorkOrder> _stateWorkOrderRepository;
        ILoggedInUserService _loggedInUser;
        #endregion

        #region ctor.
        public WorkOrderValidator(ValidationMode mode,
           ILoggedInUserService loggedInUser,
           IRepository<NWC_EventWorkOrder> eventWorkOrderRepository,
           IRepository<NWC_Contract> contractRepository,
           IRepository<NWC_ContractStations> contractStationsRepository,
           IRepository<NWC_ContractTariff> contractTariffRepository,
           IRepository<NWC_ContractAccessory> contractAccessoryRepository,
           IRepository<NWC_CustomerLocation> customerLocationRepository,
           IRepository<NWC_CustomerAccount> customerAccountRepository,
           IRepository<NWC_StateWorkOrder> stateWorkOrderRepository)
        {
            this._loggedInUser = loggedInUser;
            this._eventWorkOrderRepository = eventWorkOrderRepository;
            this._contractRepository = contractRepository;
            this._contractStationsRepository = contractStationsRepository;
            this._contractTariffRepository = contractTariffRepository;
            this._contractAccessoryRepository = contractAccessoryRepository;
            this._customerLocationRepository = customerLocationRepository;
            this._customerAccountRepository = customerAccountRepository;
            this._stateWorkOrderRepository = stateWorkOrderRepository;

            Initialize(mode);
        }
        #endregion

        private void Initialize(ValidationMode mode)
        {
            switch (mode)
            {
                case ValidationMode.Create:
                    RuleFor(a => a.OrderNumber).Must(IsOrderNumberNotExist).WithMessage(ValidationMessagesKeys.OrderNumberAlreadyExist);

                    //RuleFor(a => a.CustomerLocationID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseCustomerLocation);
                    RuleFor(a => a.CustomerAccountId).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseCustomerAccount);
                    RuleFor(a => a.StationID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseStation);
                    //RuleFor(a => a.ServiceTypeID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseServiceType);
                    RuleFor(a => a.OrderQuantity).GreaterThan(0).WithMessage(ValidationMessagesKeys.InvalidQuantity);

                    //RuleFor(a => a.StationID).Must(IsValidContractStation).WithMessage(ValidationMessagesKeys.StationDoesNotAssignedToContract);
                    //RuleFor(a => a).Must(IsValidContractTariff).WithMessage(ValidationMessagesKeys.ContractStationDoesNotHasTariff);

                    RuleFor(a => a).Must(IsValidContract).WithMessage(ValidationMessagesKeys.InvalidContract);
                    //RuleFor(a => a.CustomerLocationID).Must(IsValidCustomerLocation).WithMessage(ValidationMessagesKeys.CustomerLocationDoesNotExist);
                    RuleFor(a => a.CustomerAccountId).Must(IsValidCustomerAccount).WithMessage(ValidationMessagesKeys.CustomerLocationDoesNotExist);
                    RuleFor(a => a).Must(IsValidContractAccessory).WithMessage(ValidationMessagesKeys.ContractStationDoesNotHasAccessories);
                    break;
                case ValidationMode.Update:
                    RuleFor(a => a.StationID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseStation);
                    //RuleFor(a => a.ServiceTypeID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseServiceType);
                    RuleFor(a => a.OrderQuantity).GreaterThan(0).WithMessage(ValidationMessagesKeys.InvalidQuantity);
                    //RuleFor(a => a.CustomerLocationID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseCustomerLocation);
                    RuleFor(a => a.CustomerAccountId).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseCustomerAccount);
                    RuleFor(a => a.CustomerAccountId).Must(IsValidCustomerAccount).WithMessage(ValidationMessagesKeys.CustomerLocationDoesNotExist);
                    RuleFor(a => a).Must(IsValidStatusForUpdate).WithMessage(ValidationMessagesKeys.NotValidStatusForUpdate);
                    break;
            }
        }

        private bool IsOrderNumberNotExist(EventWorkOrderDTO model, string orderNumber)
        {
            return !this._eventWorkOrderRepository.GetQuery()
                .Any(s => s.OrderNumber == orderNumber && s.SubID == _loggedInUser.LoggedInUser.SubscriberId);
        }

        private bool IsValidContractStation(EventWorkOrderDTO model, Guid stationId)
        {
            if (model == null || stationId == null || stationId == Guid.Empty) return true;

            var contract = _contractRepository.GetQuery().Where(x => x.ContractStatusID.Value == (int)ContractStatusEnum.Active &&
                            (!x.IsDeleted.HasValue || x.IsDeleted.Value != true) &&
                            x.ContractEndDate >= model.ScheduledDeliveryTime
                            && x.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Terminated
                            && x.NWC_ContractTariff.Where(ct => ct.ServiceTypeID == model.ServiceTypeID).Any()
                            //&& (x.IsTerminated == null || x.IsTerminated == false) 
                            //&& x.NWC_ContractTariff.Where(t => t.IsDeleted != true && t.StationID == stationID && t.ServiceTypeID == serviceTypeID)
                            && x.NWC_ContractStations.Where(s => s.IsDeleted != true).Select(s => s.StationID).ToList().Contains(model.StationID)).FirstOrDefault();

            if (contract == null)
                return false;

            this.contractId = contract.ID;
            return true;
        }

        private bool IsValidContract(EventWorkOrderDTO model)
        {
            if (model == null || this.contractId == null) return true;

            return this._contractRepository.GetQuery()
                .Any(s => s.ID == this.contractId
                            && s.IsDeleted != true
                            && s.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Terminated
                            && s.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Finished
                            && s.ContractStartDate < model.ScheduledDeliveryTime
                            && s.ContractEndDate > model.ScheduledDeliveryTime
                            && s.SubID == _loggedInUser.LoggedInUser.SubscriberId);

        }

        private bool IsValidCustomerLocation(EventWorkOrderDTO model, long customerLocationId)
        {
            if (model == null || customerLocationId == 0L) return true;

            return this._customerLocationRepository.GetQuery()
                .Any(s => s.ID == customerLocationId && s.IsDeleted != true && s.SubID == _loggedInUser.LoggedInUser.SubscriberId);
        }

        private bool IsValidContractTariff(EventWorkOrderDTO model)
        {
            if (model == null
                || this.contractId == null
                || model.StationID == null || model.StationID == Guid.Empty
                || model.ServiceTypeID == 0
                || model.CustomerLocationID == 0L
                ) return true;

            var customerLocation = this._customerLocationRepository.GetQuery()
               .FirstOrDefault(s => s.ID == model.CustomerLocationID && s.IsDeleted != true && s.SubID == _loggedInUser.LoggedInUser.SubscriberId);

            if (customerLocation == null) return true;

            var scheduledDeliveryHijri = DateTimeHelper.ConvertDateToHijriAsLong(model.ScheduledDeliveryTime);

            return this._contractTariffRepository.GetQuery()
               .Where(s => s.ContractID == this.contractId
                         && s.StationID == model.StationID
                         && s.ServiceTypeID == model.ServiceTypeID
                         && s.CustomerLocationClassID == customerLocation.ClassID
                         && ((s.ZoneID == null) || s.ZoneID == customerLocation.ZoneID)
                         && !s.IsDeleted
                         && (s.IsActive == null || s.IsActive != false)
                         && s.DateToHijri > scheduledDeliveryHijri).Any();
        }

        private bool IsValidContractAccessory(EventWorkOrderDTO model)
        {
            if (model.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                return true;

            if (model == null
                || model.StationID == null || model.StationID == Guid.Empty
                || this.contractId == null
                || model.Accessories == null || !model.Accessories.Any()) return true;

            var supportedAcc = this._contractAccessoryRepository.GetQuery()
                    .Where(s => !s.IsDeleted
                            && s.IsActive != false
                            && s.ContractID == this.contractId
                            && s.StationID == model.StationID)
                    .Select(a => a.AccessoryID);


            foreach (var item in model.Accessories)
            {
                if (!supportedAcc.Contains(item.ID))
                    return false;
            }
            return true;
        }

        private bool IsValidCustomerAccount(EventWorkOrderDTO model, long? customerAccountId)
        {
            if (model == null || customerAccountId == null || customerAccountId == 0L) return true;

            return this._customerAccountRepository.GetQuery()
                .Any(s => s.ID == customerAccountId
                        && s.IsDeleted != true
                        && s.NWC_CustomerLocation.IsDeleted != null
                        && s.NWC_CustomerLocation.StatusID != 1 //black list
                        && s.NWC_CustomerLocation.SubID == _loggedInUser.LoggedInUser.SubscriberId);
        }

        private bool IsValidStatusForUpdate(EventWorkOrderDTO model)
        {
            //Check work order status for update
            var stateWorkOrder = this._stateWorkOrderRepository.FindById(model.WorkOrderID);

            return stateWorkOrder != null && (stateWorkOrder.LastStatusID == (int)WorkOrderStatusEnum.Onhold || stateWorkOrder.LastStatusID == (int)WorkOrderStatusEnum.New);
        }
    }
}
