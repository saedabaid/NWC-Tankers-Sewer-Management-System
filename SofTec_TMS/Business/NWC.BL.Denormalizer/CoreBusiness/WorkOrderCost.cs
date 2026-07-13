using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BL.Denormalizer.CoreBusiness
{
    public static class WorkOrderCost
    {
        private static decimal vat
        {
            get
            {
                decimal _vat;
                decimal.TryParse(ConfigurationManager.AppSettings["VAT"] != null ?
                    ConfigurationManager.AppSettings["VAT"] : string.Empty, out _vat);

                return _vat > 0 ? _vat : 0.15M;
            }
        }

        public static decimal CalculateWorkOrderCost(long workOrderID, DateTime scheduledDeliveryTime, int customerLocationClassID, int serviceTypeID, NWC_ZoneStations zoneStation, int orderQuantity)
        {
            var ctx = new NWCContext();

            decimal totalCost = -1;
            long? zoneID = zoneStation.ZoneID > 0 ? zoneStation.ZoneID : (long?)null;

            var contract = ctx.NWC_Contract.Where(x => x.ContractStatusID.Value == (int)ContractStatusEnum.Active &&
                            (!x.IsDeleted.HasValue || x.IsDeleted.Value != true) &&
                            x.ContractEndDate >= scheduledDeliveryTime
                            && x.NWC_ContractStatus.EnumId != (int)ContractStatusEnum.Terminated &&
                            x.NWC_ContractTariff.Where(ct => ct.ServiceTypeID == serviceTypeID && ct.CustomerLocationClassID == customerLocationClassID).Any()
                            //&& (x.IsTerminated == null || x.IsTerminated == false) 
                            //&& x.NWC_ContractTariff.Where(t => t.IsDeleted != true && t.StationID == stationID && t.ServiceTypeID == serviceTypeID)
                            && x.NWC_ContractStations.Where(s => s.IsDeleted != true).Select(s => s.StationID).ToList().Contains(zoneStation.StationID)).FirstOrDefault();

            if (contract == null)
                return totalCost;

            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("contract == null: {0}", contract == null)));
            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("contractID: {0}", contract != null ? contract.ID : 0)));

            var scheduledDeliveryHijri = DateTimeHelper.ConvertDateToHijriAsLong(scheduledDeliveryTime);

            var contractStationTariaff = ctx.NWC_ContractTariff.Where(x => x.ContractID == contract.ID && x.IsDeleted != true &&
                            x.StationID == zoneStation.StationID && x.CustomerLocationClassID == customerLocationClassID && x.ServiceTypeID == serviceTypeID &&
                            (x.ZoneID == null || x.ZoneID == zoneID) && (x.TanckerCapacityId == null || x.TanckerCapacityId == orderQuantity) &&
                            x.ServiceTypeID == serviceTypeID &&
                            x.DateFromHijri <= scheduledDeliveryHijri && x.DateToHijri >= scheduledDeliveryHijri).FirstOrDefault();

            if (contractStationTariaff == null)
                return totalCost;

            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("contractStationTariaff == null: {0}", contractStationTariaff == null)));
            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("contractStationTariaffID: {0}", contractStationTariaff != null ? contractStationTariaff.ID : 0)));

            var defaultDistance = zoneStation.Distance.HasValue ? zoneStation.Distance.Value : 0;
            var diffrentDistance = defaultDistance - contractStationTariaff.AfterFirstKM;

            var totalQuantityCost = contractStationTariaff.CubicMeterCharge * orderQuantity;
            var extraKMCost = contractStationTariaff.DistanceCharge * (diffrentDistance > 0 ? diffrentDistance : 0);

            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("totalQuantityCost: {0}", totalQuantityCost)));
            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("extraKMCost: {0}", extraKMCost)));

            if (workOrderID > 0)
            {
                var orderAccessoryCharges = ctx.vw_NWC_ContractWorkOrderAccessory.Where(x => x.WorkOrderID == workOrderID
                            && x.StationID == zoneStation.StationID && x.ContractID == contract.ID).ToList();

                //Accessories1 - n cost = Sum1 - n[Accessory charge]
                foreach (var accessoryCharge in orderAccessoryCharges)
                {
                    totalQuantityCost += accessoryCharge.Charge;
                }
            }

            //Total cost = Total quantity cost +Extra KM cost +Accessories1 - n cost 
            totalCost = (totalQuantityCost + extraKMCost);

            LoggerManager.LogMsg(c => c.TrackingMsg(string.Format("totalCost: {0}", totalCost)));

            return totalCost;
        }

        public static decimal CalculateVat(decimal netCost)
        {
            return netCost > -1 ? netCost * vat : -1;
        }
    }
}
