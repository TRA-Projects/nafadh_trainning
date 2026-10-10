using Bidding_Common;
using Bidding_Models;
using Bidding_Models.DTOs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Bidding_Core.Helpers
{
    public class MolIntegration
    {
        private readonly IConfiguration configuration;

        public MolIntegration(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<FreelancerDataFromMol> GetDataFromMol(string CivilId)
        {

            try
            {
                string MolBaseUrl = configuration["Mol:MolBaseUrl"].ToString();
                string ServiceUserName = configuration["Mol:ServiceUserName"].ToString().Decrypt();
                string ServicePassword = configuration["Mol:ServicePassword"].ToString().Decrypt();
                using var client = new HttpClient();
                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ServiceUserName}:{ServicePassword}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                var responseMessage = await client.GetAsync($"{MolBaseUrl}?civilId={CivilId}");
                responseMessage.EnsureSuccessStatusCode();
                var responseJson = await responseMessage.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<CivilIdValidationResponse>(responseJson);
                if (response.Status != 1)
                    return null;

                return response.Data;
            }
            catch
            {
                return null;
            }
        }
    }
}