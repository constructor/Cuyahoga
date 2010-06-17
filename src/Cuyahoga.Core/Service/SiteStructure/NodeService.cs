using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Services.Transaction;

using Cuyahoga.Core.Domain;
using Cuyahoga.Core.DataAccess;

namespace Cuyahoga.Core.Service.SiteStructure
{
	/// <summary>
	/// Provides functionality to manage nodes (pages).
	/// </summary>
	[Transactional]
	public class NodeService : INodeService
	{
		private ISiteStructureDao _siteStructureDao;
		private ICommonDao _commonDao;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="siteStructureDao"></param>
		/// <param name="commonDao"></param>
		public NodeService(ISiteStructureDao siteStructureDao, ICommonDao commonDao)
		{
			this._siteStructureDao = siteStructureDao;
			this._commonDao = commonDao;
		}

		#region INodeService Members
		    public Node GetNodeById(int nodeId)
		    {
			    return (Node)this._commonDao.GetObjectById(typeof(Node), nodeId, true);
		    }

		    public IList<Node> GetRootNodes(Site site)
		    {
			    return this._siteStructureDao.GetRootNodes(site);
		    }

		    public Node GetRootNodeByCultureAndSite(string culture, Site site)
		    {
			    return this._siteStructureDao.GetRootNodeByCultureAndSite(culture, site);
		    }

		    public Node GetNodeByShortDescriptionAndSite(string shortDescription, Site site)
		    {
			    return this._siteStructureDao.GetNodeByShortDescriptionAndSite(shortDescription, site);
		    }

		    public IList GetNodesByTemplate(Template template)
		    {
			    return this._siteStructureDao.GetNodesByTemplate(template);
		    }

            public IList GetNodesBySite(Site site)
            {
                return _siteStructureDao.GetNodesBySite(site);
            }

		    public IList GetMenusByRootNode(Node rootNode)
		    {
			    return this._siteStructureDao.GetMenusByRootNode(rootNode);
		    }

		    [Transaction(TransactionMode.RequiresNew)]
		    public void SaveNode(Node node)
		    {
			    this._commonDao.SaveOrUpdateObject(node);
		    }

		    [Transaction(TransactionMode.Requires)]
		    public void UpdateNode(Node node, bool propagatePermissionsToChildNodes, bool propagatePermissionsToSections)
		    {
			    this._commonDao.SaveOrUpdateObject(node);
			    if (propagatePermissionsToChildNodes)
			    {
				    PropagatePermissionsToChildNodes(node, propagatePermissionsToSections);
			    }
			    if (propagatePermissionsToSections)
			    {
				    PropagatePermissionsToSections(node);
			    }
		    }

		    [Transaction(TransactionMode.RequiresNew)]
		    public void DeleteNode(Node node)
		    {
			    if (node.Sections.Count > 0)
			    {
				    throw new DeleteForbiddenException("NodeDeleteForbiddenSectionsException");
			    }
			    if (node.ChildNodes.Count > 0)
			    {
				    throw new DeleteForbiddenException("NodeDeleteForbiddenChildNodesException");
			    }

			    // Clear query cache for nodes
			    this._commonDao.RemoveQueryFromCache("Nodes");

			    // Remove node from collections
			    IList<Node> containingCollection;
			    if (node.IsRootNode)
			    {
				    // Explicitly get the rootnodes. When fetching via node.Site.RootNodes, NHibernate tries to update the Site property of the 
				    // node to null before deleting the node -> booom.
				    containingCollection = this._siteStructureDao.GetRootNodes(node.Site);
			    }
			    else
			    {
				    containingCollection = node.ParentNode.ChildNodes;
			    }
			    containingCollection.Remove(node);
			    ReOrderNodePositions(containingCollection);

			    // Remove node from menu's
			    IList menus = this._siteStructureDao.GetMenusByParticipatingNode(node);
			    foreach (CustomMenu menu in menus)
			    {
				    // HACK: due to a bug with proxies IList.Remove(object) always removes the first object in
				    // the list. Also IList.IndexOf always returns 0. Therefore, we'll loop through the collection
				    // and find the right index. Btw, when turning off proxies everything works fine.
				    int positionFound = -1;
				    for (int i = 0; i < menu.Nodes.Count; i++)
				    {
					    if (((Node)menu.Nodes[i]).Id == node.Id)
					    {
						    positionFound = i;
						    break;
					    }
				    }
				    if (positionFound > -1)
				    {
					    menu.Nodes.RemoveAt(positionFound);
				    }
				    this._commonDao.SaveOrUpdateObject(menu);
			    }

			    this._commonDao.DeleteObject(node);
		    }

		    [Transaction(TransactionMode.RequiresNew)]
		    public void SortNodes(int parentNodeId, int[] orderedChildNodeIds)
		    {
			    Node parentNode = GetNodeById(parentNodeId);
			    for (int i = 0; i < orderedChildNodeIds.Length; i++)
			    {
				    Node childNode = parentNode.ChildNodes.Single(n => n.Id == orderedChildNodeIds[i]);
				    childNode.Position = i;
			    }
			    // Invalidate cache
			    this._commonDao.RemoveCollectionFromCache("Cuyahoga.Core.Domain.Node.ChildNodes", parentNode.Id);
		    }

