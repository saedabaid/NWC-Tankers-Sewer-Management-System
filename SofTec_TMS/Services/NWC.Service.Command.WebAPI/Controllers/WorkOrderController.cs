using NWC.BL.Denormalizer;
using NWC.BL.Denormalizer.Denormalizers;
using NWC.BLL.Interfaces;
using NWC.BLL.Services;
using NWC.DTO.Common;
using NWC.DTO.Constants;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/WorkOrder")]
    public class WorkOrderController : ApiControllerBase
    {
        private IWorkOrderService _workOrderService;
        private IDenormalizer _denormalizer;
        private LookupService _LookupService;
        private static bool AssignInEntryGate
        {
            get
            {
                var AssignInEntryGate = ConfigurationManager.AppSettings["AssignInEntryGate"];
                if (string.IsNullOrEmpty(AssignInEntryGate) || AssignInEntryGate.ToLower() == "true")
                {
                    return true;
                }
                return false;
            }
        }

        public WorkOrderController()
        {
            _workOrderService = new WorkOrderService(loggedInService);
            _denormalizer = new Denormalizer();
            _LookupService = new LookupService(loggedInService);
        }

        [HttpPost]
        [Route("CreateWorkOrder")]
        public DescriptiveResponse<EventWorkOrderDTO> CreateWorkOrder(EventWorkOrderDTO request)
        {
            OnActionExecuting();
            if (_LookupService.IsBranchWithRestrictedSpecialOrders(request.StationID) !=null && _LookupService.IsBranchWithRestrictedSpecialOrders(request.StationID).Value)
            {
                if (!IsUserAllowToCreateSpecialOrder() && request.CategoryID != 1 && request.ServiceTypeID != (int)ServiceTypeEnum.SewageRemoval)
                {
                    var errors = new List<string>();
                    errors.Add(LanguageIsEnglish ? "you don't have a permission to ctreate special order" : "ليست لديك صلاحية إنشاء طلب خاص");
                    return DescriptiveResponse<EventWorkOrderDTO>.Error(errors);
                }
            }
            var eventWorkOrder = new EventWorkOrderDTO();
            var response = _workOrderService.CreateWorkOrder(request, out eventWorkOrder);

            if (response.IsErrorState)
            {
                return DescriptiveResponse<EventWorkOrderDTO>.Error(response.Errors);
            }

            var result = _denormalizer.DenormalizeStates(response.Value);
            if(result.Value)
            {
             var OrderDetails=   _workOrderService.GetOrderBasicDetailsByOrderNumber(eventWorkOrder.OrderNumber);
          if(!OrderDetails.IsErrorState && OrderDetails.Value.ServiceTypeID == (int)ServiceTypeEnum.SewageRemoval)
                {
                    var SignalR = _workOrderService.CreateSignalRNotification(OrderDetails.Value.WorkOrderID);
                }
            }

           
            return new DescriptiveResponse<EventWorkOrderDTO>()
            {
                Value = eventWorkOrder,
                ErrorDescription = result.ErrorDescription,
                IsErrorState = result.IsErrorState
            };
        }

        [HttpPost]
        [Route("BulkCreateWorkOrder")]
        public DescriptiveResponse<AddItemsResponse> BulkCreateWorkOrder(EventWorkOrderDTO request)
        {
            OnActionExecuting();
            if (_LookupService.IsBranchWithRestrictedSpecialOrders(request.StationID).Value)
            {
                if (!IsUserAllowToCreateSpecialOrder() && request.CategoryID != 1)
                {
                    var errors = new List<string>();
                    errors.Add(LanguageIsEnglish ? "you don't have a permission to ctreate special order" : "ليست لديك صلاحية إنشاء طلب خاص");
                    return DescriptiveResponse<AddItemsResponse>.Error(errors);
                }
            }
            var eventWorkOrder = new EventWorkOrderDTO();
            var response = _workOrderService.BulkCreateWorkOrder(request);

            if (response.IsErrorState)
            {
                return DescriptiveResponse<AddItemsResponse>.Error(response.Errors);
            }

            var result = _denormalizer.DenormalizeStates(response.Value);

            return new DescriptiveResponse<AddItemsResponse>()
            {
                Value = new AddItemsResponse()
                {
                    success = response.Value.Count,
                    failed = (request.BC_NoOfOrders - response.Value.Count)
                },
                ErrorDescription = result.ErrorDescription,
                IsErrorState = result.IsErrorState
            };
        }

        [HttpPost]
        [Route("AssignWorkOrder")]
        public DescriptiveResponse<Boolean> AssignWorkOrder(DispatchWorkOrderDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.AssignWorkOrder(request);

            if (!response.IsErrorState && response.Value.Any())
            {
                var states = _denormalizer.DenormalizeStates(response.Value);

                return states;
            }

            return DescriptiveResponse<bool>.Error(response.ErrorDescription);
        }
        [HttpPost]
        [Route("SewerConfirmWorkOrder")]
        public DescriptiveResponse<Boolean> SewerConfirmWorkOrder(EventWorkOrderDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.SewerConfirmWorkOrder(request);

            if (!response.IsErrorState && response.Value.Any())
            {
                var states = _denormalizer.DenormalizeStates(response.Value);

                return states;
            }

            return DescriptiveResponse<bool>.Error(response.ErrorDescription);
        }

        [HttpPost]
        [Route("SewerCompleteWorkOrder")]
        public DescriptiveResponse<Boolean> SewerCompleteWorkOrder(EventWorkOrderDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.SewerCompleteWorkOrder(request);

            if (!response.IsErrorState && response.Value.Any())
            {
                var states = _denormalizer.DenormalizeStates(response.Value);

                return states;
            }

            return DescriptiveResponse<bool>.Error(response.ErrorDescription);
        }
        [HttpPost]
        [Route("DeassignWorkOrder")]
        public DescriptiveResponse<Boolean> DeassignWorkOrder(DeassignDTO deassignDTO)
        {
            OnActionExecuting();

            var deassignEventIds = _workOrderService.DeassignWorkOrder(deassignDTO.request);

            if (deassignEventIds.IsErrorState)
            {
                return new DescriptiveResponse<bool>()
                {
                    Value = false,
                    ErrorDescription = deassignEventIds.ErrorDescription,
                    IsErrorState = deassignEventIds.IsErrorState
                };
            }

            var deassignResult = _denormalizer.DenormalizeStates(deassignEventIds.Value);

            if (deassignResult.Value && deassignDTO.request.EventWorkOrderVehicleDTO.StatusID == 0 // available tanker
                && deassignDTO.eventWorkOrderDTO != null && deassignDTO.eventWorkOrderDTO.WorkOrderID > 0 && deassignDTO.eventWorkOrderDTO.ServiceTypeID != (int)ServiceTypeEnum.SewageRemoval)
            {
                var assignEventIds = _workOrderService.AssignWorkOrder(new DispatchWorkOrderDTO()
                {
                    EventWorkOrderVehicleDTO = deassignDTO.request.EventWorkOrderVehicleDTO,
                    EventWorkOrderDTO = deassignDTO.eventWorkOrderDTO
                });

                if (!assignEventIds.IsErrorState && assignEventIds.Value.Any())
                {
                    var assignResult = _denormalizer.DenormalizeStates(assignEventIds.Value);

                    return assignResult;
                }
            }

            return deassignResult.Value ? deassignResult : new DescriptiveResponse<Boolean>() { Value = false };
        }


        [HttpPost]
        [Route("PreAssignWorkOrder")]
        public DescriptiveResponse<Boolean> PreAssignWorkOrder(DeassignDTO deassignDTO)
        {
            OnActionExecuting();

            var deassignEventIds = _workOrderService.PreAssignWorkOrder(deassignDTO.request);

            if (deassignEventIds.IsErrorState)
            {
                return new DescriptiveResponse<bool>()
                {
                    Value = false,
                    ErrorDescription = deassignEventIds.ErrorDescription,
                    IsErrorState = deassignEventIds.IsErrorState
                };
            }

            var deassignResult = _denormalizer.DenormalizeStates(deassignEventIds.Value);

            if (deassignResult.Value && deassignDTO.request.EventWorkOrderVehicleDTO.StatusID == 0 // available tanker
                && deassignDTO.eventWorkOrderDTO != null && deassignDTO.eventWorkOrderDTO.WorkOrderID > 0)
            {
                var assignEventIds = _workOrderService.AssignWorkOrder(new DispatchWorkOrderDTO()
                {
                    EventWorkOrderVehicleDTO = deassignDTO.request.EventWorkOrderVehicleDTO,
                    EventWorkOrderDTO = deassignDTO.eventWorkOrderDTO
                });

                if (!assignEventIds.IsErrorState && assignEventIds.Value.Any())
                {
                    var assignResult = _denormalizer.DenormalizeStates(assignEventIds.Value);

                    return assignResult;
                }
            }

            return deassignResult.Value ? deassignResult : new DescriptiveResponse<Boolean>() { Value = false };
        }

        [HttpPost]
        [Route("OutForDeliveryWorkOrder")]
        public DescriptiveResponse<Boolean> OutForDeliveryWorkOrder(EventWorkOrderDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.OutForDeliveryWorkOrder(request);
            if (response.IsErrorState == false)
            {
                var states = _denormalizer.DenormalizeStates(response.Value);

                return states;
            }
            else
            {
                return DescriptiveResponse<Boolean>.Error(response.Errors);
            }
        }

        [HttpPost]
        [Route("ArrivedWorkOrder")]
        public DescriptiveResponse<Boolean> ArrivedWorkOrder(EventWorkOrderDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.ArrivedWorkOrder(request);
            if (response.IsErrorState == false)
            {
                var states = _denormalizer.DenormalizeStates(response.Value);

                return states;
            }
            else
            {
                return DescriptiveResponse<Boolean>.Error(response.Errors);
            }
        }

        [HttpPost]
        [Route("DeliveredWorkOrder")]
        public DescriptiveResponse<Boolean> DeliveredWorkOrder(EventWorkOrderDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.DeliveredWorkOrder(request);
            if (response.IsErrorState == false)
            {
                var states = _denormalizer.DenormalizeStates(response.Value);

                return states;
            }
            else
            {
                return DescriptiveResponse<Boolean>.Error(response.ErrorDescription);
            }
        }

        [HttpPost]
        [Route("CancelWorkOrder")]
        public DescriptiveResponse<Boolean> CancelWorkOrder(EventWorkOrderDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.CancelWorkOrder(request);

            if (!response.IsErrorState && response.Value.Any())
                return _denormalizer.DenormalizeStates(response.Value);
            else
                return DescriptiveResponse<Boolean>.Error(response.Errors);
        }

        [HttpPost]
        [Route("FailedToDeliver")]
        public DescriptiveResponse<Boolean> FailedToDeliver(EventWorkOrderDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.FailedToDeliver(request);
            if (response.IsErrorState == false)
            {
                var states = _denormalizer.DenormalizeStates(response.Value);

                return states;
            }
            else
            {
                return DescriptiveResponse<Boolean>.Error(response.Errors);
            }
        }

        [HttpPost]
        [Route("OnHold")]
        public DescriptiveResponse<Boolean> OnHold(EventWorkOrderDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.OnHold(request);
            if (response.IsErrorState == false)
            {
                var states = _denormalizer.DenormalizeStates(response.Value);

                return states;
            }
            else
            {
                return DescriptiveResponse<Boolean>.Error(response.Errors);
            }
        }

        [HttpPost]
        [Route("NotAssigned")]
        public DescriptiveResponse<Boolean> NotAssigned(EventWorkOrderDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.NotAssigned(request);
            if (response.IsErrorState == false)
            {
                var states = _denormalizer.DenormalizeStates(response.Value);

                return states;
            }
            else
            {
                return DescriptiveResponse<Boolean>.Error(response.Errors);
            }
        }

        [Route("AddComment")]
        [HttpPost]
        public DescriptiveResponse<Boolean> AddComment(WorkOrderCommentDTO dto)
        {
            OnActionExecuting();

            return _workOrderService.AddComment(dto);
        }

        [Route("DeleteComment")]
        [HttpDelete]
        public DescriptiveResponse<Boolean> DeleteComment(int id)
        {
            OnActionExecuting();

            return _workOrderService.DeleteComment(id);
        }

        [HttpPut]
        [ActionName("UpdateWorkOrderShipment")]
        public DescriptiveResponse<Boolean> UpdateWorkOrderShipment(EventWorkOrderDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.UpdateWorkOrder(request);
            if (response.IsErrorState == false)
            {
                var result = _denormalizer.DenormalizeStates(response.Value);
                return result;
            }
            else
            {
                return DescriptiveResponse<Boolean>.Error(response.Errors);
            }
        }

        [HttpPut]
        [ActionName("UpdateWorkOrderAssignRetrials")]
        public DescriptiveResponse<Boolean> UpdateWorkOrderAssignRetrials(long workOrderID, int holdInterval)
        {
            OnActionExecuting();

            return _workOrderService.UpdateWorkOrderAssignRetrials(workOrderID, holdInterval);
        }

        [HttpPut]
        [ActionName("UpdateWorkOrderCancelRetrials")]
        public DescriptiveResponse<Boolean> UpdateWorkOrderCancelRetrials(long workOrderID, int holdInterval)
        {
            OnActionExecuting();

            return _workOrderService.UpdateWorkOrderCancelRetrials(workOrderID, holdInterval);
        }

        [HttpPost]
        [Route("WOVehicleArrivedStation")]
        public DescriptiveResponse<Boolean> WOVehicleArrivedStation(WOVArrivedStationDTO request)
        {
            OnActionExecuting();

            var response = _workOrderService.WOVehicleArrivedStation(request);
            if (response.IsErrorState == false)
            {
                var result = _denormalizer.DenormalizeStates(response.Value);


            
                if(AssignInEntryGate)
                {
                    if (result.Value)
                    {
                        var workOrderToAssign = _workOrderService.GetMatchedWorkOrderToAssign(new Guid(request.VehicleID));

                        if (workOrderToAssign != null && workOrderToAssign.EventWorkOrderDTO != null && workOrderToAssign.EventWorkOrderDTO.WorkOrderID > 0)
                        {
                            var assignResponse = _workOrderService.AssignWorkOrder(workOrderToAssign);

                            if (!assignResponse.IsErrorState && assignResponse.Value.Any())
                            {
                                var states = _denormalizer.DenormalizeStates(assignResponse.Value);
                            }
                        }
                    }

                }
               

                return result;
            }
            else
            {
                return DescriptiveResponse<Boolean>.Error(response.ErrorDescription);
            }
        }

        [HttpPost]
        [Route("UpdateWorkOrdersStation")]
        public DescriptiveResponse<AddItemsResponse> UpdateWorkOrdersStation([FromUri]Guid stationID, [FromBody]WorkOrderSearchCriteriaDTO searchCriteria)
        {
            OnActionExecuting();

            searchCriteria.StatusIDs = searchCriteria.StatusIDs.Where(a => a == 1 || a == 2).ToList();

            var response = new DescriptiveResponse<AddItemsResponse>();
            response.Value = new AddItemsResponse
            {
                success = 0,
                failed = 0
            };
            //response.Value.success = 0;
            //response.Value.failed = 0;

            var workOrdersResult = _workOrderService.SearchWorkOrders(searchCriteria);

            if (workOrdersResult.Value != null && workOrdersResult.Value.TotalCount > 0)
            {
                foreach (var wo in workOrdersResult.Value.Result)
                {
                    //if (wo.LastStatusID == (int)WorkOrderStatusEnum.New)
                    //{
                    //    var onholdResponse = _workOrderService.OnHold(new EventWorkOrderDTO()
                    //    {
                    //        WorkOrderID = wo.WorkOrderID,
                    //        //StatusReasonID = (int)StatusReasonEnum.
                    //    });

                    //    if (!onholdResponse.IsErrorState && onholdResponse.Value.Any())
                    //    {
                    //        var onholdState = _denormalizer.DenormalizeStates(onholdResponse.Value);

                    //        if (onholdState.Value)
                    //        {
                    //            var updateResponse = _workOrderService.UpdateWorkOrder(new EventWorkOrderDTO()
                    //            {
                    //                WorkOrderID = wo.WorkOrderID,
                    //                OrderQuantity = wo.OrderQuantity,
                    //                ScheduledDeliveryTime = wo.ScheduledDeliveryTime.Value,
                    //                CustomerAccountId = wo.CustomerAccountID,
                    //                StationID = stationID
                    //            });

                    //            if (!updateResponse.IsErrorState && updateResponse.Value.Any())
                    //            {
                    //                var updateState = _denormalizer.DenormalizeStates(updateResponse.Value);

                    //                var newResponse = _workOrderService.NotAssigned(new EventWorkOrderDTO()
                    //                {
                    //                    WorkOrderID = wo.WorkOrderID
                    //                });

                    //                if (!newResponse.IsErrorState && newResponse.Value.Any())
                    //                {
                    //                    var newState = _denormalizer.DenormalizeStates(newResponse.Value);
                    //                }

                    //                response.Value.success += 1;
                    //            }
                    //            else
                    //                response.Value.failed += 1;
                    //        }
                    //    }
                    //}
                    //else if (wo.LastStatusID == (int)WorkOrderStatusEnum.Onhold)
                    //{
                        var updateResponse = _workOrderService.UpdateWorkOrder(new EventWorkOrderDTO()
                        {
                            WorkOrderID = wo.WorkOrderID,
                            OrderQuantity = wo.OrderQuantity,
                            ScheduledDeliveryTime = wo.ScheduledDeliveryTime.Value,
                            CustomerAccountId = wo.CustomerAccountID,
                            StationID = stationID
                        });

                        if (!updateResponse.IsErrorState && updateResponse.Value.Any())
                        {
                            var updateState = _denormalizer.DenormalizeStates(updateResponse.Value);

                            response.Value.success += 1;
                        }
                        else
                            response.Value.failed += 1;
                    //}
                }
            }

            return response;
        }

        [HttpPost]
        [Route("CancelSewerWorkOrdersReadyToCancel")]
        public DescriptiveResponse<Boolean> CancelSewerWorkOrdersReadyToCancel([FromUri] int? retrials, [FromUri] int? holdInterval)
        {
            OnActionExecuting();

            var response = _workOrderService.CancelSewerWorkOrdersReadyToCancel(retrials, holdInterval);

            if (!response.IsErrorState && response.Value.Any())
            {
                var states = _denormalizer.DenormalizeStates(response.Value);

                return states;
            }

            return DescriptiveResponse<bool>.Error(response.ErrorDescription);
        }

        [HttpPost]
        [Route("DeAssignSewerWorkOrdersAfterTimeout")]
        public DescriptiveResponse<Boolean> DeAssignSewerWorkOrdersAfterTimeout([FromUri] int period)
        {
            OnActionExecuting();

            var response = _workOrderService.DeAssignSewerWorkOrdersAfterTimeout(period);

            if (!response.IsErrorState)
            {
                if(response.Value.Any())
                {
                    var states = _denormalizer.DenormalizeStates(response.Value);
                    return states;
                }
                else
                {
                    return DescriptiveResponse<Boolean>.Success(false);
                }
           
            }

            return DescriptiveResponse<bool>.Error(response.ErrorDescription);
        }

        [HttpPost]
        [Route("ExitSewerVehicleAfterTimeout")]
        public DescriptiveResponse<List<bool>> ExitSewerVehicleAfterTimeout([FromUri] int period)
        {
            OnActionExecuting();

            var response = _workOrderService.ExitSewerVehicleAfterTimeout(period);

            if (!response.IsErrorState)
            {
                return DescriptiveResponse<List<bool>>.Success(response.Value);

            }

            return DescriptiveResponse<List<bool>>.Error(response.ErrorDescription);
        }

        [Route("UpdateHayatWorkOrderLog")]
        [HttpPost]
        public DescriptiveResponse<Boolean> UpdateHayatWorkOrderLog([FromBody] HayatWorkOrderLogDTO dto)
        {
            OnActionExecuting();

            return DescriptiveResponse<Boolean>
                .Try(() => _workOrderService.UpdateHayatWorkOrderLog(dto));
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
