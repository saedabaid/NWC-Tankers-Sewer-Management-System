using Newtonsoft.Json;
using NWC_CCB_Integration.BLL;
using NWC_CCB_Integration.BLL.Validators;
using NWC_CCB_Integration.DAL;
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using NWC_CCB_Integration.DTO.Logger;
using NWC_CCB_Integration.DTO.Models;
using NWC_CCB_Integration.DTO.Resources;
using NWC_CCB_Integration.DTO.Wrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NWC.Integration.WinService
{
    public partial class IntegrationService : ServiceBase
    {
        private Timer serviceTimer;

        private int ordersCount { get; set; }
        private bool isFinished { get; set; }

        #region Configuration
        private string authenticationAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthenticationAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["AuthenticationAPI_URL"] : string.Empty;
            }
        }

        private string commandAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["CommandAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["CommandAPI_URL"] : string.Empty;
            }
        }

        private string queryAPI_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["QueryAPI_URL"] != null ?
                    ConfigurationManager.AppSettings["QueryAPI_URL"] : string.Empty;
            }
        }

        private string username
        {
            get
            {
                return ConfigurationManager.AppSettings["UserName"] != null ?
                    ConfigurationManager.AppSettings["UserName"] : string.Empty;
            }
        }

        private string password
        {
            get
            {
                return ConfigurationManager.AppSettings["Password"] != null ?
                    ConfigurationManager.AppSettings["Password"] : string.Empty;
            }
        }

        private int take
        {
            get
            {
                int _take;
                int.TryParse(ConfigurationManager.AppSettings["Take"] != null ?
                    ConfigurationManager.AppSettings["Take"] : string.Empty, out _take);

                return _take > 0 ? _take : 10;
            }
        }
        private int timerIntervalSecs
        {
            get
            {
                int _interval;
                int.TryParse(ConfigurationManager.AppSettings["TimerIntervalSecs"] != null ?
                    ConfigurationManager.AppSettings["TimerIntervalSecs"] : string.Empty, out _interval);

                return _interval > 0 ? _interval : 10;
            }
        }

        private int retrials
        {
            get
            {
                int _retrials;
                int.TryParse(ConfigurationManager.AppSettings["Retrials"] != null ?
                    ConfigurationManager.AppSettings["Retrials"] : string.Empty, out _retrials);

                return _retrials;
            }
        }

        private int holdInterval
        {
            get
            {
                int _holdInterval;
                int.TryParse(ConfigurationManager.AppSettings["HoldInterval"] != null ?
                    ConfigurationManager.AppSettings["HoldInterval"] : string.Empty, out _holdInterval);

                return _holdInterval;
            }
        }

        private long defaultZoneID
        {
            get
            {
                long zoneID;

                long.TryParse(ConfigurationManager.AppSettings["DefaultZoneID"] != null ?
                    ConfigurationManager.AppSettings["DefaultZoneID"] : string.Empty, out zoneID);

                return zoneID;
            }
        }

        private int defaultPriorityID
        {
            get
            {
                int priorityID;

                int.TryParse(ConfigurationManager.AppSettings["DefaultPriorityID"] != null ?
                    ConfigurationManager.AppSettings["DefaultPriorityID"] : string.Empty, out priorityID);

                return priorityID;
            }
        }

        private int defaultCategoryID
        {
            get
            {
                int catID;

                int.TryParse(ConfigurationManager.AppSettings["DefaultCategoryID"] != null ?
                    ConfigurationManager.AppSettings["DefaultCategoryID"] : string.Empty, out catID);

                return catID;
            }
        }

        private int defaultStatusID
        {
            get
            {
                int catID;

                int.TryParse(ConfigurationManager.AppSettings["DefaultStatusID"] != null ?
                    ConfigurationManager.AppSettings["DefaultStatusID"] : string.Empty, out catID);

                return catID;
            }
        }

        private string sourceApplication
        {
            get
            {
                return ConfigurationManager.AppSettings["SOURCEAPPLICATION"] != null ?
                    ConfigurationManager.AppSettings["SOURCEAPPLICATION"] : string.Empty;
            }
        }
        #endregion

        #region Properties
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> Accessories { get; set; }
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> ServiceTypes { get; set; }
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> CL_Priorities { get; set; }
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> CL_Categories { get; set; }
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> CL_Statuses { get; set; }
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> WorkOrderStatus { get; set; }
        #endregion

        public IntegrationService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Task.Delay(10000);

                if (serviceTimer == null)
                {
                    this.isFinished = true;

                    TimeSpan tsInterval = new TimeSpan(0, 0, this.timerIntervalSecs);

                    serviceTimer = new Timer(new TimerCallback(DoWork), null, tsInterval, tsInterval);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - Start Timer : " + ex.Message.ToString());
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (serviceTimer != null)
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg("---------------- Stopping Timer ----------------"));
                    serviceTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    serviceTimer.Dispose();
                    serviceTimer = null;

                    this.ordersCount = 0;
                    this.isFinished = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - Stop Timer : " + ex.Message.ToString());
            }
        }

        public void DoWork(object state)
        {
            if (this.ordersCount == 0 && this.isFinished)
            {
                try
                {
                    this.isFinished = false;

                    var integrationBLL = new IntegrationBLL(null);
                    var userDTO = integrationBLL.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);

                    var workOrderRequestList = new List<NWC_Int_ObjectStatus>();

                    if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                    {
                        var statusIDs = new List<int>() { (int)OrderRequestStatuEnum.Pending, (int)OrderRequestStatuEnum.Fail };
                        workOrderRequestList = integrationBLL.GetObjectStatusList(statusIDs, this.retrials, this.take);

                        if (workOrderRequestList != null)
                        {
                            this.ordersCount = workOrderRequestList.Count;

                            if (workOrderRequestList.Any())
                                LoadLookups(userDTO.Value.Token);

                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("***************** Integration: Total WorkOrder Request Count: {0}", this.ordersCount)));

                            foreach (var orderRequest in workOrderRequestList)
                            {
                                if (orderRequest.OperationTypeID == (int)OperationTypeEnum.CreateWorkOrder)
                                {
                                    CreateWorkOrder(orderRequest, userDTO.Value.Token);
                                }
                                //else if (orderRequest.OperationTypeID == (int)OperationTypeEnum.UpdateWorkOrder)
                                //{
                                //    UpdateWorkOrder(orderRequest, userDTO.Value.Token);
                                //}

                                this.ordersCount--;
                                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("***************** Integration: Total WorkOrder Request Count: {0}", this.ordersCount)));
                            }
                        }

                        this.isFinished = true;
                    }

                    this.isFinished = true;
                }
                catch (Exception ex)
                {
                    LoggerManager.LogMsg(c => c.Log(ex));
                }
                finally
                {
                    this.ordersCount = 0;
                    this.isFinished = true;
                }
            }
        }

        //private void UpdateWorkOrder(NWC_Int_ObjectStatus orderRequest, string token)
        //{
        //    int opTypeID = (int)OperationTypeEnum.UpdateWorkOrderInfo;
        //    int opStepID = (int)OperationStepEnum.ParsingXML;
        //    string orderNumber = orderRequest.OrderNumber;
        //    //string requestToken = orderRequest.Token;

        //    var integrationBLL = new IntegrationBLL(null);

        //    try
        //    {
        //        //parse orderRequest.DTO to DTO object
        //        bool isChangeStatus;
        //        var requestDTO = integrationBLL.ParsingXMLForUpdatingWO(orderRequest.XML, out isChangeStatus);

        //        if (isChangeStatus)
        //            ChangeWorkOrderStatus(integrationBLL, orderRequest, requestDTO, token);
        //    }
        //    catch (Exception ex)
        //    {
        //        integrationBLL.SaveLogOperation(opTypeID, opStepID, orderNumber, null, token);
        //        ExceptionManager.GetExceptionLogger().LogException(ex);
        //    }
        //}

        //private void ChangeWorkOrderStatus(IntegrationBLL integrationBLL, NWC_Int_ObjectStatus orderRequest, EventWorkOrderDTO requestDTO, string token)
        //{
        //    long integrationID = orderRequest.ID;
        //    int opTypeID = (int)OperationTypeEnum.ChangeWorkOrderStatus;
        //    int opStepID = (int)OperationStepEnum.ParsingXML;
        //    string orderNumber = orderRequest.OrderNumber;
        //    //string requestToken = orderRequest.Token;
        //    object objLogDTO = requestDTO;
        //    int workOrderStatusID = 0;
        //    int statusReasonID = 0;

        //    try
        //    {
        //        if (orderRequest.DTO == null)
        //        {
        //            integrationBLL.UpdateObjectIntegrationStatus(orderRequest.ID, objLogDTO);
        //            integrationBLL.SaveLogOperation(opTypeID, opStepID, orderNumber, objLogDTO, token);
        //        }

        //        workOrderStatusID = this.WorkOrderStatus.Value.FirstOrDefault(x => x.IntegrationId == requestDTO.StatusID.ToString()).Id;

        //        var statusReasons = integrationBLL.GetReasonsByStatusId(this.queryAPI_URL, workOrderStatusID, token);
        //        statusReasonID = statusReasons.Value.FirstOrDefault(x => x.IntegrationId == requestDTO.StatusReasonID.ToString()).Id;

        //        var eventWorkOrderDTO = new EventWorkOrderDTO()
        //        {
        //            WorkOrderID = requestDTO.WorkOrderID,
        //            OrderNumber = requestDTO.OrderNumber,
        //            StatusID = workOrderStatusID,
        //            StatusReasonID = statusReasonID,
        //            StatusTime = requestDTO.StatusTime,
        //            StatusComment = requestDTO.StatusComment
        //        };

        //        opStepID = (int)OperationStepEnum.DTOMapping;
        //        objLogDTO = requestDTO;

        //        integrationBLL.SaveLogOperation(opTypeID, opStepID, orderNumber, objLogDTO, token);

        //        var changeWorkOrderStatusResponse = integrationBLL.ChangeWorkOrderStatus(this.commandAPI_URL, token, eventWorkOrderDTO);

        //        //update ObjectIntegrationStatus
        //        if (changeWorkOrderStatusResponse.Value)
        //            integrationBLL.UpdateObjectIntegrationStatus(integrationID, (int)OrderRequestStatuEnum.Success, null);
        //        else
        //        {
        //            objLogDTO = changeWorkOrderStatusResponse;
        //            integrationBLL.SaveLogOperation(opTypeID, opStepID, orderNumber, objLogDTO, token);
        //            integrationBLL.UpdateObjectIntegrationStatus(integrationID, (int)OrderRequestStatuEnum.Fail, null);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        integrationBLL.UpdateObjectIntegrationStatus(integrationID, (int)OrderRequestStatuEnum.Fail, null);
        //        integrationBLL.SaveLogOperation(opTypeID, opStepID, orderNumber, null, token);
        //        ExceptionManager.GetExceptionLogger().LogException(ex);
        //    }
        //}

        private void CreateWorkOrder(NWC_Int_ObjectStatus orderRequest, string token)
        {
            var workOrderRequestDTO = new WorkOrderRequestDTO();
            long integrationID = orderRequest.ID;
            string orderNumber = orderRequest.OrderNumber;
            string zoneIntID = string.Empty;
            var failureMessage = string.Empty;
            //string requestToken = orderRequest.Token;
            //long zoneID = 0;

            var integrationBLL = new IntegrationBLL(null);

            try
            {
                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg("---------------------------------------------------------------------------------------------------------------------------"));

                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format(">>>>>>>>>>>>>>>>> Start Create Order: {0} >>>>>>>>>>>>>>>>>", orderNumber)));

                //Parse xml to dto
                workOrderRequestDTO = JsonConvert.DeserializeObject<WorkOrderRequestDTO>(orderRequest.DTO);

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.Deserialize_WorkOrderRequestDTO, JsonConvert.SerializeObject(workOrderRequestDTO))));

                // Validations
                var validator = new CreateWorkOrderValidator(false);

                var results = validator.Validate(workOrderRequestDTO);

                if (!results.IsValid)
                {
                    failureMessage = string.Join(",", results.Errors.Select(s => s.ErrorMessage));

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - Validations_Errors: {0}", JsonConvert.SerializeObject(failureMessage))));

                    var addedDeferredOrder = integrationBLL.AddDeferredWorkOrder(workOrderRequestDTO, failureMessage);

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - Validations_Errors - DeferredWorkOrder_ADDED: {0}", addedDeferredOrder)));

                    return;
                }

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - validate_DTO: {0}", results.IsValid)));

                if (orderRequest.DTO == null)
                {
                    workOrderRequestDTO = integrationBLL.ParsingXMLForCreatingWO(orderRequest.XML);

                    integrationBLL.UpdateObjectIntegrationStatus(orderRequest.ID, workOrderRequestDTO);

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.ParsingXML, JsonConvert.SerializeObject(workOrderRequestDTO))));
                }

                var eventWorkOrderDTO = workOrderRequestDTO.WrapToEventWorkOrderDTO(this.Accessories.Value, this.ServiceTypes.Value);

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.DTOMapping, JsonConvert.SerializeObject(eventWorkOrderDTO))));

                #region GIS / DeferredOrder
                if (workOrderRequestDTO.ZoneID == 0)
                {
                    zoneIntID = integrationBLL.CallGISService(workOrderRequestDTO.PremiseCoordinates, orderNumber, this.sourceApplication, workOrderRequestDTO.TransactionID);

                    if (zoneIntID == null || string.IsNullOrEmpty(zoneIntID) || (!string.IsNullOrEmpty(zoneIntID) && zoneIntID.ToUpper() == "NOCOVERAGE"))
                    {
                        failureMessage = "Zone is not configured in GIS";

                        // Log
                        LoggerManager.LogMsg(c => c.TrackingMsg(failureMessage));

                        integrationBLL.UpdateObjectIntegrationStatus(integrationID, (int)OrderRequestStatuEnum.Fail, failureMessage);

                        //var addedDeferredOrder = integrationBLL.AddDeferredWorkOrder(workOrderRequestDTO, failureMessage);

                        //// Log
                        //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("Zone is not configured in GIS - DeferredWorkOrder_ADDED: {0}", addedDeferredOrder));

                        return;
                    }
                    else
                    {
                        // Log
                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - GetZoneByIntegrationID: {0}", zoneIntID)));

                        var zoneResult = integrationBLL.GetZoneByIntegrationID(this.queryAPI_URL, token, zoneIntID).Result;

                        if (zoneResult.Value == null || (zoneResult.Value != null && zoneResult.Value.ID == 0))
                        {
                            failureMessage = string.Format("Zone is not covered in TMS: {0}", zoneIntID);

                            // Log
                            LoggerManager.LogMsg(c => c.TrackingMsg(failureMessage));

                            integrationBLL.UpdateObjectIntegrationStatus(integrationID, (int)OrderRequestStatuEnum.Fail, failureMessage);

                            //var addedDeferredOrder = integrationBLL.AddDeferredWorkOrder(workOrderRequestDTO, failureMessage);

                            //// Log
                            //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("Zone is not covered in TMS: {0} - DeferredWorkOrder_ADDED: {0}", zoneIntID, addedDeferredOrder));

                            return;
                        }
                        else
                        {
                            eventWorkOrderDTO.ZoneID = zoneResult.Value.ID;
                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - GetZoneByIntegrationID - ZoneID: {0}", zoneResult.Value.ID)));
                        }
                    }
                }
                #endregion

                #region Customer / CustomerLocation / CustomerAccount
                if (eventWorkOrderDTO.CustomerLocationID == 0)
                {
                    var customerBalanceResponse = integrationBLL.CreateCustomerBalance(workOrderRequestDTO, eventWorkOrderDTO.ZoneID, 
                         this.defaultPriorityID, this.defaultCategoryID, this.defaultStatusID, eventWorkOrderDTO.ServiceTypeID, this.commandAPI_URL, token).Result;

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.AfterCreateCustomer, JsonConvert.SerializeObject(customerBalanceResponse))));

                    if (customerBalanceResponse.Value == null || customerBalanceResponse.Value.Account == null)
                    {
                        failureMessage = JsonConvert.SerializeObject(customerBalanceResponse.Errors);
                        integrationBLL.UpdateObjectIntegrationStatus(integrationID, (int)OrderRequestStatuEnum.Fail, failureMessage);

                        var addedDeferredOrder = integrationBLL.AddDeferredWorkOrder(workOrderRequestDTO, failureMessage);

                        // Log
                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - Fail-To-Create-CustomerBalance - DeferredWorkOrder_ADDED: {0}, ZoneID: {1}", addedDeferredOrder, eventWorkOrderDTO.ZoneID))) ;

                        return;
                    }

                    eventWorkOrderDTO.CustomerAccountId = customerBalanceResponse.Value.Account.ID;
                }
                #endregion

                #region Zone Station
                if ((eventWorkOrderDTO.CustomerAccountId.HasValue && eventWorkOrderDTO.CustomerAccountId.Value > 0) && 
                    (eventWorkOrderDTO.StationID == null || eventWorkOrderDTO.StationID == Guid.Empty))
                {
                    var zoneStationResponse = integrationBLL.GetZoneStation(this.queryAPI_URL, token, eventWorkOrderDTO.CustomerAccountId.Value).Result;

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.GettingMainZoneStation, JsonConvert.SerializeObject(zoneStationResponse))));

                    if (zoneStationResponse.Value == null && !zoneStationResponse.Value.Any())
                    {
                        failureMessage = JsonConvert.SerializeObject(zoneStationResponse);
                        integrationBLL.UpdateObjectIntegrationStatus(integrationID, (int)OrderRequestStatuEnum.Fail, failureMessage);

                        var addedDeferredOrder = integrationBLL.AddDeferredWorkOrder(workOrderRequestDTO, failureMessage);

                        // Log
                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - Fail-To-Get-MainZoneStation - DeferredWorkOrder_ADDED: {0} - ZoneID: {1}", addedDeferredOrder, eventWorkOrderDTO.ZoneID)));

                        return;
                    }

                    eventWorkOrderDTO.StationID = zoneStationResponse.Value.FirstOrDefault().Id;
                }
                #endregion

                #region Create WorkOrder
                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.BeforeCreateWorkOrder, JsonConvert.SerializeObject(eventWorkOrderDTO))));

                var createWorkOrderResponse = integrationBLL.CreateWorkOrder(this.commandAPI_URL, token, eventWorkOrderDTO).Result;

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.AfterCreateWorkOrder, JsonConvert.SerializeObject(createWorkOrderResponse))));

                //update ObjectIntegrationStatus
                if (createWorkOrderResponse.Value != null && createWorkOrderResponse.Value.WorkOrderID > 0)
                {
                    integrationBLL.UpdateObjectIntegrationStatus(integrationID, (int)OrderRequestStatuEnum.Success, null);

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.CreateWorkOrderSuccess, JsonConvert.SerializeObject(createWorkOrderResponse))));

                    //Update CC&B service
                    var updateResult = integrationBLL.CallUpdateWONotificationService(eventWorkOrderDTO.OrderNumber, eventWorkOrderDTO.CISDivision, eventWorkOrderDTO.TransactionID, token);
                }
                else
                {
                    var errorList = new List<string>();

                    foreach (var error in createWorkOrderResponse.Errors)
                    {
                        var errorMsg = Properties.NWCResources.ResourceManager.GetString(error);
                        errorList.Add(errorMsg);
                    }

                    failureMessage = string.Format("{0} - ZoneIntegrationID: {1} - Errors: {2}", OperationStepEnum.CreateWorkOrderFail, zoneIntID, JsonConvert.SerializeObject(errorList));

                    integrationBLL.UpdateObjectIntegrationStatus(integrationID, (int)OrderRequestStatuEnum.Fail, failureMessage);

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(failureMessage));

                    var addedDeferredOrder = integrationBLL.AddDeferredWorkOrder(workOrderRequestDTO, failureMessage);

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - Fail-To-Create-WorkOrder - DeferredWorkOrder_ADDED: {0}", addedDeferredOrder)));
                }
                #endregion
            }
            catch (Exception ex)
            {
                failureMessage = JsonConvert.SerializeObject(ex);
                integrationBLL.UpdateObjectIntegrationStatus(integrationID, (int)OrderRequestStatuEnum.Fail, failureMessage);

                var addedDeferredOrder = integrationBLL.AddDeferredWorkOrder(workOrderRequestDTO, failureMessage);

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1} - DeferredWorkOrder_ADDED: {2}", OperationStepEnum.CreateWorkOrderException, JsonConvert.SerializeObject(ex), addedDeferredOrder)));

                LoggerManager.LogMsg(c => c.Log(ex));
            }
        }

        private void LoadLookups(string token)
        {
            try
            {
                var integrationBLL = new IntegrationBLL(null);

                this.Accessories = integrationBLL.GetAccessories(this.queryAPI_URL, token);
                this.ServiceTypes = integrationBLL.GetServiceTypes(this.queryAPI_URL, token);

                this.CL_Priorities = integrationBLL.GetCustomerLocationPriorities(this.queryAPI_URL, token);
                this.CL_Categories = integrationBLL.GetCustomerLocationCategories(this.queryAPI_URL, token);
                this.CL_Statuses = integrationBLL.GetCustomerLocationStatuses(this.queryAPI_URL, token);

                this.WorkOrderStatus = integrationBLL.GetWorkOrderStatuses(this.queryAPI_URL, token);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
            }
        }
    }
}
