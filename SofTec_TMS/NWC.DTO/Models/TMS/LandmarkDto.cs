using NWC.DAL.NWCEntities;
using System;

namespace NWC.DTO.Models
{
    public class LandmarkDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public Guid? LandmarkTypeId { get; set; }
        public string LandmarkTypeName { get; set; }
        public Guid? BranchId { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public Guid? ContactPerson { get; set; }
        public Guid? GeofenceId { get; set; }
        public int? AVGAddonFillingTime { get; set; }
        public int? WaterSourceID { get; set; }
        public int? StationOwnershipID { get; set; }
        public int? StatusID { get; set; }

        public LandmarkDto() { }
        public LandmarkDto(Landmark entity)
        {
            Id = entity.Id;
            Name = entity.name;
            Description = entity.descr;
            Code = entity.code;
            LandmarkTypeId = entity.landmarkTypeId;
            LandmarkTypeName = entity.landmarkTypeName;
            BranchId = entity.branchId;
            Address = entity.address;
            Telephone = entity.telephone;
            Mobile = entity.mobile;
            Fax = entity.fax;
            Latitude = entity.latitude;
            Longitude = entity.longitude;
            ContactPerson = entity.contactPerson;
            GeofenceId = entity.GeofenceId;
            AVGAddonFillingTime = entity.AVGAddonFillingTime;
            WaterSourceID = entity.WaterSourceID;
            StationOwnershipID = entity.StationOwnershipID;
            StatusID = entity.StatusID;
        }

        public Landmark MapToEntity() => MapToEntity(new Landmark());
        public Landmark MapToEntity(Landmark entity)
        {
            entity.Id = Id;
            entity.name = Name;
            entity.descr = Description;
            entity.code = Code;
            entity.landmarkTypeId = LandmarkTypeId;
            entity.landmarkTypeName = LandmarkTypeName;
            entity.branchId = BranchId;
            entity.address = Address;
            entity.telephone = Telephone;
            entity.mobile = Mobile;
            entity.fax = Fax;
            entity.latitude = Latitude;
            entity.longitude = Longitude;
            entity.contactPerson = ContactPerson;
            entity.GeofenceId = GeofenceId;
            entity.AVGAddonFillingTime = AVGAddonFillingTime;
            entity.WaterSourceID = WaterSourceID;
            entity.StationOwnershipID = StationOwnershipID;
            entity.StatusID = StatusID;
            return entity;
        }
    }
}
