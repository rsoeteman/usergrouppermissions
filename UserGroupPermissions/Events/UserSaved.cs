using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.BusinessLogic;
using UserGroupPermissions.Businesslogic;
using umbraco.businesslogic;

namespace UserGroupPermissions.Events
{
    public class UserSaved : ApplicationStartupHandler
    {
        public UserSaved()
        {
            User.Saving += new User.SavingEventHandler(User_Saving);
        }

        void User_Saving(User sender, EventArgs e)
        {
            //User is already saved but the object itself is not updated yet.
            //Get user from database and use that user to copy the permissions
            User savedUser = User.GetUser(sender.Id);

            if (sender.UserType.Alias != savedUser.UserType.Alias)
            {
                //Only save when usertype is changed
                UserTypePermissions.CopyPermissionsForSingleUser(savedUser);

            }

        }

    }
}