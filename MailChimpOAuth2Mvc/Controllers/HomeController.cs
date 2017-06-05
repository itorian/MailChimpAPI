using MailChimpOAuth2Mvc.Models.Custom;
using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace MailChimpOAuth2Mvc.Controllers
{
    public class HomeController : Controller
    {
        // Get this from here http://developer.mailchimp.com/documentation/mailchimp/guides/how-to-use-oauth2/
        Dictionary<string, string> _configuration = new Dictionary<string, string>(){
        {"client_id", "xxxxx"},
        {"client_secret", "xxxxx"},
        {"redirect_uri", "https://mailchimpapi.azurewebsites.net/home/redirect"},
        {"authorize_uri", "https://login.mailchimp.com/oauth2/authorize"},
        {"access_token_uri", "https://login.mailchimp.com/oauth2/token"},
        {"base_uri", "https://login.mailchimp.com/oauth2/"}};

        public ActionResult Index()
        {
            MailChimpOAuth auth = new MailChimpOAuth(_configuration);
            return View(auth);
        }

        public ActionResult Redirect()
        {
            // check code query string
            if (string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                RedirectToAction("index");
            }
            _configuration.Add("code", Request.QueryString["code"]);

            MailChimpOAuth auth = new MailChimpOAuth(_configuration);
            var session = auth.getSession();
            var rest_info = (auth.GetMetaData());
            var api_key = session + "-" + rest_info.dc;

            //TODO: Save api_key in database








            // create default lists
            var sampleList1 = JsonConvert.SerializeObject(
                new
                {
                    name = "ProductsAddedInCartList",
                    contact = new
                    {
                        company = "MailChimp",
                        address1 = "675 Ponce De Leon Ave NE",
                        address2 = "Suite 5000",
                        city = "zip",
                        state = "GA",
                        zip = "30308",
                        country = "US",
                        phone = ""
                    },
                    permission_reminder = "You'\''re receiving this email because you signed up for updates about Freddie'\''s newest hats.",
                    campaign_defaults = new
                    {
                        from_name = "Tenant Name",
                        from_email = "tenantemail@knorish.com", //set default user email
                        subject = "Tenant Name Demo",
                        language = "en",
                    },
                    email_type_option = true
                });

            var sampleList2 = JsonConvert.SerializeObject(
                new
                {
                    name = "ProductDeletedFromCartList",
                    contact = new
                    {
                        company = "Tenant Name",
                        address1 = "675 Ponce De Leon Ave NE",
                        address2 = "Suite 5000",
                        city = "zip",
                        state = "GA",
                        zip = "30308",
                        country = "US",
                        phone = ""
                    },
                    permission_reminder = "You'\''re receiving this email because you signed up for updates about Freddie'\''s newest hats.",
                    campaign_defaults = new
                    {
                        from_name = "Tenant Name",
                        from_email = "tenantemail@knorish.com", //set default user email
                        subject = "Tenant Name Demo",
                        language = "en",
                    },
                    email_type_option = true
                });

            MailChimpApi.CreateList(api_key, sampleList1);
            MailChimpApi.CreateList(api_key, sampleList2);



            

            //MailChimpApi.AddOrUpdateListMember("us16", api_key, "","subscriberemail@knorish.com", "first name", "last name", true);






            return RedirectToAction("list");
        }




        public string List()
        {
            MailChimpOAuth auth = new MailChimpOAuth(_configuration);
            var session = auth.getSession();
            var rest_info = (auth.GetMetaData());
            var api_key = session + "-" + rest_info.dc;

            var list = MailChimpApi.GetLists(api_key);

            return list;
        }
    }
}