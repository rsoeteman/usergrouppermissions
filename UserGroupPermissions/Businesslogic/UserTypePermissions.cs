using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.DataLayer;
using UserGroupPermissions.ExtensionMethods;

namespace UserGroupPermissions.Businesslogic
{
    /// <summary>
    /// 
    /// </summary>
    public class UserTypePermissions
    {
            public int NodeId { get; private set; }
            public int UserTypeId { get; private set; }
            public char PermissionId { get; private set; }

            /// <summary>
            /// Private constructor, this class cannot be directly instantiated
            /// </summary>
            private UserTypePermissions() { }

            private static ISqlHelper SqlHelper
            {
                get { return Application.SqlHelper; }
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public static void MakeNew(umbraco.BusinessLogic.UserType userType, umbraco.cms.businesslogic.CMSNode Node, char PermissionKey)
            {
                IParameter[] parameters = new IParameter[] { SqlHelper.CreateParameter("@userTypeId", userType.Id),
                                                         SqlHelper.CreateParameter("@nodeId", Node.Id),
                                                         SqlHelper.CreateParameter("@permission", PermissionKey.ToString()) };

                // Method is synchronized so exists remains consistent (avoiding race condition)
                bool exists = SqlHelper.ExecuteScalar<int>("SELECT COUNT(userTypeId) FROM UserGroupPermissions_UserType2NodePermission WHERE userTypeId = @userTypeId AND nodeId = @nodeId AND permission = @permission",
                                                           parameters) > 0;
                if (!exists)
                    SqlHelper.ExecuteNonQuery("INSERT INTO UserGroupPermissions_UserType2NodePermission (userTypeId, nodeId, permission) VALUES (@userTypeId, @nodeId, @permission)",
                                              parameters);
            }

            /// <summary>
            /// Returns the permissions for a user
            /// </summary>
            /// <param name="user"></param>
            /// <returns></returns>
            public static IEnumerable<UserTypePermissions> GetUserTypePermissions(UserType userType)
            {
                var items = new List<UserTypePermissions>();
                using (IRecordsReader dr = SqlHelper.ExecuteReader("select * from UserGroupPermissions_UserType2NodePermission where userTypeId = @userTypeId order by nodeId", SqlHelper.CreateParameter("@userTypeId", userType.Id)))
                {
                    while (dr.Read())
                    {
                        items.Add(new UserTypePermissions()
                        {
                            NodeId = dr.GetInt("nodeId"),
                            PermissionId = Convert.ToChar(dr.GetString("permission")),
                            UserTypeId = dr.GetInt("userTypeId")
                        });
                    }
                }
                return items;
            }

            /// <summary>
            /// Returns the permissions for a node
            /// </summary>
            /// <param name="user"></param>
            /// <returns></returns>
            public static IEnumerable<UserTypePermissions> GetNodePermissions(CMSNode node)
            {
                var items = new List<UserTypePermissions>();
                using (IRecordsReader dr = SqlHelper.ExecuteReader("select * from UserGroupPermissions_UserType2NodePermission where nodeId = @nodeId order by nodeId", SqlHelper.CreateParameter("@nodeId", node.Id)))
                {
                    while (dr.Read())
                    {
                        items.Add(new UserTypePermissions()
                        {
                            NodeId = dr.GetInt("nodeId"),
                            PermissionId = Convert.ToChar(dr.GetString("permission")),
                            UserTypeId = dr.GetInt("userTypeId")
                        });
                    }
                }
                return items;
            }


            /// <summary>
            /// Copies the usertype permissions for single user 
            /// </summary>
            /// <param name="user">The user.</param>
            public static void CopyPermissionsForSingleUser(User user)
            {
                var permissions = GetUserTypePermissions(user.UserType);

                foreach (var permission in permissions)
                {
                    Permission.MakeNew(user, new CMSNode(permission.NodeId), permission.PermissionId);
                    user.initCruds();
                }

            }

            /// <summary>
            /// Copies all  permissions to related users of the usertype
            /// </summary>
            /// <param name="userType">Type of the user.</param>
            /// <param name="node">The node.</param>
            public static void CopyPermissions(UserType userType, CMSNode node)
            {
                string permissions = userType.GetPermissions(node.Path);
                foreach (User user in userType.GetAllRelatedUsers())
                {
                    if (!user.IsAdmin() && !user.Disabled)
                    {
                        Permission.UpdateCruds(user, node, permissions);
                        user.initCruds();
                    }
                }
            }

            /// <summary>
            /// Delets all permissions for the node/user combination
            /// </summary>
            /// <param name="user"></param>
            /// <param name="node"></param>
            public static void DeletePermissions(UserType userType, CMSNode node)
            {
                // delete all settings on the node for this user
                SqlHelper.ExecuteNonQuery("delete from UserGroupPermissions_UserType2NodePermission where userTypeId = @userTypeId and nodeId = @nodeId",
                    SqlHelper.CreateParameter("@userTypeId", userType.Id), SqlHelper.CreateParameter("@nodeId", node.Id));
            }

            /// <summary>
            /// deletes all permissions for the user
            /// </summary>
            /// <param name="user"></param>
            public static void DeletePermissions(UserType userType)
            {
                // delete all settings on the node for this user
                SqlHelper.ExecuteNonQuery("delete from UserGroupPermissions_UserType2NodePermission where userTypeId = @userTypeId",
                    SqlHelper.CreateParameter("@userTypeId", userType.Id));
            }

            public static void DeletePermissions(int userTypeId, int[] iNodeIDs)
            {
                string sql = "DELETE FROM UserGroupPermissions_UserType2NodePermission WHERE nodeID IN ({0}) AND userTypeId=@userTypeId";
                string nodeIDs = string.Join(",", Array.ConvertAll<int, string>(iNodeIDs, Converter));
                sql = string.Format(sql, nodeIDs);
                SqlHelper.ExecuteNonQuery(sql,
                    new IParameter[] { SqlHelper.CreateParameter("@userTypeId", userTypeId) });
            }
            //public static void DeletePermissions(int iUserID, int iNodeID)
            //{
            //    DeletePermissions(iUserID, new int[] { iNodeID });
            //}
            private static string Converter(int from)
            {
                return from.ToString();
            }

            /// <summary>
            /// delete all permissions for this node
            /// </summary>
            /// <param name="node"></param>
            public static void DeletePermissions(CMSNode node)
            {

                SqlHelper.ExecuteNonQuery("delete from umbracoUser2NodePermission where nodeId = @nodeId",
                    SqlHelper.CreateParameter("@nodeId", node.Id));
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public static void UpdateCruds(UserType userType, CMSNode node, string permissions)
            {
                // delete all settings on the node for this user
                DeletePermissions(userType, node);

                // Loop through the permissions and create them
                foreach (char c in permissions.ToCharArray())
                    MakeNew(userType, node, c);
            }
        }
    }
