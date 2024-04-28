using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Iamsigner_Integration.Pages.Templates
{
    public class AddTemplateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public AddTemplateModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public string TemplateID { get; set; }
        [BindProperty]
        public string RegisterSignID { get; set; }
        [BindProperty]
        public string MasterDocID { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public IActionResult OnGet()
        {
            TempData.Keep("authToken");

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Get the integration auth token from TempData
                if (!TempData.TryGetValue("authToken", out var authToken) || authToken == null)
                {
                    return RedirectToPage("/Error");
                }

                var client = _clientFactory.CreateClient("AuthAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.ToString());

                var url = "api/integrations/AddTemplate";

                // Create the request body
                var requestBody = new
                {
                    TemplateID = TemplateID,
                    RegisterSignID = RegisterSignID,
                    MasterDocID = MasterDocID
                };

                // Serialize the request body to JSON
                var jsonRequest = JsonSerializer.Serialize(requestBody);

                // Create the request content with JSON data
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Send the PUT request
                var response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    //SuccessMessage = "Template added successfully.";
                    SuccessMessage = await response.Content.ReadAsStringAsync(); 

                }
                else
                {
                    ErrorMessage = "Failed to add template. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while processing your request. Please try again later.";
            }

            return Page();
        }
    }
}
