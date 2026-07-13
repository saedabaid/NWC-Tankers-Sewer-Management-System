using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;
using NWC_CCB_Integration.BLL;
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using NWC_CCB_Integration.DTO.Helpers;
using NWC_CCB_Integration.DTO.Logger;
using NWC_CCB_Integration.DTO.Models;
using NWC_CCB_Integration.DTO.Models.Soqya;
using NWC_CCB_Integration.DTO.Wrapper;

namespace Soqya.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Soqya : ISoqya
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

        private int take
        {
            get
            {
                int _take;

                int.TryParse(ConfigurationManager.AppSettings["Take"] != null ?
                    ConfigurationManager.AppSettings["Take"] : string.Empty, out _take);

                return _take;
            }
        }
        #endregion

        public Output AddSoqyaCustomerBalance(Schema schema)
        {
            var response = new Output();
            var result = new DescriptiveResponse<bool>();

            try
            {
                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg("---------------------------------------AddSoqyaCustomerBalance - Starting---------------------------------------"));
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AddSoqyaCustomerBalance - Request object before mapping: {0}", JsonConvert.SerializeObject(schema))));


                var integrationBLL = new IntegrationBLL(null);
                var userDTO = integrationBLL.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);

                if (userDTO.Value != null && !string.IsNullOrEmpty(userDTO.Value.Token))
                {
                    string zoneIntID = integrationBLL.CallGISService(schema.Input.Premise.XYCOORDINATESGF, string.Empty, this.sourceApplication, "TRANSACTIONID");

                    if (zoneIntID == null || string.IsNullOrEmpty(zoneIntID) || (!string.IsNullOrEmpty(zoneIntID) && zoneIntID.ToUpper() == "NOCOVERAGE"))
                    {
                        response.COVERAGESTATUS = "NO";

                        response.STATUS = ResponseStatusEnum.OK.ToString();
                        response.RESPONSECODE = "0";
                        response.RESPONSEDESCRIPTION = "NOCOVERAGE";
                        response.ERRORCODE = null;
                        response.ERRORDESCRIPTION = null;

                        // Log
                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AddSoqyaCustomerBalance - CallGISService: {0}", OperationStepEnum.NoCoverage)));
                    }
                    else
                    {
                        var zoneResult = integrationBLL.GetZoneByIntegrationID(this.queryAPI_URL, userDTO.Value.Token, zoneIntID).Result;

                        // Log
                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AddSoqyaCustomerBalance - GettingZoneByIntegrationID: {0}", JsonConvert.SerializeObject(zoneResult))));

                        if (zoneResult.Value == null || (zoneResult.Value != null && zoneResult.Value.ID == 0))
                        {
                            response.COVERAGESTATUS = "NO";

                            response.STATUS = ResponseStatusEnum.OK.ToString();
                            response.RESPONSECODE = "0";
                            response.RESPONSEDESCRIPTION = "NOCOVERAGE";
                            response.ERRORCODE = null;
                            response.ERRORDESCRIPTION = null;
                        }
                        else
                        {
                            var dto = schema.WrapToSoqyaCustomerBalance(zoneResult.Value.ID, this.defaultServiceTypeID, this.defaultPriorityID, this.defaultCategoryID, this.defaultStatusID);

                            // Log
                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AddSoqyaCustomerBalance - Request object after mapping: {0}", JsonConvert.SerializeObject(dto))));

                            result = integrationBLL.AddSoqyaCustomerBalance(this.commandAPI_URL, userDTO.Value.Token, dto).Result;

                            // Log
                            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("AddSoqyaCustomerBalance - Response: {0}", JsonConvert.SerializeObject(result))));


                            if (result.Value)
                            {
                                response.STATUS = ResponseStatusEnum.OK.ToString();
                                response.RESPONSECODE = "0";
                                response.RESPONSEDESCRIPTION = "Data Inserted Successfully";
                                response.ERRORCODE = null;
                                response.ERRORDESCRIPTION = null;
                            }
                            else
                            {
                                response.STATUS = ResponseStatusEnum.KO.ToString();
                                response.RESPONSECODE = "1";
                                response.RESPONSEDESCRIPTION = ErrorStatus.INTERNAL_ERROR.ToString();
                                response.ERRORCODE = "1";
                                response.ERRORDESCRIPTION = (result.Errors != null && result.Errors.Any()) ? string.Join(",", result.Errors) : ErrorStatus.INTERNAL_ERROR.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));
                //ExceptionManager.GetExceptionLogger().LogException(ex);

                response.STATUS = ResponseStatusEnum.KO.ToString();
                response.RESPONSECODE = "1";
                response.RESPONSEDESCRIPTION = ex.Message;
                response.ERRORCODE = ((int)ErrorStatus.UNEXPECTED_ERROR).ToString();
                response.ERRORDESCRIPTION = ex.Message;
            }

            return response;
        }
    }
}
