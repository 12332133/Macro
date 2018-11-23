using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public static class BitMexApiHelper
    {
        private static string BuildQueryData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            var b = new StringBuilder();
            foreach (var item in param)
                b.Append(string.Format("&{0}={1}", item.Key, WebUtility.UrlEncode(item.Value)));

            try
            {
                return b.ToString().Substring(1);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string BuildJSON(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            var entries = new List<string>();
            foreach (var item in param)
                entries.Add(string.Format("\"{0}\":\"{1}\"", item.Key, item.Value));

            return "{" + string.Join(",", entries) + "}";
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private static long GetNonce()
        {
            var yearBegin = new DateTime(1990, 1, 1);
            return DateTime.UtcNow.Ticks - yearBegin.Ticks;
        }

        private static long GetExpires()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600; // set expires one hour in the future
        }

        private static string Query(string domain, string apiSecret, string apiKey, 
                        string method, string function, Dictionary<string, string> param = null, 
                        bool auth = false, bool json = false)
        {
            string paramData = json ? BuildJSON(param) : BuildQueryData(param);
            string url = "/api/v1" + function + ((method == "GET" && paramData != "") ? "?" + paramData : "");
            string postData = (method != "GET") ? paramData : "";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(domain + url);
            webRequest.Method = method;
            webRequest.AllowAutoRedirect = false;
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Proxy = null;
            webRequest.CookieContainer = null;

            if (auth)
            {
                string expires = GetExpires().ToString();
                string message = method + url + expires + postData;
                byte[] signatureBytes = BitMexApiHelper.hmacsha256(Encoding.UTF8.GetBytes(apiSecret), Encoding.UTF8.GetBytes(message));
                string signatureString = ByteArrayToString(signatureBytes);

                webRequest.Headers.Add("api-expires", expires);
                webRequest.Headers.Add("api-key", apiKey);
                webRequest.Headers.Add("api-signature", signatureString);
            }

            try
            {
                if (postData != "")
                {
                    webRequest.ContentType = json ? "application/json" : "application/x-www-form-urlencoded";
                    var data = Encoding.UTF8.GetBytes(postData);
                    using (var stream = webRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (WebResponse webResponse = webRequest.GetResponse())
                using (Stream str = webResponse.GetResponseStream())
                using (StreamReader sr = new StreamReader(str))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    if (response == null)
                        throw;

                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
        
        private static byte[] hmacsha256(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA256(keyByte))
            {
                return hash.ComputeHash(messageBytes);
            }
        }

        //#region RateLimiter

        //private static long lastTicks = 0;
        //private static object thisLock = new object();

        //private static void RateLimit()
        //{
        //    lock (thisLock)
        //    {
        //        long elapsedTicks = DateTime.Now.Ticks - lastTicks;
        //        var timespan = new TimeSpan(elapsedTicks);
        //        if (timespan.TotalMilliseconds < rateLimit)
        //            Thread.Sleep(rateLimit - (int)timespan.TotalMilliseconds);
        //        lastTicks = DateTime.Now.Ticks;
        //    }
        //}

        //#endregion RateLimiter

        public static string GetAccount(BitMexSession session, string domain)
        {
            return BitMexApiHelper.Query(domain, session.ApiSecret, session.ApiKey, "GET", "/user", null, true);
        }

        public static string GetReferral(BitMexSession session, string domain)
        {
            return BitMexApiHelper.Query(domain, session.ApiSecret, session.ApiKey, "GET", "/user/affiliateStatus", null, true);
        }

        public static string GetActiveInstruments(string domain)
        {
            return BitMexApiHelper.Query(domain, "", "", "GET", "/instrument/active", null, false);
        }
    }
}
