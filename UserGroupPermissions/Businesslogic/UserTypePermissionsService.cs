﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Persistence;
using UserGroupPermissions.ExtensionMethods;
using UserGroupPermissions.Models;

namespace UserGroupPermissions.Businesslogic
{
    /// <summary>
    /// 
    /// </summary>
    public class UserTypePermissionsService
    {

            /// <summary>
            /// Private constructor, this class cannot be directly instantiated
            /// </summary>
            private UserTypePermissionsService() { }

            private static Database SqlHelper
            {
                get { return ApplicationContext.Current.DatabaseContext.Database; }
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public static void Insert(IUserType userType, IContent node, char permissionKey)
            {

                // Method is synchronized so exists remains consistent (avoiding race condition)

                bool exists = SqlHelper.Fetch<int>("SELECT UserTypeId FROM UserTypePermissions WHERE UserTypeId = @0 AND NodeId = @1 AND PermissionId = @2", userType.Id, node.Id, permissionKey.ToString()).Any();
                if (!exists)
                {
                    var newPerms = new UserTypePermission
                    {
                        NodeId = node.Id,
                        PermissionId = permissionKey,
                        UserTypeId = userType.Id,
                    };

                    SqlHelper.Insert(newPerms);

                }
                

            }

        /// <summary>
        /// Returns the permissions for a user
        /// </summary>
        /// <param name="userType"></param>
        /// <returns></returns>
        public static IEnumerable<UserTypePermission> GetUserTypePermissions(IUserType userType)
            {

                var items = SqlHelper.Fetch<UserTypePermission>(
                    "select * from UserTypePermissions  where UserTypeId = @0 order by NodeId", userType.Id);

                return items;
            }

        /// <summary>
        /// Returns the permissions for a node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<UserTypePermission> GetNodePermissions(IContent node)
            {
                
                var items = SqlHelper.Fetch<UserTypePermission>(
                    "select * from UserTypePermissions where NodeId = @0 order by nodeId", node.Id);

                return items;
            }


            /// <summary>
            /// Copies the usertype permissions for single user 
            /// </summary>
            /// <param name="user">The user.</param>
            public static void CopyPermissionsForSingleUser(IUser user)
            {
                var permissions = GetUserTypePermissions(user.UserType);

                foreach (var permission in permissions)
                {
                    var node = ApplicationContext.Current.Services.ContentService.GetById(permission.NodeId);

                    ApplicationContext.Current.Services.ContentService.AssignContentPermission(node, permission.PermissionId, new int[]{user.Id});

                }

            }

            /// <summary>
            /// Copies all  permissions to related users of the usertype
            /// </summary>
            /// <param name="userType">Type of the user.</param>
            /// <param name="node">The node.</param>
            public static void CopyPermissions(IUserType userType, IContent node)
            {
                string permissions = userType.GetPermissions(node.Path);
                foreach (IUser user in userType.GetAllRelatedUsers())
                {
                    if (!user.IsAdmin() && !user.Disabled())
                    {
                        ApplicationContext.Current.Services.UserService.ReplaceUserPermissions(user.Id, permissions, node.Id);

                    }
                }
            }

        /// <summary>
        /// Delets all permissions for the node/user combination
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userType"></param>
        /// <param name="node"></param>
        public static void DeletePermissions(IUserType userType, IContent node)
            {
                // delete all settings on the node for this user

                SqlHelper.Execute("delete from UserTypePermissions where UserTypeId=@0 and NodeId = @1", userType.Id, node.Id);

            }

        /// <summary>
        /// deletes all permissions for the user
        /// </summary>
        /// <param name="userType"></param>
        public static void DeletePermissions(IUserType userType)
            {
                // delete all settings on the node for this user

                SqlHelper.Execute("delete from UserTypePermissions where UserTypeId=@0 ", userType.Id);

            }

            public static void DeletePermissions(int userTypeId, int[] iNodeIDs)
            {
                
                string nodeIDs = string.Join(",", Array.ConvertAll<int, string>(iNodeIDs, Converter));

                SqlHelper.Execute("delete from UserTypePermissions where NodeId IN (@0) AND UserTypeId=@1 ", nodeIDs, userTypeId);

            }

            private static string Converter(int from)
            {
                return from.ToString();
            }

            /// <summary>
            /// delete all permissions for this node
            /// </summary>
            /// <param name="node"></param>
            public static void DeletePermissions(IContent node)
            {
                SqlHelper.Execute("delete from UserTypePermissions where NodeId = @0", node.Id);
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public static void UpdateCruds(IUserType userType, IContent node, string permissions)
            {
                // delete all settings on the node for this user
                DeletePermissions(userType, node);

                // Loop through the permissions and create them
                foreach (char c in permissions.ToCharArray())
                    Insert(userType, node, c);
            }
        }
    }
