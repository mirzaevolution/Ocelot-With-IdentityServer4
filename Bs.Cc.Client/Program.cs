using System;
using IdentityModel;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Bs.Cc.Client
{
    class Program
    {
        static async Task CallApi()
        {
            using(HttpClient client = new HttpClient())
            {
                var config = await client.GetDiscoveryDocumentAsync("https://localhost:44312");
                var token = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = config.TokenEndpoint,
                    ClientId = "bs.cc.api",
                    ClientSecret = "default",
                    Scope = "weather_api.read"

                });
                if (token.IsError)
                {
                    Console.WriteLine(token.Error);
                    return;
                }
                client.SetBearerToken(token.AccessToken);
                var response = await client.GetAsync("https://localhost:44313/WeatherForecast");
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonContent);
                }
                else
                {
                    Console.WriteLine("An error occured");
                    Console.WriteLine(response.StatusCode);
                }
                
            }    
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to send the request...");
            Console.ReadLine();
            CallApi().Wait();
            Console.ReadLine();
        }
    }
}
