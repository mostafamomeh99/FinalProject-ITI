using IRepositoryService;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RepositoryFactory
{
    public class GrokService : IGrokService
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey;
        private readonly string grokEndPoint;

        public GrokService(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
             grokEndPoint = configuration["GrokEndPoint"];
             apiKey = configuration["GrokKey"];
        }
        public async Task<JsonObject> SendMessage(List<string> inputmessage, List<string> inputrole)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var arrayrequest = new object[inputmessage.Count];

            for(int i=0;i<arrayrequest.Length;i++)
            {
                arrayrequest[i] = new { role = inputrole[i], content = inputmessage[i] };
            }


            var requestBody = new
            {
                model = "llama3-8b-8192",
                messages = arrayrequest,
                temperature = 0.7
            };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = await _httpClient.PostAsync(grokEndPoint, content);
            var responseString = await request.Content.ReadAsStringAsync();
            var statuscode = request.StatusCode;
            var jsonResponse = JsonSerializer.Deserialize<JsonObject>(responseString);
            
            return jsonResponse;
            
        }

        public void VerifiyMessageSize(List<string> inputmessage,List<string> inputroles
            , int limit=15000)
        {
            var messages = string.Join(',', inputmessage);
            // remove ',' due to split
            var clearmessage = Regex.Replace(messages, ",", "");

            if (clearmessage.Length<= limit)
            {
                return ;
            }

                int NumberOfRemove = 0;
                // use it as compare between string after remove , remoe actual from list
                StringBuilder clearmessagecompare = new StringBuilder(clearmessage);

                while (clearmessagecompare.Length > limit&& NumberOfRemove < inputmessage.Count)
                {
                    inputmessage.Remove(inputmessage[NumberOfRemove]);
                inputroles.Remove(inputroles[NumberOfRemove]);
                    clearmessagecompare.Remove(0, inputmessage[NumberOfRemove].Length);
                }
            
            return;
        }
    }
}
