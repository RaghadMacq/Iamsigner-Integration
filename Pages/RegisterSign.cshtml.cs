using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Iamsigner_Integration.Pages
{
    public class RegisterSignModel : PageModel
    {
        private readonly ILogger<RegisterSignModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public RegisterSignModel(ILogger<RegisterSignModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {
            TempData.Keep("authToken");
        }

        public async Task<IActionResult> OnPostAsync(string referenceNumber, string description)
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
                    ReferenceNumber = referenceNumber ?? "", // If null, set to empty string
                    Description = description ?? "" // If null, set to empty string
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
