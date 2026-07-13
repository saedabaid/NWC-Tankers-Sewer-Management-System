using NWC_CCB_Integration.DTO.Common;
using NWC_CCB_Integration.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Wrapper
{
    public static class WorkOrderWrapper
    {
        public static EventWorkOrderDTO WrapToEventWorkOrderDTO(this WorkOrderRequestDTO input, 
            IEnumerable<LookUpDTO<int>> accessories, IEnumerable<LookUpDTO<int>> servicesTypes)
        {
            var woAccessories = new List<AccessoryDTO>();
            var serviceType = servicesTypes.FirstOrDefault(x => x.IntegrationId == input.ServiceTypeCode);

            if (accessories != null && input.TankerAccessoriesCode != null)
            {
                var accessoryList = accessories.Where(x => input.TankerAccessoriesCode.Contains(x.IntegrationId));

                if (accessoryList != null && accessoryList.Any())
                {
                    foreach (var acc in accessoryList)
                    {
                        woAccessories.Add(new AccessoryDTO() { ID = acc.Id });
                    }
                }
            }

            return new EventWorkOrderDTO()
            {
                OrderNumber = input.OrderNumber,
                OrderQuantity = input.OrderQuantity,
                ServiceTypeID = serviceType.Id, 
                ScheduledDeliveryTime = input.ScheduledDeliveryTime,
                //CustomerLocationID = input.c,
                //StationID = input.StationID,
                Accessories = woAccessories, 
                CustomerLocationID = 0, 
                SourceApplication = input.SourceApplication, 
                CISDivision = input.CISDivision, 
                TransactionID = input.TransactionID, 
                ZoneID = input.ZoneID, 
                ConfirmationCode = input.ConfirmationCode
            };
        }
    }
}
