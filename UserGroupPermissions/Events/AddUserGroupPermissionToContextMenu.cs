using Umbraco.Core;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Trees;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;

namespace UserGroupPermissions.Events
{
    public class AddUserGroupPermissionToContextMenu : ApplicationEventHandler 
    {
        public AddUserGroupPermissionToContextMenu()
        {
            TreeControllerBase.MenuRendering += TreeControllerBase_MenuRendering;

            //TreeControllerBase.TreeNodesRendering += BaseContentTree_BeforeNodeRender;

            //BaseContentTree.BeforeNodeRender += new BaseTree.BeforeNodeRenderEventHandler(BaseContentTree_BeforeNodeRender);
        }
        
        //the event listener method:
        void TreeControllerBase_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            //this example will add a custom menu item for all admin users
            // for all content tree nodes
            IUser currentUser = sender.Security.CurrentUser;
            if (sender.TreeAlias == "content"
                && currentUser.UserType.Alias == "admin")
            {
                var menuItem = new MenuItem("UserGroupPermissions", "User Group permissions") {Icon = "vcard"};

                menuItem.LaunchDialogUrl("/App_Plugins/UserGroupPermissions/Dialogs/SetUsergroupPermissions.aspx?id=" + e.NodeId, "User Group permissions");

                e.Menu.Items.Add(menuItem);

            }
        }
    }
}