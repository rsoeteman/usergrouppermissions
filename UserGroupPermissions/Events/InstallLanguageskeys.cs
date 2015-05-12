using Umbraco.Core;
using UserGroupPermissions.Businesslogic;

namespace UserGroupPermissions.Events
{
    public class InstallLanguageskeys : ApplicationEventHandler
    {
        public InstallLanguageskeys()
        {
            Languagefiles.InstallLanguageKey("UsergroupPermissions", "User group permissions");
        }
    }
}