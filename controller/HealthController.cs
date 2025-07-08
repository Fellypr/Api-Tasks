using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

namespace ListTaskApi.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public HealthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult GetHealthStatus()
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                return Ok(new
                {
                    status = "API funcionando",
                    database = "Conexão com o banco OK"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "API funcionando",
                    database = "Erro ao conectar com o banco",
                    error = ex.Message
                });
            }
        }
    }
}
