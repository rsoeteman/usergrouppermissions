using System;
using System.Linq;
using System.Collections;
using Umbraco.Core;
using Umbraco.Core.Models.Membership;
using UserGroupPermissions.Models;

namespace UserGroupPermissions.ExtensionMethods
{
    public static class UsertypeExtensions
    {
        public static string GetPermissions(this IUserType userType, string path)
        {
            //string defaultPermissions = userType.DefaultPermissions;

            //var defaultPermissions = userType.GetAllRelatedUsers().FirstOrDefault().DefaultPermissions;
            
            var defaultPermissions = "";

            Hashtable permissions = GetPermissions(userType.Id);

                foreach (string nodeId in path.Split(','))
                {
                    if (permissions.ContainsKey(int.Parse(nodeId)))
                        defaultPermissions = permissions[int.Parse(nodeId)].ToString();
                }

            return defaultPermissions;
        }

        /// <summary>
        /// Initializes the user node permissions
        /// </summary>
        private static Hashtable GetPermissions(int userTypeId)
        {
            Hashtable permissions = new Hashtable();

            var db = ApplicationContext.Current.DatabaseContext.Database;

            var perms = db.Fetch<UserTypePermission>("select * from UserTypePermissions where UserTypeId = @0", userTypeId);

            foreach (var perm in perms)
            {
                if (!permissions.ContainsKey(perm.NodeId))
                {
                    permissions.Add(perm.NodeId, String.Empty);
                }
                permissions[perm.NodeId] += perm.PermissionId.ToString();
            }

            return permissions;

        }


        /// <summary>
        /// Gets all users related to the doctype
        /// </summary>
        /// <returns></returns>
        public static IUser[] GetAllRelatedUsers(this IUserType userType)
        {
            int total;

            return ApplicationContext.Current.Services.UserService.GetAll(int.MaxValue, int.MaxValue, out total).Where(x=>x.UserType == userType).OrderBy(x=>x.Name).ToArray();

        }
    }
}