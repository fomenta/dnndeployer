using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Net;

namespace Build.DotNetNuke.Deployer.Client
{
    public class AdminBaseClient
    {
        #region Constants
        public const int REST_TIMEOUT_MSEC = 15 * 60 * 1000; // 15 minutes
        public const string DEPLOYER_BASE_URL_STRINGFORMAT = "{0}/DesktopModules/Deployer/API";
        #endregion

        #region Constructor
        public AdminBaseClient()
        {
            // [2015-10-26] PE: to ignore errors in SSL certificates
            // The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.
            ServicePointManager.ServerCertificateValidationCallback +=
                                            (sender, certificate, chain, sslPolicyErrors) => true;
        }

        public AdminBaseClient(string targetDnnRootUrl, string userName, string password) : base()
        {
            SetupDnn(targetDnnRootUrl, userName, password);
        }

        public void SetupDnn(string targetDnnRootUrl, string userName, string password)
        {
            if (targetDnnRootUrl.EndsWith("/")) { targetDnnRootUrl = targetDnnRootUrl.Substring(0, targetDnnRootUrl.Length - 1); }
            this.TargetDotNetNukeRootUrl = targetDnnRootUrl;
            this.UserName = userName;
            this.Password = password;
        }
        #endregion

        #region Property
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ServiceUrl { get { return string.Format(DEPLOYER_BASE_URL_STRINGFORMAT, TargetDotNetNukeRootUrl); } }
        public string TargetDotNetNukeRootUrl { get; set; }
        public string LastRequestUrl { get; protected set; }
        public IRestResponse LastResponse { get; protected set; }

        protected RestClient restClient;
        #endregion

        #region Methods
        public bool IsDeployerInstalled()
        {
            try { GetVersion(); }
            catch { }
            return LastResponse.StatusCode == HttpStatusCode.OK;
        }

        public string GetVersion()
        {
            var response = REST_Execute("/");
            return (string)SimpleJson.DeserializeObject(response.Content);
        }
        #endregion

        #region Protected IRestResponse
        protected RestRequest REST_CreateRequest(string actionUrl, Method method = Method.GET, Dictionary<string, string> args = null)
        {
            this.restClient = new RestClient(ServiceUrl);
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                this.restClient.Authenticator = new HttpBasicAuthenticator(UserName, Password);
                this.restClient.CookieContainer = new CookieContainer();
            }
            var request = new RestRequest(actionUrl, method);
            request.Timeout = REST_TIMEOUT_MSEC;
            if (args != null)
            {
                // NOTE: bug on RestSharp 104.4.0.0 does not accept null values for parameters
                foreach (var arg in args) { request.AddUrlSegment(arg.Key, arg.Value ?? ""); }
            }
            return request;
        }


        protected IRestResponse REST_Execute(string actionUrl, Dictionary<string, string> args = null)
        { return REST_Execute(actionUrl, Method.GET, args); }

        protected IRestResponse REST_Execute(string actionUrl, Method method, Dictionary<string, string> args = null)
        {
            var request = REST_CreateRequest(actionUrl, method, args);
            var response = REST_Execute(request);
            return response;
        }

        protected IRestResponse REST_Execute(RestRequest request)
        {
            var response = this.restClient.Execute(request);

            LastRequestUrl = string.Format("{0}/{1}", ServiceUrl, request.Resource);
            LastResponse = response;
            CheckAndDisplayError(response);

            return response;
        }
        #endregion

        #region Protected Generics
        protected T REST_Execute<T>(string actionUrl, Dictionary<string, string> args = null) where T : new()
        { return REST_Execute<T>(actionUrl, Method.GET, args); }

        protected T REST_Execute<T>(string actionUrl, Method method, Dictionary<string, string> args = null) where T : new()
        {
            var request = REST_CreateRequest(actionUrl, method, args);
            return REST_Execute<T>(request);
        }

        protected T REST_Execute<T>(RestRequest request) where T : new()
        {
            var response = this.restClient.Execute<T>(request);

            LastRequestUrl = string.Format("{0}/{1}", ServiceUrl, request.Resource);
            LastResponse = response;
            CheckAndDisplayError(response);

            return response.Data;
        }
        #endregion

        #region Check for errors
        protected void CheckAndDisplayError(IRestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK) { return; }

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw new Exception(string.Format("{0}: {1}", response.StatusCode, response.ErrorMessage));
            }
            else if (response.ErrorException != null)
            { throw response.ErrorException; }

            string errorMessage;
            if (response.Content != null && response.Content.Contains("ExceptionMessage"))
            {
                try
                {
                    var dictJson = new JsonDeserializer().Deserialize<Dictionary<string, string>>(response);
                    errorMessage = dictJson["ExceptionMessage"];
                }
                catch { errorMessage = FormatJSON(response.Content); }
            }
            else { errorMessage = FormatJSON(response.Content); }

            throw new Exception(string.Format("{0}: {1}", response.StatusCode, errorMessage));
        }
        #endregion

        #region Json Formatter
        public static string FormatJSON(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) { return json; }
            if (json == "True" || json == "False") { return json; }

            try
            { return JToken.Parse(json).ToString(); }
            catch (JsonReaderException)
            { return json; }
        }
        #endregion
    }
}
