using Newtonsoft.Json;
using NWC_CCB_Integration.BLL;
using NWC_CCB_Integration.BLL.Validators;
using NWC_CCB_Integration.DAL;
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using NWC_CCB_Integration.DTO.Helpers;
using NWC_CCB_Integration.DTO.Logger;
using NWC_CCB_Integration.DTO.Models;
using NWC_CCB_Integration.DTO.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FromCCBToNWC.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WorkOrder" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WorkOrder.svc or WorkOrder.svc.cs at the Solution Explorer and start debugging.
    public class WorkOrder : IWorkOrder
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
        #endregion

        public NWC_CCB_Integration.DTO.Models.CreateWorkOrder.Output CreateWorkOrder(NWC_CCB_Integration.DTO.Models.CreateWorkOrder.Schema schema)
        {
            var response = new NWC_CCB_Integration.DTO.Models.CreateWorkOrder.Output();
            var dto = new WorkOrderRequestDTO();
            var xml = XMLConverter.ToXElement2<NWC_CCB_Integration.DTO.Models.CreateWorkOrder.Schema>(new NWC_CCB_Integration.DTO.Models.CreateWorkOrder.Schema());
            string failureMessage = null;

            try
            {
                //Sewer Test
    
                //End Sewer Test

                var integrationBLL = new IntegrationBLL(null);
                var integrationStatus = new NWC_Int_ObjectStatus();
                var userDTO = integrationBLL.AuthenticateUser(this.authenticationAPI_URL, this.username, this.password);
                LoggerManager.LogMsg(c => c.TrackingMsg("---------------------------------------CreateWorkOrder - Starting---------------------------------------"));
                //ExceptionManager.GetExceptionLogger().LogInformation("---------------------------------------------------------------------------------------------------------------------------");

                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - recived-XML_schema: {0}", JsonConvert.SerializeObject(schema))));
                //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("Create-Order - recived-XML_schema: {0}", JsonConvert.SerializeObject(schema)));

                xml = XMLConverter.ToXElement2<NWC_CCB_Integration.DTO.Models.CreateWorkOrder.Schema>(schema);

                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - parseTO_XML: {0}", JsonConvert.SerializeObject(xml))));
                //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("Create-Order - parseTO_XML: {0}", JsonConvert.SerializeObject(xml)));

                dto = integrationBLL.ParsingXMLForCreatingWO(xml.ToString());

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order parseTO-DTO: {0}", JsonConvert.SerializeObject(dto))));
                //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("Create-Order parseTO-DTO: {0}", JsonConvert.SerializeObject(dto)));

                // Validations
                var validator = new CreateWorkOrderValidator(true);

                var results = validator.Validate(dto);

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - validate_DTO: {0}", results.IsValid)));
                //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("Create-Order - validate_DTO: {0}", results.IsValid));

                var failureMsgs = string.Empty;

                if (!results.IsValid)
                {
                    #region iS NOT VALID
                    failureMsgs = string.Join(",", results.Errors.Select(s => s.ErrorMessage));

                    integrationStatus = integrationBLL.AddObjectIntegrationStatus(dto.OrderNumber, (int)OrderRequestStatuEnum.Fail, (int)OperationTypeEnum.CreateWorkOrder, dto, xml.ToString(), null, failureMessage, 0);

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - WorkOrder_Integration_Added: {0}", JsonConvert.SerializeObject(integrationStatus))));
                    //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("Create-Order - WorkOrder_Integration_Added: {0}", JsonConvert.SerializeObject(integrationStatus)));

                    // Response
                    response.STATUS = ResponseStatusEnum.KO.ToString();
                    response.RESPONSECODE = "1";
                    response.RESPONSEDESCRIPTION = failureMsgs;
                    response.ERRORCODE = ((int)ErrorStatus.UNEXPECTED_ERROR).ToString();
                    response.ERRORDESCRIPTION = failureMsgs;

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - Validations_Errors: {0}", JsonConvert.SerializeObject(failureMsgs))));
                    //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("Create-Order - Validations_Errors: {0}", JsonConvert.SerializeObject(failureMsgs)));

                    #endregion
                }
                else
                {
                    integrationStatus = integrationBLL.AddObjectIntegrationStatus(dto.OrderNumber, (int)OrderRequestStatuEnum.Pending, (int)OperationTypeEnum.CreateWorkOrder, dto, xml.ToString(), null, failureMessage, 0);

                    // Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - WorkOrder_Integration_Added: {0}", JsonConvert.SerializeObject(integrationStatus))));
                    //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("Create-Order - WorkOrder_Integration_Added: {0}", JsonConvert.SerializeObject(integrationStatus)));

                    //success
                    if (integrationStatus != null && integrationStatus.ID > 0)
                    {
                        response.CASEID = integrationStatus.OrderNumber;
                        response.STATUS = ResponseStatusEnum.OK.ToString();
                        response.RESPONSECODE = "0";
                        response.RESPONSEDESCRIPTION = "Data Inserted Successfully";
                        response.ERRORCODE = null;
                        response.ERRORDESCRIPTION = null;
                    }
                    //Faile
                    else
                    {
                        response.STATUS = ResponseStatusEnum.KO.ToString();
                        response.RESPONSECODE = "1";
                        response.RESPONSEDESCRIPTION = "Adding WorkOrder fail";
                        response.ERRORCODE = ((int)ErrorStatus.UNEXPECTED_ERROR).ToString();
                        response.ERRORDESCRIPTION = failureMsgs;

                        // Log
                        LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Create-Order - Adding WorkOrder fail: {0}", JsonConvert.SerializeObject(failureMsgs))));
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

        public NWC_CCB_Integration.DTO.Models.UpdateWorkOrder.Output UpdateWorkOrder(NWC_CCB_Integration.DTO.Models.UpdateWorkOrder.Schema schema)
        {
            var response = new NWC_CCB_Integration.DTO.Models.UpdateWorkOrder.Output();

            try
            {
                var xml = XMLConverter.ToXElement2<NWC_CCB_Integration.DTO.Models.UpdateWorkOrder.Schema>(schema);

                var integrationBLL = new IntegrationBLL(null);
                bool isChangeStatus;

                var dto = integrationBLL.ParsingXMLForUpdatingWO(xml.ToString(), out isChangeStatus);

                var integrationStatus = integrationBLL.AddObjectIntegrationStatus(dto.OrderNumber, (int)OrderRequestStatuEnum.Pending, (int)OperationTypeEnum.UpdateWorkOrder, null, xml.ToString(), null, null, 0);

                if (integrationStatus != null)
                {
                    response.CASEID = integrationStatus.OrderNumber;
                    response.STATUS = ResponseStatusEnum.OK.ToString();
                    response.RESPONSECODE = "0";
                    response.RESPONSEDESCRIPTION = "Data Updated Successfully";
                    response.ERRORCODE = null;
                    response.ERRORDESCRIPTION = null;
                }
                else
                {
                    response.STATUS = ResponseStatusEnum.OK.ToString();
                    response.RESPONSECODE = "2";
                    response.RESPONSEDESCRIPTION = ErrorStatus.COMMIT_FAIL.ToString();
                    response.ERRORCODE = null;
                    response.ERRORDESCRIPTION = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);

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