		    [Transaction(TransactionMode.RequiresNew)]
		    public void MoveNode(int nodeIdToMove, int nodeIdToMoveTo)
		    {
			    Node nodeToMove = GetNodeById(nodeIdToMove);
			    Node newParentNode = GetNodeById(nodeIdToMoveTo);

			    nodeToMove.ChangeParent(newParentNode);
		    }

		    [Transaction(TransactionMode.RequiresNew)]
		    public Node CopyNode(int nodeIdToCopy, int newParentNodeId)
		    {
			    Node nodeToCopy = GetNodeById(nodeIdToCopy);
			    Node newParentNode = GetNodeById(newParentNodeId);

			    Node newNode = nodeToCopy.Copy(newParentNode);
			    this._commonDao.SaveObject(newNode);
			    // also save sections, these don't cascade
			    foreach (Section section in newNode.Sections)
			    {
				    this._commonDao.SaveObject(section);
			    }
			    return newNode;
		    }

		    [Transaction(TransactionMode.RequiresNew)]
		    public Node CreateRootNode(Site site, Node newNode)
		    {
			    // ShortDescription is equal to language part of culture by default.
			    CultureInfo ci = new CultureInfo(newNode.Culture);
			    newNode.ShortDescription = ci.TwoLetterISOLanguageName;
			    newNode.Site = site;
			    newNode.Position = site.RootNodes.Count;
			    site.RootNodes.Add(newNode);
			    this._commonDao.SaveObject(newNode);
			    return newNode;
		    }

		    [Transaction(TransactionMode.RequiresNew)]
		    public Node CreateNode(Node parentNode, Node newNode)
		    {
			    newNode.ParentNode = parentNode;
			    newNode.Site = parentNode.Site;
			    newNode.CreateShortDescription();
			    newNode.Culture = parentNode.Culture;
			    newNode.Position = parentNode.ChildNodes.Count;
			    newNode.CopyRolesFromParent();
			    parentNode.ChildNodes.Add(newNode);
			    this._commonDao.SaveObject(newNode);
			    return newNode;
		    }

		    [Transaction(TransactionMode.RequiresNew)]
		    public void SetNodePermissions(Node node, int[] viewRoleIds, int[] editRoleIds, bool propagateToChildPages, bool propagateToChildSections)
		    {
			    node.NodePermissions.Clear();
			    IList<Role> viewRoles = this._commonDao.GetByIds<Role>(viewRoleIds);
			    foreach (Role role in viewRoles)
			    {
				    node.NodePermissions.Add(new NodePermission() { Node = node, Role = role, ViewAllowed = true });
			    }
			    IList<Role> editRoles = this._commonDao.GetByIds<Role>(editRoleIds);
			    foreach (Role role in editRoles)
			    {
				    if (viewRoles.Contains(role))
				    {
					    node.NodePermissions.OfType<NodePermission>().Single(np => np.Role == role).EditAllowed = true;
				    }
				    else
				    {
					    node.NodePermissions.Add(new NodePermission() { Node = node, Role = role, EditAllowed = true });
				    }
			    }
			    UpdateNode(node, propagateToChildPages, propagateToChildSections);
		    }
		#endregion

		private void PropagatePermissionsToChildNodes(Node parentNode, bool alsoPropagateToSections)
		{
			foreach (Node childNode in parentNode.ChildNodes)
			{
				childNode.NodePermissions.Clear();
				foreach (NodePermission pnp in parentNode.NodePermissions)
				{
					NodePermission childNodePermission = new NodePermission();
					childNodePermission.Node = childNode;
					childNodePermission.Role = pnp.Role;
					childNodePermission.ViewAllowed = pnp.ViewAllowed;
					childNodePermission.EditAllowed = pnp.EditAllowed;
					childNode.NodePermissions.Add(childNodePermission);
				}
				if (alsoPropagateToSections)
				{
					PropagatePermissionsToSections(childNode);
				}
				PropagatePermissionsToChildNodes(childNode, alsoPropagateToSections);
				this._commonDao.SaveOrUpdateObject(childNode);
			}
		}

		private void PropagatePermissionsToSections(Node node)
		{
			foreach (Section section in node.Sections)
			{
				section.SectionPermissions.Clear();
				foreach (NodePermission np in node.NodePermissions)
				{
					SectionPermission sp = new SectionPermission();
					sp.Section = section;
					sp.Role = np.Role;
					sp.ViewAllowed = np.ViewAllowed;
					sp.EditAllowed = np.EditAllowed;
					section.SectionPermissions.Add(sp);
				}
			}
			this._commonDao.SaveOrUpdateObject(node);
		}

		private void ReOrderNodePositions(IList<Node> nodes)
		{
			// Iterate the given collection of nodes and make sure there are no gaps between the positions. 
			for (int i = 0; i < nodes.Count; i++)
			{
				Node node = nodes[i];
				node.Position = i;
			}
		}

        //Custom
        public void ApplyTemplateAllNodesInSite(Cuyahoga.Core.Domain.Template template, Cuyahoga.Core.Domain.Site site)
        {
            this._siteStructureDao.ApplyTemplateAllNodesInSite(template, site);
        }
        
	}
}
