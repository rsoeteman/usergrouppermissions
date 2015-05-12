using System;
using System.Linq;
using umbraco.cms.businesslogic;
using UserGroupPermissions.Businesslogic;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Services;
using User = umbraco.BusinessLogic.User;

namespace UserGroupPermissions.Events
{
    public class UserSaved : ApplicationEventHandler
    {

        private readonly UserTypePermissionsService _userTypePermissionsService;

        public UserSaved()
        {
            _userTypePermissionsService = new UserTypePermissionsService();

            UserService.SavingUser += User_Saving;

            //User.Saving += UserService.SavingUserSavingEventHandler(User_Saving);
        }

        void User_Saving(IUserService service, SaveEventArgs<IUser> e)
        {
            //User is already saved but the object itself is not updated yet.
            //Get user from database and use that user to copy the permissions
            IUser savedUser = service.GetUserById(e.SavedEntities.FirstOrDefault().Id);

            if (e.SavedEntities.FirstOrDefault().UserType.Alias != savedUser.UserType.Alias)
            {
                //Only save when usertype is changed
                _userTypePermissionsService.CopyPermissionsForSingleUser(savedUser);
            }
        }

    }
}