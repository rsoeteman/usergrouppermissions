using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.interfaces;

namespace UserGroupPermissions.MenuActions
{
    public class UsergroupPermissions : IAction
    {
        private readonly static UsergroupPermissions _instance = new UsergroupPermissions();

        public static UsergroupPermissions Instance
        {
            get
            {
                return _instance;
            }
        }

        public string Alias
        {
            get { return "UsergroupPermissions"; }
        }

        public bool CanBePermissionAssigned
        {
            get { return false; }
        }

        public string Icon
        {
            get
            {
                return string.Format(".sprDummy' style='background: transparent url({0}) center center no-repeat", umbraco.GlobalSettings.Path + "/plugins/usergrouppermissions/images/group_edit.png");
            }
        }


        public string JsFunctionName
        {
            get
            {
                return "openUserGroupDialog()";

            }
        }

        public string JsSource
        {
            get
            {
                return "/umbraco/plugins/usergrouppermissions/scripts/usergrouppermissions.js";
            }
        }

        public char Letter
        {
            get { return 'œ'; }
        }

        public bool ShowInNotifier
        {
            get { return true; }
        }
    }
}