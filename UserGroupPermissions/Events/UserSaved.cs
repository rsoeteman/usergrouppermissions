using System;
using System.Linq;
using umbraco.cms.businesslogic;
using UserGroupPermissions.Businesslogic;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
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

            var savedEntity = e.SavedEntities.FirstOrDefault();

            if (savedEntity != null)
            {
                if (!savedEntity.IsNewEntity())
                {
                    // if entity is not new get the old version to check against.
                    IUser savedUser = service.GetUserById(savedEntity.Id);
                    if (savedEntity.UserType.Alias != savedUser.UserType.Alias)
                    {
                        //Only save when usertype is changed (CopyPermissionsForSingleUser will not allow over saving of admin users)
                        _userTypePermissionsService.CopyPermissionsForSingleUser(savedUser);
                    }
                }
                else
                {
                    //user is new so copy permissions
                    _userTypePermissionsService.CopyPermissionsForSingleUser(savedEntity);
                }
            }
        }
    }
}