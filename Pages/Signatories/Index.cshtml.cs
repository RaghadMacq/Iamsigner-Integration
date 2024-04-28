using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Iamsigner_Integration.Models;

namespace Iamsigner_Integration.Pages.Signatories
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<Signatory> Signatories { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //TempData.Keep("RegisterSignID");

            try
            {
                // Get the auth token from TempData
                if (!TempData.TryGetValue("authToken", out var authToken) || authToken == null)
                {
                    // Handle case when authToken is null or not found in TempData
                    return RedirectToPage("/Error");
                }

                // Get the RegisterSignID from TempData
                string registerSignID = "8a123c7f-37e3-4570-9531-0dc98a014a1b";
                

                var client = _clientFactory.CreateClient("AuthAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.ToString());

                var url = $"api/integrations/GetSignatories";
                if (!string.IsNullOrEmpty(registerSignID))
                {
                    url += $"?RegisterSignID={Uri.EscapeDataString(registerSignID)}";
                }

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var responseData = JsonSerializer.Deserialize<ResponseData>(responseBody);
                    if (responseData.response != null && responseData.response.Count > 0)
                    {
                        Signatories = responseData.response;
                    }
                }
                else
                {
                    // Handle HTTP error response
                    // You can set an error message or redirect to an error page
                    return RedirectToPage("/Error");
                }
                TempData.Keep("authToken");

                return Page();
            }
            catch (Exception)
            {
                // Log or handle the exception
                // You can set an error message or redirect to an error page
                return RedirectToPage("/Error");
            }
        }

        // Method to handle the delete operation
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            try
            {
                // Get the auth token from TempData
                if (!TempData.TryGetValue("authToken", out var authToken) || authToken == null)
                {
                    // Handle case when authToken is null or not found in TempData
                    return RedirectToPage("/Error");
                }

                var client = _clientFactory.CreateClient("AuthAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.ToString());

                var url = $"api/integrations/DeleteSignatory";

                // Create a JSON payload for the request body
                var requestBody = new
                {
                    SignatoriesID = id
                };

                // Serialize the request body to JSON
                var jsonRequest = JsonSerializer.Serialize(requestBody);

                // Create a request content with JSON data
                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                // Send the PUT request
                var response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Signatory deleted successfully, you may want to reload the page or update the signatory list
                    return RedirectToPage();
                }
                else
                {
                    // Handle HTTP error response
                    // You can set an error message or redirect to an error page
                    return RedirectToPage("/Error");
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
