using NWC.DAL.NWCEntities;
using NWC.DTO.Models;
using NWC.DTO.Models.TMS;
using NWC.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Wrapper
{
    public static class StaffWrapper
    {
        public static StaffListDTO WarapToStaffListDTO(this Staff input)
        {
            return new StaffListDTO()
            {
                Id = input.ID,
                StaffName = $"{input.FirstName} {input.MiddleName} {input.LastName}",
                StaffCode = input.code,
                Allocation = $"{input.Branch?.name}, {input.Landmark?.name}",
                StaffRole = input.StaffRoles?.name,
            };
        }

        public static StaffRolesDTO WarapToStaffRolesDTO(this StaffRoles input)
        {
            return new StaffRolesDTO()
            {
                ID = input.ID,
                Category =input.StaffRoleCategory?.NameAr,
                Name=input.NameAr,
                Description=input.descr,
                PageId= (input.PageId!=null)?input.PageId.Value:Guid.Empty,
                isDefault = input.isDefault.Value,
            };
        }
        public static Staff WrapDtoToStaff(this StaffExcelDTO input) => input.WrapDtoToStaff(new Staff());
        public static Staff WrapDtoToStaff(this StaffExcelDTO input, Staff entity)
        {
            if (input == null)
                return null;

            entity.personalID = input.IDs;
            entity.code = input.Code;
            entity.FirstName = input.FirstName;
            entity.mobileNumber = input.Mobile;
            entity.MiddleName = input.MiddleName;
            entity.LastName = input.LastName;
            entity.Email = input.Email;
            entity.StaffRoleName = input.StaffRole;
            return entity;
        }

        public static StaffDTO WarapToStaffDTO(this Staff staff)
        {
            if (staff == null)
                return null;

            return new StaffDTO()
            {
                ID = staff.ID,
                FirstName = staff.FirstName,
                descr = staff.descr,
                code = staff.code,
                subID = staff.subID,
                staffRoleCategoryID = staff.StaffRoles?.category,
                staffRoleID = staff.staffRoleID,
                CreatedBy = staff.CreatedBy,
                CreationTime = staff.CreationTime,
                image = staff.image,
                isAllocated = staff.isAllocated,
                personalID = staff.personalID,
                AllocatedBranch = staff.Landmark?.Branch?.parentBranchId,
                AllocatedSubBranch = staff.Landmark?.branchId,
                AllocatedLandmark = staff.AllocatedLandmark,
                status = staff.status,
                employmentType = staff.employmentType,
                employmentDate = staff.employmentDate,
                salary = staff.salary,
                mobileNumber = staff.mobileNumber,
                MonitorFlag = staff.MonitorFlag,
                HireDate = staff.HireDate,
                LaborRate = staff.LaborRate,
                LisenceNumber = staff.LisenceNumber,
                LisenceIssueDate = staff.LisenceIssueDate,
                LisenceExpiryDate = staff.LisenceExpiryDate,
                InsuranceNumber = staff.InsuranceNumber,
                InsuranceIssueDate = staff.InsuranceIssueDate,
                InsuranceCompanyID = staff.InsuranceCompanyID,
                Birthday = staff.Birthday,
                MaritalStatus = staff.MaritalStatus,
                Gender = staff.Gender,
                Address = staff.Address,
                PostalCode = staff.PostalCode,
                EmergencyContactName1 = staff.EmergencyContactName1,
                EmergencyContactName2 = staff.EmergencyContactName2,
                EmergencyContactPhone1 = staff.EmergencyContactPhone1,
                EmergencyContactPhone2 = staff.EmergencyContactPhone2,
                MiddleName = staff.MiddleName,
                LastName = staff.LastName,
                UserID = staff.UserID,
                Email = staff.Email,
                StaffRoleName = staff.StaffRoleName,
                LastModifiedBy = staff.LastModifiedBy,
                LastModificationDate = staff.LastModificationDate,
                isDeleted = staff.isDeleted,
                EmploymentDateHijri = staff.EmploymentDateHijri,
                HireDateHijri = staff.HireDateHijri,
                LicenseIssueDateHijri = staff.LicenseIssueDateHijri,
                LicenseExpiryDateHijri = staff.LicenseExpiryDateHijri,
                InsuranceIssueDateHijri = staff.InsuranceIssueDateHijri,
                BirthdayHijri = staff.BirthdayHijri
            };
        }


    }
}
