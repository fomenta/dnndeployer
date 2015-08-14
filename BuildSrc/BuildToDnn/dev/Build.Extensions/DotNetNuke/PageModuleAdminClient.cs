using Build.DotNetNuke.Deployer.Library;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Build.Extensions.DotNetNuke
{
    public class PageModuleAdminClient : AdminBaseClient
    {
        #region Constants
        public const string REST_PAGEMODULE_ADD = "pagemodule/add?clearExisting={clearExisting}";
        public const string REST_PAGEMODULE_GET = "pagemodule/get";
        public const string REST_PAGEMODULE_CLEAR = "pagemodule/clear";
        #endregion

        #region Constructor
        public PageModuleAdminClient() { }
        public PageModuleAdminClient(string targetDnnRootUrl, string userName, string password) : base(targetDnnRootUrl, userName, password) { }
        #endregion

        #region Modules on Page
        public int Add(PageModuleDto module, PageGetRequest page, bool clearExisting = true)
        {
            var requestParams = new PageModulesAddRequest { Module = module, Page = page, ClearExisting = clearExisting };

            var request = REST_CreateRequest(REST_PAGEMODULE_ADD, Method.PUT,
                    new Dictionary<string, string> { 
                        { "clearExisting", clearExisting.ToString() },
                    });
            request.RequestFormat = DataFormat.Json;
            request.AddBody(requestParams);

            var response = REST_Execute(request);

            try { return Int32.Parse(response.Content); }
            catch { throw new ArgumentException(string.Format("Unexpected response: '{0}'", response.Content)); }
        }

        public string Get(PageGetRequest page)
        {
            var request = REST_CreateRequest(REST_PAGEMODULE_GET, Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddObject(page);

            var response = REST_Execute(request);

            return response.Content;
        }

        public bool Clear(PageGetRequest page)
        {
            var request = REST_CreateRequest(REST_PAGEMODULE_CLEAR, Method.DELETE);

            request.RequestFormat = DataFormat.Json;
            request.AddBody(page);

            var response = REST_Execute(request);

            return (response.Content ?? "").ToUpper() == "TRUE";
        }
        #endregion
    }
}
