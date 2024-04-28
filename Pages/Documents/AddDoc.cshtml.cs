using Iamsigner_Integration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Iamsigner_Integration.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Iamsigner_Integration.Pages.Documents
{
    public class AddDocModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public AddDocModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public SupportingDocument Document { get; set; }
        [BindProperty]
        public IFormFile SupportingDocFile { get; set; } // Add property for file upload

        public IActionResult OnGet()
        {
            TempData.Keep("authToken");
            TempData.Keep("RegisterSignID");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            TempData.Keep("authToken");
            TempData.Keep("RegisterSignID");
            var authToken = TempData["authToken"] as string;

            try
            {
                var client = _clientFactory.CreateClient("AuthAPI");

                // Handle file upload and convert file content to Base64 string
                if (SupportingDocFile != null && SupportingDocFile.Length > 0)
                {
                    Document.SupportingDocFile = await ConvertFileToBase64String(SupportingDocFile);
                }

                var requestData = new
                {
                    RegisterSignID = "8a123c7f-37e3-4570-9531-0dc98a014a1b",
                    SupportingDocs = new List<SupportingDocument> { Document }
                };

                var jsonRequest = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                if (authToken != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                    var response = await client.PostAsync("api/integrations/AddSupportingDocs", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonSerializer.Deserialize<ResponseData>(jsonResponse);

                        if (responseObject != null && responseObject.successStatus)
                        {
                            //TempData["SuccessMessage"] = "Document added successfully!";
                            TempData["SuccessMessage"] = jsonResponse;

                            return RedirectToPage("./SupportingDocuments");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, responseObject?.message ?? "Failed to add supporting documents.");
                            return Page();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to communicate with the server.");
                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Authorization token not found.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing the request.");
                return Page();
            }
        }

        // Helper method to convert file content to Base64 string
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
