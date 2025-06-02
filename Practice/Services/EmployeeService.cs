using Practice.Models;
using Practice.Services.Interfaces;

using System.Text.Json;

namespace Practice.Services
{
    public class EmployeeService(ApiService client) : IEmployeeService
    {
        public async Task<EmployeeDataModel> Create(EmployeeDataModel employee)
        {
            string requestJson = JsonSerializer.Serialize(employee);

            var result = await client.SendAsync("Employee", HttpMethod.Post, json: requestJson);
            if (result.IsSuccessStatusCode)
            {
                string data = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<EmployeeDataModel>(data);
            }
            else
            {
                return null;
            }
        }

        public async Task Delete(int id)
        {
            var result = await client.SendAsync($"Employee/{id}", HttpMethod.Delete);
            result.EnsureSuccessStatusCode();
        }

        public async Task<EmployeeDataModel> GetEmployeeById(int employeeId)
        {
            var result = await client.SendAsync($"Employee/{employeeId}", HttpMethod.Get);
            if (result.IsSuccessStatusCode)
            {
                var responseData = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<EmployeeDataModel>(responseData);
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<EmployeeDataModel>> GetEmployees()
        {
            var result = await client.SendAsync($"Employee", HttpMethod.Get);
            if (result.IsSuccessStatusCode)
            {
                var responsedata = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<EmployeeDataModel>>(responsedata);
            }
            else
            {
                return null;
            }
        }

        public async Task UpdateEmployee(int id, EmployeeDataModel employee)
        {
            var result = await client.SendAsync($"Employee/{id}", HttpMethod.Put, json: JsonSerializer.Serialize(employee));
            result.EnsureSuccessStatusCode();
        }
    }
}
