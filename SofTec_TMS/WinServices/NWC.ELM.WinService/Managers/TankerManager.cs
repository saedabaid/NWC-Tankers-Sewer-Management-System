using NLog;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NWC.Sewer.WinService.Managers
{
    public class TankerManager
    {
        private readonly string _token;

        public TankerManager(string token)
        {
            _token = token;
        }

        //هذه الي بترحل
        public DescriptiveResponse<bool> SaveTankerToTransporter(string commandUrl,string _token, Guid subid, int? retrials)
        {
            try
            {
               
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/tanker/", commandUrl));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", _token);
                    //هان بتيجي فاضييه في وقت ما الان جربت خليتها فاضه
                    LogManager.GetLogger("NWC.ELM.Services").Log(LogLevel.Error, "token : " + _token);

                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandUrl);
                    var postTask = client.PostAsJsonAsync(string.Format("{0}/api/tanker/SaveTankerToTransporter?subid={1}&retrials={2}", commandUrl, subid, retrials), new { });

                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<bool>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;
                        if (!returnResult.IsErrorState && returnResult.Value)
                            return DescriptiveResponse<bool>.Success(returnResult.Value);
                        else
                            return DescriptiveResponse<bool>.Error(returnResult.ErrorDescription);
                    }

                }
                return DescriptiveResponse<bool>.Error(ErrorStatus.COMMIT_FAIL);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("NWC.ELM.Services").Log(LogLevel.Error, ex, $"TankerManager.SaveTankerToTransporter : { ex.Message}");
                return DescriptiveResponse<bool>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

    }
}
