using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Iamsigner_Integration.Models;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Iamsigner_Integration.Models;

namespace Iamsigner_Integration.Pages.Signatories
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public CreateModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public Signatory Signatory { get; set; }

        public IActionResult OnGet()
        {
            TempData.Keep("authToken");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            TempData.Keep("authToken");
            //Signatory.registerSignID = TempData["RegisterSignID"].ToString();
            //    if (TempData["RegisterSignID"] != null)
            //    {
            //        if (Guid.TryParse(TempData["RegisterSignID"].ToString(), out Guid registerSignID))
            //        {
            //            Signatory.registerSignID = registerSignID;
            //        }
            //    }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Save the Signatory data to the database
                //_context.Signatories.Add(Signatory);
                // _context.Signatory.Add(Signatory);
                //await _context.SaveChangesAsync();

                var client = _clientFactory.CreateClient("AuthAPI");

                // Prepare the request body
                var requestData = new
                {
                    RegisterSignID = Signatory.registerSignID,
                    Signatories = new List<Signatory> { Signatory }
                };

                // Serialize the request data to JSON
                var jsonRequest = JsonSerializer.Serialize(requestData);

                // Create the request content
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Get the auth token from TempData
                if (!TempData.TryGetValue("authToken", out var authToken) || authToken == null)
                {
                    // Handle case when authToken is null or not found in TempData
                    return RedirectToPage("/Error");
                }
                //string authToken = TempData["authToken"] as string;

                // Check if authToken is not null
                if (authToken != null)
                {
                    // Add authorization header
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.ToString());

                    // Send the POST request
                    var response = await client.PostAsync("api/integrations/AddSignatories", content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Deserialize the response content
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonSerializer.Deserialize<ResponseData>(jsonResponse);

                        // Check if signatories were successfully created
                        if (responseObject != null && responseObject.successStatus)
                        {
                            // Redirect to Index page
                            return RedirectToPage("./Index");
                        }
                        else
                        {
                            // Handle unsuccessful response
                            ModelState.AddModelError(string.Empty, responseObject?.message ?? "Failed to create signatories.");
                            return Page();
                        }
                    }
                    else
                    {
                        // Handle HTTP error response
                        ModelState.AddModelError(string.Empty, "Failed to communicate with the server.");
                        return Page();
                    }
                }
                else
                {
                    // Handle case when authToken is null or not found in TempData
                    ModelState.AddModelError(string.Empty, "Authorization token not found.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                ModelState.AddModelError(string.Empty, "An error occurred while processing the request.");
                return Page();
            }
        }
    }
}
