using FromCCBToNWC.API.Infrastructure.Core;
using NWC_CCB_Integration.BLL;
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using NWC_CCB_Integration.DTO.Models;
using System;

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace FromCCBToNWC.API.Controllers
{
    [RoutePrefix("api/RF")]
    public class RFController : ApiControllerBase
    {
        #region Properties
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
        #endregion
        [HttpPost]
        [Route("GetTransporterBasedOnRFID")]
        public DescriptiveResponse<RFIDDTO> GetTransporterBasedOnRFID([FromBody] string RFID, string token)
        {
            var RFIDDTO = new RFIDDTO();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/RFID/GetTransporterBasedOnRFID", this.queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", this.queryAPI_URL);

                    var postTask = client.PostAsJsonAsync("RFID", RFID);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<RFIDDTO>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;

                        RFIDDTO = readTask.Result.Value;
                    }
                    else
                    {
                        return DescriptiveResponse<RFIDDTO>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<RFIDDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }

            return DescriptiveResponse<RFIDDTO>.Success(RFIDDTO);
        }


    }
}
