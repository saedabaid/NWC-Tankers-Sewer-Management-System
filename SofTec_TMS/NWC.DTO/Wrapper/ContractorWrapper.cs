using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Models;

namespace NWC.DTO.Wrapper
{
    public static class ContractorWrapper
    {

        #region Domain ==> DTO
        public static ContractorDTO WrapToContractorDTO(this vw_NWC_ContractorList from)
        {
            var contractorDto = new ContractorDTO();

            //contractDto.SubID = from.SubID;
            contractorDto.ID = from.ID;
            contractorDto.Code = from.Code;
            contractorDto.ContractorFullName = from.ContractorFullName;
            contractorDto.CommercialIDNumber= from.CommercialIDNumber;
            contractorDto.TaxNumber= from.TaxNumber;
            contractorDto.MOI = from.MOI;
            contractorDto.CompanyAddressCityID= from.CompanyAddressCityID;
            contractorDto.CompanyAddressPostalCode = from.CompanyAddressPostalCode;
            contractorDto.CompanyAddress= from.CompanyAddress;
            contractorDto.IsActive = from.IsActive;
            contractorDto.IsBlackListed = from.IsBlackListed;

            //contact person
            contractorDto.FirstName= from.FirstName;
            contractorDto.SecondName = from.SecondName;
            contractorDto.LastName = from.LastName;
            contractorDto.FullName= from.FullName;
            contractorDto.Position= from.Position;
            contractorDto.MobileCode = from.MobileCode;
            contractorDto.Mobile= from.Mobile;
            contractorDto.LandlineNumbeCode = from.LandlineNumbeCode;
            contractorDto.LandlineNumber = from.LandlineNumber;
            contractorDto.Email = from.Email;
            contractorDto.PersonalIDType = from.PersonalIDType;
            contractorDto.PersonalIDNumber= from.PersonalIDNumber;
            contractorDto.PersonAddressPostalCode = from.PersonAddressPostalCode;
            contractorDto.PersonAddress = from.PersonAddress;

            //area
            contractorDto.AreaId = from.AreaId;
            contractorDto.AreaName = from.AreaName;
            contractorDto.CityName = from.CityName;
            

            return contractorDto;
        }

        public static AttachmentDTO WrapContractorAttachment(this NWC_ContractorAttachment from)
        {
            if (from == null) return null;

            var to = new AttachmentDTO();
            to.ID = from.ID;
            to.DocumentName = from.DocumentName;
            to.RelativePath = from.RelativePath;
            to.IsDeleted = from.IsDeleted;

            return to;
        }
        #endregion

        #region DTO ==> Domain
        public static NWC_Contractor WrapToContractor(this ContractorDTO from)
        {
            var contractor = new NWC_Contractor();

            contractor.ID = from.ID;
            contractor.Code = from.Code;
            contractor.ContractorFullName = from.ContractorFullName;
            contractor.CommercialIDNumber = from.CommercialIDNumber;
            contractor.TaxNumber = from.TaxNumber;
            contractor.MOI = from.MOI;
            contractor.CompanyAddressCityID = from.CompanyAddressCityID;
            contractor.CompanyAddressPostalCode = from.CompanyAddressPostalCode;
            contractor.CompanyAddress = from.CompanyAddress;
            //contractorDto.IsActive = from.IsActive;
            //contractorDto.IsBlackListed = from.IsBlackListed;

            return contractor;
        }

        public static NWC_ContactPerson WrapToContactperson(this ContractorDTO from)
        {
            if (from == null || string.IsNullOrEmpty(from.FirstName)) return null;

            var person = new NWC_ContactPerson();

            person.FirstName = from.FirstName;
            person.SecondName = from.SecondName;
            person.LastName = from.LastName;

            person.FullName = string.IsNullOrEmpty(person.SecondName)
                                   ? $"{person.FirstName} {person.LastName}"
                                   : $"{person.FirstName} {person.SecondName} {person.LastName}";

            person.Position = from.Position;
            person.MobileCode = from.MobileCode;
            person.Mobile = from.Mobile;
            person.LandlineNumbeCode = from.LandlineNumbeCode;
            person.LandlineNumber = from.LandlineNumber;
            person.Email = from.Email;
            person.PersonalIDType = from.PersonalIDType;
            person.PersonalIDNumber = from.PersonalIDNumber;
            person.PersonAddressPostalCode = from.PersonAddressPostalCode;
            person.PersonAddress = from.PersonAddress;

            return person;
        }

        #endregion

    }
}
