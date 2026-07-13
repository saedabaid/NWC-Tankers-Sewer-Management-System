using NWC_CCB_Integration.BLL;
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using NWC_CCB_Integration.DTO.Helpers;
using NWC_CCB_Integration.DTO.Logger;
using NWC_CCB_Integration.DTO.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoqyaSchedules.WinService
{
    public partial class PushSoqyaSchedules : ServiceBase
    {
        private Timer serviceTimer;

        private int totalCount { get; set; }
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
        private int timerIntervalMins
        {
            get
            {
                int _interval;
                int.TryParse(ConfigurationManager.AppSettings["TimerIntervalMins"] != null ?
                    ConfigurationManager.AppSettings["TimerIntervalMins"] : string.Empty, out _interval);

                return _interval > 0 ? _interval : 10;
            }
        }

        private string csvFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["CSVFilePath"] != null ?
                    ConfigurationManager.AppSettings["CSVFilePath"] : string.Empty;
            }
        }
        #endregion

        public PushSoqyaSchedules()
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
                   
                   TimeSpan tsInterval = new TimeSpan( this.timerIntervalMins,0, 0);

                    serviceTimer = new Timer(new TimerCallback(PushSoqyaSchedule), null, tsInterval, tsInterval);

                    PushSoqyaSchedule(null);
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
                    ExceptionManager.GetExceptionLogger().LogInformation("---------------- Stopping Timer ----------------");
                    serviceTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    serviceTimer.Dispose();
                    serviceTimer = null;

                    this.totalCount = 0;
                    this.isFinished = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error - Stop Timer : " + ex.Message.ToString());
            }
        }

        public void PushSoqyaSchedule(object state)
        {
            List<int> WorkingHours =new List<int>() { 9,10,12,4 };
            if (this.totalCount == 0 && this.isFinished && WorkingHours.Contains( DateTime.Now.Hour))
            {
                try
                {
                    
                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg("---------------------------------------PushSoqyaSchedule - Starting---------------------------------------"));

                    this.isFinished = false;

                    var integrationBLL = new IntegrationBLL(null);
                    var userDTO = integrationBLL.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);

                    var soqyaSchedulesResponse = new DescriptiveResponse<SearchResult<SoqyaScheduleDTO>>();

                    if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                    {
                        var searchDate = DateTime.Now;
                        var startDate = new DateTime(searchDate.Year, searchDate.Month, searchDate.Day, 0, 0, 1);
                        var endDate = new DateTime(searchDate.Year, searchDate.Month, searchDate.Day, 23, 59, 59);

                        var searchCriteria = new SoqyaScheduleSC()
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                            PageFilter = new PageFilter() { PageSize = this.take, PageIndex = 1 }
                        };

                        soqyaSchedulesResponse = integrationBLL.GetSoqyaSchedules(this.queryAPI_URL, userDTO.Value.Token, searchCriteria).Result;

                        if (soqyaSchedulesResponse.Value != null && soqyaSchedulesResponse.Value.Result != null)
                        {
                            this.totalCount = soqyaSchedulesResponse.Value.TotalCount;

                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("PushSoqyaSchedule: Total Soqya Schedule Count: {0}", soqyaSchedulesResponse.Value.TotalCount)));

                            string fileName = $"TMS_CCB_SoqyaDailySchedule_{DateTime.Now.ToString("ddMMyyyy")}_{DateTime.Now.ToString("hhmmss")}.csv";
                            string fileFullName = $"{csvFilePath}{fileName}";
                            var csvWriter = new CsvFileWriter(fileFullName);

                            // Header
                            csvWriter.Write($"H,{DateTime.Now.ToString("dd-MM-yyyy")} {DateTime.Now.ToString("hh:mm:ss")}");
                            csvWriter.WriteLine();

                            // Data
                            foreach (var sch in soqyaSchedulesResponse.Value.Result)
                            {
                                var r = new CsvRow();
                                r.Add("D");
                                r.Add(sch.AccountId); // account id 
                                r.Add(sch.Quantity.ToString()); // tanker size
                                
                                csvWriter.WriteRow(r);

                                this.totalCount--;

                                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("PushSoqyaSchedule: AccountID: {0}, Quantity: {1}", sch.AccountId, sch.Quantity)));
                            }

                            // Footer
                            csvWriter.Write($"T,{soqyaSchedulesResponse.Value.TotalCount}");

                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("PushSoqyaSchedule: Total Soqya Schedule Count: {0}", soqyaSchedulesResponse.Value.TotalCount)));

                            // Close file
                            csvWriter.Flush();
                            csvWriter.Close();
                        }

                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("PushSoqyaSchedule: Total Soqya Schedule Count: 0")));

                        this.isFinished = true;
                    }
                    else
                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Authenticate User Fail")));

                    this.isFinished = true;
                }
                catch (Exception ex)
                {
                    LoggerManager.LogMsg(c => c.Log(ex));
                    //ExceptionManager.GetExceptionLogger().LogException(ex);
                }
                finally
                {
                    this.totalCount = 0;
                    this.isFinished = true;
                }
            }
        }
    }
}
