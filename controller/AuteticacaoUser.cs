using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ListTaskApi.services;
using Microsoft.AspNetCore.Identity;


namespace ListTaskApi.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuteticacaoUser : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuteticacaoUser(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Login([FromBody] AuteticacaoUserProp User)
        {
            try
            {
                var senhaHash = new PasswordHasher<string>();
                string hash = senhaHash.HashPassword(null, User.Password);

                var connectsring = _config.GetConnectionString("DefaultConnection");
                using (var connection = new NpgsqlConnection(connectsring))
                {
                    await connection.OpenAsync();
                    // Verifica se o email já existe
                    var checkQuery = "SELECT COUNT(*) FROM listtask WHERE emaildousuario = @Email";
                    var checkCmd = new NpgsqlCommand(checkQuery, connection);
                    checkCmd.Parameters.AddWithValue("@Email", User.Email);

                    var count = (long)await checkCmd.ExecuteScalarAsync();
                    if (count > 0)
                    {
                        return BadRequest("Este e-mail já está cadastrado.");
                    }

                    var query = @"INSERT INTO listtask (nomedousuario,emaildoUsuario,senhadousuario)
                                    VALUES(@nome,@email,@senha);";

                    var command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@nome", User.NameUser);
                    command.Parameters.AddWithValue("@email", User.Email);
                    command.Parameters.AddWithValue("@senha", hash);


                    await command.ExecuteNonQueryAsync();

                    return Ok();

                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("login")]
        public async Task<IActionResult> FazendoLogin([FromBody] Login User)
        {
            try
            {
                var connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    var query = @"SELECT id, nomeDoUsuario, senhadousuario FROM listtask WHERE emaildoUsuario = @email";

                    var command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@email", User.EmailDoUsuario);

                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var senhaHash = new PasswordHasher<string>();
                            var hash = reader.GetString(reader.GetOrdinal("SenhaDoUsuario")); // Ajuste o nome da coluna se for outro
                            var result = senhaHash.VerifyHashedPassword(null, hash, User.SenhaDoUsuario);
                            if (result == PasswordVerificationResult.Success)
                            {
                                // Pega o id e nome do usuário
                                var userId = reader.GetInt32(reader.GetOrdinal("id"));
                                var nome = reader.GetString(reader.GetOrdinal("nomeDoUsuario"));

                                // Retorna o id e nome para o frontend
                                return Ok(new { userId = userId, nome = nome });
                            }
                        }
                    }
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}