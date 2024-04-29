using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Iamsigner_Integration.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace Iamsigner_Integration.Pages.Documents
{
    public class ListModel : PageModel
    {
        public DocumentStatus Document { get; set; }
        public List<SupportingDocument> Documents { get; set; }

        private readonly ILogger<ListModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public ListModel(ILogger<ListModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            TempData.Keep("authToken");
            TempData.Keep("RegisterSignID");
            var RegisterSignID = "8a123c7f-37e3-4570-9531-0dc98a014a1b";
            var authToken = TempData["authToken"].ToString();

            // Construct the URL with the query string parameter
            var url = $"api/integrations/GetDocumentStatus?RegisterSignID={Uri.EscapeDataString(RegisterSignID)}";
            //var url = $"api/integrations/GetSupportingDocs?RegisterSignID={Uri.EscapeDataString(RegisterSignID)}";

            // Create the HttpClient instance using the named client configured in Program.cs
            var httpClient = _httpClientFactory.CreateClient("AuthAPI");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Execute the GET request and store the response
            using HttpResponseMessage response = await httpClient.GetAsync(url);
            TempData.Keep("authToken");
            TempData.Keep("RegisterSignID");
            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content
                var responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into a list of signatories
                //Signatories = JsonSerializer.Deserialize<List<Signatory>>(responseBody);
                var responseData = JsonSerializer.Deserialize<ResponseDocData>(responseBody);
                if (responseData.response != null)
                {
                    Document = responseData.response;
                    // Return the page with the populated signatories list
                    return Page();
                }
                else
                {
                    // TempData["failure"] = "No signatories were found.";
                    //return RedirectToPage("/Error");
                    TempData["NotFound"] = "No Documents were found.";
                    return Page();
                }
                // Return the page with the populated signatories list
                //return RedirectToPage("/Signatories/Index");
            }
            else
            {
                // Handle unsuccessful response
                // For example, you can read the response status code
                var statusCode = (int)response.StatusCode;
                TempData["failure"] = $"Failed to retrieve Documents. Status code: {statusCode}";

                // Redirect to an error page
                return RedirectToPage("/Error");
            }
        }

        


    }

}
