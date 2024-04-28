using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Iamsigner_Integration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Iamsigner_Integration.Pages.Templates
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<Template> Templates { get; set; }
        public string ErrorMessage { get; set; }
        public IActionResult OnGet()
        {
            TempData.Keep("authToken");

            return Page();
        }
        public async Task<IActionResult> OnPostGetTemplatesAsync()
        {
            TempData.Keep("authToken");

            try
            {
                // Get the integration auth token from TempData
                if (!TempData.TryGetValue("authToken", out var authToken) || authToken == null)
                {
                    return RedirectToPage("/Error");
                }

                var client = _clientFactory.CreateClient("AuthAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.ToString());

                var url = "api/integrations/GetTemplates";

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var responseData = JsonSerializer.Deserialize<ResponseData>(responseBody);
                    if (responseData.response != null && responseData.response.Any())
                    {
                        // Deserialize templates if response is not empty
                        Templates = JsonSerializer.Deserialize<List<Template>>(responseData.response.ToString());
                    }
                    else
                    {
                        // Handle case when no templates are available
                        // For example, display a message to the user
                        TempData["NoTemplatesMessage"] = "No templates available.";
                    }
                    //Templates = JsonSerializer.Deserialize<List<Template>>(responseBody);
                    TempData["responseBody"]= responseBody;
                    return Page();
                }
                else
                {
                    ErrorMessage = "Failed to fetch templates. Please try again later.";
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
