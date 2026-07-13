using NLog;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NWC.Sewer.WinService.Managers
{
    public class GISManager
    {
        private readonly string _token;

        public GISManager(string token)
        {
            _token = token;
        }

        public Task<DescriptiveResponse<IEnumerable<TransporterDTO>>> GetAvailableTransportersInZone(string queryAPI_URL, long zoneId, double zoneLatitude, double zoneLongitude)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/Lookup/", queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", _token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync("GetPermittedCities");
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<IEnumerable<TransporterDTO>>>();
                        //readTask.Wait();

                        return readTask;
                    }
                }

                LogManager.GetLogger("Sewer.WinService").Log(LogLevel.Info, $"This method GISManager.GetAvailableTransportersInZone return unsuccess response.");

                return Task<DescriptiveResponse<IEnumerable<CityDTO>>>.FromResult(DescriptiveResponse<IEnumerable<TransporterDTO>>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("Sewer.WinService").Log(LogLevel.Error, ex, $"GISManager.GetAvailableTransportersInZone : { ex.Message}");

                return Task<DescriptiveResponse<IEnumerable<CityDTO>>>.FromResult(DescriptiveResponse<IEnumerable<TransporterDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }
    }
}
