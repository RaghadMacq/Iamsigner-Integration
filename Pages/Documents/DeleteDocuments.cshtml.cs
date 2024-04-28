using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace Iamsigner_Integration.Pages.Documents
{
    public class DeleteDocumentsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public DeleteDocumentsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public void OnGet()
        {
            TempData.Keep("authToken");

        }
        public async Task<IActionResult> OnPostAsync(string masterDocID)
        {
            if (string.IsNullOrEmpty(masterDocID))
            {
                // Handle case when masterDocID is empty
                return RedirectToPage("/Error");
            }

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

                var url = "api/integrations/DeleteDocuments";

                // Create the request body
                var requestBody = new
                {
                    MasterDocID = masterDocID
                };

                // Serialize the request body to JSON
                var jsonRequest = JsonSerializer.Serialize(requestBody);

                // Create the request content with JSON data
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Send the DELETE request
                //var response = await client.DeleteAsync(url, content);
                var response = await client.PutAsync(url, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                TempData["DeleteMessage"] = jsonResponse.ToString();


                if (response.IsSuccessStatusCode)
                {
                    // Document deleted successfully
                    return Page();
                }
                else
                {
                    // Handle HTTP error response
                    // You can set an error message or redirect to an error page
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
