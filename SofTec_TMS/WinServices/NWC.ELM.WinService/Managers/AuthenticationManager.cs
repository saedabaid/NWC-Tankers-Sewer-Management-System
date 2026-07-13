using NLog;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NWC.Sewer.WinService.Managers
{
    public class AuthenticationManager
    {
        public DescriptiveResponse<LoginDTO> AuthenticateUser(string authenticationAPI_URL, string userName, string password)
        {
            var userDTO = new LoginDTO();
            try
            {
                var accountDTO = new AccountDTO()
                {
                    Name = userName,
                    Password = password
                };

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Origin", authenticationAPI_URL);

                    var postTask = client.PostAsJsonAsync(string.Format("{0}/api/User/AuthenticateUser", authenticationAPI_URL), accountDTO);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<LoginDTO>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;
                        userDTO = readTask.Result.Value;

                        return DescriptiveResponse<LoginDTO>.Success(userDTO);
                    }

                    LogManager.GetLogger("Sewer.WinService").Log(LogLevel.Info, $"This method AuthenticationManager.AuthenticateUser return unsuccess response.");
                    return DescriptiveResponse<LoginDTO>.Error(ErrorStatus.COMMIT_FAIL);
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("Sewer.WinService").Log(LogLevel.Error, ex, $"AuthenticationManager.AuthenticateUser : { ex.Message}");
                return DescriptiveResponse<LoginDTO>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }
    }
}
