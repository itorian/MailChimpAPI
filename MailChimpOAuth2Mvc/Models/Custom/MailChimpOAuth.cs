using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace MailChimpOAuth2Mvc.Models.Custom
{
    /// <summary>
    /// This is what gets returned from MailChimp to be used for the API.
    /// </summary>
    public class McRestAPI
    {
        public string dc { get; set; }
        public string login_url { get; set; }
        public string api_endpoint { get; set; }
    }

    public class MailChimpOAuth : OAuth2
    {

        public MailChimpOAuth(Dictionary<string, string> p)
            : base(p)
        {

        }

        /// <summary>
        /// Gets the login URL based off cconfiguration settings.
        /// </summary>
        /// <returns>The login/Auth URL</returns>
        public string GetLoginURL()
        {
            var p = new Dictionary<string, string>() { { "response_type", "code" }, { "client_id", _variables["client_id"] }, { "redirect_uri", _variables["redirect_uri"] } };
            return getURI(_variables["authorize_uri"], p);
        }

        /// <summary>
        /// Gets the information needed to complete the 
        /// </summary>
        /// <returns>The MailChimp REST_API configuration</returns>
        public McRestAPI GetMetaData()
        {
            var data = api("metadata", new Dictionary<string, string>());
            var serializer = new JavaScriptSerializer();
            var mc = serializer.Deserialize<McRestAPI>(data);
            return mc;
        }
    }
}