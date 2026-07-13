using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models.TMS
{
    public class StaffDTO
    {
        #region Properties
        public System.Guid ID { get; set; }
        public string FirstName { get; set; }
        public string descr { get; set; }
        public string code { get; set; }
        public System.Guid subID { get; set; }
        public Nullable<System.Guid> staffRoleID { get; set; }
        public Nullable<int> staffRoleCategoryID { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public System.DateTime CreationTime { get; set; }
        public string image { get; set; }
        public bool isAllocated { get; set; }
        public string personalID { get; set; }
        public Nullable<System.Guid> AllocatedBranch { get; set; }
        public Nullable<System.Guid> AllocatedSubBranch { get; set; }
        public Nullable<System.Guid> AllocatedPermittedBranch { get; set; }
        public Nullable<System.Guid> AllocatedLandmark { get; set; }
        public Nullable<System.Guid> status { get; set; }
        public Nullable<System.Guid> employmentType { get; set; }
        public Nullable<System.DateTime> employmentDate { get; set; }
        public Nullable<decimal> salary { get; set; }
        public string mobileNumber { get; set; }
        public Nullable<bool> MonitorFlag { get; set; }
        public Nullable<System.DateTime> HireDate { get; set; }
        public Nullable<double> LaborRate { get; set; }
        public string LisenceNumber { get; set; }
        public Nullable<System.DateTime> LisenceIssueDate { get; set; }
        public Nullable<System.DateTime> LisenceExpiryDate { get; set; }
        public string InsuranceNumber { get; set; }
        public Nullable<System.DateTime> InsuranceIssueDate { get; set; }
        public Nullable<System.Guid> InsuranceCompanyID { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public Nullable<int> MaritalStatus { get; set; }
        public Nullable<int> Gender { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string EmergencyContactName1 { get; set; }
        public string EmergencyContactName2 { get; set; }
        public string EmergencyContactPhone1 { get; set; }
        public string EmergencyContactPhone2 { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Nullable<System.Guid> UserID { get; set; }
        public string Email { get; set; }
        public string StaffRoleName { get; set; }
        public Nullable<System.Guid> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModificationDate { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public string EmploymentDateHijri { get; set; }
        public string HireDateHijri { get; set; }
        public string LicenseIssueDateHijri { get; set; }
        public string LicenseExpiryDateHijri { get; set; }
        public string InsuranceIssueDateHijri { get; set; }
        public string BirthdayHijri { get; set; }

        public List<Nullable<Guid>> PermittedBranch { get; set; }
        public List<Nullable<Guid>> PermittedSubBranch { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        #endregion

        #region Constructor
        public StaffDTO()
        {

        }

        public StaffDTO(Staff staff)
        {
            this.ID = staff.ID;
            this.FirstName = staff.FirstName;
            this.descr = staff.descr;
            this.code = staff.code;
            this.subID = staff.subID;
            this.staffRoleID = staff.staffRoleID;
            this.CreatedBy = staff.CreatedBy;
            this.CreationTime = staff.CreationTime;
            this.image = staff.image;
            this.isAllocated = staff.isAllocated;
            this.personalID = staff.personalID;
            this.AllocatedBranch = staff.Branch?.parentBranchId;
            this.AllocatedSubBranch = staff.AllocatedBranch;
            this.AllocatedLandmark = staff.AllocatedLandmark;
            this.status = staff.status;
            this.employmentType = staff.employmentType;
            this.employmentDate = staff.employmentDate;
            this.salary = staff.salary;
            this.mobileNumber = staff.mobileNumber;
            this.MonitorFlag = staff.MonitorFlag;
            this.HireDate = staff.HireDate;
            this.LaborRate = staff.LaborRate;
            this.LisenceNumber = staff.LisenceNumber;
            this.LisenceIssueDate = staff.LisenceIssueDate;
            this.LisenceExpiryDate = staff.LisenceExpiryDate;
            this.InsuranceNumber = staff.InsuranceNumber;
            this.InsuranceIssueDate = staff.InsuranceIssueDate;
            this.InsuranceCompanyID = staff.InsuranceCompanyID;
            this.Birthday = staff.Birthday;
            this.MaritalStatus = staff.MaritalStatus;
            this.Gender = staff.Gender;
            this.Address = staff.Address;
            this.PostalCode = staff.PostalCode;
            this.EmergencyContactName1 = staff.EmergencyContactName1;
            this.EmergencyContactName2 = staff.EmergencyContactName2;
            this.EmergencyContactPhone1 = staff.EmergencyContactPhone1;
            this.EmergencyContactPhone2 = staff.EmergencyContactPhone2;
            this.MiddleName = staff.MiddleName;
            this.LastName = staff.LastName;
            this.UserID = staff.UserID;
            this.Email = staff.Email;
            this.StaffRoleName = staff.StaffRoleName;
            this.LastModifiedBy = staff.LastModifiedBy;
            this.LastModificationDate = staff.LastModificationDate;
            this.isDeleted = staff.isDeleted;
            this.EmploymentDateHijri = staff.EmploymentDateHijri;
            this.HireDateHijri = staff.HireDateHijri;
            this.LicenseIssueDateHijri = staff.LicenseIssueDateHijri;
            this.LicenseExpiryDateHijri = staff.LicenseExpiryDateHijri;
            this.InsuranceIssueDateHijri = staff.InsuranceIssueDateHijri;
            this.BirthdayHijri = staff.BirthdayHijri;

        }
        #endregion

        #region Helpper
        public static Staff MapToStaff(StaffDTO dto)
        {
            return new Staff()
            {
                ID = dto.ID,
                FirstName = dto.FirstName,
                descr = dto.descr,
                code = dto.code,
                subID = dto.subID,
                staffRoleID = dto.staffRoleID,
                CreatedBy = dto.CreatedBy,
                CreationTime = dto.CreationTime,
                image = dto.image,
                isAllocated = dto.isAllocated,
                personalID = dto.personalID,
                AllocatedBranch = dto.AllocatedBranch,
                AllocatedLandmark = dto.AllocatedLandmark,
                status = dto.status,
                employmentType = dto.employmentType,
                employmentDate = dto.employmentDate,
                salary = dto.salary,
                mobileNumber = dto.mobileNumber,
                MonitorFlag = dto.MonitorFlag,
                HireDate = dto.HireDate,
                LaborRate = dto.LaborRate,
                LisenceNumber = dto.LisenceNumber,
                LisenceIssueDate = dto.LisenceIssueDate,
                LisenceExpiryDate = dto.LisenceExpiryDate,
                InsuranceNumber = dto.InsuranceNumber,
                InsuranceIssueDate = dto.InsuranceIssueDate,
                InsuranceCompanyID = dto.InsuranceCompanyID,
                Birthday = dto.Birthday,
                MaritalStatus = dto.MaritalStatus,
                Gender = dto.Gender,
                Address = dto.Address,
                PostalCode = dto.PostalCode,
                EmergencyContactName1 = dto.EmergencyContactName1,
                EmergencyContactName2 = dto.EmergencyContactName2,
                EmergencyContactPhone1 = dto.EmergencyContactPhone1,
                EmergencyContactPhone2 = dto.EmergencyContactPhone2,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                UserID = dto.UserID,
                Email = dto.Email,
                StaffRoleName = dto.StaffRoleName,
                LastModifiedBy = dto.LastModifiedBy,
                LastModificationDate = dto.LastModificationDate,
                isDeleted = dto.isDeleted,
                EmploymentDateHijri = dto.EmploymentDateHijri,
                HireDateHijri = dto.HireDateHijri,
                LicenseIssueDateHijri = dto.LicenseIssueDateHijri,
                LicenseExpiryDateHijri = dto.LicenseExpiryDateHijri,
                InsuranceIssueDateHijri = dto.InsuranceIssueDateHijri,
                BirthdayHijri = dto.BirthdayHijri
            };
        }
        #endregion
    }
}
