using NWC.DAL.NWCEntities;
using NWC.DTO.Models;

namespace NWC.DTO.Wrapper
{
    public static class DeviceMeterWrapper
    {
        #region Device Meter Reading
        #region Domain ==> DTO
        //public static DeviceMeterDTO WrapToDeviceMeterDTO(this vw_NWC_DeviceMeter input)
        //{
        //    return new DeviceMeterDTO()
        //    {
        //        ID = input.ID, 
        //        ConnectorTubeNumber = input.ConnectorTubeNumber,
        //        MeterSerialNumber = input.MeterSerialNumber,
        //        ManholeNumber = input.ManholeNumber,
        //        IsScadaAutoReading = input.IsScadaAutoReading,
        //        ServiceTypeID = input.ServiceTypeID,
        //        StationID = input.StationID
        //    };
        //}
        #endregion
        #region DTO ==> Domain
        public static NWC_DeviceMeter WrapToDeviceMeter(this DeviceMeterDTO dto)
        {
            if (dto == null) return null;

            return new NWC_DeviceMeter()
            {
                ConnectorTubeNumber = dto.ConnectorTubeNumber,
                MeterSerialNumber = dto.MeterSerialNumber,
                ManholeNumber = dto.ManholeNumber,
                IsScadaAutoReading = dto.IsScadaAutoReading,
                ServiceTypeID = dto.ServiceTypeID,
                StationID = dto.StationID
            };
        }
        #endregion
        #endregion

        #region Device Meter Reading

        #region Domain ==> DTO
        public static MeterReadingDTO WrapToReadingDTO(this vw_NWC_DeviceMeterReading from)
        {
            var reading = new MeterReadingDTO();

            reading.ID = from.ID;
            reading.DeviceMeterID = from.DeviceMeterID;
            reading.MeterReading = from.MeterReading;
            reading.ReadingTime = from.ReadingTime;
            reading.ReadingComment = from.ReadingComment;

            //DeviceMeter
            reading.ConnectorTubeNumber = from.ConnectorTubeNumber;
            reading.MeterSerialNumber = from.MeterSerialNumber;
            reading.ManholeNumber = from.ManholeNumber;
            reading.IsScadaAutoReading = from.IsScadaAutoReading;
            reading.ServiceTypeID = from.ServiceTypeID;
            
            //station
            reading.StationID = from.StationID;
            reading.StationName = from.StationName;
            reading.StationCode = from.StationCode;

            return reading;
        }

        #endregion

        #region DTO ==> Domain
        public static NWC_DeviceMeterReading WrapToReading(this MeterReadingDTO from)
        {
            if (from == null) return null;

            var reading = new NWC_DeviceMeterReading();

            reading.ID = from.ID;
            reading.DeviceMeterID = from.DeviceMeterID;
            reading.MeterReading = from.MeterReading;
            reading.ReadingTime = from.ReadingTime;
            reading.ReadingComment = from.ReadingComment;

            return reading;
        }

        #endregion


        #endregion
    }
}
