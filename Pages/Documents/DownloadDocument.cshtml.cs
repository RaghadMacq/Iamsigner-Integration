using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Iamsigner_Integration.Pages.Documents
{
    public class DownloadDocumentModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public DownloadDocumentModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnPostAsync(string masterDocID, bool isDownloadAuditTrail)
        {
            try
            {
                // Get the integration auth token from TempData
                if (!TempData.TryGetValue("authToken", out var authToken) || authToken == null)
                {
                    return RedirectToPage("/Error");
                }

                var client = _clientFactory.CreateClient("AuthAPI");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken.ToString());

                // Construct the URL with query parameters
                var url = $"api/integrations/DownloadDocument?MasterDocID={masterDocID}&IsDownloadAuditTrail={isDownloadAuditTrail}";

                // Send the GET request
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // If the response is successful, return the file content
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    TempData["DownloadDocument"] = jsonResponse.ToString();
                    var fileContent = await response.Content.ReadAsByteArrayAsync();
                    var contentType = response.Content.Headers.ContentType.ToString();
                    var fileName = $"DownloadedDocument_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.pdf"; // Adjust the file name and extension based on the response
                    return File(fileContent, contentType, fileName);
                }
                else
                {
                    // If the response is not successful, handle the error
                    var errorMessage = "An error occurred while downloading the document. Please try again.";
                    ViewData["ErrorMessage"] = errorMessage;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                var errorMessage = "An error occurred while processing your request. Please try again later.";
                ViewData["ErrorMessage"] = errorMessage;
                return Page();
            }
        }
    }
}
