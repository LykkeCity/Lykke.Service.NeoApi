using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Lykke.Service.NEO.Api.Services.Helpers
{
    public static class JsonRpcHelper
    {
        private static readonly string Url = "http://104.40.151.227:20332";
        public static JObject InvokeMethod(string a_sMethod, params object[] a_params)
        {
            //var ret = InvokeMethod("getblockhash", index);

            var webRequest = (HttpWebRequest)WebRequest.Create(Url);
            //webRequest.Credentials = Credentials;

            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";

            var joe = new JObject
            {
                ["jsonrpc"] = "1.0",
                ["id"] = "1",
                ["method"] = a_sMethod
            };

            if (a_params != null)
            {
                if (a_params.Length > 0)
                {
                    var props = new JArray();
                    foreach (var p in a_params)
                    {
                        props.Add(p);
                    }
                    joe.Add(new JProperty("params", props));
                }
            }

            var s = JsonConvert.SerializeObject(joe);
            // serialize json for the request
            var byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;

            try
            {
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            catch (WebException we)
            {
                //inner exception is socket
                //{"A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond 23.23.246.5:8332"}
                throw;
            }
            WebResponse webResponse = null;
            try
            {
                using (webResponse = webRequest.GetResponse())
                {
                    using (var str = webResponse.GetResponseStream())
                    {
                        using (var sr = new StreamReader(str))
                        {
                            return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                        }
                    }
                }
            }
            catch (WebException webex)
            {
                using (var str = webex.Response.GetResponseStream())
                {
                    using (var sr = new StreamReader(str))
                    {
                        var tempRet = JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                        return tempRet;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
