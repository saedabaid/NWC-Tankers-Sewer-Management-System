using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Web;
using System.Xml.Serialization;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Net;
using System.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Configuration;
using NWC.BLL.Interfaces;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using System.Web.Caching;
using Infrastructure;
using NWC.DAL.NWCEntities;
using NWC.DTO.Models;
using System.Data;
using System.Xml;
using System.Data.Entity;
using NWC_CCB_Integration.DTO.Logger;

namespace NWC.BLL.Services
{
    public class MapUtilsService : IMapUtilsService
    {
        #region Properties
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<NWC_WorkOrderPlannedRout> _WorkOrderPlannedRoutrRepository;
        private readonly ILoggedInUserService _loggedInUser;
        #endregion

        #region Constructors
        public MapUtilsService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            this._loggedInUser = loggedInUser;

            var ctx = (context == null ? new NWCContext() : context);
            this._unitofWork = new UnitofWork(ctx);

            this._WorkOrderPlannedRoutrRepository = new Repository<NWC_WorkOrderPlannedRout>(ctx);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// CalculateRoute
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public async Task<DescriptiveResponse<DTO.Common.DirectionsResponse>> CalculateRoute(DirectionsServiceRequestObject requestParams)
        {
            try
            {
                HttpClient client = new HttpClient();
                var Routingparams = "origin=" + requestParams.source + "&destination=" + requestParams.destination;
                if (!string.IsNullOrEmpty(requestParams.waypoints))
                {
                    Routingparams += "&waypoints=" + requestParams.waypoints;
                }
                var requestUrl = ConfigurationManager.AppSettings["GoogleDirectionApiUrl"] + Routingparams + "&key=" + ConfigurationManager.AppSettings["GoogleDirectionApiToken"] + "&units=meters";
                var response = await client.GetAsync(requestUrl);

                var responseString = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseString))
                {
                    //Parse to JSON object
                    JObject responseJson = JObject.Parse(responseString);

                    //Convert to Directions Response
                    DirectionsResponse DirectionsResponseObject = responseJson.ToObject<DirectionsResponse>();

                    return new DescriptiveResponse<DirectionsResponse>
                    {
                        Value = DirectionsResponseObject,
                        IsErrorState = false
                    };
                }
                return new DescriptiveResponse<DirectionsResponse>
                {
                    Value = null,
                    IsErrorState = true
                };
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "MapUtilsService => CalculateRoute: "));
                return new DescriptiveResponse<DirectionsResponse>
                {
                    IsErrorState = true,
                    ErrorMetadata = ErrorStatus.UNEXPECTED_ERROR,
                    ErrorDescription = ErrorStatus.UNEXPECTED_ERROR.ToString()
                };
            }
        }


        public DescriptiveResponse<Boolean> SaveOrderRoute(WorkOrderPlannedRoutDTO route)
        {
            try
            {
                using (_unitofWork)
                {
                    _WorkOrderPlannedRoutrRepository.Add(new NWC_WorkOrderPlannedRout()
                    {
                        WorkOrderID = route.WorkOrderID,
                        CreatedBy = _loggedInUser.LoggedInUser.StaffId,
                        CreatedTime = route.CreatedTime,
                        IsDeleted = route.IsDeleted,
                        LastModifiedBy = route.LastModifiedBy,
                        LastModifiedDate = route.LastModifiedDate,
                        RouteJSON = route.RouteJSON,
                        RouteLatLngString = route.RouteLatLngString,
                        DrivingTime = route.DrivingTime,
                        Distance = route.Distance
                    });
                }

                return DescriptiveResponse<Boolean>.Success(true);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "MapUtilsService => SaveOrderRoute: "));
                return DescriptiveResponse<Boolean>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public async Task<DescriptiveResponse<DirectionsResponse>> GetDistanceAsync(DirectionsServiceRequestObject requestParams)
        {
            //string url = @"http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + origin + "&destinations=" + destination + "&sensor=false";

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //WebResponse response = request.GetResponse();
            //Stream dataStream = response.GetResponseStream();
            //StreamReader sreader = new StreamReader(dataStream);
            //string responsereader = sreader.ReadToEnd();
            //response.Close();

            //DataSet ds = new DataSet();
            //ds.ReadXml(new XmlTextReader(new StringReader(responsereader)));
            //if (ds.Tables.Count > 0)
            //{
            //    if (ds.Tables["element"].Rows[0]["status"].ToString() == "OK")
            //    {
            //        var Duration = ds.Tables["duration"].Rows[0]["text"].ToString();
            //        var Distance = ds.Tables["distance"].Rows[0]["text"].ToString();
            //    }
            //}

            try
            {
                HttpClient client = new HttpClient();
                var Routingparams = "origin=" + requestParams.source + "&destination=" + requestParams.destination;

                var requestUrl = ConfigurationManager.AppSettings["GoogledistancematrixApiUrl"] + Routingparams + "&key=" + ConfigurationManager.AppSettings["GoogleDirectionApiToken"] + "&units=meters";
                var response = await client.GetAsync(requestUrl);

                var responseString = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseString))
                {
                    //Parse to JSON object
                    JObject responseJson = JObject.Parse(responseString);

                    //Convert to Directions Response
                    DirectionsResponse DirectionsResponseObject = responseJson.ToObject<DirectionsResponse>();

                    return new DescriptiveResponse<DirectionsResponse>
                    {
                        Value = DirectionsResponseObject,
                        IsErrorState = false
                    };
                }
                return new DescriptiveResponse<DirectionsResponse>
                {
                    Value = null,
                    IsErrorState = true
                };
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "MapUtilsService => GetDistanceAsync: "));
                return new DescriptiveResponse<DirectionsResponse>
                {
                    IsErrorState = true,
                    ErrorMetadata = ErrorStatus.UNEXPECTED_ERROR,
                    ErrorDescription = ErrorStatus.UNEXPECTED_ERROR.ToString()
                };
            }
        }
        #endregion
    }
}
