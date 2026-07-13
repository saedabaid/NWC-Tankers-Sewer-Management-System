using Logger;
using Newtonsoft.Json;
using NWC_CCB_Integration.BLL;
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using NWC_CCB_Integration.DTO.Helpers;
using NWC_CCB_Integration.DTO.Logger;
using NWC_CCB_Integration.DTO.Models;
using NWC_CCB_Integration.DTO.Output;
using System;
using NLog;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using NLog.Fluent;

namespace FromCCBToNWC.WCF
{
    // NoTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Tanker" in code, svc and config file together.
    // NoTE: In order to launch WCF Test Client for testing this service, please select Tanker.svc or Tanker.svc.cs at the Solution Explorer and start debugging.
    public class Tanker : ITanker
    {
    
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

        private int defaultClassID
        {
            get
            {
                int classID;

                int.TryParse(ConfigurationManager.AppSettings["DefaultClassID"] != null ?
                    ConfigurationManager.AppSettings["DefaultClassID"] : string.Empty, out classID);

                return classID;
            }
        }

        private int defaultServiceTypeID
        {
            get
            {
                int serviceTypeID;

                int.TryParse(ConfigurationManager.AppSettings["DefaultServiceTypeID"] != null ?
                    ConfigurationManager.AppSettings["DefaultServiceTypeID"] : string.Empty, out serviceTypeID);

                return serviceTypeID;
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

        public NWC_CCB_Integration.DTO.Models.AvailableTankerSize.Output GetAvailableTankerSizes(NWC_CCB_Integration.DTO.Models.AvailableTankerSize.Schema schema)
        {
            var response = new NWC_CCB_Integration.DTO.Models.AvailableTankerSize.Output();
            var result = new SearchResult<AvailableTankerSizesDTO>();
            var integrationBLL = new IntegrationBLL(null);
            var SizesStringToLog = "";
            var zoneIntID = "";
            DateTime TimeBeforeGIS = DateTime.Now; ;
            DateTime TimeAfterGIS = DateTime.Now; ;

            try
            {
   
                var userDTO = integrationBLL.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);
        
                if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                {
                    LoggerManager.LogMsg(c => c.TrackingMsg("---------------------------------------GetAvailableTankerSizes - Starting---------------------------------------"));
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("GetAvailableTankerSizesRequest:CISDIVISION:{0}, LONGLAT:{1}", schema.Input.CISDIVISION, schema.Input.LONGLAT)));
                    TimeBeforeGIS = DateTime.Now;
                    zoneIntID = integrationBLL.CallGISService(schema.Input.LONGLAT, string.Empty, this.sourceApplication, schema.Input.TRANSACTIONID);
                    TimeAfterGIS = DateTime.Now;
                    if (schema.Input.TANKERTYPE == "SEWAGE")
                    {
                        response.TANKERSIZES = new List<NWC_CCB_Integration.DTO.Models.AvailableTankerSize.TankerSize> { new NWC_CCB_Integration.DTO.Models.AvailableTankerSize.TankerSize { TANKERSIZEVALUE=32,TANKERPRICE=0} };

                        response.COVERAGESTATUS = "YES";

                        response.STATUS = ResponseStatusEnum.OK.ToString();
                        response.RESPONSECODE = "0";
                        response.RESPONSEDESCRIPTION = "Data Retrieved Successfully";
                        response.ERRORCODE = null;
                        response.ERRORDESCRIPTION = null;
                    }
                    else if (zoneIntID == null || string.IsNullOrEmpty(zoneIntID) || (!string.IsNullOrEmpty(zoneIntID) && zoneIntID.ToUpper() == "NoCOVERAGE"))
                    {
                        //Fixed Values To Test
                        //response.TANKERSIZES = new List<NWC_CCB_Integration.DTO.Models.AvailableTankerSize.TankerSize> { new NWC_CCB_Integration.DTO.Models.AvailableTankerSize.TankerSize { TANKERSIZEVALUE = 4, TANKERPRICE = 3 }, new NWC_CCB_Integration.DTO.Models.AvailableTankerSize.TankerSize { TANKERSIZEVALUE = 5, TANKERPRICE = 4 } };

                        response.TANKERSIZES = integrationBLL.GetDefaultTankerSizes(this.queryAPI_URL, userDTO.Value.Token, schema.Input.CISDIVISION);
                   
                        response.COVERAGESTATUS = "NO";

                        response.STATUS = ResponseStatusEnum.OK.ToString();
                        response.RESPONSECODE = "0";
                        response.RESPONSEDESCRIPTION = "Data Retrieved Successfully";
                        response.ERRORCODE = null;
                        response.ERRORDESCRIPTION = null;

                        // Log

                        LoggerManager.LogMsg(c => c.TrackingMsg($"{OperationStepEnum.NoCoverage.ToString()}: Getting Default Tankers"));
                    }
                    else
                    {
                        var zone = integrationBLL.GetZoneByIntegrationID(this.queryAPI_URL, userDTO.Value.Token, zoneIntID).Result;
                        if (integrationBLL.IsZoneWithoutTankersByZoneID(queryAPI_URL, userDTO.Value.Token, zoneIntID).Value)
                        {
                            response.COVERAGESTATUS = "YES";
                            response.STATUS = ResponseStatusEnum.KO.ToString();
                            response.RESPONSECODE = "1";
                            response.RESPONSEDESCRIPTION = "Data Retrieved Successfully";
                            response.ERRORCODE = "DWT";
                            response.ERRORDESCRIPTION = "This zone without tankers";
                            return response;
                        }
                        // Log
                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.GettingZoneByIntegrationID, JsonConvert.SerializeObject(zone))));

