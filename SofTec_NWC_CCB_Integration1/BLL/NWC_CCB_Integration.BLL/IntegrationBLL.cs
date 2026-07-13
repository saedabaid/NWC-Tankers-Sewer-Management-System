using Newtonsoft.Json;
using NWC_CCB_Integration.DAL;
using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Enums;
using NWC_CCB_Integration.DTO.ExceptionLogger;
using NWC_CCB_Integration.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using System.Xml;
using System.Globalization;
using NWC_CCB_Integration.BLL.UpdateWONotificationServiceRef;
using NWC_CCB_Integration.BLL.GISServiceRef;
using NWC_CCB_Integration.DTO.Logger;
using NWC_CCB_Integration.DTO.Models.AvailableTankerSize;

namespace NWC_CCB_Integration.BLL
{
    public class IntegrationBLL
    {
        public NWC_CCBEntities Context { get; private set; }

        public IntegrationBLL(NWC_CCBEntities ctx)
        {
            if (ctx == null)
                Context = new NWC_CCBEntities();
            else
                Context = ctx;
        }

        #region Integration
        public NWC_Int_ObjectLog SaveLogOperation(int opTypeID, int opStepID, string orderNo, object dto, string token)
        {
            var log = new NWC_Int_ObjectLog
            {
                OrderNumber = orderNo,
                OperationTypeID = opTypeID,
                OperationStepID = opStepID,
                CreatedDate = DateTime.Now
            };

            if (dto != null)
            {
                log.NWC_Int_ObjectLogDTO.Add(new NWC_Int_ObjectLogDTO
                {
                    DTO = JsonConvert.SerializeObject(dto)
                });
            }

            this.Context.NWC_Int_ObjectLog.Add(log);

            this.Context.SaveChanges();

            return log;
        }

        public NWC_Int_ObjectStatus AddObjectIntegrationStatus(string orderNo, int statusID, int operationTypeID, object dto, string xml, string token, string failureMessage, int retrials)
        {
            try
            {
                if (this.Context.NWC_Int_ObjectStatus.Where(x => x.OrderNumber == orderNo).Any())
                    return null;

                var ObjIntegrationStatus = new NWC_Int_ObjectStatus()
                {
                    OrderNumber = orderNo,
                    StatusID = statusID,
                    StatusTime = DateTime.Now,
                    OperationTypeID = operationTypeID,
                    Retrials = retrials,
                    DTO = dto != null ? JsonConvert.SerializeObject(dto) : null,
                    XML = xml,
                    Token = token,
                    FailureMessage = failureMessage
                    

                    
                };

                this.Context.NWC_Int_ObjectStatus.Add(ObjIntegrationStatus);

                Context.SaveChanges();
                return ObjIntegrationStatus;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return null;
            }
        }

        public bool UpdateObjectIntegrationStatus(long integrationID, int statusID, string failureMessage)
        {
            try
            {
                var integrationObj = this.Context.NWC_Int_ObjectStatus.FirstOrDefault(x => x.ID == integrationID);

                integrationObj.Retrials += 1;
                integrationObj.StatusID = statusID;

                if (!string.IsNullOrEmpty(failureMessage))
                    integrationObj.FailureMessage = failureMessage;

                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return false;
            }
        }

        public bool UpdateObjectIntegrationStatus(long integrationID, object dto)
        {
            try
            {
                var integrationObj = this.Context.NWC_Int_ObjectStatus.FirstOrDefault(x => x.ID == integrationID);

                integrationObj.DTO = dto != null ? JsonConvert.SerializeObject(dto) : null;

                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return false;
            }
        }

        public List<NWC_Int_ObjectStatus> GetObjectStatusList(List<int> statusIDs, int retrials, int take)
        {
            try
            {
                var workOrderRequestList = new List<NWC_Int_ObjectStatus>();

                workOrderRequestList = this.Context.NWC_Int_ObjectStatus.Where(x => statusIDs.Contains(x.StatusID.Value) && x.Retrials < retrials).Take(take).ToList();

                return workOrderRequestList;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return null;
            }
        }
        #endregion

        #region Authentication
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

                    return DescriptiveResponse<LoginDTO>.Error(ErrorStatus.COMMIT_FAIL);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<LoginDTO>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion

