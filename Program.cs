var builder = WebApplication.CreateBuilder(args);

// Adiciona política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend",
        policy => policy
            .WithOrigins("http://localhost:5173", "https://my-task-list-ten.vercel.app")
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

builder.Services.AddControllers();
builder.Configuration.AddEnvironmentVariables();


var app = builder.Build();


// Usa a política de CORS
app.UseCors("PermitirFrontend");

app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", async (IConfiguration config) =>
{
    try
    {
        using var conn = new Npgsql.NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();
        return Results.Ok("API e Banco funcionando");
    }
    catch (Exception ex)
    {
        return Results.Problem("Erro ao conectar com o banco: " + ex.Message);
    }
});


app.Run();
