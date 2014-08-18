using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.BusinessLogic;
using UserGroupPermissions.Businesslogic;
using umbraco.businesslogic;

namespace UserGroupPermissions.Events
{
    public class InstallLanguageskeys : ApplicationStartupHandler 
    {
        public InstallLanguageskeys()
        {
            Languagefiles.InstallLanguageKey("UsergroupPermissions", "User group permissions");
        }
    }
}