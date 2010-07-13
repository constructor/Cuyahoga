using System;
using System.Linq;
using System.IO;
using System.Collections;

using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service.Membership;
using Cuyahoga.Core.Util;
using Cuyahoga.Web.UI;

namespace Cuyahoga.Modules.Downloads.Web
{
	/// <summary>
	/// Summary description for EditFile.
	/// </summary>
	public class EditFile : ModuleAdminBasePage
	{
		private DownloadsModule _downloadsModule;
		private int _fileId;
		private FileResource _file;
        private IUserService _userService;

		protected System.Web.UI.WebControls.Button btnSave;
		protected System.Web.UI.WebControls.Button btnDelete;
		protected System.Web.UI.WebControls.Panel pnlFileName;
		protected System.Web.UI.HtmlControls.HtmlInputFile filUpload;
		protected System.Web.UI.WebControls.Button btnUpload;
		protected System.Web.UI.WebControls.RequiredFieldValidator rfvFile;
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.Repeater rptRoles;
		protected Cuyahoga.ServerControls.Calendar calDatePublished;
		protected System.Web.UI.WebControls.TextBox txtTitle;
		protected System.Web.UI.WebControls.RequiredFieldValidator rfvDatePublished;
		protected System.Web.UI.WebControls.TextBox txtFile;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
            _userService = IoC.Resolve<IUserService>();

			// The base page has already created the module, we only have to cast it here to the right type.
			this._downloadsModule = base.Module as DownloadsModule;
			
			this._fileId = Int32.Parse(Request.QueryString["FileId"]);
			if (this._fileId > 0)
			{
				this._file = this._downloadsModule.GetFileById(this._fileId);
				if (! this.IsPostBack)
				{
					BindFile();
				}
				this.btnDelete.Visible = true;
				this.btnDelete.Attributes.Add("onclick", "return confirm('Are you sure?');");
			}
			else
			{
				// It is possible that a new file is already uploaded and in the database. The
				// tempFileId parameter in the viewstate should indicate this.
                if (ViewState["tempFileId"] != null)
				{
					int tempFileId = int.Parse(ViewState["tempFileId"].ToString());
					this._file = this._downloadsModule.GetFileById(tempFileId);
				}
				else
				{
					// New file.
					this._file = new FileResource();
					// Copy roles that have view rights from parent section.
					foreach (Permission permission in this._downloadsModule.Section.SectionPermissions)
					{
						this._file.ContentItemPermissions.Add(new ContentItemPermission { ContentItem = this._file, Role = permission.Role, ViewAllowed = permission.ViewAllowed });
					}
					this.calDatePublished.SelectedDate = DateTime.Now;
				}
				this.btnDelete.Visible = false;
			}
			if (! this.IsPostBack)
			{
				BindRoles();
			}
		}

		private void BindFile()
		{
			this.txtFile.Text = this._file.PhysicalFilePath;
			this.txtTitle.Text = this._file.Title;

            if(this._file.PublishedAt != null)
			this.calDatePublished.SelectedDate = (DateTime)this._file.PublishedAt;
		}

		private void BindRoles()
		{
			// Get roles via SectionPermissions of the section.
			this.rptRoles.DataSource = base.Section.SectionPermissions;
			this.rptRoles.DataBind();
		}

		private string CreateServerFilename(string clientFilename)
		{
			if (clientFilename.LastIndexOf(System.IO.Path.DirectorySeparatorChar) > -1)
			{
				return clientFilename.Substring(clientFilename.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1);
			}
			else
			{
				return clientFilename;
			}
		}

