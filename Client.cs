using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WpfDead
{
    public class Client
    {
        private static HttpClient httpClient;

        public static HttpClient HttpClient 
        {
            get
            {
                if (httpClient == null)
                {
                    httpClient = new HttpClient()
                    {
                        BaseAddress = new Uri("https://localhost:7012/api/")
                    }; 
                }
                return httpClient;
            }
        }
        public static void SetToken(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
