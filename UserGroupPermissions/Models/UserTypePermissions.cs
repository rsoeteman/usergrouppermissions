using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;

namespace UserGroupPermissions.Models
{
    [TableName("UserTypePermissions")]
    public class UserTypePermission
    {
        public int NodeId { get; set; }
        public int UserTypeId { get; set; }
        public char PermissionId { get; set; }
    }
}