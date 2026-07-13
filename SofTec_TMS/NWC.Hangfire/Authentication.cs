using Microsoft.Extensions.Configuration;
using NWC.BLL.Interfaces;
using NWC.DTO.Common;
using NWC.DTO.DomainModels;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace NWC.Hangfire.Jobs
{
    public class Authentication
    {
        private IConfiguration Configuration { get; }
        private readonly ILoggedInUserService _loggedInUserService;

        public Authentication(IConfiguration configuration, ILoggedInUserService loggedInUserService)
        {
            Configuration = configuration;
            _loggedInUserService = loggedInUserService;
        }

        public DescriptiveResponse<LoggedInUser> AuthenticateUser()
        {
            try
            {
                var accountDTO = new AccountDTO()
                {
                    Name = Configuration["_Username"],
                    Password = Configuration["_Password"]
                };

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Origin", Configuration["authenticationApiUrl"]);

                    var postTask = client.PostAsJsonAsync(string.Format("{0}/api/User/AuthenticateUser", Configuration["authenticationApiUrl"]), accountDTO);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadFromJsonAsync<DescriptiveResponse<LoginDTO>>();
                        readTask.Wait();

                        var loginDTO = readTask.Result.Value;
                        _loggedInUserService.SetLoggedInUserData(loginDTO.Token, loginDTO.Context.Account.SubID, loginDTO.Context.Account.StaffID, loginDTO.Context.Account.StaffRoleID, null);

                        return DescriptiveResponse<LoggedInUser>.Success(_loggedInUserService.LoggedInUser);
                    }

                    return DescriptiveResponse<LoggedInUser>.Error(ErrorStatus.COMMIT_FAIL);
                }
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<LoggedInUser>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }
    }
}
