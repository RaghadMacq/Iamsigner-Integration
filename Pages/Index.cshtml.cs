using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Iamsigner_Integration.Models;

namespace Iamsigner_Integration.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }


        public List<Template> Templates { get; set; }
        public string ErrorMessage { get; set; }


        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            // Create an instance of AuthModel and populate its properties
            var authModel = new AuthModel
            {
                SecretKey = "IWxYqzHYKWitDH4D866fxhDloG0Ixqlx",
                ApiKey = "7eYgGKlBXCKzxsvDwOlBMnddtqbNog-P"
            };

            // Serialize the information to be sent in the request body
            var jsonContent = new StringContent(JsonSerializer.Serialize(authModel),
                Encoding.UTF8,
                "application/json");

            // Create the HttpClient instance using the named client configured in Program.cs
            var httpClient = _httpClientFactory.CreateClient("AuthAPI");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TWFjcXVpcmVzLTg5QDg1OTg6UGFzczk4NUBAJiY=");

            // Execute the POST request and store the response
            using HttpResponseMessage response = await httpClient.PostAsync("api/integrations/GenerateIntegrationAccessToken", jsonContent);

            // Read the response content
            var responseString = await response.Content.ReadAsStringAsync();

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response to extract the token
                var responseObject = JsonSerializer.Deserialize<AuthResponse>(responseString);
                var token = responseObject?.response?.token;

                // Do something with the token, like storing it in TempData
                TempData["authToken"] = token;
                TempData.Keep("authToken");

                //TempData.Add("response", responseString);

                // Handle successful response
                TempData["success"] = "Integration access token generated successfully.";
                return Page(); // Redirect to a success page
            }
            else
            {
                // Handle unsuccessful response
                // For example, you can read the response status code
                var statusCode = (int)response.StatusCode;
                TempData["failure"] = $"Failed to generate integration access token. Status code: {statusCode}";
                return Page(); // Redirect to an error page
            }
        }

        public async Task<IActionResult> OnPostSecondForm()
        {
            try
            {
                // Get the auth token from TempData
                if (!TempData.TryGetValue("authToken", out var authToken) || authToken == null)
                {
                    // Handle case when authToken is null or not found in TempData
                    return RedirectToPage("/Error");
                }

                var client = _httpClientFactory.CreateClient("AuthAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.ToString());

                var url = "api/integrations/SendAndSign";

                // Create the request body
                var requestBody = new
                {
                    RegisterSignID = "8a123c7f-37e3-4570-9531-0dc98a014a1b"
                };

                // Serialize the request body to JSON
                var jsonRequest = JsonSerializer.Serialize(requestBody);

                // Create the request content with JSON data
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Send the POST request
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Request successful, you may want to redirect to a confirmation page or perform other actions
                    TempData["response"] = await response.Content.ReadAsStringAsync();
                    return Page();
                }
                else
                {
                    // Handle HTTP error response
                    // You can set an error message or redirect to an error page
                   // return RedirectToPage("/Error");
                    TempData["response"] = await response.Content.ReadAsStringAsync();
                    return Page();
                }
            }
            catch (Exception)
            {
                // Log or handle the exception
                // You can set an error message or redirect to an error page
                return RedirectToPage("/Error");
            }
        }
        public async Task<IActionResult> OnPostThirdForm()
        {
            try
            {
                // Get the auth token from TempData
                if (!TempData.TryGetValue("authToken", out var authToken) || authToken == null)
                {
                    // Handle case when authToken is null or not found in TempData
                    return RedirectToPage("/Error");
                }

                var client = _httpClientFactory.CreateClient("AuthAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.ToString());

                var url = "api/integrations/CreateRegisterSign";

                // Create the request body
                var requestBody = new
                {
                    ReferenceNumber = "123456",
                    Description = "dddddddd"
                };

                // Serialize the request body to JSON
                var jsonRequest = JsonSerializer.Serialize(requestBody);

                // Create the request content with JSON data
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Send the POST request
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Request successful, you may want to redirect to a confirmation page or perform other actions
                    TempData["response"] = await response.Content.ReadAsStringAsync();
                    return Page();
                }
                else
                {
                    // Handle HTTP error response
                    // You can set an error message or redirect to an error page
                    TempData["response"] = await response.Content.ReadAsStringAsync();
                    return Page();
                }
            }
            catch (Exception)
            {
                // Log or handle the exception
                // You can set an error message or redirect to an error page
                return RedirectToPage("/Error");
            }
        }

    }
}
