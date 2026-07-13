using NWC.BLL.Interfaces;
using NWC.BLL.Validators.TMS;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.DTO.Wrapper;
using NWC.DTO.Wrapper.TMS;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NWC.BLL.Services
{
    public partial class ControlPanelService : IControlPanelService
    {
        #region Query
        public DescriptiveResponse<IEnumerable<BranchSettingDTO>> GetBranchSettings(string branchName)
        {
            try
            {
                var myquery = this._BranchSetting_View.GetQuery()
                    .Where(s => s.IsSubBranch 
                                && s.SubId == this._loggedInUser.LoggedInUser.SubscriberId);

                if (!string.IsNullOrEmpty(branchName))
                {
                    var searchText = branchName.Trim();
                    myquery = myquery.Where(s => s.name.Contains(searchText) || s.code.Contains(searchText));
                }

                IEnumerable<BranchSettingDTO> result = null;
                if (myquery != null && myquery.Any())
                {
                    result = myquery.AsEnumerable().Select(a => a.WrapToBranchSettingDTO()).ToList();
                }

                return DescriptiveResponse<IEnumerable<BranchSettingDTO>>.Success(result);
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "ControlPanelService => GetBranchSettings: "));
                return DescriptiveResponse<IEnumerable<BranchSettingDTO>>.Error(ErrorStatus.UNEXPECTED_ERROR);
            }
        }

        public DescriptiveResponse<AreaDTO> getAreaById(Guid Id)
        {
            try
            {
                var area = new AreaDTO();
                area = _branchRepository.FindById(Id).WarapTOAreaDTO();

                return DescriptiveResponse<AreaDTO>.Success(area);
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<AreaDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }

        public DescriptiveResponse<AreaDTO> getLandmarkById(Guid Id)
        {
            try
            {
                var landmark = new AreaDTO();
                landmark = _landmarkRepository.FindById(Id).WrapToLandmarkDTO();

                return DescriptiveResponse<AreaDTO>.Success(landmark);
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<AreaDTO>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }

        #endregion

        #region Command
        public DescriptiveResponse<bool> SaveBranchSettings(IEnumerable<BranchSettingDTO> dto)
        {
            using (this._unitofWork)
            {
                foreach (var item in dto)
                {
                    var dbObj = this._BranchSetting.GetQuery()
                        .FirstOrDefault(s => s.BranchID == item.Id);

                    if (dbObj != null)
                    {
                        #region update props
                        dbObj.ShowPrice = item.ShowPrice;
                        dbObj.ShowInvoice = item.ShowInvoice;
                        dbObj.TankerQuotaNo = item.TankerQuotaNo;
                        dbObj.ShowCustomerClassEntryGate = item.ShowCustomerClassEntryGate;
                        dbObj.AutoCancelationNewOrdersHours = item.AutoCancelationNewOrdersHours;
                        dbObj.AutoCancelationOnHoldOrdersHours = item.AutoCancelationOnHoldOrdersHours;
                        #endregion

                        this._BranchSetting.Update(dbObj);
                    }
                    else
                    {
                        var newObj = new NWC_BranchSetting
                        {
                            BranchID = item.Id,
                            ShowPrice = item.ShowPrice,
                            ShowInvoice = item.ShowInvoice,
                            TankerQuotaNo = item.TankerQuotaNo,
                            ShowCustomerClassEntryGate = item.ShowCustomerClassEntryGate,
                            AutoCancelationNewOrdersHours =item.AutoCancelationNewOrdersHours,
                            AutoCancelationOnHoldOrdersHours = item.AutoCancelationOnHoldOrdersHours
                        };

                        this._BranchSetting.Add(newObj);
                    }


                }

            }
            return DescriptiveResponse<Boolean>.Success(true);
        }

        public DescriptiveResponse<bool> AddArea(AreaDTO dto)
        {
            try
            {
                #region Validations
                var validator = new BranchValidator(ValidationMode.Create, this._loggedInUser, this._branchRepository);
                var results = validator.Validate(dto);
                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    return DescriptiveResponse<bool>.Error(failures);
                }
                #endregion


                using (_unitofWork)
                {
                    var branch = AreaDTO.MapToArea(dto);

                    branch.Id = Guid.NewGuid();
                    branch.isDeleted = false;
                    branch.CreatedBy = _loggedInUser.LoggedInUser.StaffId;
                    branch.CreationTime = DateTimeHelper.GetDateTimeNow();
                    branch.SubId = _loggedInUser.LoggedInUser.SubscriberId;

                    branch.IsActive = true;
                    branch.isPermenant = false;

                    this._branchRepository.Add(branch);
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }

        public DescriptiveResponse<bool> EditArea(AreaDTO dto)
        {
            try
            {
                #region Validations
                var validator = new BranchValidator(ValidationMode.Update, this._loggedInUser, this._branchRepository);
                var results = validator.Validate(dto);
                if (!results.IsValid)
                {
                    var failures = results.Errors.Select(s => s.ErrorMessage);
                    return DescriptiveResponse<bool>.Error(failures);
                }
                #endregion


                using (_unitofWork)
                {
                    var branch = _branchRepository.FindById(dto.Id); //AreaDTO.MapToArea(dto);

                    branch.LastModifiedBy = _loggedInUser.LoggedInUser.StaffId;
                    branch.LastModificationDate = DateTimeHelper.GetDateTimeNow();

                    branch.name = dto.name;
                    branch.descr = dto.description;
                    branch.address = dto.location;
                    branch.telephone = dto.telephone1;
                    branch.telephone2 = dto.telephone2;
                    branch.mobile = dto.mobile;
                    branch.fax = dto.fax;
                    branch.website = dto.website;

                    this._branchRepository.Update(branch);
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }

        public DescriptiveResponse<bool> AddLandmark(AreaDTO dto)
        {
            try
            {
                using (_unitofWork)
                {
                    var landmark = AreaDTO.MapToLandmark(dto);

                    landmark.Id = Guid.NewGuid();
                    landmark.isDeleted = false;
                    landmark.CreatedBy = _loggedInUser.LoggedInUser.StaffId;
                    landmark.CreationTime = DateTimeHelper.GetDateTimeNow();
                    landmark.SubId = _loggedInUser.LoggedInUser.SubscriberId;

                    landmark.StatusID = 1;
                    landmark.landmarkTypeId = Guid.Parse("95F5585D-0B62-4AD7-B169-F367864E2E8A");
                    landmark.IsVirtualStation = false;

                    this._landmarkRepository.Add(landmark);
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }

        public DescriptiveResponse<bool> EditLandmark(AreaDTO dto)
        {
            try
            {
                using (_unitofWork)
                {
                    var landmark = _landmarkRepository.FindById(dto.Id);

                    landmark.LastModifiedBy = _loggedInUser.LoggedInUser.StaffId;
                    landmark.LastModificationDate = DateTimeHelper.GetDateTimeNow();

                    landmark.name = landmark.name;
                    landmark.address = landmark.address;
                    landmark.descr = landmark.descr;
                    landmark.telephone = landmark.telephone;
                    landmark.telephone2 = landmark.telephone2;
                    landmark.mobile = landmark.mobile;
                    landmark.fax = landmark.fax;

                    this._landmarkRepository.Update(landmark);
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }

        public DescriptiveResponse<bool> DeleteArea(Guid id)
        {
            try
            {
                using (_unitofWork)
                {
                    var branch = this._branchRepository.FindById(id);

                    branch.isDeleted = true;
                    branch.LastModifiedBy = _loggedInUser.LoggedInUser.StaffId;
                    branch.LastModificationDate = DateTimeHelper.GetDateTimeNow();

                    this._branchRepository.Update(branch);
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }

        public DescriptiveResponse<bool> DeleteLandmark(Guid id)
        {
            try
            {
                using (_unitofWork)
                {
                    var landMark = this._landmarkRepository.FindById(id);

                    landMark.isDeleted = true;
                    landMark.LastModifiedBy = _loggedInUser.LoggedInUser.StaffId;
                    landMark.LastModificationDate = DateTimeHelper.GetDateTimeNow();

                    this._landmarkRepository.Update(landMark);
                }

                return DescriptiveResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return DescriptiveResponse<bool>.Error(ErrorStatus.UNEXPECTED_ERROR);
                throw;
            }
        }
        #endregion
    }
}
