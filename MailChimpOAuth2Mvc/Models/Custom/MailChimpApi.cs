using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;

namespace MailChimpOAuth2Mvc.Models.Custom
{
    public class MailChimpApi
    {
        /// <summary>
        /// If listId is empty, all lists will be retrieved.
        /// </summary>
        public static string GetLists(string apiKey, string listId = "")
        {
            string dataCenter = GetDataCenter(apiKey);

            var uri = string.Format("https://{0}.api.mailchimp.com/3.0/lists/{1}", dataCenter, listId);

            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("Accept", "application/json");
                    webClient.Headers.Add("Authorization", "apikey " + apiKey);

                    return webClient.DownloadString(uri);
                }
            }
            catch (WebException we)
            {
                using (var sr = new StreamReader(we.Response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static string CreateList(string apiKey, string sampleList)
        {
            string dataCenter = GetDataCenter(apiKey);

            var uri = string.Format("https://{0}.api.mailchimp.com/3.0/lists", dataCenter);

            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("Accept", "application/json");
                    webClient.Headers.Add("Authorization", "apikey " + apiKey);

                    return webClient.UploadString(uri, "POST", sampleList);
                }
            }
            catch (WebException we)
            {
                using (var sr = new StreamReader(we.Response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// If subscriberHash is empty, all list members within this list will be retrieved.
        /// </summary>
        public static string GetListMember(string apiKey, string listId, string subscriberEmail = "")
        {
            string dataCenter = GetDataCenter(apiKey);

            var hashedEmailAddress = string.IsNullOrEmpty(subscriberEmail) ? "" : CalculateMD5Hash(subscriberEmail.ToLower());
            var uri = string.Format("https://{0}.api.mailchimp.com/3.0/lists/{1}/members/{2}", dataCenter, listId, hashedEmailAddress);
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("Accept", "application/json");
                    webClient.Headers.Add("Authorization", "apikey " + apiKey);

                    return webClient.DownloadString(uri);
                }
            }
            catch (WebException we)
            {
                using (var sr = new StreamReader(we.Response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static string AddOrUpdateListMember(string apiKey, string listId, string subscriberEmail, string fname, string lname, bool issubscribed)
        {
            string dataCenter = GetDataCenter(apiKey);

            var sampleListMember = JsonConvert.SerializeObject(
                new
                {
                    email_address = subscriberEmail,
                    merge_fields =
                    new
                    {
                        FNAME = fname,
                        LNAME = lname
                    },
                    status_if_new = issubscribed == true ? "subscribed" : "unsubscribed"
                });

            var hashedEmailAddress = string.IsNullOrEmpty(subscriberEmail) ? "" : CalculateMD5Hash(subscriberEmail.ToLower());
            var uri = string.Format("https://{0}.api.mailchimp.com/3.0/lists/{1}/members/{2}", dataCenter, listId, hashedEmailAddress);
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("Accept", "application/json");
                    webClient.Headers.Add("Authorization", "apikey " + apiKey);

                    return webClient.UploadString(uri, "PUT", sampleListMember);
                }
            }
            catch (WebException we)
            {
                using (var sr = new StreamReader(we.Response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private static string GetDataCenter(string apiKey)
        {
            return apiKey.Substring(apiKey.LastIndexOf('-') + 1);
        }

        private static string CalculateMD5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input.
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // Step 2, convert byte array to hex string.
            var sb = new StringBuilder();
            foreach (var @byte in hash)
            {
                sb.Append(@byte.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}