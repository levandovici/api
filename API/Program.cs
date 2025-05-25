using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API
{
    class Program
    {
        private static readonly HttpClient client;

        static Program()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    Console.WriteLine($"SSL Certificate: {cert.Subject}, Errors: {errors}");
                    return true; // Bypass for testing (remove in production)
                }
            };
            client = new HttpClient(handler);
        }

        static async Task Main(string[] args)
        {
            client.BaseAddress = new Uri("https://api.limonadoent.com/");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Test root endpoint
                await TestRootRequest();
                // Test GET endpoint
                await TestGetRequest();
                // Test POST endpoint
                await TestPostRequest();
            }
            catch (HttpRequestException ex) when (ex.InnerException is System.Security.Authentication.AuthenticationException)
            {
                Console.WriteLine($"SSL/TLS error: {ex.Message}. Check certificate or TLS version.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Dispose();
            }
        }

        static async Task TestRootRequest()
        {
            Console.WriteLine("Testing GET /index.php...");
            HttpResponseMessage response = await client.GetAsync("index.php");
            Console.WriteLine($"Status: {response.StatusCode}");
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {jsonResponse}");
        }

        static async Task TestGetRequest()
        {
            Console.WriteLine("\nTesting GET /api/get-data...");
            HttpResponseMessage response = await client.GetAsync("api/get-data");
            Console.WriteLine($"Status: {response.StatusCode}");
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {jsonResponse}");
        }

        static async Task TestPostRequest()
        {
            Console.WriteLine("\nTesting POST /api/post-data...");
            var requestData = new { name = "Test User", value = 456 };
            string jsonRequest = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("api/post-data", content);
            Console.WriteLine($"Status: {response.StatusCode}");
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {jsonResponse}");
        }
    }
}