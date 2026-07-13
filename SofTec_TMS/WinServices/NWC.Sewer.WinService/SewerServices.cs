using NLog;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.Sewer.WinService.Managers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Timers;

namespace NWC.Sewer.WinService
{
    public class SewerServices
    {
        #region prop
        //Method Enable
        private readonly bool NewOrderEnabled;
        private readonly bool AssignOrderEnabled;
        private readonly bool DeAssignOrderEnabled;
        private readonly bool CancelOrderBasedOnAssignRetryCounterEnabled;
        private readonly bool CancelOrderBasedOnPushTimeEnabled;
        private readonly bool ExitVehicleEnabled;

        private System.Timers.Timer _timer;
        private Thread NewOrderThread, AssignOrderThread, CancelRetryPreAssignThread, CancelPushTimeThread, DeAssignOrderThread,ExitVehicleThread;

        //Sewer Setting Values
        private readonly int SewerPreAssignRetryCounter;
        private readonly int PushTimePeriodToCancelOrderInMinutes;
        private readonly int DeAssignTimePeriodInMinutes;
        private readonly int ExitVehicleInMinutes;

        private  string _token;
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
        #endregion prop

        #region ctor

        public SewerServices()
        {
            try
            {
                var loginDto = new AuthenticationManager().AuthenticateUser(authenticationAPI_URL, username, password);
                _token = loginDto.Value != null && !string.IsNullOrEmpty(loginDto.Value.Token) ? loginDto.Value.Token : null;

                if (_token != null)
                {
                    //Method Enable and Disable
                    this.NewOrderEnabled = KeyConfig.NewOrderEnabled;
                    this.AssignOrderEnabled = KeyConfig.AssignOrderEnabled;
                    
                    this.DeAssignOrderEnabled = KeyConfig.DeAssignOrderEnabled;
                    this.CancelOrderBasedOnAssignRetryCounterEnabled = KeyConfig.CancelOrderBasedOnAssignRetryCounterEnabled;
                    this.CancelOrderBasedOnPushTimeEnabled = KeyConfig.CancelOrderBasedOnPushTimeEnabled;

                    //Sewer Setting from WebConfig - or from database
                    this.SewerPreAssignRetryCounter = KeyConfig.SewerPreAssignRetryCounter;

                    //Exit Vehicle
                    this.ExitVehicleEnabled = KeyConfig.ExitVehicleEnabled;
                    this.ExitVehicleInMinutes = KeyConfig.ExitVehicleInMinutes;

                    this.PushTimePeriodToCancelOrderInMinutes = KeyConfig.PushTimePeriodToCancelOrderInMinutes;
                    this.DeAssignTimePeriodInMinutes = KeyConfig.DeAssignTimePeriodInMinutes;

                    //To set job interval in Minutes
                    var jobInterval = KeyConfig.WindowsServiceTimerIntervalMinutes;
                    _timer = new System.Timers.Timer(1000 * 60 * jobInterval) { AutoReset = true };
                    _timer.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);

                    RegisterLog(LogLevel.Info, "***** Sewer_Win_Services Started At " + DateTime.Now);
                }
                else
                {
                    RegisterLog(LogLevel.Error, "***** Sewer_Win_Services Login failed At " + DateTime.Now);
                    throw new Exception("Login failed.");
                }
            }
            catch (Exception ex)
            {
                RegisterLog(LogLevel.Error, "***** Error At  :" + DateTime.Now + " - " + ex.Message);
            }
        }

        #endregion ctor

        #region Timer Event and Setting

        public void Start()
        {
            _timer.Start();
            ServiceTimer_Tick(null, null);
        }

        public void Stop()
        {
            _timer.Stop();
        }

        #region Setting

        public void RegisterLog(LogLevel level, string message)
        {
            LogManager.GetLogger("Sewer.WinService").Log(level, message);
        }

