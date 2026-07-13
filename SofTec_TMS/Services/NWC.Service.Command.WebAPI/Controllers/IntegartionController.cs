using NWC.BL.Denormalizer.Denormalizers;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;

using System;
using System.Linq;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/Integration")]
    public class IntegartionController : ApiControllerBase
    {
        private IWorkOrderService _workOrderService;
        private IDenormalizer _denormalizer;
        private ICustomerService _customerService;
        private ILookupService _LookupService;


        public IntegartionController()
        {
            _workOrderService = new WorkOrderService(loggedInService);
            _denormalizer = new Denormalizer();
            _customerService = new CustomerService(loggedInService);
            _LookupService = new LookupService(loggedInService);
        }

        [HttpPost]
        [Route("EditDeferredOrder")]
        public DescriptiveResponse<Boolean> EditDeferredOrder(DeferredOrderDTO deferredOrder)
        {
            OnActionExecuting();

            var canUpdate = _workOrderService.CheckCanUpdateDeferredOrder(deferredOrder);
            if (canUpdate.IsErrorState || canUpdate.Value == false)
            {
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.NOT_FOUNT);
            }

            #region Create Customer
            var customer = new CustomerDTO
            {
                IntegrationId = deferredOrder.ACCOUNTID,
                Code = deferredOrder.PERSONID,
                Email = "mail123@mailTest123.com",
                IDNumber = deferredOrder.PERSONIDVALUE,
                FullName = deferredOrder.PERSONPRIMARYNAME,
                IDTypeID = deferredOrder.helper_PersonIdTypeID,
                Mobile = deferredOrder.MOBILENUMBER
            };

            var customerResponse = _customerService.CreateCustomer(customer);

            if (customerResponse.IsErrorState)
            {
                return new DescriptiveResponse<bool>
                {
                    IsErrorState = customerResponse.IsErrorState,
                    ErrorDescription = customerResponse.ErrorDescription,
                    Errors = customerResponse.Errors,
                    ResponseCode = customerResponse.ResponseCode,
                };
            }

            #endregion

            #region Create Customer Location
            var customerLocation = new CustomerLocationDTO
            {
                CustomerID = customerResponse.Value.ID,
                IntegrationId = deferredOrder.PREMISEID,
                Code = deferredOrder.PREMISEID,
                PriorityID = 1,
                CategoryID = 1,
                ClassID = deferredOrder.helper_CustomerClassId,
                StatusID = 2,
                Latitude = deferredOrder.helper_latitude,
                Longitude = deferredOrder.helper_longitude,
                ZoneID = deferredOrder.helper_ZoneId
            };

            var customerLocationResponse = _customerService.CreateCustomerLocation(customerLocation);

            if (customerLocationResponse.IsErrorState)
            {
                return new DescriptiveResponse<bool>
                {
                    IsErrorState = customerLocationResponse.IsErrorState,
                    ErrorDescription = customerLocationResponse.ErrorDescription,
                    Errors = customerLocationResponse.Errors,
                    ResponseCode = customerLocationResponse.ResponseCode,
                };
            }

            #endregion


            #region Customer Account
            var customerAccount = new CustomerAccountDTO
            {
                CustomerId = customerResponse.Value.ID,
                CustomerLocationId = customerLocationResponse.Value.ID,
                ServiceTypeId = deferredOrder.helper_ServiceTypeId,
                AccountId_Integration = deferredOrder.ACCOUNTID
            };

            var customerAccountResponse = _customerService.CreateCustomerAccount(customerAccount);

            if (customerAccountResponse.IsErrorState)
            {
                return new DescriptiveResponse<bool>
                {
                    IsErrorState = customerResponse.IsErrorState,
                    ErrorDescription = customerResponse.ErrorDescription,
                    Errors = customerResponse.Errors,
                    ResponseCode = customerResponse.ResponseCode,
                };
            }

            #endregion


            #region Get Main Station

            var mainStationResponse = _LookupService.GetMainZoneStation(deferredOrder.helper_ZoneId);

            if (mainStationResponse.IsErrorState)
            {
                return new DescriptiveResponse<bool>
                {
                    IsErrorState = mainStationResponse.IsErrorState,
                    ErrorDescription = mainStationResponse.ErrorDescription,
                    Errors = mainStationResponse.Errors,
                    ResponseCode = mainStationResponse.ResponseCode,
                };
            }

            #endregion


            #region Create order

            var order = new EventWorkOrderDTO
            {
                OrderNumber = deferredOrder.ORDERNUMBER,

                StationID = mainStationResponse.Value.Id,
                //CustomerLocationID = customerLocationResponse.Value.ID,
                //ServiceTypeID = deferredOrder.helper_ServiceTypeId,
                CustomerAccountId = customerAccountResponse.Value.ID,
                OrderQuantity = int.Parse(deferredOrder.TANKERSIZE),
                RecieverName = deferredOrder.CONTACTNAME,
                RecieverMobile = deferredOrder.CONTACTMOBILE,
                Comments = deferredOrder.COMMENT,


                SourceApplication = deferredOrder.SOURCEAPPLICATION,
                CISDivision = deferredOrder.CISDIVISION,
                TransactionID = deferredOrder.TRANSACTIONID,
                ScheduledDeliveryTime = deferredOrder.helper_scheduleTime,

                ConfirmationCode = deferredOrder.CONFIRMATIONCODE,

            };

            var eventWorkOrder = new EventWorkOrderDTO();
            var orderServiceResponse = _workOrderService.CreateWorkOrder(order, out eventWorkOrder);
            if (orderServiceResponse.IsErrorState)
            {
                return new DescriptiveResponse<bool>
                {
                    IsErrorState = orderServiceResponse.IsErrorState,
                    ErrorDescription = orderServiceResponse.ErrorDescription,
                    Errors = orderServiceResponse.Errors,
                    ResponseCode = orderServiceResponse.ResponseCode,
                };
            }

            // call denormalizer
            var denprmalizerResponse = _denormalizer.DenormalizeStates(orderServiceResponse.Value);
            if (denprmalizerResponse.IsErrorState)
            {
                return new DescriptiveResponse<bool>
                {
                    IsErrorState = denprmalizerResponse.IsErrorState,
                    ErrorDescription = denprmalizerResponse.ErrorDescription,
                    Errors = denprmalizerResponse.Errors,
                    ResponseCode = denprmalizerResponse.ResponseCode,
                };
            }

            #endregion

            return _workOrderService.UpdateDeferredOrder(deferredOrder);
        }


        [HttpPost]
        [Route("CancelDeferredOrder")]
        public DescriptiveResponse<Boolean> CancelDeferredOrder([FromBody] int id)
        {
            OnActionExecuting();

            return _workOrderService.CancelDeferredOrder(id);
        }

        [HttpPost]
        [Route("AddSoqyaCustomerBalance")]
        public DescriptiveResponse<Boolean> AddSoqyaCustomerBalance([FromBody] SoqyaCustomerBalanceDTO dto)
        {
            OnActionExecuting();

            return _customerService.AddSoqyaCustomerBalance(dto);
        }
    }
}