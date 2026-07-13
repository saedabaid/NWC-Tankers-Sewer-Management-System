using Newtonsoft.Json;
using NWC.BL.Denormalizer.UpdateWONotificationServiceRef;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BL.Denormalizer.OutServices
{
    public class ConsumingUpdateService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="status"></param>
        /// <param name="cis"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public static bool CallUpdateWONotificationService(string orderNumber, TMSWONotificationRequestOrderStatus status, string cis, string transactionId, Guid createdBy,
            int currentStatusID, int newStatusID, string comment = null, string reasonIntegrationId = null)
        {
            try
            {
                var hayatServiceClient = new TMSNwcUpdateWONotification_pttClient();

                var requestParameters = new List<TMSWONotificationRequestParameter>();
                requestParameters.Add(new TMSWONotificationRequestParameter()
                {
                    parameterName = "TMSID",
                    parameterValue = orderNumber
                });
                requestParameters.Add(new TMSWONotificationRequestParameter()
                {
                    parameterName = "COMMENT",
                    parameterValue = string.IsNullOrEmpty(comment) ? string.Empty : comment
                });

                if (status == TMSWONotificationRequestOrderStatus.Item8)
                {
                    requestParameters.Add(new TMSWONotificationRequestParameter()
                    {
                        parameterName = "CANREASON",
                        parameterValue = string.IsNullOrEmpty(reasonIntegrationId) ? string.Empty : reasonIntegrationId
                    });
                }
                else if (status == TMSWONotificationRequestOrderStatus.Item3)
                {
                    requestParameters.Add(new TMSWONotificationRequestParameter()
                    {
                        parameterName = "FAILREASON",
                        parameterValue = string.IsNullOrEmpty(reasonIntegrationId) ? string.Empty : reasonIntegrationId
                    });
                }

                var hayatRequest = new TMSWONotificationRequest()
                {
                    sourceApplication = TMSWONotificationRequestSourceApplication.TMS,
                    orderNumber = orderNumber,
                    cisDivision = GetCISDivison(cis),
                    orderStatus = status,
                    updateDateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                    parameters = requestParameters.ToArray(),
                    transactionId = transactionId
                };

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Calling update service request: {0}", JsonConvert.SerializeObject(hayatRequest))));

                AddHayatOrderStatusLog(orderNumber, createdBy, currentStatusID, newStatusID, hayatRequest);

                if (ConfigurationManager.AppSettings["CallHayatDirectly"] != null &&
                    ConfigurationManager.AppSettings["CallHayatDirectly"].ToLower() == "true")
                {
                    var result = Task.Run(async () => await hayatServiceClient.updateWorkOrserStatusAsync(hayatRequest)).ConfigureAwait(false);

                    var updateResponse = result.GetAwaiter().GetResult().TMSWONotificationResponse;

                    //Log
                    LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("Calling update service response: {0}", JsonConvert.SerializeObject(updateResponse))));

                    if (updateResponse != null && updateResponse.status.ToUpper() == "OK")
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ConsumingUpdateService => CallUpdateWONotificationService: "));
                return false;
            }
        }

        private static void AddHayatOrderStatusLog(string orderNumber, Guid createdBy, int currentStatusID, int newStatusID, TMSWONotificationRequest hayatRequest)
        {
            try
            {
                var ctx = new NWCContext();

                ctx.NWC_Hayat_OrderStatusLog.Add(new NWC_Hayat_OrderStatusLog()
                {
                    OrderNumber = orderNumber,
                    CreateTime = DateTime.Now,
                    CreatedBy = createdBy,
                    CurrentStatusID = currentStatusID,
                    NewStatusID = newStatusID,
                    Retrials = 0,
                    StatusID = (int)HayatWorkOrderLogStatusEnum.Pending,
                    HayatRequest = JsonConvert.SerializeObject(hayatRequest)
                });

                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ConsumingUpdateService => AddHayatOrderStatusLog: "));
            }
        }

        private static cisDivison GetCISDivison(string cis)
        {
            var cisDiv = cisDivison.AS;

            if (string.IsNullOrEmpty(cis))
                return cisDiv;

            if (cis.ToUpper() == cisDivison.BA.ToString())
            {
                cisDiv = cisDivison.BA;
            }
            if (cis.ToUpper() == cisDivison.HA.ToString())
            {
                cisDiv = cisDivison.HA;
            }
            if (cis.ToUpper() == cisDivison.HS.ToString())
            {
                cisDiv = cisDivison.HS;
            }
            if (cis.ToUpper() == cisDivison.JC.ToString())
            {
                cisDiv = cisDivison.JC;
            }
            if (cis.ToUpper() == cisDivison.JCBU.ToString())
            {
                cisDiv = cisDivison.JCBU;
            }
            if (cis.ToUpper() == cisDivison.JF.ToString())
            {
                cisDiv = cisDivison.JF;
            }
            if (cis.ToUpper() == cisDivison.JZ.ToString())
            {
                cisDiv = cisDivison.JZ;
            }
            if (cis.ToUpper() == cisDivison.JZBU.ToString())
            {
                cisDiv = cisDivison.JZBU;
            }
            if (cis.ToUpper() == cisDivison.MC.ToString())
            {
                cisDiv = cisDivison.MC;
            }
            if (cis.ToUpper() == cisDivison.MCBU.ToString())
            {
                cisDiv = cisDivison.MCBU;
            }
            if (cis.ToUpper() == cisDivison.MD.ToString())
            {
                cisDiv = cisDivison.MD;
            }
            if (cis.ToUpper() == cisDivison.MK.ToString())
            {
                cisDiv = cisDivison.MK;
            }
            if (cis.ToUpper() == cisDivison.NA.ToString())
            {
                cisDiv = cisDivison.NA;
            }
            if (cis.ToUpper() == cisDivison.NJ.ToString())
            {
                cisDiv = cisDivison.NJ;
            }
            if (cis.ToUpper() == cisDivison.QS.ToString())
            {
                cisDiv = cisDivison.QS;
            }
            if (cis.ToUpper() == cisDivison.RC.ToString())
            {
                cisDiv = cisDivison.RC;
            }
            if (cis.ToUpper() == cisDivison.RCBU.ToString())
            {
                cisDiv = cisDivison.RCBU;
            }
            if (cis.ToUpper() == cisDivison.RI.ToString())
            {
                cisDiv = cisDivison.RI;
            }
            if (cis.ToUpper() == cisDivison.SH.ToString())
            {
                cisDiv = cisDivison.SH;
            }
            if (cis.ToUpper() == cisDivison.TB.ToString())
            {
                cisDiv = cisDivison.TB;
            }
            if (cis.ToUpper() == cisDivison.TC.ToString())
            {
                cisDiv = cisDivison.TC;
            }
            if (cis.ToUpper() == cisDivison.TCBU.ToString())
            {
                cisDiv = cisDivison.TCBU;
            }

            return cisDiv;
        }
    }
}
