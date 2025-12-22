using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Services
{
    public class HttpClientService
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl;

        public HttpClientService(string baseUrl)
        {
            this.baseUrl = baseUrl.TrimEnd('/');
            httpClient = new HttpClient();
        }


        public async Task<List<Phone>> GetPhones()
        {
            var response = await httpClient.GetAsync($"{baseUrl}/phones");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Phone>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


        public async Task<List<Company>?> GetCompanies()
        {
            var response = await httpClient.GetAsync($"{baseUrl}/companies");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Company>>(json, new JsonSerializerOptions 
            { PropertyNameCaseInsensitive = true });
        }


        public async Task DeletePhones(int id)
        {
            var response = await httpClient.DeleteAsync($"{baseUrl}/phones/{id}");
            response.EnsureSuccessStatusCode();
        }


        public async Task AddPhones(Phone phone)
        {
            var json = JsonSerializer.Serialize(phone);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{baseUrl}/phones", content);
            response.EnsureSuccessStatusCode();
        }


        public async Task EditPhone(Phone phone)
        {
            var json = JsonSerializer.Serialize(phone);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"{baseUrl}/phones/{phone.Id}", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
