using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Bidding_Models.Integrations;

namespace ConstructionHub_Core.Helpers
{
    public class MociHelper
    {
        private readonly string _appUrl;
        private readonly string _appId;
        private readonly string _appPassword;
        private readonly HttpClient _httpClient;

        public MociHelper(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            // support a few common configuration key variants
            _appUrl = configuration["Moci:BaseUrl"] ?? throw new ArgumentNullException();
            _appId = configuration["Moci:AppId"] ?? throw new ArgumentNullException();
            _appPassword = configuration["Moci:AppPassword"] ?? throw new ArgumentNullException();

            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Calls the MOCI GetAllData endpoint and deserializes the response into <see cref="MociApiResponseDto"/>.
        /// App ID and password are read from configuration provided to the constructor.
        /// </summary>
        /// <param name="crNo">CR number to query.</param>
        /// <returns>MociApiResponseDto on success, null on failure or when response can't be deserialized.</returns>
        public async Task<MociApiResponseDto> GetDataByCrAsync(string crNo)
        {
            if (string.IsNullOrWhiteSpace(crNo)) throw new ArgumentException("crNo is required", nameof(crNo));
            if (string.IsNullOrWhiteSpace(_appId) || string.IsNullOrWhiteSpace(_appPassword))
                throw new InvalidOperationException("MOCI app credentials are not configured.");

            var baseUrl = _appUrl;
            //var baseUrl = "http://10.42.55.10:8080/MOCIClientProduction/GetAllData";
            var url = $"{baseUrl}?crNo={crNo}&appID={_appId}&appPassword={_appPassword}";

            // The remote endpoint expects a POST request (per requirements). Send an empty JSON body.
            using var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // network error - return null to indicate failure
                return null;
            }

            if (!response.IsSuccessStatusCode)
                return null;

            var respString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(respString)) return null;

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dto = JsonSerializer.Deserialize<MociApiResponseDto>(respString, options);
                return dto;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
