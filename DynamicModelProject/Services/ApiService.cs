using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicModelProject.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        public ApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> GetSchemaJsonAsync()
        {
            await Task.Delay(500);

            // Simulated API response: key = property name, value = property type
            var schemaJson = @"
            {
                ""firstName"": ""string"",
                ""password"": ""password"",
                ""age"": ""int"",
                ""email"": ""email"",
                ""gender"": ""radio:Male,Female,Other"",
                ""country"": ""select:India,USA,Canada"",
                ""dob"": ""date"",
                ""isMember"": ""bool"",
                ""submit"": ""button""
            }";



            return schemaJson;
        }
    }
}