                        if (zone.Value == null || (zone.Value != null && zone.Value.ID == 0))
                        {
                            response.TANKERSIZES = integrationBLL.GetDefaultTankerSizes(this.queryAPI_URL, userDTO.Value.Token, schema.Input.CISDIVISION);

                            response.COVERAGESTATUS = "NO";

                            response.STATUS = ResponseStatusEnum.OK.ToString();
                            response.RESPONSECODE = "0";
                            response.RESPONSEDESCRIPTION = "Data Retrieved Successfully";
                            response.ERRORCODE = null;
                            response.ERRORDESCRIPTION = null;

                            // Log
                            LoggerManager.LogMsg(c => c.TrackingMsg($"{OperationStepEnum.NoCoverage.ToString()}: Getting Default Tankers"));
                        }
                        else
                        {
                            result = integrationBLL. GetAvailableTankerSizesByZoneID(this.queryAPI_URL, userDTO.Value.Token, zone.Value.ID, this.defaultZoneID, this.defaultClassID, this.defaultServiceTypeID).Value;
                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}/api/Vehicle/GetAvailableTankerSizesByZoneIntID?zoneID={1}&defaultZoneID={2}&classID={3}&serviceTypeID={4}",
                                        queryAPI_URL, zone.Value.ID, defaultZoneID, this.defaultClassID, this.defaultServiceTypeID)));
                            if (result != null && result.Result != null && result.Result.Any())
                            {
                                response.COVERAGESTATUS = "YES";

                                response.STATUS = ResponseStatusEnum.OK.ToString();
                                response.RESPONSECODE = "0";
                                response.RESPONSEDESCRIPTION = "Data Retrieved Successfully";
                                response.ERRORCODE = null;
                                response.ERRORDESCRIPTION = null;
                                foreach (var tankerSize in result.Result)
                                {
                                    response.TANKERSIZES.Add(new NWC_CCB_Integration.DTO.Models.AvailableTankerSize.TankerSize()
                                    {
                                        TANKERDELIVERYDDTM = DateTime.Now.ToUniversalTime(),
                                        TANKERPRICE = tankerSize.TankerPrice > 0 ? tankerSize.TankerPrice : 0,
                                        TANKERSIZEVALUE = tankerSize.TankerSize,
                                        TANKERACCESSORIZES = new List<NWC_CCB_Integration.DTO.Models.AvailableTankerSize.TankerAccessorize>()
                                        {

                                        }
                                    });
                                }
                            }
                            else
                            {
                                response.TANKERSIZES = new List<NWC_CCB_Integration.DTO.Models.AvailableTankerSize.TankerSize>();

                                response.COVERAGESTATUS = "YES";

                                response.STATUS = ResponseStatusEnum.KO.ToString();
                                response.RESPONSECODE = "1";
                                response.RESPONSEDESCRIPTION = "Data Retrieved Successfully";
                                response.ERRORDESCRIPTION = "No Tankers Available";

                                DateTime dtFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);
                                DateTime dtTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 0, 0);

                                LoggerManager.LogMsg(c => c.TrackingMsg($"DateFrom: {dtFrom.ToString()}"));
                                LoggerManager.LogMsg(c => c.TrackingMsg($"DateTo: {dtTo.ToString()}"));
                                response.ERRORCODE = "1002";

                                // Log
                                LoggerManager.LogMsg(c => c.TrackingMsg($"ERRORCODE: {response.ERRORCODE}"));
                            }

                            // Log
                            LoggerManager.LogMsg(c => c.TrackingMsg($"{OperationStepEnum.GettingAvailableTankers.ToString()}: {response.TANKERSIZES.Count}"));
                        }
                    }
             
                    if (response.STATUS == null && !response.TANKERSIZES.Any())
                    {
                        response.TANKERSIZES = new List<NWC_CCB_Integration.DTO.Models.AvailableTankerSize.TankerSize>();
                        response.STATUS = ResponseStatusEnum.KO.ToString();
                        response.RESPONSECODE = "1";
                        response.RESPONSEDESCRIPTION = ErrorStatus.INTERNAL_ERROR.ToString();
                        response.ERRORCODE = ((int)ErrorStatus.UNEXPECTED_ERROR).ToString();
                        response.ERRORDESCRIPTION = ErrorStatus.INTERNAL_ERROR.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                LogManager.GetLogger("Tanker Sizes WCF").Log(LogLevel.Info,"Error");
                response.STATUS = ResponseStatusEnum.KO.ToString();
                response.RESPONSECODE = "1";
                response.RESPONSEDESCRIPTION = ex.Message;
                response.ERRORCODE = ((int)ErrorStatus.UNEXPECTED_ERROR).ToString();
                response.ERRORDESCRIPTION = ex.Message;
            }
          
            SizesStringToLog = integrationBLL.GetTankerSizesString(response.TANKERSIZES);
            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("GetAvailableTankerSizesResponce:COVERAGESTATUS:{0}, STATUS:{1},TANKERSIZES:{2}", response.COVERAGESTATUS, response.STATUS, SizesStringToLog)));
            var gisTime = TimeAfterGIS -TimeBeforeGIS;
            LogManager.GetLogger("Tanker Sizes WCF").Log(LogLevel.Info, string.Format("{0},{1},{2},{3},{4},{5},{6}", schema.Input.CISDIVISION, schema.Input.LONGLAT, zoneIntID, response.TANKERSIZES.Count.ToString(), SizesStringToLog, "EBranch", gisTime.ToString()));
            return response;
        }
    }
}
