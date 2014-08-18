using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.BasePages;
using System.Collections;
using umbraco;
using System.Web.UI.HtmlControls;
using UserGroupPermissions.ExtensionMethods;
using umbraco.BusinessLogic;
using UserGroupPermissions.Businesslogic;

namespace UserGroupPermissions.Dialogs
{
    public partial class SetUsergroupPermissions : UmbracoEnsuredPage
    {
        private ArrayList permissions = new ArrayList();
        private umbraco.cms.businesslogic.CMSNode node;


        protected void Page_Load(object sender, System.EventArgs e)
        {
            Button1.Text = ui.Text("update");
            pane_form.Text = "Set Usergroup permissions for the page " + node.Text;

            // Put user code to initialize the page here
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);

            node = new umbraco.cms.businesslogic.CMSNode(int.Parse(helper.Request("id")));

            HtmlTable ht = new HtmlTable();
            ht.CellPadding = 4;

            HtmlTableRow captions = new HtmlTableRow();
            captions.Cells.Add(new HtmlTableCell());

            ArrayList actionList = umbraco.BusinessLogic.Actions.Action.GetAll();
            foreach (umbraco.interfaces.IAction a in actionList)
            {
                if (a.CanBePermissionAssigned)
                {
                    HtmlTableCell hc = new HtmlTableCell();
                    hc.Attributes.Add("class", "guiDialogTinyMark");
                    hc.Controls.Add(new LiteralControl(ui.Text("actions", a.Alias)));
                    captions.Cells.Add(hc);
                }
            }
            ht.Rows.Add(captions);

            foreach (umbraco.BusinessLogic.UserType userType in umbraco.BusinessLogic.UserType.GetAllUserTypes())
            {
                // Not disabled users and not system account
                if (userType.Id > 0 && userType.Alias != "admin")
                {
                    HtmlTableRow hr = new HtmlTableRow();

                    HtmlTableCell hc = new HtmlTableCell();
                    hc.Attributes.Add("class", "guiDialogTinyMark");
                    hc.Controls.Add(new LiteralControl(userType.Name));
                    hr.Cells.Add(hc);

                    foreach (umbraco.interfaces.IAction a in umbraco.BusinessLogic.Actions.Action.GetAll())
                    {
                        CheckBox c = new CheckBox();
                        c.ID = userType.Id + "_" + a.Letter;
                        if (a.CanBePermissionAssigned)
                        {
                            if (userType.GetPermissions(node.Path).IndexOf(a.Letter) > -1)
                                c.Checked = true;
                            HtmlTableCell cell = new HtmlTableCell();
                            cell.Style.Add("text-align", "center");
                            cell.Controls.Add(c);
                            permissions.Add(c);
                            hr.Cells.Add(cell);
                        }

                    }
                    ht.Rows.Add(hr);
                }
            }
            PlaceHolder1.Controls.Add(ht);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        protected void Button1_Click(object sender, System.EventArgs e)
        {
            Hashtable allUserTypes = new Hashtable();

            foreach (umbraco.BusinessLogic.UserType u in UserType.GetAllUserTypes())
            {
                allUserTypes.Add(u.Id, "");
            }

            foreach (CheckBox c in permissions)
            {
                // Update the user with the new permission
                if (c.Checked)
                    allUserTypes[int.Parse(c.ID.Substring(0, c.ID.IndexOf("_")))] = (string)allUserTypes[int.Parse(c.ID.Substring(0, c.ID.IndexOf("_")))] + c.ID.Substring(c.ID.IndexOf("_") + 1, c.ID.Length - c.ID.IndexOf("_") - 1);
            }


            // Loop through the users and update their Cruds...
            IDictionaryEnumerator uEnum = allUserTypes.GetEnumerator();
            while (uEnum.MoveNext())
            {
                string cruds = "-";
                if (uEnum.Value.ToString().Trim() != "")
                {
                    cruds = uEnum.Value.ToString();
                }
                UserType usertype = umbraco.BusinessLogic.UserType.GetUserType(int.Parse(uEnum.Key.ToString()));
                UserGroupPermissions.Businesslogic.UserTypePermissions.UpdateCruds(usertype, node, cruds);

                if (ReplacePermissionsOnChildren.Checked)
                {
                    //Replace permissions on all children
                    UserTypePermissions.CopyPermissions(usertype, node);
                }
            }

            // Update feedback message
            feedback1.type = umbraco.uicontrols.Feedback.feedbacktype.success;
            feedback1.Text = "Usergroup permissions saved ok ";
            pane_form.Visible = false;
            panel_buttons.Visible = false;
        }
    }
}
