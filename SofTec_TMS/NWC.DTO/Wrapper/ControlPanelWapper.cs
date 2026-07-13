using NWC.DAL.NWCEntities;
using NWC.DTO.Models;

namespace NWC.DTO.Wrapper
{
    public static class ControlPanelWapper
    {
        public static BranchSettingDTO WrapToBranchSettingDTO(this vw_NWC_BranchSetting from)
        {
            var to = new BranchSettingDTO()
            {
                Id = from.Id,
                name = from.name,
                code = from.code,
                ShowPrice = from.ShowPrice,
                ShowInvoice = from.ShowInvoice,
                TankerQuotaNo = from.TankerQuotaNo,
                ShowCustomerClassEntryGate = from.ShowCustomerClassEntryGate,
                AutoCancelationNewOrdersHours = from.AutoCancelationNewOrdersHours,
                AutoCancelationOnHoldOrdersHours = from.AutoCancelationOnHoldOrdersHours,
                ApprovalLevelsCount = from.ApprovalLevelsCount,
                ValidationApprovalRequired = from.ValidationApprovalRequired,
                RequiresPayment = from.RequiresPayment,
                ValidateConfermationCode = from.ValidateConfermationCode
            };

            return to;
        }

        public static NWC_BranchSetting WrapFromBranchSettingDTO(this BranchSettingDTO from)
        {
            return WrapFromBranchSettingDTO(from, new NWC_BranchSetting());
        }
        public static NWC_BranchSetting WrapFromBranchSettingDTO(this BranchSettingDTO from, NWC_BranchSetting entity)
        {
            entity.ShowPrice = from.ShowPrice;
            entity.ShowInvoice = from.ShowInvoice;
            entity.TankerQuotaNo = from.TankerQuotaNo;
            entity.ShowCustomerClassEntryGate = from.ShowCustomerClassEntryGate;
            entity.AutoCancelationNewOrdersHours = from.AutoCancelationNewOrdersHours;
            entity.AutoCancelationOnHoldOrdersHours = from.AutoCancelationOnHoldOrdersHours;
            entity.RequiresPayment = from.RequiresPayment;
            entity.ApprovalLevelsCount = from.ApprovalLevelsCount;
            entity.ValidationApprovalRequired = from.ValidationApprovalRequired;
            entity.ValidateConfermationCode = from.ValidateConfermationCode;
            return entity;
        }
    }
}
