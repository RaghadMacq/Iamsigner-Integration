using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Iamsigner_Integration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Iamsigner_Integration.Models;

namespace Iamsigner_Integration.Pages.Signatories
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public EditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            Signatory = new Signatory();
        }

        [BindProperty]
        public Signatory Signatory { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            TempData["RegisterSignID"]= Signatory.registerSignID;
            TempData["signatoriesID"] = id;
            try
            {
                // Get the auth token from TempData
                if (!TempData.TryGetValue("authToken", out var authToken) || authToken == null)
                {
                    // Handle case when authToken is null or not found in TempData
                    return RedirectToPage("/Error");
                }

                // Get the RegisterSignID from TempData
                string registerSignID = null;
                if (TempData.ContainsKey("RegisterSignID"))
                {
                    registerSignID = TempData["RegisterSignID"]?.ToString();
                }

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

                    // Find the signatory with the matching ID
                    var signatory = responseData.response.FirstOrDefault(s => s.signatoriesID.ToString() == id);
                    if (signatory != null)
                    {
                        Signatory = signatory; // Assign the matching signatory to Signatories
                    }
                }
                else
                {
                    // Handle HTTP error response
                    // You can set an error message or redirect to an error page
                    return RedirectToPage("/Error");
                }
                TempData.Keep("authToken");
                TempData.Keep("signatoriesID");

                return Page();
            }
            catch (Exception)
            {
                // Log or handle the exception
                // You can set an error message or redirect to an error page
                return RedirectToPage("/Error");
            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            TempData.Keep("authToken");
            TempData.Keep("signatoriesID");

            if (!ModelState.IsValid)
            {
                return Page();
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

                var url = $"api/integrations/UpdateSignatory";
                if (TempData["signatoriesID"] != null && Guid.TryParse(TempData["signatoriesID"].ToString(), out Guid signatoriesId))
                {
                    Signatory.signatoriesID = signatoriesId;
                }
                // Create a JSON payload for the request body
                var requestBody = JsonSerializer.Serialize(Signatory);

                // Create a request content with JSON data
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // Send the PUT request
                var response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Signatory updated successfully, you may want to redirect to a confirmation page
                    return RedirectToPage("./Index");
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
