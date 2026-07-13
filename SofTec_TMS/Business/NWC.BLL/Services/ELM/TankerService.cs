using Infrastructure;
using LinqKit;
using Newtonsoft.Json;
using NWC.BLL.Interfaces;
using NWC.BLL.Interfaces.ELM;
using NWC.BLL.Validators;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common.ELM;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.Models.ELM;
using NWC.DTO.Models.TMS;
using NWC.DTO.SearchCriteria;
using NWC.DTO.Wrapper;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace NWC.BLL.Services.ELM
{
    public class TankerService : ITankerService
    {
        #region Properties
        private readonly ILoggedInUserService _loggedInUser;
        private readonly DbContext ctx;
        private readonly IUnitofWork _unitOfWork;
        private readonly IRepository<Transporter> _repository;
        private readonly IRepository<TransporterType> _repositoryTransporterType;
        private readonly IRepository<TransporterStatus> _StatusRepository;
        private readonly IRepository<Transporter_Staff> _repositoryTransporterStaff;
        private readonly IRepository<Staff> _staffRepository;
        private readonly IRepository<ELM_Tanker> _elmTankerRepository;
        private readonly IRepository<ELMTransaction> _elmTransactionRepository;
        private readonly IRepository<NWC_ContractTermsViolations> _NWC_ContractViolationsRepository;
        private readonly IRepository<UserBranchPermission> _userBranchPermistionRepository;
        private readonly IRepository<aspnet_Users> _userRepository;

        private MembershipCreateStatus status;

        #endregion
        public TankerService(ILoggedInUserService loggedInUser, DbContext context = null)
        {
            _loggedInUser = loggedInUser;
            ctx = (context == null ? new NWCContext() : context);
            _unitOfWork = new UnitofWork(ctx);
            _repository = new Repository<Transporter>(ctx);
            _StatusRepository = new Repository<TransporterStatus>(ctx);
            _repositoryTransporterStaff = new Repository<Transporter_Staff>(ctx);
            _staffRepository = new Repository<Staff>(ctx);
            _repositoryTransporterType = new Repository<TransporterType>(ctx);
            _elmTankerRepository = new Repository<ELM_Tanker>(ctx);
            _elmTransactionRepository = new Repository<ELMTransaction>(ctx);
            _NWC_ContractViolationsRepository = new Repository<NWC_ContractTermsViolations>(ctx);
            _NWC_ContractViolationsRepository = new Repository<NWC_ContractTermsViolations>(ctx);
            _userRepository = new Repository<aspnet_Users>(ctx);
            _userBranchPermistionRepository = new Repository<UserBranchPermission>(ctx);

        }
        /// <summary>
        /// serialize elm record as json object in elm tanker table
        /// </summary>
        /// <param name="TankerDTO"></param>
        /// <returns></returns>
        public DescriptiveResponse<string> Add(TankerDTO TankerDTO)
        {
            try
            {
                ELM_Tanker tanker = new ELM_Tanker();
                if (TankerDTO.Drivers != null && !TankerDTO.Drivers.Any())
                {
                    return DescriptiveResponse<string>.Error("Empty list", ErrorStatus.Empty_List);
                }
                string ValidationMessage = ValidateTanker(TankerDTO);
                if (!string.IsNullOrEmpty(ValidationMessage))
                {
                    return DescriptiveResponse<string>.Error("Missing required data", ErrorStatus.MissingRequiredDate);
                }
                if (!isEngLanguage(TankerDTO.transporterBrand))
                    return DescriptiveResponse<string>.Error("Not supported language", ErrorStatus.language_Not_Supported);

                string IsPlateExistance = ValidatePlateExistance(TankerDTO.PlateNo);
                if (!string.IsNullOrEmpty(IsPlateExistance))
                {
                    return DescriptiveResponse<string>.Error("Exists before", ErrorStatus.Existsbefore);
                }
                string dataTypesCheck = ValidateDataTypes(TankerDTO);
                if (!string.IsNullOrEmpty(dataTypesCheck))
                {
                    return DescriptiveResponse<string>.Error("Invalid data type format", ErrorStatus.Invaliddatatype);
                }
                tanker.DTO = JsonConvert.SerializeObject(TankerDTO);
                tanker.CreateDate = DateTime.Now;
                tanker.IsInserted = false;
                var Transporter = _repository.GetQuery().FirstOrDefault(x => x.plateNo == TankerDTO.PlateNo);
                if (Transporter != null)
                    tanker.permitNo = Transporter.permitNo;
                else
                    tanker.permitNo = Guid.NewGuid();

                tanker.retrials = 0;
                using (_unitOfWork)
                {
                    _elmTankerRepository.Add(tanker);
                    return DescriptiveResponse<string>.Success("Done", tanker.permitNo.ToString());

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + '|' + ex.InnerException?.InnerException.Message);
                LoggerManager.LogMsg(c => c.Log(ex, "TankerService => SaveTankerPermit: "));
                return DescriptiveResponse<string>.Error(ErrorStatus.INTERNAL_ERROR);

            }


        }

        /// <summary>
        /// Service to get tanker status from TMS system.
        /// Add Transaction Record in Transaction Table
        /// </summary>
        /// <param name="TankerDTO"></param>
        /// <returns></returns>
        public DescriptiveResponse<ELMTransactionDTO> TankerAccessAuthorization(TankerAccessDTO TankerDTO)
        {
            var Transporter = _repository.GetQuery().FirstOrDefault(x => x.plateNo == TankerDTO.PlateNo);
            try
            {
                bool status;
                if (Transporter == null)
                    return DescriptiveResponse<ELMTransactionDTO>.Error("Not found", ErrorStatus.NotFound);
                else
                {
                    var ELmTransaction = new ELMTransaction();
                    string ValidationMessage = ValidateAccessAuth(TankerDTO);
                    if (!string.IsNullOrEmpty(ValidationMessage))
                    {
                        return DescriptiveResponse<ELMTransactionDTO>.Error("Missing required data", ErrorStatus.MissingRequiredDate);
                    }
                    ELmTransaction.ID = Guid.NewGuid();
                    ELmTransaction.ELMTransactionId = TankerDTO.TransactionId;
                    ELmTransaction.TransactionId = Guid.NewGuid();
                    ELmTransaction.Value = GetTransporterStatus(Transporter);
                    ELmTransaction.permitNo = TankerDTO.permitNo;
                    ELmTransaction.plateNo = TankerDTO.PlateNo;
                    ELmTransaction.ResponseTime = DateTime.Now;
                    if (TankerDTO.RequestTime == DateTime.MinValue || TankerDTO.RequestTime == null)
                        TankerDTO.RequestTime = DateTime.Now;
                    ELmTransaction.RequestTime = TankerDTO.RequestTime;

                    using (_unitOfWork)
                    {
                        var ELMTransactionDTO = new ELMTransactionDTO();
                        ELMTransactionDTO.TransactionId = ELmTransaction.TransactionId;
                        ELMTransactionDTO.value = ELmTransaction.Value;
                        ELMTransactionDTO.ResponseTime = ELmTransaction.ResponseTime;

                        _elmTransactionRepository.Add(ELmTransaction);
                        return DescriptiveResponse<ELMTransactionDTO>.Success("Done", ELMTransactionDTO);

                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "TankerService => TankerAccessAuthorization: "));
                return DescriptiveResponse<ELMTransactionDTO>.Error(ErrorStatus.INTERNAL_ERROR);
            }

        }

        /// <summary>
        /// Service to be used by eGate to get validate access authorization and automatically open gate accordingly.
        /// </summary>
        /// <param name="baseurl"></param>
        /// <param name="TankerDTO"></param>
        /// <returns></returns>
        public DescriptiveResponse<ELMTransactionDTO> TankerCheckStatus(string baseurl, TankerAccessDTO TankerDTO)
        {
            var transporter = _repository.GetQuery().FirstOrDefault(x => x.plateNo == TankerDTO.PlateNo);

            try
            {
                var ELMTransactionDTO = new ELMTransactionDTO();

                bool status;
                if (transporter == null)
                    return DescriptiveResponse<ELMTransactionDTO>.Error("Not found", ErrorStatus.NotFound);
                else
                {

                    var ELmTransaction = new ELMTransaction();
                    string ValidationMessage = ValidateCheckStatus(TankerDTO);
                    if (!string.IsNullOrEmpty(ValidationMessage))
                    {
                        return DescriptiveResponse<ELMTransactionDTO>.Error("Missing required data", ErrorStatus.MissingRequiredDate);
                    }
                    ELmTransaction.ID = Guid.NewGuid();
                    ELmTransaction.TransactionId = TankerDTO.TransactionId;
                    ELmTransaction.Value = TankerDTO.status;
                    ELmTransaction.permitNo = TankerDTO.permitNo;
                    ELmTransaction.plateNo = TankerDTO.PlateNo;
                    ELmTransaction.ResponseTime = DateTime.Now;
                    ELmTransaction.Reason = TankerDTO.Reason;
                    if (TankerDTO.RequestTime == DateTime.MinValue || TankerDTO.RequestTime == null)
                        TankerDTO.RequestTime = DateTime.Now;
                    ELmTransaction.RequestTime = TankerDTO.RequestTime;

                    using (_unitOfWork)
                    {
                        ELMTransactionDTO.TransactionId = ELmTransaction.TransactionId;
                        ELMTransactionDTO.value = TankerDTO.status;
                        ELMTransactionDTO.ResponseTime = ELmTransaction.ResponseTime;
                        _elmTransactionRepository.Add(ELmTransaction);
                    }
                    if (TankerDTO.status == true)
                    {
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(string.Format("{0}/api/Vehicle/", baseurl));
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                client.DefaultRequestHeaders.Add("Authorization", TankerDTO.token);
                                client.DefaultRequestHeaders.Add("IsIntigration", "true");
                                client.DefaultRequestHeaders.Add("Lang", "en");
                                client.DefaultRequestHeaders.Add("Origin", baseurl);
                                var postTask = client.PostAsJsonAsync(string.Format("{0}/api/Vehicle/ArriveVehicleToStation?vehicleID=" + transporter.ID, baseurl), new { });

                                postTask.Wait();

                                var result = postTask.Result;
                                if (result.IsSuccessStatusCode)
                                {
                                    var readTask = result.Content.ReadAsAsync<DescriptiveResponse<bool>>();
                                    readTask.Wait();

                                    var returnResult = readTask.Result;
                                    if (!returnResult.IsErrorState)
                                        return DescriptiveResponse<ELMTransactionDTO>.Success("Done", ELMTransactionDTO);
                                    else
                                        return DescriptiveResponse<ELMTransactionDTO>.Error(returnResult.ErrorDescription);
                                }
                            }
                            return DescriptiveResponse<ELMTransactionDTO>.Error(ErrorStatus.COMMIT_FAIL);
                        }
                        catch (Exception ex)
                        {
                            LoggerManager.LogMsg(c => c.Log(ex, "TankerService => TankerCheckStatus: "));
                            return DescriptiveResponse<ELMTransactionDTO>.Error(ex.Message, ErrorStatus.UNEXPECTED_ERROR);
                        }

                        //using (_unitOfWork)
                        //{
                        //    _elmTransactionRepository.Add(ELmTransaction);
                        //    return DescriptiveResponse<ELMTransaction>.Success(ELmTransaction);

                        //}
                    }
                    else
                    {
                        return DescriptiveResponse<ELMTransactionDTO>.Success("Done", ELMTransactionDTO);
                    }
                }

            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "TankerService => TankerAccessAuthorization: "));
                return DescriptiveResponse<ELMTransactionDTO>.Error(ErrorStatus.INTERNAL_ERROR);
            }

        }

        /// <summary>
        /// Deserialized the Elm Table and add it in Transporter Table
        /// </summary>
        /// <param name="subid"></param>
        /// <returns></returns>
        public DescriptiveResponse<string> SaveTankerToTransporter(Guid subid, int? retrials)
        {
            try
            {
                var dto = _elmTankerRepository.GetQuery().Where(x => x.IsInserted == false && x.retrials < retrials).ToList();
                var count = 0;
                var Total = dto.Count;
                foreach (var ElmTanker in dto)
                {

                    LoggerManager.LogMsg(c => c.Log("================================================================"));
                    try
                    {
                        count++;

                        LoggerManager.LogMsg(c => c.Log(string.Format("TankerService => SaveTankerToTransporter-{0} of {1}", count, Total)));

                        var transporterDTO = JsonConvert.DeserializeObject<TankerDTO>(ElmTanker.DTO);

                        string ValidationMessage = ValidateTanker(transporterDTO);
                        if (!string.IsNullOrEmpty(ValidationMessage))
                        {
                            ElmTanker.Exception = ValidationMessage;
                            ElmTanker.retrials += 1;

                            using (_unitOfWork)
                            {
                                _elmTankerRepository.Update(ElmTanker);
                            }
                        }
                        else
                        {
                            var newTranporter = new Transporter();
                            newTranporter.ID = Guid.NewGuid();
                            newTranporter.plateNo = transporterDTO.PlateNo;
                            newTranporter.code = string.Empty;
                            newTranporter.transporterTypeName = string.Empty;
                            newTranporter.maxSpeed = 0;
                            newTranporter.literPerKm = 0.0000M;
                            newTranporter.image = null;
                            newTranporter.color = null;
                            newTranporter.SIMCardNo = null;
                            newTranporter.providerName = null;
                            newTranporter.deviceCode = null;
                            newTranporter.engineNo = null;
                            newTranporter.insuranceNo = null;
                            newTranporter.insuredBy = null;
                            newTranporter.plateNo_Characters = transporterDTO.plateNo_Characters;
                            newTranporter.ELM_transporterBrand = transporterDTO.transporterBrand;
                            newTranporter.ELM_transporterProductionYear = transporterDTO.transporterProductionYear;

                            newTranporter.licenseNo = transporterDTO.licenseNo;
                            newTranporter.licenseExpiryDate = transporterDTO.licenseExpiryDate;
                            newTranporter.entranceDate = transporterDTO.entranceDate;
                            newTranporter.TankCapacity = transporterDTO.TankCapacity;
                            newTranporter.plateNo_Numbers = transporterDTO.plateNo_Numbers;
                            newTranporter.plateNo_Characters = transporterDTO.plateNo_Characters;
                            newTranporter.TransporterColor = transporterDTO.TransporterColor;
                            newTranporter.SubID = subid;
                            newTranporter.TransporterSource = (int)TransporterSource.ELM;//ELM
                            newTranporter.permitNo = ElmTanker.permitNo;//ELM
                            //newTranporter.TransactionId = Guid.NewGuid();//ELM
                            newTranporter.CreationTime = DateTime.Now;
                            newTranporter.PermitVersion = transporterDTO.PermitVersion;
                            newTranporter.PermitIssue = transporterDTO.PermitIssue;
                            if (!(transporterDTO.PermitExpiration == null && transporterDTO.PermitExpiration == DateTime.MinValue))
                                newTranporter.PermitExpiration = transporterDTO.PermitExpiration;

                            ElmTanker.IsInserted = true;
                            ElmTanker.InsertedDate = DateTime.Now;

                            using (_unitOfWork)
                            {

                                _repository.Add(newTranporter);


                                _elmTankerRepository.Update(ElmTanker);
                                LoggerManager.LogMsg(c => c.Log("TankerService => Elm tanker table updated"));

                            }
                            try
                            {
                                foreach (var driver in transporterDTO.Drivers)
                                {
                                    var user = Membership.CreateUser(driver.DriverIDValue, "Aa123456", driver.DriverIDValue + "@test.com", null, null, true, out status);

                                    if (status != MembershipCreateStatus.Success)
                                    {
                                        LoggerManager.LogMsg(c => c.Log("TankerService => Elm Add User"));
                                    }

                                    var newStaff = new Staff();
                                    newStaff.ID = Guid.NewGuid();

                                    newStaff.UserID = (Guid)user.ProviderUserKey;
                                    newStaff.Email = user.Email;

                                    newStaff.personalID = driver.DriverIDValue;
                                    newStaff.FirstName = driver.DriverFullNameAR;
                                    newStaff.mobileNumber = driver.DriverMobileNumber;
                                    newStaff.LisenceNumber = driver.DriverDrivingLicenseNumber;
                                    if (!string.IsNullOrEmpty(driver.DriverDrivingLicenseExpiryDate))
                                        newStaff.LisenceExpiryDate = DateTime.Parse(driver.DriverDrivingLicenseExpiryDate);
                                    newStaff.subID = subid;
                                    newStaff.CreationTime = DateTimeHelper.GetDateTimeNow();
                                    using (_unitOfWork)
                                    {
                                        _staffRepository.Add(newStaff);
                                        LoggerManager.LogMsg(c => c.Log("TankerService => staff record added"));
                                    }
                                    var TransporterStaff = new Transporter_Staff();
                                    TransporterStaff.Id = Guid.NewGuid();
                                    TransporterStaff.Staff = newStaff.ID;
                                    TransporterStaff.Transporter = newTranporter.ID;
                                    TransporterStaff.VehicleReceivingDate = DateTimeHelper.GetDateTimeNow();
                                    TransporterStaff.SubId = subid;
                                    TransporterStaff.CreationTime = DateTimeHelper.GetDateTimeNow();
                                    using (_unitOfWork)
                                    {
                                        _repositoryTransporterStaff.Add(TransporterStaff);
                                        LoggerManager.LogMsg(c => c.Log("TankerService => transporter staff updated"));

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LoggerManager.LogMsg(c => c.Log(ex, "TankerService => SaveTankerToTransporter - Driver Add: "));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerManager.LogMsg(c => c.Log(ex, "TankerService => SaveTankerToTransporter: "));
                        using (_unitOfWork)
                        {
                            LoggerManager.LogMsg(c => c.Log("TankerService => elm tanker error added"));

                        }

                    }

                }
            }
            catch (Exception ex) { }
            return DescriptiveResponse<string>.Success("Technical success. ");

        }




        #region Get Tanker Status

        /// <summary>
        /// Get Transporter Status
        /// </summary>
        /// <param name="transporter"></param>
        /// <returns></returns>
        bool GetTransporterStatus(Transporter transporter)
        {
            bool status = true;
            status = (transporter.licenseExpiryDate < DateTime.Now) ? false : true;
            if (transporter.status == (int)VehicleStatusEnum.Blacklisted)
                status = false;

            var VehicleViolation = _NWC_ContractViolationsRepository.GetQuery().FirstOrDefault(x => x.VehicleId == transporter.ID);
            if (VehicleViolation != null)
            {
                if (VehicleViolation.AddVehicleToBlacklist == true)
                    status = false;
            }
            return status;
        }
        #endregion
        #region Validate Tanker

        string ValidatePlateExistance(string plateNo)
        {
            string ValidationMessage = "";
            var isPlateExist = _repository.GetQuery().FirstOrDefault(x => x.plateNo == plateNo);
            if (isPlateExist != null)
                ValidationMessage += "Cannot add duplicate PlateNo";
            return ValidationMessage;
        }
        string ValidateDataTypes(TankerDTO TankerDTO)
        {
            string ValidationMessage = "";
            var isNumeric = int.TryParse(TankerDTO.transporterProductionYear, out int n);

            if (TankerDTO.transporterProductionYear.Length > 4 || isNumeric == false)
                ValidationMessage += "wrong datatype";
            return ValidationMessage;
        }
        string ValidateAccessAuth(TankerAccessDTO TankerDTO)
        {
            string validationMessage = "";
            validationMessage = "";
            if (TankerDTO.TransactionId == null || TankerDTO.TransactionId == Guid.Empty || TankerDTO.RequestTime == null || TankerDTO.RequestTime == DateTime.MinValue || TankerDTO.PlateNo == null)
                validationMessage += "Missing required data";
            return validationMessage;
        }
        string ValidateCheckStatus(TankerAccessDTO TankerDTO)
        {
            string validationMessage = "";
            validationMessage = "";
            if (TankerDTO.TransactionId == null || TankerDTO.TransactionId == Guid.Empty || TankerDTO.RequestTime == null || TankerDTO.RequestTime == DateTime.MinValue || TankerDTO.PlateNo == null || TankerDTO.status == null)
                validationMessage += "Missing required data";
            return validationMessage;
        }

        //bool ValidateTankerLanguage(TankerDTO dto)
        //{

        //}

        bool isEngLanguage(string str)
        {
            Regex regex = new Regex("^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z ]+[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z-_ ]*$");

            return regex.IsMatch(str);
        }

        string ValidateTanker(TankerDTO TankerDTO)
        {
            string validationMessage = "";
            validationMessage = "";
            if (TankerDTO.PlateNo == null || string.IsNullOrEmpty(TankerDTO.PlateNo))
                validationMessage += "PlateNo is Required,";
            if (TankerDTO.Drivers == null)
                validationMessage += "Drivers is Required,";
            if (TankerDTO.transporterBrand == null || string.IsNullOrEmpty(TankerDTO.transporterBrand))
                validationMessage += "TransporterBrand is Required,";
            if (TankerDTO.licenseNo == null || string.IsNullOrEmpty(TankerDTO.licenseNo))
                validationMessage += "licenseNo is Required,";
            if (TankerDTO.plateNo_Characters == null || string.IsNullOrEmpty(TankerDTO.plateNo_Characters))
                validationMessage += "plateNo_Characters is Required,";
            if (TankerDTO.TankCapacity == null || string.IsNullOrEmpty(TankerDTO.TankCapacity.ToString()))
                validationMessage += "TankCapacity is Required,";
            if (TankerDTO.entranceDate == null)
                validationMessage += "entranceDate is Required,";
            if (TankerDTO.licenseExpiryDate == null)
                validationMessage += "licenseExpiryDate is Required,";
            if (TankerDTO.transporterProductionYear == null || string.IsNullOrEmpty(TankerDTO.transporterProductionYear))
                validationMessage += "transporterProductionYear is Required,";
            return validationMessage;
        }
        #endregion
    }
}
