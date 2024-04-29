using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text;
using Iamsigner_Integration.Models;
using System.Net.Http.Headers;

namespace Iamsigner_Integration.Pages
{
    public class UploadDocumentsModel : PageModel
    {
        
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public Models.Document Document { get; set; }
        [BindProperty]
        public IFormFile DocumentFile { get; set; } // Add property for file upload

        public UploadDocumentsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public void OnGet()
        {
            TempData.Keep("authToken");

        }
        public async Task<IActionResult> OnPostAsync()
        {
            var authToken = TempData["authToken"] as string;

            try
            {
                var client = _clientFactory.CreateClient("AuthAPI");
                if (DocumentFile != null && DocumentFile.Length > 0)
                {
                    Document.docFile = await ConvertFileToBase64String(DocumentFile);
                }
                var request = new UploadDocumentsRequest
                {
                    registerSignID = "8a123c7f-37e3-4570-9531-0dc98a014a1b",
                    PrimaryDocs = new List<Models.Document> { Document }
                };

                var jsonRequest = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                if (authToken != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                    var response = await client.PostAsync("api/integrations/UploadDocuments", content);
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                {
                    TempData["response"] = jsonResponse;
                    // Handle successful response
                    return RedirectToPage("Index");
                }
                else
                {
                    // Handle unsuccessful response
                    ModelState.AddModelError(string.Empty, "Failed to upload document.");
                    return Page();
                }
            }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to communicate with the server.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                ModelState.AddModelError(string.Empty, "An error occurred while processing the request.");
                return Page();
            }
        }
        private async Task<string> ConvertFileToBase64String(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }
}
