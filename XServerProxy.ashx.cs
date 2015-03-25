//--------------------------------------------------------------
// Copyright (c) 2011 PTV Planung Transport Verkehr AG
//--------------------------------------------------------------

using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;

namespace XServerAspProxy
{
    /// <summary>
    /// This class just relays an xServer request to another url.
    /// This can be useful to access an xMapServer behind a firewall, allow cross-domain and cross-scheme calls,
    /// inject your credentials, add some load-balancing, fail-over, etc. The Web-client can still use xServer 
    /// the same way as for a peer-to-peer connection.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class XServerProxy : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string type = Regex.Match(context.Request.RawUrl, "(?<=/XServerProxy/).*?(?=/)").ToString();
            string request = Regex.Match(context.Request.RawUrl, "(?<=/XServerProxy/).*").ToString();

            // Case 1: xServer internet
            // var url = BuildXServerInternetRequestUrl(type, request); // case xserver internet

            // Case 2: your on-premise xServer
            //string url = BuildXServerOnPremiseRequest(type, request);

            // Case 3: our internal demo server, dont't use for production!
            string url = "http://80.146.239.180/" + request;

            var original = context.Request;
            var newRequest = (HttpWebRequest)WebRequest.Create(url);
            
            // in case of xserver internet or xserver /w basic auth inject your user/pwd or token
            // newRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("xtok:" + "<your token here>"));

            newRequest.ContentType = original.ContentType;
            newRequest.Method = original.HttpMethod;

            // copy the data for post
            if(original.HttpMethod == "POST")
                original.InputStream.CopyTo(newRequest.GetRequestStream());

            // make the request
            var newResponse = newRequest.GetResponse();

            // copy the response data
            context.Response.ContentType = newResponse.ContentType;
            newResponse.GetResponseStream().CopyTo(context.Response.OutputStream);
        }

        public string BuildXServerInternetRequestUrl(string type, string request)
        {
            string cluster = "eu-n-test";
            
            if (type.ToLower() == "wms")
            {
                type = "xmap";
                request = request + "&xtok=" + "<insert your token>"; // wms requests need the token as parameter
            }

            return string.Format("https://{0}-{1}.cloud.ptvgroup.com/{2}", type, cluster, request);
        }

        public string BuildXServerOnPremiseRequestUrl(string type, string request)
        {
            string ip = "127.0.0.1";
            int port;
            switch (type.ToLower())
            {
                case "xlocate":
                    port = 50020;
                    break;
                case "xroute":
                    port = 50030;
                    break;
                default: // xmap and ajaxmaps/wms
                    port = 50010;
                    break;
            }

            return string.Format("http://{0}:{1}/{2}", ip, port, request);
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
