using RestSharp;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApiClient
{
    class Program
    {

        static void Main(string[] args)
        {
            Thread.Sleep(20000);
            var client = new RestClient("http://host.docker.internal");
            var request = new RestRequest("/home/getfiles", Method.GET);
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
    }
}
