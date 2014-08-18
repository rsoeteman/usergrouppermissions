function openUserGroupDialog() 
{
    if (UmbClientMgr.mainTree().getActionNode().nodeId != '-1' && UmbClientMgr.mainTree().getActionNode().nodeType != '') {
        UmbClientMgr.openModalWindow("plugins/usergrouppermissions/dialogs/permissions.aspx?id=" + UmbClientMgr.mainTree().getActionNode().nodeId + '&rnd=', 'Set role permissions', true, 800, 400);
    }
}