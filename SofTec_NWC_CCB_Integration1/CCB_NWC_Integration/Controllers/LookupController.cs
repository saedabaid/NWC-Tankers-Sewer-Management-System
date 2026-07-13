using FromCCBToNWC.API.Infrastructure.Core;
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
    [RoutePrefix("api/Lookup")]
    public class LookupController : ApiControllerBase
    {
        #region Properties
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
        #endregion

        [HttpGet]
        [Route("GetCancelationReasons")]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCancelationReasons(string token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", this.queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GeReasonsByStatusId?statusId={1}", this.queryAPI_URL, 8)); // 8 is id for cancel status
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<IEnumerable<LookUpDTO<int>>>>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    else
                    {
                        return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        [HttpGet]
        [Route("GetFailToDeliverReasons")]
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetFailToDeliverReasons(string token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", this.queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GeReasonsByStatusId?statusId={1}", this.queryAPI_URL, 3)); // 3 is id for FailToDeliver status
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<IEnumerable<LookUpDTO<int>>>>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    else
                    {
                        return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
    }
}
