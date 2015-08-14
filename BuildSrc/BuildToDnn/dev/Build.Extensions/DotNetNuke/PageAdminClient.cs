using Build.DotNetNuke.Deployer.Library;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace Build.Extensions.DotNetNuke
{
    public class PageAdminClient : AdminBaseClient
    {
        #region Constants
        public const string REST_PAGE_PORTALS = "page/portals";
        public const string REST_PAGE_PORTAL = "page/portal?id={id}&alias={alias}";
        public const string REST_PAGE_PORTALID = "page/portalID?alias={alias}";
        //
        private const string PAGE_GET_QUERYSTRING = "tabPath={tabPath}&tabFullName={tabFullName}&portalAlias={portalAlias}&id={id}&portalID={portalID}";
        //
        public const string REST_PAGE_GET = "page/get";
        public const string REST_PAGE_ADD = "page/add";
        public const string REST_PAGE_DELETE = "page/delete?deleteDescendants={deleteDescendants}";
        public const string REST_PAGE_LIST = "page/list?portalID={portalID}&portalAlias={portalAlias}";
        public const string REST_PAGE_LIST_TO_DICTIONARY = "page/listtodictionary?portalID={portalID}&portalAlias={portalAlias}";
        public const string REST_PAGE_IMPORT = "page/import?portalID={portalID}&portalAlias={portalAlias}&parentPagePath={parentPagePath}&beforePagePath={beforePagePath}&afterPagePath={afterPagePath}&parentPageFullName={parentPageFullName}&beforePageFullName={beforePageFullName}&afterPageFullName={afterPageFullName}";
        #endregion

        #region Constructor
        public PageAdminClient() { }
        public PageAdminClient(string targetDnnRootUrl, string userName, string password) : base(targetDnnRootUrl, userName, password) { }
        #endregion

        #region REST for Module
        public string Portals()
        {
            var response = REST_Execute(REST_PAGE_PORTALS);
            return response.Content;
        }

        public string Portal(string alias = "", int? id = null)
        {
            var response = REST_Execute(REST_PAGE_PORTAL, new Dictionary<string, string> { { "alias", alias }, { "id", id.ToString() } });
            return response.Content;
        }

        public int PortalId(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            { throw new ArgumentNullException("alias", "Alias cannot be empty."); }

            var response = REST_Execute(REST_PAGE_PORTALID, new Dictionary<string, string> { { "alias", alias } });

            try { return Int32.Parse(response.Content); }
            catch { throw new ArgumentException(string.Format("Unexpected response: '{0}'", response.Content)); }
        }

        public string PageGet(PageGetRequest page)
        {
            var request = REST_CreateRequest(REST_PAGE_GET);
            request.RequestFormat = DataFormat.Json;
            request.AddObject(page);

            var response = REST_Execute(request);
            return response.Content;
        }

        public int PageAdd(PageInfoDto page)
        {

            var request = REST_CreateRequest(REST_PAGE_ADD, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(page);

            var response = REST_Execute(request);

            try { return Int32.Parse(response.Content); }
            catch { throw new ArgumentException(string.Format("Unexpected response: '{0}'", response.Content)); }
        }

        public bool PageDelete(PageGetRequest page, bool deleteDescendants = false)
        {
            var request = REST_CreateRequest(REST_PAGE_DELETE, Method.DELETE,
                    new Dictionary<string, string> { 
                        { "deleteDescendants", deleteDescendants.ToString() } 
                    });

            request.RequestFormat = DataFormat.Json;
            request.AddBody(page);

            var response = REST_Execute(request);
            return (response.Content ?? "").ToUpper() == "TRUE";
        }

        public string PageList(string portalAlias = "", int? portalID = null)
        {
            var response = REST_Execute(REST_PAGE_LIST, new Dictionary<string, string> { { "portalAlias", portalAlias }, { "portalID", portalID.ToString() } });
            return response.Content;
        }

        public Dictionary<string, int> PageListToDictionary(string portalAlias = "", int? portalID = null)
        {
            return REST_Execute<Dictionary<string, int>>(REST_PAGE_LIST_TO_DICTIONARY, new Dictionary<string, string> { { "portalAlias", portalAlias }, { "portalID", portalID.ToString() } });
        }

        public int PageImport(string pageTemplateFilePath, string portalAlias = null, int? portalID = null,
                              string parentPagePath = null, string beforePagePath = null, string afterPagePath = null,
                              string parentPageFullName = null, string beforePageFullName = null, string afterPageFullName = null)
        {
            var request = REST_CreateRequest(REST_PAGE_IMPORT, Method.PUT,
                new Dictionary<string, string> {
                    { "portalAlias", portalAlias }, { "portalID", portalID.ToString() },
                    { "parentPagePath", parentPagePath },
                    { "beforePagePath", beforePagePath }, 
                    { "afterPagePath", afterPagePath },
                    { "parentPageFullName", parentPageFullName },
                    { "beforePageFullName", beforePageFullName },
                    { "afterPageFullName", afterPageFullName },
                });
            // add files to upload
            request.AddFile(Path.GetFileName(pageTemplateFilePath), pageTemplateFilePath);

            // execute the request
            var response = REST_Execute(request);

            try { return Int32.Parse(response.Content); }
            catch { throw new ArgumentException(string.Format("Unexpected response: '{0}'", response.Content)); }
        }
        #endregion
    }
}
