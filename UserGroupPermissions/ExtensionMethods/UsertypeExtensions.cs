using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.BusinessLogic;
using System.Collections;
using umbraco.DataLayer;

namespace UserGroupPermissions.ExtensionMethods
{
    public static class UsertypeExtensions
    {
        public static string GetPermissions(this UserType userType, string path)
        {
            string defaultPermissions = userType.DefaultPermissions;

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


            using (IRecordsReader dr = Application.SqlHelper.ExecuteReader("select * from UserGroupPermissions_UserType2NodePermission where userTypeId = @userTypeId order by nodeId", Application.SqlHelper.CreateParameter("@userTypeId", userTypeId)))
            {
                //	int currentId = -1;
                while (dr.Read())
                {
                    if (!permissions.ContainsKey(dr.GetInt("nodeId")))
                        permissions.Add(dr.GetInt("nodeId"), String.Empty);

                    permissions[dr.GetInt("nodeId")] += dr.GetString("permission");
                }
            }

            return permissions;
        }


        /// <summary>
        /// Gets all users related to the doctype
        /// </summary>
        /// <returns></returns>
        public static User[] GetAllRelatedUsers(this UserType userType)
        {

            IRecordsReader dr;
            dr = Application.SqlHelper.ExecuteReader("Select id from umbracoUser where userType = @userType", Application.SqlHelper.CreateParameter("userType", userType.Id));

            List<User> users = new List<User>();

            while (dr.Read())
            {
                users.Add(User.GetUser(dr.GetInt("id")));
            }
            dr.Close();

            return users.OrderBy(x => x.Name).ToArray();
        }
    }
}