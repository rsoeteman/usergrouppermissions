using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.cms.presentation.Trees;
using UserGroupPermissions.MenuActions;
using umbraco.interfaces;
using umbraco.BusinessLogic;
using umbraco.BusinessLogic.Actions;
using umbraco.businesslogic;

namespace UserGroupPermissions.Events
{
    public class AddUserGroupPermissionToContextMenu : ApplicationStartupHandler 
    {
        public AddUserGroupPermissionToContextMenu()
        {
            BaseContentTree.BeforeNodeRender += new BaseTree.BeforeNodeRenderEventHandler(BaseContentTree_BeforeNodeRender);
        }

        void BaseContentTree_BeforeNodeRender(ref XmlTree sender, ref XmlTreeNode node, EventArgs e)
        {
             if (node.Menu!= null && node.NodeType =="content")  
             {  
                 //Find the publish action and add 1 for the index, so the position of the ubpublish  is direct after the publish menu item  
                 IAction action = node.Menu.Find(p => p.Alias == "rights");
                 if (action != null)
                 {
                     int index = node.Menu.FindIndex(p => p.Alias == "rights");
                    //Insert unpublish action  
                     node.Menu.Insert(index, UsergroupPermissions.Instance);
                     node.Menu.Insert(index, ContextMenuSeperator.Instance);
                 }
                 
             }
        }
    }
}