        #region WorkOrder
        public Task<DescriptiveResponse<EventWorkOrderDTO>> CreateWorkOrder(string commandAPI_URL, string token, EventWorkOrderDTO request)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.PostAsJsonAsync("CreateWorkOrder", request);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<EventWorkOrderDTO>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<EventWorkOrderDTO>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<EventWorkOrderDTO>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        public DescriptiveResponse<Boolean> ChangeWorkOrderStatus(string commandAPI_URL, string token, EventWorkOrderDTO request)
        {
            try
            {
                string actionName = string.Empty;

                switch (request.StatusID)
                {
                    case 6:// OutForDelivery
                        actionName = "OutForDeliveryWorkOrder";
                        break;
                    case 7:// Arrived
                        actionName = "ArrivedWorkOrder";
                        break;
                    case 4:// Delivered
                        actionName = "DeliveredWorkOrder";
                        break;
                    case 3:// FailedToDeliver
                        actionName = "FailedToDeliver";
                        break;
                    case 8:// Cancelled
                        actionName = "CancelWorkOrder";
                        break;
                    case 2:// Onhold
                        actionName = "OnHold";
                        break;
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/WorkOrder/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.PostAsJsonAsync(actionName, request);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<Boolean>>();
                        readTask.Wait();

                        var returnResult = readTask.Result;

                        return readTask != null ? readTask.Result : DescriptiveResponse<Boolean>.Error(ErrorStatus.COMMIT_FAIL); ;
                    }
                    else
                    {
                        return DescriptiveResponse<Boolean>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion

        #region Zone
        public Task<DescriptiveResponse<ZoneDTO>> GetZoneByIntegrationID(string queryAPI_URL, string token, string zoneIntegrationId)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Zone/GetZoneByIntegrationID?ZoneIntegrationId={1}", queryAPI_URL, zoneIntegrationId.Trim()));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<ZoneDTO>>();
                        return readTask;
                    }
                    else
                    {
                        return Task<DescriptiveResponse<ZoneDTO>>.FromResult(DescriptiveResponse<ZoneDTO>.Error(ErrorStatus.COMMIT_FAIL));
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<ZoneDTO>>.FromResult(DescriptiveResponse<ZoneDTO>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>> GetZoneStations(string queryAPI_URL, string token, long zoneId)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GetZoneStations?ZoneId={1}", queryAPI_URL, zoneId));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    else
                    {
                        return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public Task<DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>> GetZoneStation(string queryAPI_URL, string token, long customerID)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/getCustomerAccountStations?customerAccountId={1}", queryAPI_URL, customerID));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>>();
                        readTask.Wait();

                        return readTask;
                    }
                    else
                    {
                        return Task<DescriptiveResponse<LookUpDTO<Guid>>>.FromResult(DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.COMMIT_FAIL));
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>>.FromResult(DescriptiveResponse<IEnumerable<LookUpDTO<Guid>>>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        public Task<DescriptiveResponse<LookUpDTO<Guid>>> GetMainZoneStation(string queryAPI_URL, string token, long zoneId)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GetMainZoneStation?ZoneId={1}", queryAPI_URL, zoneId));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<LookUpDTO<Guid>>>();
                        readTask.Wait();

                        return readTask;
                    }
                    else
                    {
                        return Task<DescriptiveResponse<LookUpDTO<Guid>>>.FromResult(DescriptiveResponse<LookUpDTO<Guid>>.Error(ErrorStatus.COMMIT_FAIL));
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<LookUpDTO<Guid>>>.FromResult(DescriptiveResponse<LookUpDTO<Guid>>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }


        public DescriptiveResponse<List<int>> GetDefefaultTankerSizesByCIS(string queryAPI_URL, string token, string CIS)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Vehicle/GetDefefaultTankerSizesByCIS?CIS={1}",
                                        queryAPI_URL, CIS));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<List<int>>>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    else
                    {
                        return DescriptiveResponse<List<int>>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<List<int>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<SearchResult<AvailableTankerSizesDTO>> GetAvailableTankerSizesByZoneID(string queryAPI_URL, string token, long zoneID, long defaultZoneID, int classID, int serviceTypeID)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Vehicle/GetAvailableTankerSizesByZoneIntID?zoneID={1}&defaultZoneID={2}&classID={3}&serviceTypeID={4}",
                                        queryAPI_URL, zoneID, defaultZoneID, classID, serviceTypeID));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<SearchResult<AvailableTankerSizesDTO>>>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    else
                    {
                        return DescriptiveResponse<SearchResult<AvailableTankerSizesDTO>>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<SearchResult<AvailableTankerSizesDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
     
                public DescriptiveResponse<Boolean> IsZoneWithoutTankersByZoneID(string queryAPI_URL, string token, string zoneID)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/WorkOrder/IsZoneWithoutTankersByZoneInt?zoneIntegrationID={1}",
                                        queryAPI_URL, zoneID));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<Boolean>>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    else
                    {
                        return DescriptiveResponse<Boolean>.Error(ErrorStatus.COMMIT_FAIL);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<Boolean>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        #endregion

        #region Customer
        public Task<DescriptiveResponse<SoqyaCustomerBalanceDTO>> CreateCustomerBalance(WorkOrderRequestDTO workOrderRequestDTO,
            long zoneID, int priorityID, int categoryID, int statusID, int serviceTypeID, string commandAPI_URL, string token)
        {
            try
            {
                var coordinatesArr = workOrderRequestDTO.PremiseCoordinates.Split(' ');
                double lat;
                double lng;
                double.TryParse(coordinatesArr[1], out lat);
                double.TryParse(coordinatesArr[0], out lng);

                var customer = new CustomerDTO()
                {
                    IntegrationId = workOrderRequestDTO.AccountID.Trim(),
                    Code = workOrderRequestDTO.CustomerCode.Trim(),
                    Email = !string.IsNullOrEmpty(workOrderRequestDTO.Email) ? workOrderRequestDTO.Email.Trim() : "mail123@mailTest123.com",
                    IDNumber = workOrderRequestDTO.IDNumber.Trim(),
                    FullName = workOrderRequestDTO.PersonName.Trim(),
                    IntegrationId_IDType = workOrderRequestDTO.IDTypeID, 
                    Mobile = workOrderRequestDTO.PersonMobile.Trim()
                };

                var customerLocation = new CustomerLocationDTO()
                {
                    //CustomerID = customerID,
                    IntegrationId = workOrderRequestDTO.PremiseID.Trim(),
                    Code = !string.IsNullOrEmpty(workOrderRequestDTO.PremiseCode) ? workOrderRequestDTO.PremiseCode.Trim() : workOrderRequestDTO.PremiseID.Trim(),
                    PriorityID = priorityID,
                    CategoryID = categoryID,
                    IntegartionId_Class = workOrderRequestDTO.ClassID,
                    StatusID = statusID,
                    Latitude = lat,
                    Longitude = lng,
                    ZoneID = zoneID
                };

                var customerAccount = new CustomerAccountDTO()
                {
                    //CustomerId = customerID,
                    //CustomerLocationId = customerLocationID,
                    AccountId_Integration = workOrderRequestDTO.AccountID.Trim(),
                    ServiceTypeId = serviceTypeID, 
                    SoqyaBalance = 0
                };

                var dto = new SoqyaCustomerBalanceDTO()
                {
                    Customer = customer, 
                    CustomerLocation = customerLocation, 
                    Account = customerAccount
                };

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.BeforeCreateCustomer, JsonConvert.SerializeObject(dto))));
                //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("{0}: {1}", OperationStepEnum.BeforeCreateCustomer, JsonConvert.SerializeObject(customer)));

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/Customer/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.PostAsJsonAsync("CreateCustomerBalance", dto);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<SoqyaCustomerBalanceDTO>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<CustomerDTO>>.FromResult(DescriptiveResponse<SoqyaCustomerBalanceDTO>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<CustomerDTO>>.FromResult(DescriptiveResponse<SoqyaCustomerBalanceDTO>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        public Task<DescriptiveResponse<CustomerDTO>> CreateCustomer(WorkOrderRequestDTO workOrderRequestDTO, IEnumerable<LookUpDTO<int>> idTypes, string commandAPI_URL, string token)
        {
            try
            {
                var idType = idTypes.FirstOrDefault(x => x.IntegrationId == workOrderRequestDTO.IDTypeID);

                var customer = new CustomerDTO()
                {
                    IntegrationId = workOrderRequestDTO.AccountID.Trim(),
                    Code = workOrderRequestDTO.CustomerCode.Trim(),
                    Email = !string.IsNullOrEmpty(workOrderRequestDTO.Email) ? workOrderRequestDTO.Email.Trim() : "mail123@mailTest123.com",
                    IDNumber = workOrderRequestDTO.IDNumber.Trim(),
                    FullName = workOrderRequestDTO.PersonName.Trim(),
                    IDTypeID = idType != null ? idType.Id : 0,
                    Mobile = workOrderRequestDTO.PersonMobile.Trim()
                };

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.BeforeCreateCustomer, JsonConvert.SerializeObject(customer))));
                //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("{0}: {1}", OperationStepEnum.BeforeCreateCustomer, JsonConvert.SerializeObject(customer)));

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/Customer/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.PostAsJsonAsync("CreateCustomer", customer);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<CustomerDTO>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<CustomerDTO>>.FromResult(DescriptiveResponse<CustomerDTO>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<CustomerDTO>>.FromResult(DescriptiveResponse<CustomerDTO>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        public Task<DescriptiveResponse<CustomerLocationDTO>> CreateCustomerLocation(WorkOrderRequestDTO workOrderRequestDTO, IEnumerable<LookUpDTO<int>> priorities, IEnumerable<LookUpDTO<int>> categories,
            IEnumerable<LookUpDTO<int>> classes, IEnumerable<LookUpDTO<int>> statuses, long customerID, long zoneID, int priorityID, int categoryID, int statusID, string commandAPI_URL, string token)
        {
            try
            {
                var cl_Class = classes.FirstOrDefault(x => x.IntegrationId == workOrderRequestDTO.ClassID);

                var coordinatesArr = workOrderRequestDTO.PremiseCoordinates.Split(' ');
                double lat;
                double lng;
                double.TryParse(coordinatesArr[1], out lat);
                double.TryParse(coordinatesArr[0], out lng);

                var customerLocation = new CustomerLocationDTO()
                {
                    CustomerID = customerID,
                    IntegrationId = workOrderRequestDTO.PremiseID.Trim(),
                    Code = !string.IsNullOrEmpty(workOrderRequestDTO.PremiseCode) ? workOrderRequestDTO.PremiseCode.Trim() : workOrderRequestDTO.PremiseID.Trim(),
                    PriorityID = priorityID,
                    CategoryID = categoryID,
                    ClassID = cl_Class != null ? cl_Class.Id : 0,
                    StatusID = statusID,
                    Latitude = lat,
                    Longitude = lng,
                    ZoneID = zoneID
                };

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.BeforeCreateCustomerLocation, JsonConvert.SerializeObject(customerLocation))));
                //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("{0}: {1}", OperationStepEnum.BeforeCreateCustomerLocation, JsonConvert.SerializeObject(customerLocation)));

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/Customer/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.PostAsJsonAsync("CreateCustomerLocation", customerLocation);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<CustomerLocationDTO>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<CustomerLocationDTO>>.FromResult(DescriptiveResponse<CustomerLocationDTO>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<CustomerLocationDTO>>.FromResult(DescriptiveResponse<CustomerLocationDTO>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        public Task<DescriptiveResponse<CustomerAccountDTO>> CreateCustomerAccount(WorkOrderRequestDTO workOrderRequestDTO, long customerID, long customerLocationID, int serviceTypeID, string commandAPI_URL, string token)
        {
            try
            {
                var customerAccount = new CustomerAccountDTO()
                {
                    CustomerId = customerID,
                    CustomerLocationId = customerLocationID,
                    AccountId_Integration = workOrderRequestDTO.AccountID.Trim(),
                    ServiceTypeId = serviceTypeID
                };

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.BeforeCreateCustomerAccount, JsonConvert.SerializeObject(customerAccount))));
                //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("{0}: {1}", OperationStepEnum.BeforeCreateCustomerAccount, JsonConvert.SerializeObject(customerAccount)));

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/Customer/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.PostAsJsonAsync("CreateCustomerAccount", customerAccount);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<CustomerAccountDTO>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<CustomerLocationDTO>>.FromResult(DescriptiveResponse<CustomerAccountDTO>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<CustomerLocationDTO>>.FromResult(DescriptiveResponse<CustomerAccountDTO>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }
        #endregion

        #region Lookups
        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetWorkOrderStatuses(string queryAPI_URL, string token)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GetWorkOrderStatuses", queryAPI_URL));
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
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetReasonsByStatusId(string queryAPI_URL, int statusId, string token)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GeReasonsByStatusId?statusId={1}", queryAPI_URL, statusId));
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
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetAccessories(string queryAPI_URL, string token)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GetAccessories", queryAPI_URL));
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
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetServiceTypes(string queryAPI_URL, string token)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GetServiceTypes", queryAPI_URL));
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
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationPriorities(string queryAPI_URL, string token)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GetCustomerLocationPriorities", queryAPI_URL));
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
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationClasses(string queryAPI_URL, string token)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GetCustomerLocationClasses", queryAPI_URL));
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
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationCategories(string queryAPI_URL, string token)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GetCustomerLocationCategories", queryAPI_URL));
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
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetPersonalIDTypes(string queryAPI_URL, string token)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GetPersonalIDTypes", queryAPI_URL));
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
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<IEnumerable<LookUpDTO<int>>> GetCustomerLocationStatuses(string queryAPI_URL, string token)
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
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.GetAsync(string.Format("{0}/api/Lookup/GetCustomerLocationStatuses", queryAPI_URL));
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
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return DescriptiveResponse<IEnumerable<LookUpDTO<int>>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }
        #endregion

        #region XML
        public WorkOrderRequestDTO ParsingXMLForCreatingWO(string strDoc)
        {
            var dto = new WorkOrderRequestDTO();
            XDocument xDoc = XDocument.Parse(strDoc);

            var orderElements = xDoc.Element("Schema").Elements("Input").Elements("Order").Elements().ToList();

            foreach (var elem in orderElements)
            {
                if (elem.Name == "ORDERNUMBER")
                    dto.OrderNumber = elem.Value;

                if (elem.Name == "SOURCEAPPLICATION")
                    dto.SourceApplication = !string.IsNullOrEmpty(elem.Value) ? elem.Value : "HAYAT";

                if (elem.Name == "CISDIVISION")
                    dto.CISDivision = elem.Value;

                if (elem.Name == "TRANSACTIONID")
                    dto.TransactionID = elem.Value;

                if (elem.Name == "SCHEDDTTM")
                {
                    var dtParams = elem.Value.Split(' ');
                    var dParams = dtParams[0].Split('-');
                    var tParams = dtParams[1].Split(':');

                    DateTime dt = new DateTime(int.Parse(dParams[2]), int.Parse(dParams[1]), int.Parse(dParams[0]), int.Parse(tParams[0]), int.Parse(tParams[1]), int.Parse(tParams[2].Split('.')[0]));
                    dto.ScheduledDeliveryTime = dt;
                }

                if (elem.Name == "CREDTTM")
                {
                    var dtParams = elem.Value.Split(' ');
                    var dParams = dtParams[0].Split('-');
                    var tParams = dtParams[1].Split(':');

                    DateTime dt = new DateTime(int.Parse(dParams[2]), int.Parse(dParams[1]), int.Parse(dParams[0]), int.Parse(tParams[0]), int.Parse(tParams[1]), int.Parse(tParams[2].Split('.')[0]));
                    dto.CreationTime = dt;
                }

                if (elem.Name == "SERVICETYPE")
                    dto.ServiceTypeCode = elem.Value;

                if (elem.Name == "CONTACTNAME")
                    dto.ReceiverName = elem.Value;

                if (elem.Name == "CONTACTMOBILE")
                    dto.ReceiverMobile = elem.Value;

                int quantity;
                if (int.TryParse(elem.Name == "TANKERSIZE" ? elem.Value : string.Empty, out quantity))
                    dto.OrderQuantity = quantity;

                if (elem.Name == "TANKERACCESSORIES")
                {
                    //Accessories
                }

                if (elem.Name == "COMMENT")
                    dto.Comment = elem.Value;

                if (elem.Name == "CONFIRMATIONCODE")
                    dto.ConfirmationCode = elem.Value;
            }

            var accountElements = xDoc.Element("Schema").Elements("Input").Elements("Account").Elements().ToList();

            foreach (var elem in accountElements)
            {
                if (elem.Name == "ACCOUNTID")
                    dto.AccountID = elem.Value;

                if (elem.Name == "CUSTOMERCLASS")
                    dto.ClassID = elem.Value;
            }

            var personElements = xDoc.Element("Schema").Elements("Input").Elements("Person").Elements().ToList();

            foreach (var elem in personElements)
            {
                if (elem.Name == "PERSONID")
                    dto.CustomerCode = elem.Value;

                if (elem.Name == "PERSONPRIMARYNAME")
                    dto.PersonName = elem.Value;

                if (elem.Name == "PERSONIDTYPE")
                    dto.IDTypeID = elem.Value.Trim();

                if (elem.Name == "PERSONIDVALUE")
                    dto.IDNumber = elem.Value;

                if (elem.Name == "MOBILENUMBER")
                    dto.PersonMobile = elem.Value;
            }

            var custLocationElements = xDoc.Element("Schema").Elements("Input").Elements("Premise").Elements().ToList();

            foreach (var elem in custLocationElements)
            {
                if (elem.Name == "PREMISEID")
                    dto.PremiseID = elem.Value;

                if (elem.Name == "XYCOORDINATESGF")
                    dto.PremiseCoordinates = elem.Value;
            }

            return dto;
        }

        public EventWorkOrderDTO ParsingXMLForUpdatingWO(string strDoc, out bool isChangeStatus)
        {
            isChangeStatus = false;

            var dto = new EventWorkOrderDTO();
            XDocument xDoc = XDocument.Parse(strDoc);

            var inputList = xDoc.Element("Schema").Elements("Input").Elements().ToList();

            foreach (var input in inputList)
            {
                if (input.Name == "ORDERNUMBER")
                    dto.OrderNumber = input.Value;

                if (input.Name == "SOURCEAPPLICATION")
                    dto.SourceApplication = input.Value;

                if (input.Name == "CISDIVISION")
                    dto.CISDivision = input.Value;

                if (input.Name == "TRANSACTIONID")
                    dto.TransactionID = input.Value;

                if (input.Name == "ORDERSTATUS")
                {
                    int statusID;
                    if (int.TryParse(!string.IsNullOrEmpty(input.Value) ? input.Value : string.Empty, out statusID))
                        dto.StatusID = statusID;
                }

                if (input.Name == "UPDATEDTTM")
                {
                    var dtParams = input.Value.Split(' ');
                    var dParams = dtParams[0].Split('-');
                    var tParams = dtParams[1].Split(':');

                    DateTime dt = new DateTime(int.Parse(dParams[2]), int.Parse(dParams[1]), int.Parse(dParams[0]), int.Parse(tParams[0]), int.Parse(tParams[1]), int.Parse(tParams[2].Split('.')[0]));
                    dto.StatusTime = dt;
                }

                if (input.Name == "PARAMETERS")
                {
                    var paramList = input.Elements().ToList();

                    foreach (var param in paramList)
                    {
                        var paramValues = param.Elements().ToList();

                        if (paramValues[0].Value == "TMSID")
                            dto.WorkOrderID = 0;// paramValues[1].Value;

                        if (paramValues[0].Value == "CANREASON")
                            dto.CancelReasonCode = paramValues[1].Value;

                        if (paramValues[0].Value == "FAILREASON")
                            dto.CancelReasonCode = paramValues[1].Value;

                        if (paramValues[0].Value == "COMMENT")
                            dto.StatusComment = paramValues[1].Value;
                    }
                }
            }

            //var paramList = xDoc.Element("Schema").Elements("Input").Elements().Elements("Parameters").Elements("Parameter").Elements().ToList();
            ////var paramList = xDoc.Element("schema").Elements("parameters").Elements("parameter").ToList();

            //foreach (var param in paramList)
            //{
            //    switch (param.Element("PARAMETERNAME").Value)
            //    {
            //        #region Common
            //        case "TMSID":
            //            long workOrderID;
            //            if (long.TryParse(param.Element("PARAMETERVALUE") != null ? param.Element("parameterValue").Value : string.Empty, out workOrderID))
            //                dto.WorkOrderID = workOrderID;
            //            break;
            //        #endregion

            //        #region Change Status
            //        case "CANREASON":
            //            int cancelReasonID;
            //            if (int.TryParse(param.Element("PARAMETERVALUE") != null ? param.Element("parameterValue").Value : string.Empty, out cancelReasonID))
            //            {
            //                if (dto.StatusReasonID == 0)
            //                    dto.StatusReasonID = cancelReasonID;
            //            }
            //            break;
            //        case "FAILREASON":
            //            int failReasonID;
            //            if (int.TryParse(param.Element("PARAMETERVALUE") != null ? param.Element("PARAMETERVALUE").Value : string.Empty, out failReasonID))
            //            {
            //                if (dto.StatusReasonID == 0)
            //                    dto.StatusReasonID = failReasonID;
            //            }
            //            break;
            //        case "COMMENT":
            //            string statusComment = param.Element("PARAMETERVALUE") != null ? param.Element("PARAMETERVALUE").Value : string.Empty;
            //            dto.StatusComment = statusComment;
            //            break;
            //        #endregion

            //        #region Update WorkOrder Info
            //        case "ORDERQUANTITY":
            //            int orderQuantity;
            //            if (int.TryParse(param.Element("ParameterValue").Value, out orderQuantity))
            //                dto.OrderQuantity = orderQuantity;
            //            break;
            //        case "SERVICETYPEID":
            //            int serviceTypeID;
            //            if (int.TryParse(param.Element("ParameterValue").Value, out serviceTypeID))
            //                dto.ServiceTypeID = serviceTypeID;
            //            break;
            //        case "STATIONID":
            //            Guid stationID;
            //            if (Guid.TryParse(param.Element("ParameterValue").Value, out stationID))
            //                dto.StationID = stationID;
            //            break;
            //        case "SCHEDULEDDELIVERYTIME":
            //            DateTime scheduledDeliveryTime;
            //            if (DateTime.TryParse(param.Element("ParameterValue").Value, out scheduledDeliveryTime))
            //                dto.ScheduledDeliveryTime = scheduledDeliveryTime;
            //            break;
            //        case "CUSTOMERLOCATIONID":
            //            long customerLocationID;
            //            if (long.TryParse(param.Element("ParameterValue").Value, out customerLocationID))
            //                dto.CustomerLocationID = customerLocationID;
            //            break;
            //        case "ACCESSORIES":
            //            if (!string.IsNullOrEmpty(param.Element("ParameterValue").Value))
            //            {
            //                string[] accArray = param.Element("ParameterValue").Value.Split(',');

            //                foreach (var acc in accArray)
            //                {
            //                    int id;
            //                    if (int.TryParse(acc, out id))
            //                        dto.Accessories.Add(new AccessoryDTO() { ID = id });
            //                }
            //            }
            //            break;
            //            #endregion
            //    }
            //}

            if (dto != null && !string.IsNullOrEmpty(dto.OrderNumber) && dto.StatusID > 0)
                isChangeStatus = true;

            return dto;
        }
        #endregion

        #region Consuming Services
        public string CallGISService(string premiseCoordinates, string orderNumber, string sourceApp, string transactionId)
        {
            try
            {
                //check if zoneID not exist call GIS service
                var gisServiceClient = new GISServiceRef.echannelGetLocationInfoPortTypeClient();

                var gisRequest = new GISServiceRef.echannelGetLocationInfoRequest()
                {
                    orgCode = "KSA",
                    locType = "LONGLAT",
                    locValue = premiseCoordinates,
                    outputinfoType = "FSZONE",
                    sourceApplication = GetGISSourceApp(sourceApp),
                    transactionId = transactionId
                };

                var result = Task.Run(async () => await gisServiceClient.echannelGetLocationInfoOperationAsync(gisRequest)).ConfigureAwait(false);

                var gisResponse = result.GetAwaiter().GetResult().echannelGetLocationInfoResponse;

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0} Response: {1}", OperationStepEnum.CallingGIS, JsonConvert.SerializeObject(gisResponse))));

                if (gisResponse != null && gisResponse.status.ToUpper() == "OK")
                    return gisResponse.outputValue;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                return null;
            }
        }

        public bool CallUpdateWONotificationService(string orderNumber, string cis, string transactionId, string token)
        {
            try
            {
                var gisServiceClient = new TMSNwcUpdateWONotification_pttClient();

                var requestParameters = new List<TMSWONotificationRequestParameter>();
                requestParameters.Add(new TMSWONotificationRequestParameter()
                {
                    parameterName = "TMSID",
                    parameterValue = orderNumber
                });

                var updateRequest = new TMSWONotificationRequest()
                {
                    sourceApplication = TMSWONotificationRequestSourceApplication.TMS,
                    orderNumber = orderNumber,
                    cisDivision = GetCISDivison(cis),
                    orderStatus = TMSWONotificationRequestOrderStatus.Item1,
                    updateDateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                    parameters = requestParameters.ToArray(),
                    transactionId = transactionId
                };

                var result = Task.Run(async () => await gisServiceClient.updateWorkOrserStatusAsync(updateRequest)).ConfigureAwait(false);

                var gisResponse = result.GetAwaiter().GetResult().TMSWONotificationResponse;

                // Log
                LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("{0}: {1}", OperationStepEnum.UpdateWONotificationService, JsonConvert.SerializeObject(gisResponse))));
                //ExceptionManager.GetExceptionLogger().LogInformation(string.Format("{0}: {1}", OperationStepEnum.UpdateWONotificationService, JsonConvert.SerializeObject(gisResponse)));

                if (gisResponse != null && gisResponse.status.ToUpper() == "OK")
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return false;
            }
        }

        private echannelGetLocationInfoRequestSourceApplication GetGISSourceApp(string source)
        {
            var sourceApp = echannelGetLocationInfoRequestSourceApplication.CCB;

            if (source.ToUpper() == echannelGetLocationInfoRequestSourceApplication.EBRANCH.ToString())
                sourceApp = echannelGetLocationInfoRequestSourceApplication.EBRANCH;
            if (source.ToUpper() == echannelGetLocationInfoRequestSourceApplication.MOBAPP.ToString())
                sourceApp = echannelGetLocationInfoRequestSourceApplication.MOBAPP;
            if (source.ToUpper() == echannelGetLocationInfoRequestSourceApplication.TMS.ToString())
                sourceApp = echannelGetLocationInfoRequestSourceApplication.TMS;

            return sourceApp;
        }

        private cisDivison GetCISDivison(string cis)
        {
            var cisDiv = cisDivison.AS;

            if (cis.ToUpper() == cisDivison.BA.ToString())
            {
                cisDiv = cisDivison.BA;
            }
            if (cis.ToUpper() == cisDivison.HA.ToString())
            {
                cisDiv = cisDivison.HA;
            }
            if (cis.ToUpper() == cisDivison.HS.ToString())
            {
                cisDiv = cisDivison.HS;
            }
            if (cis.ToUpper() == cisDivison.JC.ToString())
            {
                cisDiv = cisDivison.JC;
            }
            if (cis.ToUpper() == cisDivison.JCBU.ToString())
            {
                cisDiv = cisDivison.JCBU;
            }
            if (cis.ToUpper() == cisDivison.JF.ToString())
            {
                cisDiv = cisDivison.JF;
            }
            if (cis.ToUpper() == cisDivison.JZ.ToString())
            {
                cisDiv = cisDivison.JZ;
            }
            if (cis.ToUpper() == cisDivison.JZBU.ToString())
            {
                cisDiv = cisDivison.JZBU;
            }
            if (cis.ToUpper() == cisDivison.MC.ToString())
            {
                cisDiv = cisDivison.MC;
            }
            if (cis.ToUpper() == cisDivison.MCBU.ToString())
            {
                cisDiv = cisDivison.MCBU;
            }
            if (cis.ToUpper() == cisDivison.MD.ToString())
            {
                cisDiv = cisDivison.MD;
            }
            if (cis.ToUpper() == cisDivison.MK.ToString())
            {
                cisDiv = cisDivison.MK;
            }
            if (cis.ToUpper() == cisDivison.NA.ToString())
            {
                cisDiv = cisDivison.NA;
            }
            if (cis.ToUpper() == cisDivison.NJ.ToString())
            {
                cisDiv = cisDivison.NJ;
            }
            if (cis.ToUpper() == cisDivison.QS.ToString())
            {
                cisDiv = cisDivison.QS;
            }
            if (cis.ToUpper() == cisDivison.RC.ToString())
            {
                cisDiv = cisDivison.RC;
            }
            if (cis.ToUpper() == cisDivison.RCBU.ToString())
            {
                cisDiv = cisDivison.RCBU;
            }
            if (cis.ToUpper() == cisDivison.RI.ToString())
            {
                cisDiv = cisDivison.RI;
            }
            if (cis.ToUpper() == cisDivison.SH.ToString())
            {
                cisDiv = cisDivison.SH;
            }
            if (cis.ToUpper() == cisDivison.TB.ToString())
            {
                cisDiv = cisDivison.TB;
            }
            if (cis.ToUpper() == cisDivison.TC.ToString())
            {
                cisDiv = cisDivison.TC;
            }
            if (cis.ToUpper() == cisDivison.TCBU.ToString())
            {
                cisDiv = cisDivison.TCBU;
            }

            return cisDiv;
        }
        #endregion

        #region Soqya
        public Task<DescriptiveResponse<bool>> AddSoqyaCustomerBalance(string commandAPI_URL, string token, SoqyaCustomerBalanceDTO request)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/Integration/", commandAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", commandAPI_URL);