        public void ServiceTimer_Tick(object sender, ElapsedEventArgs e)
        {
            var loginDto = new AuthenticationManager().AuthenticateUser(authenticationAPI_URL, username, password);
            _token = loginDto.Value != null && !string.IsNullOrEmpty(loginDto.Value.Token) ? loginDto.Value.Token : null;
            RegisterLog(LogLevel.Info, "***** Timer Tick Started At " + DateTime.Now);

            //if (NewOrderEnabled && (NewOrderThread == null || !NewOrderThread.IsAlive))
            //{
            //    NewOrderThread = new Thread(new ThreadStart(NewOrderThreadMethod));
            //    NewOrderThread.Start();
            //}
            //if (AssignOrderEnabled && (AssignOrderThread == null || !AssignOrderThread.IsAlive))
            //{
            //    AssignOrderThread = new Thread(new ThreadStart(DeAssignOrderThreadMethod));
            //    AssignOrderThread.Start();
            //}

            if (CancelOrderBasedOnAssignRetryCounterEnabled && (CancelRetryPreAssignThread == null || !CancelRetryPreAssignThread.IsAlive))
            {
                CancelRetryPreAssignThread = new Thread(new ThreadStart(CancelOrderBasedSewerPreAssignRetryCounterThreadMethod));
                CancelRetryPreAssignThread.Start();
            }
            if (CancelOrderBasedOnPushTimeEnabled && (CancelPushTimeThread == null || !CancelPushTimeThread.IsAlive))
            {
                CancelPushTimeThread = new Thread(new ThreadStart(CancelOrderBasedOnPushTimeThreadMethod));
                CancelPushTimeThread.Start();
            }
            if (DeAssignOrderEnabled && (DeAssignOrderThread == null || !DeAssignOrderThread.IsAlive))
            {
                DeAssignOrderThread = new Thread(new ThreadStart(DeAssignOrderThreadMethod));
                DeAssignOrderThread.Start();
            }
            RegisterLog(LogLevel.Info, string.Format("Awies ExitVehicleEnabled : {0}", ExitVehicleEnabled));
            RegisterLog(LogLevel.Info, string.Format("Awies ExitVehicleThread is null : {0}", ExitVehicleThread = null));
         
            if (ExitVehicleEnabled && (ExitVehicleThread == null || !ExitVehicleThread.IsAlive))
            {
                ExitVehicleThread = new Thread(new ThreadStart(ExitSewerVehicleMethod));
                ExitVehicleThread.Start();

            }
        }

        #endregion Setting

        #endregion Timer Event and Setting

        #region New Order | PreAssign Order = Methods  -- Not Used
        /// <summary>
        /// This method for get new order and push it to all driver using [SignalR] except preAssign driver
        /// </summary>
        //public async void NewOrderThreadMethod()
        //{
        //    try
        //    {
        //        RegisterLog(LogLevel.Info, "|--- Started At " + DateTime.Now + " --  Call NewOrderThreadMethod");
        //        var resultWorkOrders = new WorkOrderManager(_token).GetSewerNewWorkOrdersWithZoneDetails(queryAPI_URL);
        //        if (!resultWorkOrders.IsErrorState && resultWorkOrders.Value != null)
        //        {
        //            var _GISManager = new GISManager(_token);
        //            foreach (var order in resultWorkOrders.Value)
        //            {
        //                var transporters = _GISManager.GetAvailableTransportersInZone(queryAPI_URL, order.ZoneId, order.ZoneLatitude, order.ZoneLongitude);
        //                if (transporters?.Result?.Value != null)
        //                {
        //                    IEnumerable<string> driverIds = transporters.Result.Value.Select(x => x.Id?.ToString());
        //                    //await _driversHub.SendWorkOrder(driverIds.ToList(), order.OrderNumber);
        //                }
        //            }
        //        }
        //        else throw new Exception(resultWorkOrders.ErrorDescription);
        //    }
        //    catch (Exception ex)
        //    {
        //        RegisterLog(LogLevel.Error, "***** Error :" + ex.Message);
        //    }
        //    RegisterLog(LogLevel.Info, "|--- Ended At " + DateTime.Now + " -- Call NewOrderThreadMethod");
        //}
        #endregion

        #region preAssign Order
        /// <summary>
        /// This method for preAssign new order to driver and hide order from all driver [SignalR]
        /// </summary>
        public void DeAssignOrderThreadMethod()
        {
            try
            {
                RegisterLog(LogLevel.Info, "|--- Started At " + DateTime.Now + " --  Call DeAssignOrderThreadMethod");
                
                //DeAssign Process - DeAssign Order [GetWorkOrder status Assign and lastStatusTime >  and Change its status to new and driver to parking
                //GetAssignOrder and then DeAssing
                var workOrderManager = new WorkOrderManager(_token);
                var resultWorkOrders = workOrderManager.DeAssignSewerWorkOrdersAfterTimeout(commandAPI_URL, DeAssignTimePeriodInMinutes);
                RegisterLog(LogLevel.Info,string.Format( "IsErrorState{0}" , resultWorkOrders.IsErrorState));
                RegisterLog(LogLevel.Info, string.Format("commandAPI_URL{0}", commandAPI_URL));
                RegisterLog(LogLevel.Info, string.Format("DeAssignTimePeriodInMinutes{0}", DeAssignTimePeriodInMinutes));
                if (resultWorkOrders != null && !resultWorkOrders.IsErrorState && resultWorkOrders.Value)
                    RegisterLog(LogLevel.Info, "***** Orders de-assigned.");
                else
                    RegisterLog(LogLevel.Info, "***** No orders de-assigned.");
            }
            catch (Exception ex)
            {
                RegisterLog(LogLevel.Error, "***** Error :" + ex.Message);
            }
            RegisterLog(LogLevel.Info, "|--- Ended At " + DateTime.Now + " -- Call DeAssignOrderThreadMethod");
        }

