using System;
using System.Collections;
using System.Linq;
using System.Web.UI.WebControls;
using Cuyahoga.Core.Communication;
using Cuyahoga.Core.Domain;
using Cuyahoga.Web.Admin.UI;

namespace Cuyahoga.Web.Admin
{
    public partial class ConnectionEdit : AdminBasePage
    {
        private readonly Section _activeSection;
        private IActionProvider _activeActionProvider;

        public ConnectionEdit()
        {
            _activeSection = SectionService.GetSectionById(Int32.Parse(base.Context.Request.QueryString["SectionId"]));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.Request.QueryString["SectionId"] != null)
            {
                Title = "Add connection";

                ModuleBase moduleInstance = ModuleLoader.GetModuleFromSection(_activeSection);
                if (moduleInstance is IActionProvider)
                {
                    _activeActionProvider = moduleInstance as IActionProvider;

                    if (! IsPostBack)
                    {
                        BindSection();
                        BindCompatibleSections();
                    }
                }
                else
                {
                    ShowError("The module that is connected to the section doesn't support outgoing connections.");
                }
            }
        }

        protected void BindSection()
        {
            lblSectionFrom.Text = _activeSection.FullName;
            lblModuleType.Text = _activeSection.ModuleType.Name;

            ModuleActionCollection outboundActions = _activeActionProvider.GetOutboundActions();
            foreach (
                ModuleAction moduleAction in
                    outboundActions.Cast<ModuleAction>().Where(
                        moduleAction => !_activeSection.Connections.ContainsKey(moduleAction.Name)))
            {
                ddlAction.Items.Add(moduleAction.Name);
            }
        }

        protected void BindCompatibleSections()
        {
            string selectedAction;

            if (ddlAction.SelectedIndex == -1 && ddlAction.Items.Count > 0)
            {
                selectedAction = ddlAction.Items[0].Value;
            }
            else
            {
                selectedAction = ddlAction.SelectedValue;
            }

            var compatibleModuleTypes = new ArrayList();
            // Get all ModuleTypes.
            IList moduleTypes = ModuleTypeService.GetAllModuleTypes().ToList();

            foreach (ModuleType mt in moduleTypes)
            {
                string assemblyQualifiedName = mt.ClassName + ", " + mt.AssemblyName;
                Type moduleTypeType = Type.GetType(assemblyQualifiedName);

                if (moduleTypeType != null && ModuleLoader.IsModuleActive(mt))
                {
                    ModuleBase moduleInstance = ModuleLoader.GetModuleFromType(mt);
                    if (moduleInstance is IActionConsumer)
                    {
                        var actionConsumer = moduleInstance as IActionConsumer;
                        ModuleAction currentAction =
                            _activeActionProvider.GetOutboundActions().FindByName(selectedAction);
                        if (actionConsumer.GetInboundActions().Contains(currentAction))
                        {
                            compatibleModuleTypes.Add(mt);
                        }
                    }
                }
            }

            if (compatibleModuleTypes.Count > 0)
            {
                // Retrieve all sections that have the compatible ModuleTypes
                IList compatibleSections = base.SectionService.GetSectionsByModuleTypes(compatibleModuleTypes);

                if (compatibleSections.Count > 0)
                {
                    pnlTo.Visible = true;
                    btnSave.Enabled = true;

                    // List only sections in current active site
                    foreach (Section s in compatibleSections)
                    {
                        if (ActiveSite.Id == s.Node.Site.Id)
                        {
                            ddlSectionTo.Items.Add(new ListItem(s.FullName, s.Id.ToString()));
                        }
                    }
                }
                else
                {
                    pnlTo.Visible = false;
                    btnSave.Enabled = false;
                }
            }
        }

        protected void RedirectToSectionEdit()
        {
            if (ActiveNode != null)
            {
                Context.Response.Redirect(String.Format("SectionEdit.aspx?NodeId={0}&SectionId={1}", ActiveNode.Id,
                                                        _activeSection.Id));
            }
            else
            {
                Context.Response.Redirect(String.Format("SectionEdit.aspx?SectionId={0}", _activeSection.Id));
            }
        }

        protected void BtnBackClick(object sender, EventArgs e)
        {
            RedirectToSectionEdit();
        }

        protected void DdlActionSelectedIndexChanged(object sender, EventArgs e)
        {
            BindCompatibleSections();
        }

        protected void BtnSaveClick(object sender, EventArgs e)
        {
            int sectionid = Int32.Parse(ddlSectionTo.SelectedValue);
            _activeSection.Connections[ddlAction.SelectedValue] = SectionService.GetSectionById(sectionid);

            try
            {
                SectionService.UpdateSection(_activeSection);
                RedirectToSectionEdit();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}