                    var postTask = client.PostAsJsonAsync("AddSoqyaCustomerBalance", request);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<bool>>();
                        readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<bool>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<EventWorkOrderDTO>>.FromResult(DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }

        public Task<DescriptiveResponse<SearchResult<SoqyaScheduleDTO>>> GetSoqyaSchedules(string queryAPI_URL, string token, SoqyaScheduleSC searchCriteria)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(string.Format("{0}/api/Soqya/", queryAPI_URL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("IsIntigration", "true");
                    client.DefaultRequestHeaders.Add("Lang", "en");
                    client.DefaultRequestHeaders.Add("Origin", queryAPI_URL);

                    var postTask = client.PostAsJsonAsync("SearchSoqyaSchedules", searchCriteria);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<DescriptiveResponse<SearchResult<SoqyaScheduleDTO>>>();
                        //readTask.Wait();

                        return readTask;
                    }
                }

                return Task<DescriptiveResponse<SearchResult<WorkOrderDTO>>>.FromResult(DescriptiveResponse<SearchResult<SoqyaScheduleDTO>>.Error(ErrorStatus.COMMIT_FAIL));
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return Task<DescriptiveResponse<SearchResult<WorkOrderDTO>>>.FromResult(DescriptiveResponse<SearchResult<SoqyaScheduleDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR));
            }
        }
        #endregion



        public List<NWC_CCB_Integration.DTO.Models.AvailableTankerSize.TankerSize> GetDefaultTankerSizes(string queryAPI_URL, string token, string CISDIVISION)
        {
            var defaultTankerSizes = new List<NWC_CCB_Integration.DTO.Models.AvailableTankerSize.TankerSize>();
            DescriptiveResponse< List<int>> sizes = new DescriptiveResponse<List<int>> () ;

            //Get customized per branches Sizes
            if (CISDIVISION.Length>1)
            {
                 sizes = GetDefefaultTankerSizesByCIS( queryAPI_URL,  token, CISDIVISION.Substring(0,2));
                if (sizes.Value.Count > 0)
                {
                    foreach (var size in sizes.Value)
                    {
                        defaultTankerSizes.Add(new DTO.Models.AvailableTankerSize.TankerSize()
                        {
                            TANKERSIZEVALUE = size,
                            TANKERPRICE = 0,
                        }); ;
                    }
                    return defaultTankerSizes;
                }
            }
   

            //Get Fixxed Sizes
            var tankerSizes = Context.NWC_DefaultTankerSize.ToList();
            foreach (var ts in tankerSizes)
            {
                defaultTankerSizes.Add(new DTO.Models.AvailableTankerSize.TankerSize()
                {
                    TANKERSIZEVALUE = ts.TankerSize,
                    TANKERPRICE = ts.TankerPrince,
                    TANKERDELIVERYDDTM = ts.TankerDeleviryTime
                });
            }
            return defaultTankerSizes;
        }

        public bool AddDeferredWorkOrder(WorkOrderRequestDTO dto, string failureMessage)
        {
            try
            {
                if (this.Context.NWC_DeferredWorkOrder.Where(x => x.ORDERNUMBER == dto.OrderNumber).Any())
                    return false;

                Context.NWC_DeferredWorkOrder.Add(new NWC_DeferredWorkOrder()
                {
                    ORDERNUMBER = dto.OrderNumber,
                    CISDIVISION = dto.CISDivision,
                    COMMENT = dto.Comment,
                    CONFIRMATIONCODE = dto.ConfirmationCode,
                    CONTACTMOBILE = dto.ContactMobile,
                    CONTACTNAME = dto.ContactName,
                    CREDTTM = dto.CreationTime.ToString("dd/MM/yyyy HH:mm:ss"),
                    SCHEDDTTM = dto.ScheduledDeliveryTime.ToString("dd/MM/yyyy HH:mm:ss"),
                    SERVICETYPE = dto.ServiceTypeCode,
                    SOURCEAPPLICATION = dto.SourceApplication,
                    TANKERSIZE = dto.OrderQuantity.ToString(),
                    TRANSACTIONID = dto.TransactionID,
                    ACCOUNTID = dto.AccountID,
                    CUSTOMERCLASS = dto.ClassID,
                    MOBILENUMBER = dto.PersonMobile,
                    PERSONID = dto.CustomerCode,
                    PERSONIDTYPE = dto.IDTypeID,
                    PERSONIDVALUE = dto.IDNumber,
                    PERSONPRIMARYNAME = dto.PersonName,
                    PREMISEID = dto.PremiseID,
                    XYCOORDINATESGF = dto.PremiseCoordinates,
                    ErrorMSG = failureMessage,
                    StatusId = 1,
                    CreateTime = DateTime.Now
                });

                Context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex));

                //ExceptionManager.GetExceptionLogger().LogException(ex);
                return false;
            }
        }
        public string GetTankerSizesString(List<TankerSize> sizes)
        {
            var st = "";
            if (sizes == null || sizes.Count == 0)
                return "";
            foreach (var size in sizes)
            {
                if (size != null)
                {
                    st = st + "-" + size.TANKERSIZEVALUE.ToString();
                }
            }
            return st.Length > 1 ? st.Substring(1, st.Length-1) : "";
        }
    }
}