		private void SetRoles()
		{
			this._file.ContentItemPermissions.Clear();

			foreach (RepeaterItem ri in rptRoles.Items)
			{	
				// HACK: RoleId is stored in the ViewState because the repeater doesn't have a DataKeys property.
				// Another HACK: we're only using the role id's to save database roundtrips.
				CheckBox chkRole = (CheckBox)ri.FindControl("chkRole");
				if (chkRole.Checked)
				{
					int roleId = (int)this.ViewState[ri.ClientID];
                    Role role = _userService.GetRoleById(roleId);

                    this._file.ContentItemPermissions.Add(new ContentItemPermission { ContentItem = this._file, Role = role, ViewAllowed = chkRole.Checked });
					//this._file.AllowedRoles.Add(role);
				}
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
			this.rptRoles.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.rptRoles_ItemDataBound);
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

        private void btnUpload_Click(object sender, System.EventArgs e)
		{
			HttpPostedFile postedFile = this.filUpload.PostedFile;
			if (postedFile.ContentLength > 0)
			{
                this._file.Section = this._downloadsModule.Section;
                this._file.FileName = CreateServerFilename(postedFile.FileName);
                this._file.PhysicalFilePath = this._downloadsModule.FileDir;
				this._file.Title = this.txtTitle.Text;
                this._file.CreatedBy = (User)this.User.Identity;
                this._file.CreatedAt = DateTime.Now;
                this._file.PublishedBy = (User)this.User.Identity;
                this._file.PublishedAt = DateTime.Now;
                this._file.ModifiedBy = (User)this.User.Identity;
                this._file.ModifiedAt = DateTime.Now;
    			this._file.MimeType = postedFile.ContentType;
				this._file.Length = postedFile.ContentLength;
				
				string fullFilePath = this._downloadsModule.FileDir + this._file.FileName;
				// Save the file
				try
				{
					this._downloadsModule.SaveFile(this._file, postedFile.InputStream);
					if (this._fileId <= 0 && this._file.Id > 0)
					{
						// This appears to be a new file. Store the id of the file in the viewstate
						// so the file can be deleted if the user decides to cancel.
                        ViewState.Add("tempFileId", this._file.Id);
						//ViewState["tempFileId"] = this._file.Id;
					}
					BindFile();
				}
				catch (Exception ex)
				{
					// Something went wrong
					ShowError("Error saving the file: " + fullFilePath + " " + ex.ToString());
				}
			}
		}
		
        private void btnSave_Click(object sender, System.EventArgs e)
		{
			if (this.IsValid)
			{
				this._file.PublishedBy = this.User.Identity as User;
				this._file.Title = this.txtTitle.Text;
				this._file.PublishedAt = calDatePublished.SelectedDate;
				SetRoles();

				try
				{
					// Only save meta data.
					this._downloadsModule.SaveFileInfo(this._file);
					Context.Response.Redirect("EditDownloads.aspx" + base.GetBaseQueryString());
				}
				catch (Exception ex)
				{
					ShowError("Error saving file: " + ex.Message);
				}
			}
		}
		
        private void btnDelete_Click(object sender, System.EventArgs e)
		{
			try
			{
				this._downloadsModule.DeleteFile(this._file);
				Context.Response.Redirect("EditDownloads.aspx" + base.GetBaseQueryString());
			}
			catch (Exception ex)
			{
				ShowError("Error deleting file: " + ex.Message);
			}
		}
		
        private void btnCancel_Click(object sender, System.EventArgs e)
		{
			// Check if there is a new file pending. This has to be deleted
			if (ViewState["tempFileId"] != null)
			{
				this._downloadsModule.DeleteFile(this._file);
			}
			Context.Response.Redirect("EditDownloads.aspx" + base.GetBaseQueryString());
		}

		private void rptRoles_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType != ListItemType.Header)
			{
				SectionPermission permission = e.Item.DataItem as SectionPermission;
				Role role = permission.Role;

				if (role != null)
				{
					CheckBox chkRole = (CheckBox)e.Item.FindControl("chkRole");


                    User currentUser = HttpContext.Current.User as User;
                    bool viewAllow = false;
                    foreach (Role cur in currentUser.Roles)
                    {
                        viewAllow = this._file.ViewRoles.Contains(cur) ? true : false;
                    }

                    chkRole.Checked = viewAllow;
					// Add RoleId to the ViewState with the ClientID of the repeateritem as key.
					this.ViewState[e.Item.ClientID] = role.Id;
				}
			}
		}
	
    }
}
