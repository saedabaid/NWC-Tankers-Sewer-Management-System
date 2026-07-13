using Infrastructure;
using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using NWC.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BLL.Interfaces
{
    public interface IUserService
    {
        DescriptiveResponse<SearchResult<UserListDTO>> SearchUsers(UserSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<Boolean> SaveUserPermittedLandmarks(List<UserLandmarkPermissionDTO> userPermittedLandmarks);
        DescriptiveResponse<SearchResult<UserLandmarkPermissionDTO>> GetUserPermittedLandmarks(UserStationPermissionSC searchCriteria);
        DescriptiveResponse<ProfileDTO> GetUserProfile();
        IEnumerable<StaffPermissionDTO> GetUserAuthenticatePermissions(Guid userId, Guid subId, out ReturnResult result);
        DescriptiveResponse<LoginDTO> AuthenticateUser(string userName, string password);
        DescriptiveResponse<bool> Unlock(string Name);
        DescriptiveResponse<bool> Lock(string Name);
        DescriptiveResponse<bool> Delete(string Name);
        DescriptiveResponse<bool> ChangePassword(ChangePasswordDTO model);
        LoginDTO ValidateUserAsync(string name, string password);
        DescriptiveResponse<IEnumerable<StaffPermissionDTO>> GetUserPermissions();

    }
}
