using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AbsenceAccounting.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class EmployeesController : ControllerBase
    {
        private readonly string _connectionString;

        public EmployeesController(string connectionString)
        {
            _connectionString = connectionString;
        }

        // метод получения списка сотрудников
        [HttpGet]
        public async Task<IActionResult> GetEmployeesAsync()
        {
            List<string> employeesList = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string sqlExpression = "SELECT CONCAT(id, ' ', first_name, ' ', last_name) AS employee " +
                    "FROM dbo.employees";

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employeesList.Add(reader.GetString(0));
                }
            }

            return Ok(employeesList);
        }
    }
}