        public void ExitSewerVehicleMethod()
        {
            try
            {
                RegisterLog(LogLevel.Info, "|--- Started At " + DateTime.Now + " --  Call ExitSewerVehicleMethod");

                //DeAssign Process - DeAssign Order [GetWorkOrder status Assign and lastStatusTime >  and Change its status to new and driver to parking
                //GetAssignOrder and then DeAssing
                var workOrderManager = new WorkOrderManager(_token);
                var resultWorkOrders = workOrderManager.ExitSewerVehicleAfterTimeout(commandAPI_URL, ExitVehicleInMinutes);
                RegisterLog(LogLevel.Info, string.Format("IsErrorState{0}", resultWorkOrders.IsErrorState));
                RegisterLog(LogLevel.Info, string.Format("commandAPI_URL{0}", commandAPI_URL));
                RegisterLog(LogLevel.Info, string.Format("ExitSewerVehicleMethod{0}", ExitVehicleInMinutes));
                if (resultWorkOrders != null && !resultWorkOrders.IsErrorState && resultWorkOrders.Value.Count>0)
                    RegisterLog(LogLevel.Info,string.Format( "***** {0} vehicle passed from {1}", resultWorkOrders.Value.Where(x=>x).Count(), resultWorkOrders.Value.Count));
                else
                    RegisterLog(LogLevel.Info, "***** No  vehicle passed.");
            }
            catch (Exception ex)
            {
                RegisterLog(LogLevel.Error, "***** Error :" + ex.Message);
            }
            RegisterLog(LogLevel.Info, "|--- Ended At " + DateTime.Now + " -- Call ExitSewerVehicleMethod");
        }
        #endregion

        #region Cancel Project - based on retry preAssing and based on push time

        /// <summary>
        /// This method for cancel order based on SewerPreAssignRetryCounter
        /// </summary>
        public void CancelOrderBasedSewerPreAssignRetryCounterThreadMethod()
        {
            try
            {
                if (CancelOrderBasedOnAssignRetryCounterEnabled)
                {
                    RegisterLog(LogLevel.Info, "|--- Started At " + DateTime.Now + " --  Call CancelOrderBasedSewerPreAssignRetryCounterThreadMethod");
                   
                    //if (No of pre assign based on NWC_WorkOrder_Log) > Config (SewerPreAssignRetryCounter)
                    //Then change its status to cancel
                    var workOrderManager = new WorkOrderManager(_token);
                    var resultWorkOrders = workOrderManager.CancelSewerWorkOrdersReadyToCancel(commandAPI_URL, SewerPreAssignRetryCounter, null);
                    if (resultWorkOrders != null && !resultWorkOrders.IsErrorState && resultWorkOrders.Value)
                        RegisterLog(LogLevel.Info, "***** Orders cancelled.");
                    else
                        RegisterLog(LogLevel.Info, "***** No orders cancelled.");
                }
            }
            catch (Exception ex)
            {
                RegisterLog(LogLevel.Error, "***** Error :" + ex.Message);
            }
            RegisterLog(LogLevel.Info, "|--- Ended At " + DateTime.Now + " -- Call CancelOrderBasedSewerPreAssignRetryCounterThreadMethod");
        }

        /// <summary>
        /// This method for cancel order based on PushTimePeriodToCancelOrderInMinutes
        /// </summary>
        public void CancelOrderBasedOnPushTimeThreadMethod()
        {
            try
            {
                if (CancelOrderBasedOnPushTimeEnabled)
                {
                    RegisterLog(LogLevel.Info, "|--- Started At " + DateTime.Now + " --  Call CancelOrderBasedOnPushTimeThreadMethod");
                    
                    //if (Time now - Fist Push Time) > Config (PushTimePeriodToCancelOrderInMinutes)
                    //Then change its status to cancel
                    var workOrderManager = new WorkOrderManager(_token);
                    var resultWorkOrders = workOrderManager.CancelSewerWorkOrdersReadyToCancel(commandAPI_URL, null, PushTimePeriodToCancelOrderInMinutes);
                    if (resultWorkOrders != null && !resultWorkOrders.IsErrorState && resultWorkOrders.Value)
                        RegisterLog(LogLevel.Info, "***** Orders cancelled.");
                    else
                        RegisterLog(LogLevel.Info, "***** No orders cancelled.");
                }
            }
            catch (Exception ex)
            {
                RegisterLog(LogLevel.Error, "***** Error :" + ex.Message);
            }
            RegisterLog(LogLevel.Info, "|--- Ended At " + DateTime.Now + " -- Call CancelOrderBasedOnPushTimeThreadMethod");
        }
        #endregion
    }
}
