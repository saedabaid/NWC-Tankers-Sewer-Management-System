using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Resources
{
    public static class ValidationMessagesKeys
    {
        public const string OrderNumberRequired = "OrderNumberRequired";
        public const string OrderNumberAlreadyExist = "OrderNumberAlreadyExist";
        public const string CisDivisionRequired = "CisDivisionRequired";
        public const string CreationTimeRequired = "CreationTimeRequired";
        public const string ScheduledDeliveryTimeRequired = "ScheduledDeliveryTimeRequired";
        public const string ServiceTypeCodeRequired = "ServiceTypeCodeRequired";
        public const string ContactNameRequired = "ContactNameRequired";
        public const string ContactMobileRequired = "ContactMobileRequired";
        public const string OrderQuantityRequired = "OrderQuantityRequired";
        public const string ConfirmationCodeRequired = "ConfirmationCodeRequired";
        public const string AccountIDRequired = "AccountIDRequired";
        public const string ClassIDRequired = "ClassIDRequired";
        public const string CustomerCodeRequired = "CustomerCodeRequired";
        public const string PersonNameRequired = "PersonNameRequired";
        public const string IDTypeIDRequired = "IDTypeIDRequired";
        public const string IDNumberRequired = "IDNumberRequired";
        public const string PremiseIDRequired = "PremiseIDRequired";
        public const string PremiseCoordinatesRequired = "PremiseCoordinatesRequired";
        public const string ChooseCustomerLocation = "ChooseCustomerLocation";
        public const string ChooseStation = "ChooseStation";
        public const string ChooseServiceType = "ChooseServiceType";
        public const string InvalidQuantity = "InvalidQuantity";
        public const string StationDoesNotAssignedToContract = "StationDoesNotAssignedToContract";
        public const string InvalidContract = "InvalidContract";
        public const string CustomerLocationDoesNotExist = "CustomerLocationDoesNotExist";
        public const string ContractStationDoesNotHasTariff = "ContractStationDoesNotHasTariff";
        public const string ContractStationDoesNotHasAccessories = "ContractStationDoesNotHasAccessories";
    }
}
