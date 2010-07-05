using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using RightNames = Cuyahoga.Core.Service.Membership.Rights;

namespace Cuyahoga.Core.Domain
{
	/// <summary>
	/// Summary description for Role.
	/// </summary>
	public class Role
	{
		private int _id;
		private string _name;
		private bool _isGlobal;
		private IList<Right> _rights;
		private IList<Site> _sites;
		private DateTime _updateTimestamp;

		/// <summary>
		/// Property Id (int)
		/// </summary>
		public virtual int Id
		{
			get { return this._id; }
			set { this._id = value; }
		}

		/// <summary>
		/// Property Name (string)
		/// </summary>
		public virtual string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		/// <summary>
		/// Indicates if the role is global for all sites within the Cuyahoga installation.
		/// </summary>
		public virtual bool IsGlobal
		{
			get { return _isGlobal; }
			set { _isGlobal = value; }
		}

		/// <summary>
		/// Gets or sets a list of access rights.
		/// </summary>
		public virtual IList<Right> Rights
		{
			get { return _rights; }
			set { _rights = value; }
		}

		/// <summary>
		/// Gets or sets a list of related sites.
		/// </summary>
		public virtual IList<Site> Sites
		{
			get { return _sites; }
			set { _sites = value; }
		}

		/// <summary>
		/// Gets the rights of the role as a string.
		/// </summary>
		public virtual string RightsString
		{
			get { return GetRightsAsString(); }
		}

		/// <summary>
		/// Indicates if the role is an anonymous role. This means that it only has the 'Anonymous' right and nothing else.
		/// </summary>
		public virtual bool IsAnonymousRole
		{
			//get { return _rights.Count == 1 && _rights[0].Name.Equals(RightNames.Anonymous); }
            get { return this.Name == "Anonymous User"; }
		}

		/// <summary>
		/// Property UpdateTimestamp (DateTime)
		/// </summary>
		public virtual DateTime UpdateTimestamp
		{
			get { return this._updateTimestamp; }
			set { this._updateTimestamp = value; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Role()
		{
			this._id = -1;
			this._name = null;
			//this._permissionLevel = -1;
			this._rights = new List<Right>();
			this._sites = new List<Site>();
		}

		/// <summary>
		/// Check if the role has the requested access right.
		/// </summary>
		/// <param name="rightName"></param>
		/// <returns></returns>
		public virtual bool HasRight(string rightName)
		{
			foreach (Right right in _rights)
			{
				if (right.Name.Equals(rightName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private string GetRightsAsString()
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < this._rights.Count; i++)
			{
				Right right = this._rights[i];
				sb.Append(right.Name);
				if (i < this._rights.Count - 1)
				{
					sb.Append(", ");
				}
			}

			return sb.ToString();
		}
	}
}
