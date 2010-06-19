using System.Collections.Generic;

using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Search;


namespace Cuyahoga.Core.Service.Search
{
	public interface ISearchService
	{

		void AddContent(SearchContent searchContent);
		void AddContent(IList<SearchContent> searchContents);
        void AddContent(IContentItem contentItem);

		void UpdateContent(IContentItem contentItem);
        void UpdateContent(IList<SearchContent> searchContents);
        void UpdateContent(SearchContent searchContent);

		void DeleteContent(IContentItem contentItem);
        void DeleteContent(SearchContent searchContent);

		SearchResultCollection FindContent(string queryText, int pageIndex, int pageSize);
		SearchResultCollection FindContent(string queryText, IList<string> categoryNames, int pageIndex, int pageSize);

		/// <summary>
		/// Get information about the full-text index of the current site.
		/// </summary>
		/// <returns></returns>
		SearchIndexProperties GetIndexProperties();

		/// <summary>
		/// Rebuild the full-text index.
		/// </summary>
		/// <param name="searchableModules">A list of (legacy) searchable modules in the installation.</param>
		void RebuildIndex(IEnumerable<ISearchable> searchableModules);
	}
}
