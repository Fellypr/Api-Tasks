using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ListTaskApi.services;


namespace ListTaskApi.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class Tasks : ControllerBase
    {
        private readonly IConfiguration _config;
        public Tasks(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        [HttpPost("CriarTask")]
        public async Task<ActionResult> Create([FromBody] CreateTask task)
        {
            try
            {
                var connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"INSERT INTO create_task(usuario_id,tarefa_criada)
                                VALUES (@id,@tarefa)";

                    var command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", task.Id);
                    command.Parameters.AddWithValue("@tarefa", task.Task);

                    await command.ExecuteNonQueryAsync();
                }


                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            ;

        }
        [HttpGet("ListarTask/{id}")]
        public async Task<ActionResult> List([FromRoute] int id)
        {
            try
            {
                var connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"SELECT * FROM create_task WHERE usuario_id = @id";

                    var command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);

                    var reader = await command.ExecuteReaderAsync();

                    var tasks = new List<CreateTask>();

                    while (await reader.ReadAsync())
                    {
                        var task = new CreateTask
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Task = reader["tarefa_criada"].ToString()
                        };
                        tasks.Add(task);
                    }
                    return Ok(tasks);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            ;
        }
        [HttpDelete("DeletarTask/{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"DELETE FROM create_task WHERE id = @id";

                    var command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);

                    await command.ExecuteNonQueryAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            ;
        }
        [HttpPost("CompletarTask/{id}")]
        public async Task<ActionResult> Completar([FromRoute] int id, TasksCompled tasksCompled)
        {
            try
            {
                if (tasksCompled.DateTask.Date < DateTime.Now.Date)
                {
                    return BadRequest("A data não pode ser anterior à atual.");
                }

                var connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"INSERT INTO rotinas (rotinas,data_da_rotina,criador_da_rotina) VALUES (@rotinas,@data,@id)";

                    var command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@rotinas", tasksCompled.NameTask);
                    command.Parameters.AddWithValue("@data", tasksCompled.DateTask);
                    command.Parameters.AddWithValue("@id", id);

                    await command.ExecuteNonQueryAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
        [HttpGet("ListarRotina/{id}")]
        public async Task<ActionResult> ListRotina([FromRoute] int id)
        {
            try
            {

                var connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new NpgsqlConnection(connectionString))

                {
                    await connection.OpenAsync();

                    var query = @"SELECT * FROM rotinas WHERE criador_da_rotina = @id ORDER BY data_da_rotina DESC;";

                    var command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);

                    var reader = await command.ExecuteReaderAsync();

                    var tasks = new List<TasksCompled>();

                    while (await reader.ReadAsync())
                    {
                        var task = new TasksCompled
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            NameTask = reader["rotinas"].ToString(),
                            DateTask = Convert.ToDateTime(reader["data_da_rotina"])
                        };
                        tasks.Add(task);
                    }
                    return Ok(tasks);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpDelete("DeletarRotina/{id}")]
        public async Task<ActionResult> DeletaRotina ([FromRoute] int id)
        {
            try
            {
                var connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"DELETE FROM rotinas WHERE id = @id;";

                    var command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);

                    await command.ExecuteNonQueryAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}