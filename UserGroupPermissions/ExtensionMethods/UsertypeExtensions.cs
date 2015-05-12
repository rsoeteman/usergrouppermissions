﻿using System;
using System.Linq;
using System.Collections;
using Umbraco.Core;
using Umbraco.Core.Models.Membership;
using UserGroupPermissions.Models;

namespace UserGroupPermissions.ExtensionMethods
{
    public static class UsertypeExtensions
    {
        /// <summary>
        /// Gets all users related to the doctype
        /// </summary>
        /// <returns></returns>
        public static IUser[] GetAllRelatedUsers(this IUserType userType)
        {
            int total;

            return ApplicationContext.Current.Services.UserService.GetAll(0, int.MaxValue, out total).Where(x=>x.UserType.Id== userType.Id).OrderBy(x=>x.Name).ToArray();

        }
    }
}