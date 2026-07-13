using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Wrapper
{
    public static class UserWrapper
    {
        public static UserLandmarkPermissionDTO WrapToUserLandmarkPermissionDTO(this vw_NWC_UserLandmarkPermission input)
        {
            //var permittedLandmarkIDs = new List<Guid>();

            //if (!string.IsNullOrEmpty(input.ListofLandmarks))
            //{
            //    var landmarkIDs = input.ListofLandmarks.Split(',');
            //    foreach (var id in landmarkIDs)
            //    {
            //        permittedLandmarkIDs.Add(new Guid(id));
            //    }
            //}

            List<string> DBPermittedLandmarks = null;
            if (!string.IsNullOrEmpty(input.ListofLandmarks))
            {
                DBPermittedLandmarks = input.ListofLandmarks.Split(',').ToList();
            }

            var permittedServiceIDs = new List<int>();
            if (!string.IsNullOrEmpty(input.ListofServices))
            {
                var serviceIDs = input.ListofServices.Split(',');
                foreach (var id in serviceIDs)
                {
                    permittedServiceIDs.Add( int.Parse(id) );
                }
            }
            return new UserLandmarkPermissionDTO()
            {
                StaffID = input.StaffID, 
                FullName = input.FullName, 
                DBPermittedLandmarks = DBPermittedLandmarks,
                //PermittedLandmarkIDs = permittedLandmarkIDs,
                PermittedServiceIDs = permittedServiceIDs
            };
        }
        public static UserListDTO WrapToUserListDTO(this aspnet_Users user)
        {
            return new UserListDTO()
            {
                Id = user.UserId,
                Name = user.UserName,
                Email = user.aspnet_Membership.Email,
                MobilePIN = user.aspnet_Membership.MobilePIN,
                IsLockedOut = user.aspnet_Membership.IsLockedOut
            };
        }
    }
}
