using Practice.Models;

namespace Practice.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDataModel>> GetEmployees();

        Task<EmployeeDataModel> GetEmployeeById(int employeeId);

        Task UpdateEmployee(int id, EmployeeDataModel employee);

        Task<EmployeeDataModel> Create(EmployeeDataModel employee);

        Task Delete(int id);
    }
}
