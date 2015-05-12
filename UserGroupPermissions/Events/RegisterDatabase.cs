using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using UserGroupPermissions.Models;

namespace UserGroupPermissions.Events
{
    public class RegisterDatabase : ApplicationEventHandler
    {
        //This happens everytime the Umbraco Application starts
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //Get the Umbraco Database context
            var db = applicationContext.DatabaseContext.Database;

            //Check if the DB table does NOT exist
            if (!db.TableExist("UserTypePermissions"))
            {
                //Create DB table - and set overwrite to false
                db.CreateTable<UserTypePermission>(false);
            }
        }

    }

}