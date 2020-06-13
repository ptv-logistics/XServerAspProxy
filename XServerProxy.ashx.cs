//--------------------------------------------------------------
// Copyright (c) 2011 PTV Planung Transport Verkehr AG
//--------------------------------------------------------------

using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    public class XServerProxy : HttpTaskAsyncHandler
    {
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            string type = Regex.Match(context.Request.RawUrl, "(?<=/XServerProxy/).*?(?=/)").ToString();
            string request = Regex.Match(context.Request.RawUrl, "(?<=/XServerProxy/).*").ToString();

            // Case 1: xServer internet
            var token = "<your xserver-internet token>";
            var host = BuildXServerInternetHost(type, request); // case xserver internet

            // Case 2: your on-premise xServer
            //var host = BuildXServerOnPremiseHost(type, request);

            // Case 3: our internal demo server, dont't use for production!
            // var host = "http://176.95.37.53/";

            var original = context.Request;

            var backendRequest = new HttpRequestMessage();

            // in case of xserver internet or xserver /w basic auth inject your user/pwd or token
            if (type == "WMS")
                request = request + "&xtok=" + token; // wms requests need the token as parameter
            else
                backendRequest.Headers.TryAddWithoutValidation("Authorization", "Basic "
                    + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("xtok:" + token)));

            var client = HttpClientPool.GetHttpClient(host);
            backendRequest.RequestUri = new Uri(request, UriKind.Relative);

            backendRequest.Method = (original.HttpMethod == "POST") ? HttpMethod.Post : HttpMethod.Get;
            if(!string.IsNullOrEmpty(original.ContentType))
                backendRequest.Headers.TryAddWithoutValidation("Content-Type", original.ContentType);

            // copy the data for post
            if (backendRequest.Method == HttpMethod.Post)
                backendRequest.Content = new StreamContent(original.InputStream);

            // make the request
            var response = await client.SendAsync(backendRequest);
            if (response.IsSuccessStatusCode)
            {
                if (response.Content.Headers.ContentType != null)
                    context.Response.ContentType = response.Content.Headers.ContentType.ToString();

                await response.Content.CopyToAsync(context.Response.OutputStream);
            }

            context.Response.StatusCode = (int)response.StatusCode;
        }

        public string BuildXServerInternetHost(string type, string request)
        {
            string cluster = "eu-n-test";

            if (type.ToLower() == "wms")
            {
                type = "xmap";
                request = request + "&xtok=" + "<insert your token>"; // wms requests need the token as parameter
            }

            return string.Format("https://{0}-{1}.cloud.ptvgroup.com/", type, cluster);
        }

        public string BuildXServerOnPremiseHost(string type, string request)
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

            return string.Format("http://{0}:{1}/{2}", ip, port);
        }
    }
}
