using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace XServerAspProxy
{
    public class HttpClientPool
    {
        private static Dictionary<string, HttpClient> clients = new Dictionary<string, HttpClient>();

        public static HttpClient GetHttpClient(string host)
        {
            lock (locker)
            {
                if (clients.ContainsKey(host))
                    return clients[host];

                var client = new HttpClient
                {
                    BaseAddress = new Uri(host),
                };

                clients[host] = client;

                return client;
            }
        }

        private static readonly object locker = new object();
    }
}