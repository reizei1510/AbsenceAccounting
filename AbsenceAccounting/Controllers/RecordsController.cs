using AbsenceAccounting.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AbsenceAccounting.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class RecordsController : ControllerBase
    {
        private readonly string _connectionString;

        public RecordsController(string connectionString)
        {
            _connectionString = connectionString;
        }

        // метод для получения списка записей
        [HttpGet]
        public async Task<IActionResult> GetRecordsAsync()
        {
            List<RecordData> recordsDataList = new List<RecordData>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // передается название причины
                string sqlExpression = "SELECT t.id, CONCAT(e.id, ' ', e.first_name, ' ', e.last_name) AS employee, r.reason, t.date_start, t.duration, t.taken, t.comment " +
                    "FROM dbo.timesheet t, dbo.employees e, dbo.reasons r " +
                    "WHERE e.id = t.employee_id AND r.id = t.reason_id";
                
                SqlCommand command = new SqlCommand(sqlExpression, connection);

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    RecordData recordData = new RecordData()
                    {
                        Id = reader.GetInt32(0),
                        Employee = reader.GetString(1),
                        Reason = reader.GetString(2),
                        Start = reader.GetDateTime(3).ToString("dd.MM.yyyy"), // формат "dd.MM.yyyy" для отображения в таблице
                        Duration = reader.GetInt32(4),
                        Taken = reader.GetBoolean(5),
                        Comment = reader.GetString(6)
                    };

                    recordsDataList.Add(recordData);
                }
            }

            return Ok(recordsDataList);
        }

        // метод для получения записи по id
        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetRecordAsync([FromRoute] int id)
        {
            RecordData recordData = new RecordData();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // передается id причины
                string sqlExpression = "SELECT t.id, CONCAT(e.id, ' ', e.first_name, ' ', e.last_name) AS employee, r.id, t.date_start, t.duration, t.taken, t.comment " +
                    "FROM dbo.timesheet t, dbo.employees e, dbo.reasons r " +
                    "WHERE e.id = t.employee_id AND r.id = t.reason_id AND t.id = @id";
                
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    recordData = new RecordData()
                    {
                        Id = reader.GetInt32(0),
                        Employee = reader.GetString(1),
                        Reason = reader.GetInt32(2).ToString(),
                        Start = reader.GetDateTime(3).ToString("yyyy-MM-dd"), // формат "yyyy-MM-dd" для отображения в поле формы
                        Duration = reader.GetInt32(4),
                        Taken = reader.GetBoolean(5),
                        Comment = reader.GetString(6)
                    };
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok(recordData);
        }

        // метод для получения id будущей записи (в БД через последовательность и триггер)
        [HttpGet("next-id")]
        public async Task<IActionResult> GetNextId()
        {
            long id = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sqlExpression = "EXEC GetNextRecordId";

                SqlCommand command = new SqlCommand(sqlExpression, connection);

                id = (long)await command.ExecuteScalarAsync();
            }

            return Ok(id);
        }


        // метод для добавления новой записи
        [HttpPost]
        public async Task<IActionResult> AddRecordAsync([FromBody] RecordData recordData)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sqlExpression = "INSERT INTO dbo.timesheet " +
                    "VALUES (@employee_id, @reason_id, @date_start, @duration, @taken, @comment)";

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("@employee_id", recordData.Employee);
                command.Parameters.AddWithValue("@reason_id", recordData.Reason);
                command.Parameters.AddWithValue("@date_start", recordData.Start);
                command.Parameters.AddWithValue("@duration", recordData.Duration);
                command.Parameters.AddWithValue("@taken", recordData.Taken);
                command.Parameters.AddWithValue("@comment", recordData.Comment);

                try
                {
                    await command.ExecuteNonQueryAsync();
                }
                catch
                {
                    return BadRequest();
                }
            }

            return Ok();
        }

        // метод для изменения записи по id
        [HttpPut]
        public async Task<IActionResult> EditRecordAsync([FromBody] RecordData recordData)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sqlExpression = "UPDATE dbo.timesheet " +
                    "SET employee_id = @employee_id, reason_id = @reason_id, date_start = @date_start, duration = @duration, taken = @taken, comment = @comment " +
                    "WHERE id = @id";

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("@employee_id", recordData.Employee);
                command.Parameters.AddWithValue("@reason_id", recordData.Reason);
                command.Parameters.AddWithValue("@date_start", recordData.Start);
                command.Parameters.AddWithValue("@duration", recordData.Duration);
                command.Parameters.AddWithValue("@comment", recordData.Comment);
                command.Parameters.AddWithValue("@taken", recordData.Taken);
                command.Parameters.AddWithValue("@id", recordData.Id);

                await command.ExecuteNonQueryAsync();
            }

            return Ok();
        }

        // метод для удаления записи по id
        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> DeleteRecordAsync([FromRoute] int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string sqlExpression = $"DELETE FROM dbo.timesheet " +
                    "WHERE id = @id";

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("@id", id);

                await command.ExecuteNonQueryAsync();
            }

            return Ok();
        }
    }
}
