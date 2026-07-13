using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Resources;
using NWC.Service.Authentication.WebAPI.Infrastructure.Core;
using NWC.Service.Authentication.WebAPI.Infrastructure.Filters;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace NWC.Service.Command.WebAPI.Controllers
{
    //[Authorize]
    [AuthenticationTokenFilter]
    [RoutePrefix("api/File")]
    public class FileController : ApiControllerBase
    {
        public string[] SupporttedExtensions = { "pdf", "docx", "txt", "doc", "jpg", "png" };

        [Route("UploadFiles")]
        [HttpPost]
        public DescriptiveResponse<List<AttachmentDTO>> UploadFiles(HttpRequestMessage request)
        {
            return DescriptiveResponse<List<AttachmentDTO>>.Try(() =>
            {
                var FilesNames = new List<AttachmentDTO>();
                var tempPath = ConfigurationManager.AppSettings["temp"];
                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);

                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string posted in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[posted];

                        #region validations
                        //var validations = new List<string>();

                        //var fileExtension = System.IO.Path.GetExtension(postedFile.FileName).ToLower();
                        //if (!SupporttedExtensions.Any(a => a == fileExtension))
                        //{
                        //    validations.Add(ValidationMessagesKeys.AfterFirstKMIsRequired);
                        //}
                        //if (Math.Pow(10,7) > postedFile.ContentLength)
                        //{
                        //    validations.Add(ValidationMessagesKeys.AfterFirstKMIsRequired);
                        //}

                        //if (validations.Count() > 0)
                        //{
                        //    return DescriptiveResponse<List<AttachmentDTO>>.Error(validations);
                        //}

                        #endregion

                        string namefile = $"{Guid.NewGuid()}_{postedFile.FileName}";
                        string filePath = $"{tempPath}/{namefile}";
                        postedFile.SaveAs(filePath);

                        FilesNames.Add(new AttachmentDTO
                        {
                            DocumentName = postedFile.FileName,
                            RelativePath = namefile,
                            IsDeleted = false
                        });
                    }
                }

                return DescriptiveResponse<List<AttachmentDTO>>.Success(FilesNames);
            });
        }


        [Route("DeleteFile")]
        [HttpDelete]
        public DescriptiveResponse<bool> DeleteFile(string fileName)
        {
            return DescriptiveResponse<bool>.Try(() =>
            {
                var filePath = $"{ConfigurationManager.AppSettings["temp"]}/{fileName}";
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return DescriptiveResponse<bool>.Success(true);
            });
        }


        [Route("DownloadFile")]
        [HttpGet]
        public HttpResponseMessage DownloadFiles(AttachmentType type, string relativePath, long? id)
        {
            try
            {
                string fullPath;
                #region fullPath
                if (id == null || id == 0)
                {
                    fullPath = $"{ConfigurationManager.AppSettings["temp"]}/{relativePath}";
                }
                else
                {
                    switch (type)
                    {
                        case AttachmentType.Contract:
                            fullPath = $"{ConfigurationManager.AppSettings["contract"]}/{relativePath}";
                            break;
                        case AttachmentType.WorkOrder:
                            fullPath = $"{ConfigurationManager.AppSettings["workOrder"]}/{relativePath}";
                            break;
                        case AttachmentType.Contractor:
                            fullPath = $"{ConfigurationManager.AppSettings["contractor"]}/{relativePath}";
                            break;
                        case AttachmentType.ContractViolation:
                            fullPath = $"{ConfigurationManager.AppSettings["ContractViolation"]}/{relativePath}";
                            break;
                        default:
                            fullPath = string.Empty;
                            break;
                    }
                }
                #endregion

                if (File.Exists(fullPath))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var fileStream = new FileStream(fullPath, FileMode.Open);
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    //response.Content.Headers.ContentDisposition.FileName = fileName;
                    return response;
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "FileController => DownloadFiles: "));
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }






    }
}