using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NWC.DTO.Common;
using NWC.DTO.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NWC.Service.SignalR.Security
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private IConfiguration _configuration;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorization = "";
            if (Request.Headers.ContainsKey("Authorization") || Request.Query.ContainsKey("access_token"))
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                var authQuery = Request.Query["access_token"].ToString();
                authorization = string.IsNullOrEmpty(authHeader) ? authQuery : authHeader;
            }
            if (string.IsNullOrEmpty(authorization))
                return AuthenticateResult.Fail("Missing Authorization Header");

            string token = authorization.Contains("Bearer") ? authorization.Split("Bearer ")[1] : authorization;
            var user = GetUserProfile(token);
            if (user == null)
                return AuthenticateResult.Fail("Unauthorized");
            else
                return CreateAuthenticationTicket(user);
        }

        private AuthenticateResult CreateAuthenticationTicket(ProfileDTO user)
        {
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim(ClaimTypes.MobilePhone, user.MobileNumber),
                new Claim(ClaimTypes.Locality, user.Branch.ToString()),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        public ProfileDTO GetUserProfile(string token)
        {
            if (token == null) return null;
            var queryApiUrl = _configuration["QueryApiUrl"];
            var userProfileUrl = _configuration["UserProfileUrl"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(queryApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", token);
                client.DefaultRequestHeaders.Add("IsIntigration", "true");
                client.DefaultRequestHeaders.Add("Lang", "en");
                client.DefaultRequestHeaders.Add("Origin", queryApiUrl);

                var response = client.GetAsync(userProfileUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    var resault = JsonConvert.DeserializeObject<DescriptiveResponse<ProfileDTO>>(jsonString);
                    if (!resault.IsErrorState && resault.Value != null)
                        return resault.Value;
                    else
                        return null;
                }
                else
                    return null;
            }
        }
    }
}