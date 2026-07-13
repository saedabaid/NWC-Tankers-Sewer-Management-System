using NLog;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ELM_NWC_Integration.Infrastructure.Core
{
    public class ApiControllerBase : ApiController
    {
        /// <summary>
        /// in case we need to handle a database operation 
        /// </summary>
        //protected readonly IAltairUnitOfWork _unitOfWork;
        //protected readonly IGPSUnitOfWork _gpsUnitOfWork;
        //public ApiControllerBase() { }
        //public ApiControllerBase(IAltairUnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;

        //}
        //public ApiControllerBase(IGPSUnitOfWork gpsUnitOfWork)
        //{
        //    _gpsUnitOfWork = gpsUnitOfWork;
        //}
        //public ApiControllerBase(IAltairUnitOfWork unitOfWork, IGPSUnitOfWork gpsUnitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //    _gpsUnitOfWork = gpsUnitOfWork;
        //}

        protected HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, Func<HttpResponseMessage> function)
        {
            HttpResponseMessage response = null;
            try
            {
                response = function.Invoke();
            }
            catch (DbUpdateException ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException.Message);
            }

            catch (Exception ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        private void LogError(Exception ex)
        {
            ExceptionManager.GetExceptionLogger().LogException(ex);
        }

        public void RegisterInfoLog(string message)
        {
            LogManager.GetLogger("ELM.WinService").Log(LogLevel.Info, message);
        }
        public void RegisterErrorLog(Exception ex)
        {
            LogManager.GetLogger("ELM.WinService").Log(LogLevel.Error, ex);
        }
    }
}